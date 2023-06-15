using System.Text;
using System.Text.Json.Serialization;
using dotenv.net;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using IdGen.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Volunteasy.Api.Context;
using Volunteasy.Api.Middleware;
using Volunteasy.Api.Response;
using Volunteasy.Application;
using Volunteasy.Application.Services;
using Volunteasy.Core.Data;
using Volunteasy.Core.Services;
using Volunteasy.Infrastructure.Firebase;

var builder = WebApplication.CreateBuilder(args);

#region Environment setup
    // This is meant to load env vars in a local environment.
    // Production and Staging environments should not depend on this
    if (builder.Environment.IsDevelopment())
        DotEnv.Load(options: new DotEnvOptions(ignoreExceptions: false));

    var loggerCfg = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Mvc.Infrastructure.ObjectResultExecutor", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Routing.EndpointMiddleware", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Fatal);

    if (!builder.Environment.IsDevelopment())
    {
        loggerCfg = loggerCfg.WriteTo.NewRelicLogs(
            applicationName: builder.Configuration.GetValue<string>("NR_APP") ?? "",
            licenseKey: builder.Configuration.GetValue<string>("NR_LICENSE") ?? "");
    }
    else
    {
        loggerCfg = loggerCfg.WriteTo.Console(new CompactJsonFormatter());
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
    builder.Services.AddScoped<Volunteasy.Application.ISession, Session>();
    builder.Services.AddScoped<IIdentityService, IdentityService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IOrganizationService, OrganizationService>();
    builder.Services.AddScoped<IMembershipService, MembershipService>();

#endregion

#region API Setup

    builder.Services.AddCors(x => 
        x.AddPolicy("MyPolicy", b =>
            b.AllowAnyHeader()
                .AllowCredentials()
                .AllowAnyMethod()
                .AllowAnyOrigin()
        ));

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                        builder.Configuration.GetValue<string>("FIREBASE_KEY") ?? "")),
                NameClaimType = "user_id"
            };
        });

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
    app.UseSwagger().UseSwaggerUI();

    using var scope = app.Services.CreateScope();

    scope.ServiceProvider.GetRequiredService<Data>()
        .Database.Migrate();
}

app
    .UseSerilogRequestLogging(options =>
    {
        options.GetMessageTemplateProperties = (context, s, arg3, arg4) =>
        {
            return new List<LogEventProperty>();
        };
    })
    .UseMiddleware<ExceptionMiddleware>()
    .UseAuthentication()
    .UseAuthorization();

app.MapControllers();
app.Run();