using BankingClient.Core;
using BankingClient.Core.Dto;
using BankingClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
namespace BankingClient.Ui.Pages;

public partial class Token(
  NavigationManager navigation,
  AuthenticationStateProvider authenticationStateProvider,
  IAccessTokenProvider accessTokenProvider,
  IAuthorizationService authorizationService,
  IOwnerService ownerService,
  UserStateHolder userStateHolder,
  ILogger<Token> logger
) : ComponentBase {
  
  protected override async Task OnInitializedAsync() {

    logger.LogInformation("Token: Request Id-Token");
    var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
    
    // User is authenticated
    if (authState.User.Identity?.IsAuthenticated == true) {
      logger.LogInformation("Token: User is authenticated");
      userStateHolder.CurrentUser = authState.User;
      
      // attach the owner by userId
      switch (await ownerService.GetByUserId(userStateHolder.UserId)) {
        case ResultData<OwnerDto?>.Success success:
          userStateHolder.UpdateOwnerDto(success.Data!);
          break;
        case ResultData<OwnerDto?>.Error error:
          logger.LogError("Token: GetByUserId Error: {1}", error.Exception.Message);
          //_errorMessage = error.Exception.Message;
          break;
      }
      
      // Evaluate the access Token
      logger.LogInformation("Token: Requesting Access Token");
      var result = await accessTokenProvider.RequestAccessToken();

      // get roles from the access token
      if (result.TryGetToken(out var token)) {
        logger.LogInformation($"Token: Access Token retrieved");
        userStateHolder.ProcessAccessToken(token);
        
        // Case 1 Owner does not exist, create it (remove it for the Banking API)
        var exists = await ownerService.ExistsByUserName(userStateHolder.UserName);
        if (!exists) {
          logger.LogInformation("Token: Owner does not exist, create a new owner");
          userStateHolder.CreateOwnerDto();
          await ownerService.Post(userStateHolder.OwnerDto!);
        }
        // Case 2: Owner exists (standard case for the Banking API)
        else {
          logger.LogInformation("Token: Owner exists, get owner by username");
          var resultData = await ownerService.GetByUserName(userStateHolder.UserName);
          switch (resultData) {
            case ResultData<OwnerDto?>.Success success:
              var ownerDto = success.Data!;
              userStateHolder.UpdateOwnerDto(ownerDto);
              logger.LogInformation("Token: Put {1}", userStateHolder.OwnerDto!);
              await ownerService.Put(userStateHolder.OwnerDto!);
              break;
            case ResultData<OwnerDto?>.Error error:
              logger.LogError("Token: GetByUserName Error: {1}", error.Exception.Message);
              //_errorMessage = error.Exception.Message;
              break;
          }
        }
      } // if result.TryGetToken
      else {
        _ = new Dictionary<string, string> {
          { "Error", "Failed to retrieve Access-Token." }
        };
      }
    }

    // User is not authenticated
    else {
      logger.LogInformation("Token.OnInitialize(): User is not authenticated, goto login");
      navigation.NavigateTo("authentication/login");
    }
  }
  
  private async Task LeavePage() {
    logger.LogInformation("Token: LeavePage");
    await NavigateAuthenticatedUser();
  }

  private async Task NavigateAuthenticatedUser() {
    if (!userStateHolder.IsAuthenticated) {
      logger.LogError("Token: User is not authenticated");
      navigation.NavigateTo("authentication/login");
    }

    var user = userStateHolder.CurrentUser!;
    var ownerId = userStateHolder.OwnerDto?.Id;
    if (ownerId == null) {
      logger.LogError("Token: User is authetictaed, but OwnerDto is null");
      return;
    }
    if ((await authorizationService.AuthorizeAsync(user, "AdminPolicy")).Succeeded) {
      logger.LogInformation("Token: Navigate to /owners");
      navigation.NavigateTo("owners");
    }
    else if ((await authorizationService.AuthorizeAsync(user, "UserPolicy")).Succeeded) {
      logger.LogInformation("Token: Navigate to /owner/{0}", ownerId);
      navigation.NavigateTo($"owner/{ownerId}");
    }
    else {
      navigation.NavigateTo("home");
    }
  }
}