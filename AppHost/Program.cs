using Projects;
var builder = DistributedApplication.CreateBuilder(args);

// keycloak:
// image: quay.io/keycloak/keycloak:latest
// container_name: Keycloak
// command: start-dev
// environment:
// - KC_HEALTH_ENABLED=true
//    - KEYCLOAK_ADMIN=admin
//       - KEYCLOAK_ADMIN_PASSWORD=admin
// volumes:
// - ./.containers/identity:/opt/keycloak/data
// ports:
// - 18080:8080

var keycloak = builder.AddKeycloak(name: "keycloak", port: 8080)
   .WithBindMount(
      source: "/Users/rogallab/Aspire_2025/keycloak/data",
      target: "/opt/keycloak/data",
      isReadOnly: false
   )
   .WithExternalHttpEndpoints()
   .WithEnvironment("KEYCLOAK_ADMIN", "admin")
   .WithEnvironment("KEYCLOAK_ADMIN_PASSWORD", "geh1m_")
   .WithEnvironment("KEYCLOAK_FRONTEND_URL", "http://localhost:8080/auth") // Use HTTP during development

   .WithEnvironment("KEYCLOAK_LOGLEVEL", "DEBUG")
   .WithEnvironment("KEYCLOAK_LOG_CONSOLE_COLOR", "true")
   //.WithEnvironment("QUARKUS_HTTP_CORS", "true")
   //.WithEnvironment("QUARKUS_HTTP_CORS_ORIGINS", "http://localhost:5001")
   //.WithEnvironment("QUARKUS_HTTP_CORS_METHODS", "GET,POST,PUT,OPTIONS")
   //.WithEnvironment("QUARKUS_HTTP_CORS_HEADERS", "Accept,Authorization,Content-Type,Origin,X-Requested-With")
   //weg.WithEnvironment("QUARKUS_HTTP_CORS_EXPOSE_HEADERS", "Location,Content-Disposition")
   //.WithEnvironment("QUARKUS_HTTP_CORS_ALLOW_CREDENTIALS", "true")
   //.WithEnvironment("KC_HTTPS_ENFORCED", "false") // Optional: SSL not required for local dev
   //.WithEnvironment("KC_SPI_HEADERS_STRICT_TRANSPORT_SECURITY_MAX_AGE", "0")
   .WithEnvironment("KC_HEALTH_ENABLED", "true"); // Enable health checks

   var bankingApi = builder.AddProject<Projects.BankingApi>("BankingApi")
      .WaitFor(keycloak);

   // var bankingClient = builder.AddProject<Projects.BankingClient>("BankingClientSpa")
   //     .WaitFor(bankingApi);


builder.Build().Run();