using dotenv.net;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using IdGen.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Microsoft.VisualBasic;
using Serilog;
using Serilog.Events;
using Volunteasy.WebApp;
using Volunteasy.WebApp.Middleware;
using Volunteasy.Application;
using Volunteasy.Application.Services;
using Volunteasy.Core.Data;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;
using Volunteasy.Infrastructure.Firebase;

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

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
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
    });

builder.Services.AddScoped<IAuthenticationSignInHandler, AuthHandler>();

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseMiddleware<OrganizationSlugMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.Run();