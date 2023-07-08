using dotenv.net;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using IdGen.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Volunteasy.Application;
using Volunteasy.Application.Services;
using Volunteasy.Core.Data;
using Volunteasy.Core.Services;
using Volunteasy.Infrastructure.Firebase;
using Volunteasy.Web.Auth;
using ISession = Volunteasy.Core.Services.ISession;

var builder = WebApplication.CreateBuilder(args);

// This is meant to load env vars in a local environment.
// Production and Staging environments should not depend on this
if (builder.Environment.IsDevelopment())
    DotEnv.Load(options: new DotEnvOptions(ignoreExceptions: false));

builder.Configuration.AddEnvironmentVariables();

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

builder.Services.AddSingleton(fb);
builder.Services.AddScoped<IAuthenticator>(b => new Auth(
    fb, b.GetService<ILogger<Auth>>()!, firebaseSignIn ?? ""));

builder.Services.AddScoped<ISession, SessionProvider>();
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IMembershipService, MembershipService>();
// builder.Services.AddScoped<IDistributionService, DistributionService>();
// builder.Services.AddScoped<IBenefitService, BenefitService>();
// builder.Services.AddScoped<IBenefitProvisionService, BenefitProvisionService>();
// builder.Services.AddScoped<IBenefitItemService, BenefitItemService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/admin/auth";
    });

builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapBlazorHub();

app.MapFallbackToPage("/_Host");

app.Run();