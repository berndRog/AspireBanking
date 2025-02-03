namespace BankingClient.Utils;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

public class LogNetworkTraffic(
   ILogger<LogNetworkTraffic> logger
) : DelegatingHandler {

   protected override async Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request, 
      CancellationToken cancellationToken
  ) {
      logger.LogInformation("Request: {method} {url}", request.Method, request.RequestUri);

      if (request.Content != null) {
         var requestContent = await request.Content.ReadAsStringAsync();
         logger.LogInformation("Request Content: {content}", requestContent);
      }

      var response = await base.SendAsync(request, cancellationToken);

      logger.LogInformation("Response: {statusCode}", response.StatusCode);

      if (response.Content != null) {
         var responseContent = await response.Content.ReadAsStringAsync();
         logger.LogInformation("Response Content: {content}", responseContent);
      }

      return response;
   }
}