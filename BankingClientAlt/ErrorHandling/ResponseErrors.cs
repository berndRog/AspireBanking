using System.Net;
using Microsoft.AspNetCore.Components;
namespace BankingClient.ErrorHandling;

public class ResponseErrors(
   NavigationManager navigationManager
) { 
   
   private string _message = string.Empty;
   private HttpStatusCode _statusCode = HttpStatusCode.OK;
   
   public async Task Handle<T>(
      HttpResponseMessage response,
      ILogger<T> logger
   ) {
      _message = await response.Content.ReadAsStringAsync(); 
      _statusCode = response.StatusCode;
      logger.LogError($"Http Status Code: {_statusCode} message: {_message}");
      /*
      switch(response.StatusCode) {
         case HttpStatusCode.BadRequest: navigationManager.NavigateTo("/400");
            break;
         case HttpStatusCode.Unauthorized: navigationManager.NavigateTo("/401");
            break;
         case HttpStatusCode.Forbidden: navigationManager.NavigateTo("/403");
            break;
         case HttpStatusCode.NotFound: navigationManager.NavigateTo("/404");
            break;
         case HttpStatusCode.MethodNotAllowed: navigationManager.NavigateTo("/405");
            break;
         case HttpStatusCode.Conflict: navigationManager.NavigateTo("/409");
            break;
         case HttpStatusCode.InternalServerError: navigationManager.NavigateTo("/500");
            break;
         default:
            throw new Exception($"Http Status Code: {response.StatusCode} message: {message}");
      }
      */
      navigationManager.NavigateTo($"/error/{_statusCode}/{_message}");;
   }
   
   public void Handle(
      Exception exception
   ) {
      var message = exception.Message; 
      navigationManager.NavigateTo($"/error/{message}");
   }

   private void ThrowError(HttpResponseMessage response) {
      throw new Exception($"Http Status Code: {response.StatusCode} message: Unknown Error");
   }
}