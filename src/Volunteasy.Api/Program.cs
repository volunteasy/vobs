using dotenv.net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Volunteasy.Core.Data;

var builder = WebApplication.CreateBuilder(args);

// This is meant to load env vars in a local environment.
// Production and Staging environments should not depend on this
if (builder.Environment.IsDevelopment())
    DotEnv.Load(options: new DotEnvOptions(ignoreExceptions: false));

builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);

builder.Services.AddDbContext<Data>(
    o => o.UseNpgsql(
        builder.Configuration.GetValue<string>("POSTGRES_CONNSTR") ?? "")
);

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
    .UseAuthorization();

app.MapControllers();

app.Run();