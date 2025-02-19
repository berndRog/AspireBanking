using System.Globalization;
using System.Text.Json;
using BankingClient.Core;
using BankingClient.Core.Dto;
using BankingClient.Services;
using BankingClient.Ui;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
namespace BankingClient;

public class Program {
   
   public static async Task Main(string[] args) {
   
      var builder = WebAssemblyHostBuilder.CreateDefault(args);
      builder.RootComponents.Add<App>("#app");
      builder.RootComponents.Add<HeadOutlet>("head::after");
      
      // Register Localization Service
      builder.Services.AddLocalization(opt => opt.ResourcesPath = "Resources");
      
      // Log application in the web browser console
      builder.Logging.SetMinimumLevel(LogLevel.Information);
      var logger = builder.Logging.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
      logger.LogInformation("Program: Setup Blazor WASM");

      builder.Services.AddSingleton<JsonSerializerOptions>(new JsonSerializerOptions {
         PropertyNamingPolicy = new LowerCaseNamingPolicy(),
         WriteIndented = true // Optional, depending on your needs
      });
      
      builder.Services.AddScoped<UserStateHolder>();
      
      // add Deafult HttpClient service
      builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
      
      // add CancellationTokenSource service to handle async problems
      builder.Services.AddScoped<CancellationTokenSource>();

      // Add OIDC Authetication and Authorization      
      builder.Services.AddOidcAuthentication(options => {
         // Configure rgw Keweycloak authentication provider options here.
         builder.Configuration.Bind("Keycloak", options.ProviderOptions);
         // Tell the OIDC handler that the role claim is named "roles"
         options.UserOptions.RoleClaim = "realm_access.roles";
      }); //.AddAccountClaimsPrincipalFactory<CustomUserFactory>();
      
      
      // Add Policy-based Authorization instead of using the role names directly
      // hardcoded role names are not recommended (e.g. "webtech-admin") 
      builder.Services.AddAuthorizationCore(options => {
         options.AddPolicy("AdminPolicy", policy =>
            policy.RequireRole("webtech-admin"));
         options.AddPolicy("UserPolicy", policy =>
            policy.RequireRole("webtech-user"));
         options.AddPolicy("CombinedPolicy", policy =>
            policy.RequireRole("webtech-admin", "webtech-user")
         );
      }); 
      
      
      // Configure HttpClient for BankingApi ans add bearer token automatically
      builder.Services.AddHttpClient("BankingApi", client => 
         client.BaseAddress = new Uri("http://localhost:5100/banking/v3/")
      ).AddHttpMessageHandler(sp => {
         // add AuthorizationMessageHandler
         return sp.GetRequiredService<AuthorizationMessageHandler>()
            .ConfigureHandler(
               // The URLs Blazor is allowed to attach tokens for
               authorizedUrls: new[] { "http://localhost:5100" },
               // The scopes your Banking API requires
               scopes: new[] { "openid", "profile" }
            );
      });
      
      // add the services for the BankinApi
      // WebServiceOptions<TService>
      builder.Services.AddScoped(WebServiceOptions<AccountService>.Create);
      builder.Services.AddScoped(WebServiceOptions<OwnerService>.Create);
      builder.Services.AddScoped(WebServiceOptions<BeneficiaryService>.Create);
      builder.Services.AddScoped(WebServiceOptions<TransactionService>.Create);
      builder.Services.AddScoped(WebServiceOptions<TransferService>.Create);
      // web services
      builder.Services.AddScoped<IAccountService, AccountService>();
      builder.Services.AddScoped<IOwnerService, OwnerService>();
      builder.Services.AddScoped<IBeneficiaryService, BeneficiaryService>();
      builder.Services.AddScoped<ITransactionService, TransactionService>();
      builder.Services.AddScoped<ITransferService, TransferService>();
      
      // Log all configuration values
      // foreach (var section in builder.Configuration.AsEnumerable()) {
      //    logger.LogInformation("{Key}: {Value}", section.Key, section.Value);
      // }
      logger.LogInformation("Blazor WASM is starting...");
      
      var app = builder.Build();
      
      // Set Default Culture
      var culture = new CultureInfo("de-DE");
      CultureInfo.DefaultThreadCurrentCulture = culture;
      CultureInfo.DefaultThreadCurrentUICulture = culture;
      
      await app.RunAsync();
   }
   
   public static object CreateWebServiceOptions(
      IServiceProvider serviceProvider,
      Type serviceType
   ) {
      var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
      var configuration = serviceProvider.GetRequiredService<IConfiguration>();
      var jsonOptions = serviceProvider.GetRequiredService<JsonSerializerOptions>();
      var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
   
      var genericType = typeof(WebServiceOptions<>).MakeGenericType(serviceType);
      var instance = Activator.CreateInstance(genericType,
         httpClientFactory,
         configuration,
         jsonOptions,
         new CancellationTokenSource(),
         loggerFactory.CreateLogger(serviceType)
      );
      return instance!;
   }
   
}

public class LowerCaseNamingPolicy : JsonNamingPolicy {
   public override string ConvertName(string name) =>
      name.ToLower();
}