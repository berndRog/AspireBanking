using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Xml.Linq;

namespace Authentification.Keycloak.Extensions;

public static class LoginLogoutEndpointRouteBuilderExtensions {
   public static IEndpointConventionBuilder MapLoginAndLogout(
      this IEndpointRouteBuilder endpoints
   ) {
      var group = endpoints.MapGroup("");
      group.MapGet("/login", (string? returnUrl) => TypedResults.Challenge(GetAuthProperties(returnUrl)))
         .AllowAnonymous();
      // Sign out of the Cookie and OIDC handlers. If you do not sign out with the OIDC handler,
      // the user will automatically be signed back in the next time they visit a page that requires authentication
      // without being able to choose another account.
      // group.MapGet("/logout", (string? returnUrl) => TypedResults.SignOut(GetAuthProperties(returnUrl), [CookieAuthenticationDefaults.AuthenticationScheme, "MicrosoftOidc"]));
      group.MapGet("/logout", async (HttpContext context, string? returnUrl) => {
         // Retrieve the authentication result for the current user
         var result = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
         if (result?.Principal != null) {
            // Retrieve the ID token from the properties
            var idToken = result.Properties.GetTokenValue("id_token");
            if (idToken != null) {
               // Create the logout URL with the id_token_hint
               var logoutUri =
                  $"http://localhost:8080/auth/realms/OLP_Realm/protocol/openid-connect/logout?redirect_uri={returnUrl}";
               // Sign out from both the local session and the OIDC provider
               await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
               await context.SignOutAsync("MicrosoftOidc", new AuthenticationProperties { RedirectUri = logoutUri });
               return Results.Redirect(logoutUri);
            }
         }
         // If no ID token or principal, fallback to local sign out and redirect
         await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
         return Results.Redirect(returnUrl ?? "/");
      });
      return group;
   }

   private static AuthenticationProperties GetAuthProperties(string? returnUrl) {
      // TODO: Use HttpContext.Request.PathBase instead.
      const string pathBase = "/";
      // Prevent open redirects.
      if (string.IsNullOrEmpty(returnUrl)) {
         returnUrl = pathBase;
      }
      else if (!Uri.IsWellFormedUriString(returnUrl, UriKind.Relative)) {
         returnUrl = new Uri(returnUrl, UriKind.Absolute).PathAndQuery;
      }
      else if (returnUrl[0] != '/') {
         returnUrl = $"{pathBase}{returnUrl}";
      }
      return new AuthenticationProperties { RedirectUri = returnUrl };
   }
}