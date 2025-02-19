using System.Text.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
namespace BankingClient;

using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

// public class CustomUserFactory(
//    IAccessTokenProviderAccessor accessor,
//    ILogger<CustomUserFactory> logger
// ) : AccountClaimsPrincipalFactory<RemoteUserAccount>(accessor) {
//  
//    public override async ValueTask<ClaimsPrincipal> CreateUserAsync(
//       RemoteUserAccount userAccount,
//       RemoteAuthenticationUserOptions options
//    ) {
//       var user = await base.CreateUserAsync(userAccount, options);
//
//       if (userAccount != null && user.Identity.IsAuthenticated) {
//          var identity = (ClaimsIdentity)user.Identity;
//          logger.LogInformation("CustomUserFactory: CreateUserAsync: User is authenticated");
//          
//          // Extract realm roles from JSON object
//          if (userAccount.AdditionalProperties.TryGetValue("realm_access", out var realmAccess) &&
//              realmAccess is JsonElement realmJson) {
//             
//             logger.LogInformation("CustomUserFactory: CreateUserAsync: realm_access found");
//             if (realmJson.TryGetProperty("roles", out var rolesElement) &&
//                 rolesElement.ValueKind == JsonValueKind.Array) {
//                foreach (var role in rolesElement.EnumerateArray()) {
//                   identity.AddClaim(new Claim(ClaimTypes.Role, role.GetString()));
//                }
//             }
//          }
//       }
//
//       return user;
//    }
// }
//

public class CustomUserFactory(
   IAccessTokenProviderAccessor accessor,
   ILogger<CustomUserFactory> logger
) : AccountClaimsPrincipalFactory<RemoteUserAccount>(accessor) {

   public override async ValueTask<ClaimsPrincipal> CreateUserAsync(
      RemoteUserAccount userAccount,
      RemoteAuthenticationUserOptions options
   ) {
      // Get the default ClaimsPrincipal
      var user = await base.CreateUserAsync(userAccount, options);

      if (user.Identity?.IsAuthenticated ?? false) {
         var identity = (ClaimsIdentity)user.Identity;

         if (userAccount.AdditionalProperties.TryGetValue("realm_access", out var realmAccess) &&
             realmAccess is JsonElement realmJson) {

            logger.LogInformation("CustomUserFactory: CreateUserAsync: realm_access {1}", realmJson);
            // Extract roles from the realm_access claim
            if (realmJson.TryGetProperty("roles", out var rolesElement) &&
                rolesElement.ValueKind == JsonValueKind.Array) {
            
               foreach (var role in rolesElement.EnumerateArray()) {
                  var roleValue = role.GetString();
                  if (!string.IsNullOrEmpty(roleValue) && roleValue.StartsWith("webtech")) {
                     
                     identity.AddClaim(new Claim(ClaimTypes.Role, roleValue));
                  }
               }
            }
         }
      }
      return user;
   }
}
       