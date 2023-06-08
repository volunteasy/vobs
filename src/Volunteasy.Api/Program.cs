using System.Text;
using dotenv.net;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
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

    builder.Host.UseSerilog(new LoggerConfiguration()
        .Enrich.FromLogContext()
        .WriteTo.Console(new CompactJsonFormatter())
        .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
        .MinimumLevel.Override("Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.Mvc.Infrastructure.ObjectResultExecutor", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.Routing.EndpointMiddleware", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Diagnostics", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Fatal)
        .MinimumLevel.Override("Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", LogEventLevel.Warning)
        .CreateLogger());

    builder.Configuration.AddEnvironmentVariables();
#endregion

#region Infrastructure setup

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
            fb,
            b.GetService<ILogger<Auth>>()!,
            firebaseSignIn ?? ""
        ));

#endregion

#region Application setup
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<Volunteasy.Application.ISession, Session>();
    builder.Services.AddScoped<IIdentityService, IdentityService>();
    builder.Services.AddScoped<IUserService, UserService>();

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

    builder.Services.AddHttpLogging(logging =>
    {
        logging.ResponseBodyLogLimit = 0;
        logging.RequestBodyLogLimit = 0;
        logging.LoggingFields = HttpLoggingFields.None;
    });

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "VolunteasyAPI", Version = "v1" });
        var securityScheme = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme."
        };

        c.AddSecurityDefinition("jwt", securityScheme);

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            { securityScheme, new[] { "jwt" } }
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
    .UseHttpLogging()
    .UseMiddleware<ExceptionMiddleware>()
    .UseAuthentication()
    .UseAuthorization();

app.MapControllers();
app.Run();