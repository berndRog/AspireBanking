using System.IdentityModel.Tokens.Jwt;
using Authentification.Keycloak.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using PeopleBlazorWebApp.Components;
using PeopleBlazorWebApp.Services;
namespace PeopleBlazorWebApp;

public class Program {
   public static void Main(string[] args) {
      var builder = WebApplication.CreateBuilder(args);

      // Add services to the container.
      builder.Services.AddRazorPages();
      builder.Services.AddServerSideBlazor();
      builder.Services.AddHttpContextAccessor();
      builder.Services.AddSingleton<LogoutService>();
      // Configure logging
      builder.Logging.ClearProviders();
      builder.Logging.AddConsole();

      // Configure OIDC authentication
      var oidcSettings = builder.Configuration.GetSection("OIDC");

      builder.Services.AddAuthentication(options => {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
         })
         .AddCookie(options => {
            options.Events.OnSigningOut = async e => {
               e.HttpContext.Response.Cookies.Delete(".AspNetCore.Cookies");
               await Task.CompletedTask;
            };
         })
         .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options => {
            options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.Scope.Add(OpenIdConnectScope.OfflineAccess);
            options.MetadataAddress = "https://keycloak.example.com/realms/myrealm/.well-known/openid-configuration";
            options.Authority = oidcSettings["Authority"];
            options.ClientId = oidcSettings["ClientId"];
            options.ClientSecret = oidcSettings["ClientSecret"];
            options.ResponseType = OpenIdConnectResponseType.Code;
            options.SaveTokens = false;
            options.GetClaimsFromUserInfoEndpoint = true;
            options.CallbackPath = new PathString("/signin-oidc");
            options.SignedOutCallbackPath = new PathString("/signout-callback-oidc");
            options.RemoteSignOutPath = new PathString("/signout-oidc");
            options.SignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
            options.MapInboundClaims = false;
            options.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
            options.TokenValidationParameters.RoleClaimType = "role";

            options.TokenValidationParameters = new TokenValidationParameters {
               ValidateIssuer = true,
               ValidIssuer = oidcSettings["Authority"],
               ValidateAudience = true,
               ValidAudience = oidcSettings["ClientId"],
               ValidateLifetime = true
            };

            options.Scope.Add("profile");
            options.Scope.Add("email");
         });

      builder.Services.AddAuthorization(options => {
         options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
         options.AddPolicy("AnonymousPolicy", policy => policy.RequireAssertion(_ => true));
      });

      builder.Services.AddControllersWithViews();

      var app = builder.Build();

      if (!app.Environment.IsDevelopment()) {
         app.UseExceptionHandler("/Error");
         //app.UseHsts();
      }
      app.MapGroup("/authentication").MapLoginAndLogout();
      
      // app.UseHttpsRedirection();
      app.UseStaticFiles();
      app.UseRouting();
      app.UseAuthentication();
      app.UseAuthorization();
      app.Run();
   }

   public static void TestMain(string[] args) {
      var builder = WebApplication.CreateBuilder(args);

      // Add services to the container.
      builder.Services.AddRazorComponents()
         .AddInteractiveServerComponents();

      builder.Services.AddCascadingAuthenticationState();

      builder.Services.AddAuthentication(options => {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
         })
         .AddIdentityCookies();

      var app = builder.Build();

      // Configure the HTTP request pipeline.
      if (app.Environment.IsDevelopment()) {
         app.UseMigrationsEndPoint();
      }
      else {
         app.UseExceptionHandler("/Error");
         // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
         app.UseHsts();
      }

      app.UseHttpsRedirection();

      app.UseAntiforgery();

      app.MapStaticAssets();
      app.MapRazorComponents<App>()
         .AddInteractiveServerRenderMode();

      // Add additional endpoints required by the Identity /Account Razor components.
      app.MapAdditionalIdentityEndpoints();

      app.Run();
   }
}