using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

// File: Services/LogoutService.cs
namespace PeopleBlazorWebApp.Services
{
   public class LogoutService
   {
      private readonly IHttpContextAccessor _httpContextAccessor;

      public LogoutService(IHttpContextAccessor httpContextAccessor)
      {
         _httpContextAccessor = httpContextAccessor;
      }

      public async Task LogoutAsync()
      {
         var context = _httpContextAccessor.HttpContext;
         if (context != null)
         {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await context.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
         }
      }
   }
}