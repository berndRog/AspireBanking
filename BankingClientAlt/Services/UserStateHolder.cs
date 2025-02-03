using System.Security.Claims;
using BankingClient.Dto;
using Microsoft.AspNetCore.Components.Authorization;
namespace BankingClient.Services;

public class UserStateHolder(
   AuthenticationStateProvider authenticationStateProvider
) {
   
   private ClaimsPrincipal? _currentUser = null;
   private OwnerDto? _ownerDto = null;
   
   public async Task InitializeAsync(OwnerDto ownerDto) {
      var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
      _currentUser = authState.User;
      _ownerDto = ownerDto with { UserId = Sub };
   }

   public bool IsAuthenticated => 
      _currentUser?.Identity?.IsAuthenticated ?? false;
   
   private string? Sub =>
      _currentUser?.FindFirst("sub")?.Value;


   public string? FirstName =>
      _currentUser?.FindFirst(ClaimTypes.GivenName)?.Value;
   
   public string? LastName =>
      _currentUser?.FindFirst(ClaimTypes.Surname)?.Value;
   
   public string? Email =>
      _currentUser?.FindFirst(ClaimTypes.Email)?.Value;
   
   public string? Phone =>
      _currentUser?.FindFirst(ClaimTypes.HomePhone)?.Value;
   
   public string? UserName =>
      _currentUser?.FindFirst(ClaimTypes.Name)?.Value;
   
   public string? UserRole =>
      _currentUser?.FindFirst(ClaimTypes.Role)?.Value;

   public string? GetClaimValue(string claimType) =>
      _currentUser?.FindFirst(claimType)?.Value;
   
   public OwnerDto? OwnerDto =>
      _ownerDto;
}
