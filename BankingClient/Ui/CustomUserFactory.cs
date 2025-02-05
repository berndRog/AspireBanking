using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
namespace BankingClient;

using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

public class CustomUserFactory(IAccessTokenProviderAccessor accessor)
   : AccountClaimsPrincipalFactory<RemoteUserAccount>(accessor) {
   
   public override async ValueTask<ClaimsPrincipal> CreateUserAsync(
      RemoteUserAccount account,
      RemoteAuthenticationUserOptions options) {
      // Get the default ClaimsPrincipal
      var user = await base.CreateUserAsync(account, options);

      if (user.Identity?.IsAuthenticated ?? false) {
         var identity = (ClaimsIdentity)user.Identity;

         // Get all claims of the type specified by options.RoleClaim (normally "roles")
         var rolesClaims = identity.FindAll(options.RoleClaim).ToList();

         // Remove the existing aggregated claims
         foreach (var claim in rolesClaims) {
            identity.RemoveClaim(claim);
         }

         // Process each role claim value
         foreach (var claim in rolesClaims) {
            var rawValue = claim.Value.Trim();

            // If the value contains a comma, assume it's a list of roles in one claim.
            if (rawValue.Contains(",")) {
               var roles = rawValue.Split(',')
                  .Select(role => role.Trim().Trim('[', ']', '\"'))
                  .Where(role => !string.IsNullOrWhiteSpace(role));

               foreach (var role in roles) {
                  identity.AddClaim(new Claim(options.RoleClaim, role));
               }
            }
            else {
               // Otherwise, just sanitize the single claim value.
               var sanitized = rawValue.Trim('[', ']', '\"');
               if (!string.IsNullOrWhiteSpace(sanitized)) {
                  identity.AddClaim(new Claim(options.RoleClaim, sanitized));
               }
            }
         }
      }
      return user;
   }
}