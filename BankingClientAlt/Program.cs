using BankingClient.ErrorHandling;
using BankingClient.Services;
using BankingClient.Services.Impl;
using BankingClient.Utils;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
namespace BankingClient;

public class Program {
   public static async Task Main(string[] args) {
      var builder = WebAssemblyHostBuilder.CreateDefault(args);
      builder.RootComponents.Add<App>("#app");
      builder.RootComponents.Add<HeadOutlet>("head::after");
      
      // Log application startup
      builder.Logging.SetMinimumLevel(LogLevel.Information);
      var logger = builder.Logging.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
      logger.LogInformation("Blazor WASM is starting...");
      builder.Logging.SetMinimumLevel(LogLevel.Debug);
     
      // Add services
      builder.Services.AddScoped<UserStateHolder>();
      builder.Services.AddScoped<ResponseErrors>();
      builder.Services.AddScoped<IOwnerService, OwnerService>();
      builder.Services.AddScoped<IAccountService, AccountService>();
      builder.Services.AddScoped<IBeneficiaryService, BeneficiaryService>();
      builder.Services.AddScoped<ITransferService, TransferService>();
      builder.Services.AddScoped<ImagesService, ImagesServiceImpl>();
      
      // Add an HttpClient for authorized calls, sniffing the network traffic
      builder.Services.AddTransient<LogNetworkTraffic>();
      
      // Configure HttpClient for OIDC authentication
      builder.Services.AddHttpClient("OidcAuthentication", client => 
            client.BaseAddress = new Uri("http://localhost:8080/realms/webtech"))
         .AddHttpMessageHandler<LogNetworkTraffic>();


      builder.Services.AddOidcAuthentication(options => {
         // Configure your authentication provider options here.
         // For more information, see https://aka.ms/blazor-standalone-auth
         // builder.Configuration.Bind("Local", options.ProviderOptions);
         options.ProviderOptions.Authority = "http://localhost:8080/realms/webtech";
         options.ProviderOptions.ClientId = "blazor-spa-code"; //"blazor-spa-implicit";
         options.ProviderOptions.ResponseType = "code"; //"id_token token";
         options.ProviderOptions.DefaultScopes.Add("email");

         options.ProviderOptions.RedirectUri =
            "http://localhost:5001/authentication/login-callback"; //"http://localhost:5001/authentication/login";
         options.ProviderOptions.PostLogoutRedirectUri = 
            "http://localhost:5001/authetication/logout-callback";

         // options.UserOptions.NameClaim = "name";
         // options.UserOptions.RoleClaim = "roles";
      });
      
      
      // builder.Services.AddAuthorizationCore(options => {
      //    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
      //    options.AddPolicy("User", policy => policy.RequireRole("User"));
      // });


      //builder.Services.AddSingleton<HandleImage>();
      builder.Services.AddSingleton<ResponseErrors>();
      
      var app = builder.Build();
      
      

      await app.RunAsync();
   }
}
