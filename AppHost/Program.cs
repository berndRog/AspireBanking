using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
Console.WriteLine($"UserHome: {homeDirectory}");

var configuration = builder.Configuration;
string localFolder = configuration["LocalFolder"] 
   ?? throw new Exception("LocalFolder is not set in appsettings.json");

// Keycloak -----------------------------------------------------------------------------
var keycloakConfig = configuration.GetSection("Keycloak");
int port = keycloakConfig.GetValue<int>("Port");
string mountSource = keycloakConfig["BindMountSource"] 
   ?? throw new Exception("BindMountSource is not set in appsettings.json");
string keycloakSource = Path.Combine(homeDirectory, localFolder, mountSource);
string admin = keycloakConfig["Admin"] 
   ?? throw new Exception("Admin is not set in appsettings.json");
string adminPassword = keycloakConfig["AdminPassword"] 
   ?? throw new Exception("AdminPassword is not set in appsettings.json");
string frontendUrl = keycloakConfig["FrontendUrl"] 
   ?? throw new Exception("FrontendUrl is not set in appsettings.json");
Console.WriteLine($"Keycloak: http://localhost:{port}");
Console.WriteLine($"Keycloak: bindMoutSource={keycloakSource}");
Console.WriteLine($"Keycloak: admin={admin}, adminPassword={adminPassword})");
Console.WriteLine($"Keycloak: frontendUrl={frontendUrl} to start Keycloak console");

var keycloak = builder.AddKeycloak(name: "keycloak", port: port)
   .WithBindMount(
      source: keycloakSource, //"/Users/rogallab/Aspire_2025/keycloak/data",
      target: "/opt/keycloak/data",
      isReadOnly: false
   )
   .WithExternalHttpEndpoints() // Expose HTTP endpoints outside the container
   .WithEnvironment("KEYCLOAK_ADMIN", admin)
   .WithEnvironment("KEYCLOAK_ADMIN_PASSWORD", adminPassword)
   .WithEnvironment("KEYCLOAK_FRONTEND_URL", frontendUrl) // to access keyclaok console
   .WithEnvironment("KEYCLOAK_LOGLEVEL", "DEBUG")
   .WithEnvironment("KEYCLOAK_LOG_CONSOLE_COLOR", "true")
   .WithEnvironment("KC_HEALTH_ENABLED", "true"); // Enable health checks

/*
string sqlServerSource = Path.Combine(homeDirectory, localFolder, @"Webtech/BankingApi_SqlServer/Data");

Console.WriteLine($"SqlServer: bindMoutSource={sqlServerSource}");

var sqlserver = builder.AddSqlServer(name: "BankingDb", port: 1433)
   .WithLifetime(ContainerLifetime.Persistent)
   .WithDataBindMount(source: sqlServerSource)
   .WithEnvironment("ACCEPT_EULA", "Y")
   .WithEnvironment("MSSQL_SA_PASSWORD", "P@ssword_geh1m")
   .WithEnvironment("SA_PASSWORD", "P@ssword_geh1m");
//sqlserver.AddDatabase("BankingApiV3");
*/

// BankingApi -----------------------------------------------------------------------------
var bankingApi = builder.AddProject<Projects.BankingApi>(name: "BankingApi")
   .WaitFor(keycloak);
// .WaitFor(sqlserver);

// var bankingClient = builder.AddProject<Projects.BankingClient>("BankingClientSpa")
//      .WaitFor(bankingApi);


builder.Build().Run();