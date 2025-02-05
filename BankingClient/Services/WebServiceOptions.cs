using System.Text.Json;

public class WebServiceOptions<TService> where TService : class {
   public required IHttpClientFactory HttpClientFactory { get; init; }
   public required IConfiguration Configuration { get; init; }
   public required JsonSerializerOptions JsonOptions { get; init; }
   public required CancellationTokenSource CancellationTokenSource { get; init; }
   public required ILogger<TService> Logger { get; init; }

   // Static method to create WebServiceOptions<TService>
   public static WebServiceOptions<TService> Create(IServiceProvider provider) {
      return new WebServiceOptions<TService> {
         HttpClientFactory = provider.GetRequiredService<IHttpClientFactory>(),
         Configuration = provider.GetRequiredService<IConfiguration>(),
         JsonOptions = provider.GetRequiredService<JsonSerializerOptions>(),
         CancellationTokenSource = new CancellationTokenSource(),
         Logger = provider.GetRequiredService<ILogger<TService>>()
      };
   }
}