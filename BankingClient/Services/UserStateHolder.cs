using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BankingClient.Core.Dto;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
namespace BankingClient.Services;

public class UserStateHolder(
   AuthenticationStateProvider authenticationStateProvider,
   ILogger<UserStateHolder> logger
) {

   // ID Token claims -------------------------------------------------------------------
   private ClaimsPrincipal? _currentUser = null;
   public ClaimsPrincipal? CurrentUser {
      get => _currentUser;
      set {
         _currentUser = value;
         logger.LogInformation("UserStateHolder: IsAuthenticated {1}", IsAuthenticated);
         logger.LogInformation("UserStateHolder: UserName {1}", UserName);
         logger.LogInformation("UserStateHolder: UserId {1}", UserId);
         logger.LogInformation("UserStateHolder: FirstName {1}", FirstName);
         logger.LogInformation("UserStateHolder: LastName {1}", LastName);
         logger.LogInformation("UserStateHolder: Email {1}", Email);
         logger.LogInformation("UserStateHolder: UserRole {1}", UserRole);
      }
   }
   // getter properties
   public bool IsAuthenticated => 
      _currentUser?.Identity?.IsAuthenticated ?? false;
   public string UserId => 
      _currentUser?.FindFirst("sub")?.Value ?? 
      throw new InvalidOperationException("User sub not found, authentication failed");
   public string UserName => 
      _currentUser?.FindFirst("preferred_username")?.Value ??
      throw new InvalidOperationException("UserName not found, authentication failed");
   public string? FirstName => 
      _currentUser?.FindFirst("given_name")?.Value;
   public string? LastName =>
      _currentUser?.FindFirst("family_name")?.Value;
   public string? Email => 
      _currentUser?.FindFirst("email")?.Value;
   public string? UserRole {
      get {
         var roles = _currentUser?.FindAll("roles").Select(c => c.Value);
         return roles?.FirstOrDefault(role => role.StartsWith("webtech"));
      }
   }
   
   // Access Token ----------------------------------------------------------------------
   public AccessToken? AccessToken { get; private set; }
   public Dictionary<string, string>? AccessTokenParams { get; private set; }
   public List<Claim>? AccessTokenClaims { get; private set; }
   
   public void ProcessAccessToken(AccessToken token) {
      AccessToken = token;
      AccessTokenParams = DecodeJwtToken(AccessToken.Value);
      AccessTokenClaims = new JwtSecurityTokenHandler().ReadJwtToken(token.Value).Claims.ToList();
      AccessTokenClaims.ForEach(c => logger.LogInformation("UserStateHolder: AccessTokenClaims {1}", c));
      /*
      // Check roles 
      var realmAccessClaim = AccessTokenClaims.FirstOrDefault(c => c.Type == "realm_access");
      if (realmAccessClaim != null) {
         var realmAccess = System.Text.Json.JsonDocument.Parse(realmAccessClaim.Value);
         var roles = realmAccess.RootElement
            .GetProperty("roles")
            .EnumerateArray()
            .Select(role => role.GetString())
            .ToList();
         foreach (var role in roles) {
            if (role == null) continue;
            // we are only interested in the roles starting with "webtech"
            // webtech-user or webtech-admin
            // if (role!.StartsWith("webtech")) 
            //    userStateHolder.AddWebtechRole(role);
         } // foreach
      } 
      */
   }
   
   private static Dictionary<string, string> DecodeJwtToken(string jwtToken) {
      var handler = new JwtSecurityTokenHandler();
      var token = handler.ReadJwtToken(jwtToken);

      // Group claims by type, if there are multiple claims of the same type
      return token.Claims
         .GroupBy(c => c.Type)
         .OrderBy(g => g.Key) // Sort the claims alphabetically by their type
         .ToDictionary(
            g => g.Key,
            g => string.Join(", ", g.Select(c => c.Value))
         );
   }
   
   // OwnerDto --------------------------------------------------------------------------
   public OwnerDto? OwnerDto { get; private set; } = null;
   
   public void CreateOwnerDto() {
      OwnerDto = new OwnerDto(
         Id: Guid.NewGuid(),
         FirstName: FirstName ?? "",
         LastName: LastName ?? "",
         Email: Email,
         UserName: UserName,
         UserId: UserId,
         UserRole: UserRole
      );
   }
   
   public void UpdateOwnerDto(OwnerDto dto) {
      // UserName (preferred_username) must match
      if(dto.UserName != UserName)
         throw new InvalidOperationException("UserName (preferred_username) mismatch");
      
      OwnerDto = dto;
      if(FirstName != null && FirstName != dto.FirstName)
         OwnerDto = OwnerDto with { FirstName = FirstName };
      if(LastName != null && LastName != dto.LastName)
         OwnerDto = OwnerDto with { LastName = LastName };
      if(Email != null && Email != dto.Email)
         OwnerDto = OwnerDto with { Email = Email };
      if(UserId != dto.UserId)
         OwnerDto = OwnerDto with { UserId = UserId };
      if(Email != null && UserRole != dto.UserRole)
         OwnerDto = OwnerDto with { UserRole = UserRole };
      
      logger.LogInformation("UserStateHolder: UpdateOwnerDto{1}", OwnerDto);
   }
   
   // AccountDto --------------------------------------------------------------------------
   private AccountDto? _accountDto = null;
   public AccountDto? AccountDto {
      get => _accountDto;
      set {
         logger.LogInformation("UserStateHolder: AccountDto{1}", value);
         _accountDto = value;
      }
   }
}