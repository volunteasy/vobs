using System.Text;
using System.Text.Json.Serialization;
using dotenv.net;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using IdGen.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Volunteasy.Api.Middleware;
using Volunteasy.Api.Response;
using Volunteasy.Application;
using Volunteasy.Application.Services;
using Volunteasy.Core.Data;
using Volunteasy.Core.Services;
using Volunteasy.Infrastructure.Firebase;
using Volunteasy.WebApp;
using Volunteasy.WebApp.Middleware;
using VolunteasyContext = Volunteasy.Api.Context.VolunteasyContext;

var builder = WebApplication.CreateBuilder(args);

#region Environment setup

// This is meant to load env vars in a local environment.
// Production and Staging environments should not depend on this
if (builder.Environment.IsDevelopment())
    DotEnv.Load(options: new DotEnvOptions(ignoreExceptions: false));

if (builder.Environment.IsDevelopment())
    IdentityModelEventSource.ShowPII = true;

var loggerCfg = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Mvc.Infrastructure.ObjectResultExecutor", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Routing.EndpointMiddleware", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Fatal);

loggerCfg = loggerCfg.WriteTo.Console();
if (!builder.Environment.IsDevelopment())
{
    loggerCfg = loggerCfg.WriteTo.NewRelicLogs(
        applicationName: builder.Configuration.GetValue<string>("NR_APP") ?? "",
        licenseKey: builder.Configuration.GetValue<string>("NR_LICENSE") ?? "");
}


var logger = loggerCfg.CreateLogger();

builder.Services.AddSerilog(logger);
builder.Host.UseSerilog(logger);

builder.Configuration.AddEnvironmentVariables();

#endregion

#region Infrastructure setup

builder.Services.AddIdGen(123);

builder.Services.AddDbContext<Data>(o => o.UseNpgsql(
        builder.Configuration.GetValue<string>("POSTGRES_CONNSTR") ?? "",
        op => op.MigrationsAssembly("Volunteasy.Api")
    )
);

var firebaseCredentials = builder.Configuration.GetValue<string>("FIREBASE_CREDS");
var firebaseSignIn = builder.Configuration.GetValue<string>("FIREBASE_SIGNIN_URL");
var fb = FirebaseAuth.GetAuth(FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromJson(firebaseCredentials)
}));

builder.Services.AddScoped<IAuthenticator>(b => new Auth(
    fb, b.GetService<ILogger<Auth>>()!, firebaseSignIn ?? ""));

#endregion

#region Application setup

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IVolunteasyContext, VolunteasyContext>();
builder.Services.AddScoped<IVolunteasyContext, Volunteasy.WebApp.VolunteasyContext>();
builder.Services.AddScoped<IVolunteasyContext>(x =>
{
    var ctx = x.GetService<IHttpContextAccessor>();
    if (ctx == null || ctx.HttpContext == null)
        throw new NullReferenceException("HTTP context was null when building VolunteasyContext");

    var path = ctx.HttpContext.Request.Path.ToString();
    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    if (path == null)
        throw new NullReferenceException("path was null when building VolunteasyContext");

    if (path.Contains("/api/"))
        return new Volunteasy.Api.Context.VolunteasyContext(ctx, x.GetRequiredService<ILogger<Volunteasy.Api.Context.VolunteasyContext>>());

    return new Volunteasy.WebApp.VolunteasyContext(ctx,
        x.GetRequiredService<ILogger<Volunteasy.WebApp.VolunteasyContext>>());
});
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IMembershipService, MembershipService>();
builder.Services.AddScoped<IDistributionService, DistributionService>();
builder.Services.AddScoped<IBenefitService, BenefitService>();
builder.Services.AddScoped<IBenefitProvisionService, BenefitProvisionService>();
builder.Services.AddScoped<IBenefitItemService, BenefitItemService>();
builder.Services.AddScoped<IBeneficiaryService, BeneficiaryService>();

#endregion

#region API Setup

builder.Services.AddCors(x =>
    x.AddPolicy("MyPolicy", b =>
        b.AllowAnyHeader()
            .AllowCredentials()
            .AllowAnyMethod()
            .AllowAnyOrigin()
    ));

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
    options.AppendTrailingSlash = true;
});

// Add services to the container.
builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
{
    options.Conventions.AddFolderRouteModelConvention("/Quero", model =>
    {
        foreach (var selector in model.Selectors)
        {
            var split = selector.AttributeRouteModel!.Template!.Split("/").ToList();
            split.Insert(1, "{orgSlug}");
            selector.AttributeRouteModel.Template = string.Join("/", split);
        }
    });
});

builder.Services.AddAuthentication("JWT_OR_COOKIE")
    .AddCookie(options =>
    {
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {

                var id = context.Request.RouteValues["orgSlug"]?.ToString();
                if (id == null)
                    return Task.CompletedTask;

                var uri = new Uri(context.RedirectUri);

                switch (context.Request.Path.Value?.Split("/").First(x => !string.IsNullOrEmpty(x)))
                {
                    case "quero":
                        context.RedirectUri = $"/quero/{id}/login{uri.Query}";
                        break;
                }


                context.HttpContext
                    .Response.Redirect(context.RedirectUri);

                return Task.CompletedTask;
            }
        };
    })
    .AddJwtBearer(options =>
    {
        options.Authority = "https://securetoken.google.com/volunteasy-bade3";
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async ctx =>
            {
                var userId = Convert.ToInt64(
                    ctx.Principal?.FindFirst("volunteasy_id")?.Value);

                var service = ctx.HttpContext.RequestServices
                    .GetService<IIdentityService>();

                if (service is null)
                    return;

                ctx.Principal?.AddIdentities(
                    await service.GetUserSessionClaims(userId));
            }
        };
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration.GetValue<string>("FIREBASE_ISSUER"),
            ValidAudience = builder.Configuration.GetValue<string>("GCP_PROJECT"),
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration.GetValue<string>("FIREBASE_KEY") ?? ""))
        };
    }).AddPolicyScheme("JWT_OR_COOKIE", "JWT_OR_COOKIE", options =>
    {
        // runs on each request
        options.ForwardDefaultSelector = context =>
        {
            // filter by auth type
            string authorization = context.Request.Headers[HeaderNames.Authorization];
            if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
                return "Bearer";

            // otherwise always check for cookie auth
            return "Cookies";
        };
    });

builder.Services.AddScoped<IAuthenticationSignInHandler, AuthHandler>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.Converters.Add(new IdConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "VolunteasyAPI", Version = "v1" });
    c.AddServer(new OpenApiServer
    {
        Url = "http://localhost:5000"
    });
    c.AddServer(new OpenApiServer
    {
        Url = "http://volunteasy-dev.eba-sq86vjma.sa-east-1.elasticbeanstalk.com"
    });

    var sec = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme.",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "jwt",
        },
    };

    c.AddSecurityDefinition("jwt", sec);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { sec, new[] { "jwt" } }
    });
});

#endregion

var app = builder.Build();
if (!app.Environment.IsProduction())
{
    app.UseExceptionHandler(new ExceptionHandlerOptions
    {
        ExceptionHandlingPath = "/Error",
    });
    
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    
    app.UseSwagger().UseSwaggerUI();

    using var scope = app.Services.CreateScope();

    scope.ServiceProvider.GetRequiredService<Data>()
        .Database.Migrate();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseMiddleware<OrganizationSlugMiddleware>();

app.MapControllers();

app
    .UseSerilogRequestLogging()
    .UseMiddleware<ExceptionMiddleware>()
    .UseAuthentication()
    .UseAuthorization();



app.MapRazorPages();

var port = app.Configuration.GetValue<string>("PORT");


app.Run(url: port == null ? null : $"http://*:{port}");