using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using BankingApi.Core.Misc;
using BankingApi.Di;
using BankingClient.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
namespace BankingApi;

public class Program {

   private static void Main(string[] args) {
      //
      // Configure API
      //
      WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

      // path for BankingApi images
      var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
      var wwwroot = Path.Combine(home, "Aspire_2025", "WebTech", "BankingApi", "root");
      if (!Directory.Exists(wwwroot)) Directory.CreateDirectory(wwwroot);
      builder.Environment.WebRootPath = wwwroot; // path to images;
      Debugger.Log(2, null, $"ASP.NET BankingAPI is starting...WebRootPath: {wwwroot}");

      // Add Logging Services to DI-Container
      builder.Logging.ClearProviders();
      builder.Logging.AddConsole();
      builder.Logging.AddDebug();
   
      // Configure DI-Container aka builder.Services:IServiceCollection
      // ---------------------------------------------------------------------
      // add http logging 
      builder.Services.AddHttpLogging(opts =>
         opts.LoggingFields = HttpLoggingFields.All);

      // add core
      builder.Services.AddCore();

      // add Persistence
      builder.Services.AddPersistence(builder.Configuration);

      // add Error handling
      builder.Services.AddProblemDetails();

      // add Keycloak Authentication and Authorization
      AddKeycloakAuthenticationAndAuthorization(builder);

      // add API versioning
      AddApiVersioning(builder);

      // add OpenApi/Swagger
      AddSwagger(builder);

      // limit imageFile max size
      builder.Services.Configure<FormOptions>(options => {
         options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10 MB limit
      });

      // add Cors 
      AddCorsPolicy(builder);

      // add controllers
      builder.Services.AddControllers()
         .AddJsonOptions(opt => {
            opt.JsonSerializerOptions.Converters.Add(new IsoDateTimeConverter());
            opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            opt.JsonSerializerOptions.PropertyNamingPolicy = new LowerCaseNamingPolicy();
         });


      //
      // Configure Http Pipeline
      //
      WebApplication app = builder.Build();

      // 1) Error handling (and developer exception page in dev)
      if (app.Environment.IsDevelopment())
         app.UseExceptionHandler("/error-development");
      else
         app.UseExceptionHandler("/error");

      // 2) OpenAPI/Swagger documentation
      if (app.Environment.IsDevelopment()) {
         UseSwaggerConfiguration(app);
      }

      // 3) Routing
      app.UseRouting();
      
      // 4) CORS
      app.UseCors("AllowSpecificOrigin");
      
      // 5) Authentication, Authorization
      app.UseAuthentication();
      app.UseAuthorization();
      
      // 6) Controllers
      app.MapControllers();
      app.Run();
   }

   /// <summary>
   /// Add ApiVersioning to the WebApplicationBuilder
   /// </summary>
   /// <param name="builder">WebApplicaionbuilder</param>
   private static void AddApiVersioning(
      WebApplicationBuilder builder
   ) {
      
#if DEBUG  
      Console.WriteLine($"....: AddApiVersioning: UrlSegment,Header");
#endif      
      var apiVersionReader = ApiVersionReader.Combine(
         new UrlSegmentApiVersionReader(),
         new HeaderApiVersionReader("x-api-version")
         // new MediaTypeApiVersionReader("x-api-version"),
         // new QueryStringApiVersionReader("api-version")
      );

      builder.Services.AddApiVersioning(opt => {
            opt.DefaultApiVersion = new ApiVersion(1, 0);
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.ReportApiVersions = true;
            opt.ApiVersionReader = apiVersionReader;
         })
         .AddMvc()
         .AddApiExplorer(options => {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
         });
   }
   
   /// <summary>
   /// Add Swagger to the WebApplicationBuilder
   /// </summary>
   /// <param name="builder">WebApplicationBuilder</param>
   private static void AddSwagger(
      WebApplicationBuilder builder
   ) {
#if DEBUG
      Console.WriteLine($"....: AddSwagger: SwaggerSecurity with JWT Bearer");
#endif
      
      builder.Services.AddSwaggerGen(opt => {
         var dir = new DirectoryInfo(AppContext.BaseDirectory);
         foreach (var file in dir.EnumerateFiles("*.xml")) {
            opt.IncludeXmlComments(file.FullName);
         }

         opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'"
         });

         opt.AddSecurityRequirement(new OpenApiSecurityRequirement {
            {
               new OpenApiSecurityScheme {
                  Reference = new OpenApiReference {
                     Type = ReferenceType.SecurityScheme,
                     Id = "Bearer"
                  }
               },
               new string[] {}
            }
         });
         
         // Add swagger document for every API version discovered
         var provider = builder.Services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
         foreach (var description in provider.ApiVersionDescriptions) {
            opt.SwaggerDoc(description.GroupName, new OpenApiInfo {
               Title = "BankingApi",
               Description = description.IsDeprecated ? 
                  "This API version has been deprecated." : 
                  "Prinzipbeispiel für ein Bankkonto",
               Version = description.ApiVersion.ToString(),
            });
         }
      });
      //builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
   }
   
   /// <summary>
   /// Add Keycloak Authentication and Authorization to the WebApplicationBuilder
   /// </summary>
   /// <param name="builder">WebApplicationBuilder</param>
   private static void AddKeycloakAuthenticationAndAuthorization(
      WebApplicationBuilder builder
   ) {
      // Read Keycloak settings from configuration
      var keycloakSettings = builder.Configuration.GetSection("Keycloak");

#if DEBUG
      Console.WriteLine($"....: Add Keycloak OIDC credentials code flow & Authorization");
      Console.WriteLine($"....: Keycloak:Authority: {keycloakSettings["Authority"]}");
      Console.WriteLine($"....: Keycloak:ClientId: {keycloakSettings["ClientId"]}");
      Console.WriteLine($"....: Keycloak:ClientSecret: {keycloakSettings["ClientSecret"]}");
#endif
      
      // 1. Add Authentication services
      builder.Services.AddAuthentication(options => {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
         })
         .AddJwtBearer(bearerOptions => {
            bearerOptions.RequireHttpsMetadata = false; // we are using HTTP
            bearerOptions.Authority = keycloakSettings["Authority"];
            bearerOptions.Audience = keycloakSettings["ClientId"];
            bearerOptions.TokenValidationParameters = new TokenValidationParameters {
               ValidateIssuer = true,
               ValidIssuer = keycloakSettings["Authority"],
               ValidateAudience = true,
               ValidAudiences = new[] { keycloakSettings["ClientId"] }, //i.e. webapi
               ValidateLifetime = true,
               RequireSignedTokens = true,
               ClockSkew = TimeSpan.Zero
            };

            // 2. Map the roles from Keycloak's token to .NET roles
            bearerOptions.Events = new JwtBearerEvents {
               OnTokenValidated = context => {
                  if (context.Principal?.Identity is ClaimsIdentity identity) {
                     // Extract roles from the ID token
                     // Extract roles from the realm_access field
                     var realmAccessClaim = identity.FindFirst("realm_access");
                     var realmAccessJson = realmAccessClaim?.Value;
                     if (realmAccessJson is not null) {
                        var roles = JsonDocument.Parse(realmAccessJson)
                           .RootElement
                           .GetProperty("roles")
                           .EnumerateArray()
                           .Select(r => r.GetString());
                        foreach (var role in roles) {
                           if (!string.IsNullOrWhiteSpace(role)) {
                              identity.AddClaim(new Claim(ClaimTypes.Role, role));
                              Console.WriteLine($"....: Keycloak:Roles: {role}");
                           }
                        }
                     }
                  }

                  if (context.SecurityToken is JwtSecurityToken jwtToken) {
                     var accessToken = jwtToken.RawData;
                     // Log the access token
#if DEBUG
                     Console.WriteLine($"....:Access Token: {accessToken}");
#endif
                  }

                  return Task.CompletedTask;
               }
            };
         });

      // 3. Add Authorization roles 
      builder.Services.AddAuthorizationBuilder()
         .AddPolicy("UserPolicy", policy => policy.RequireRole("webtech-user"))
         .AddPolicy("AdminPolicy", policy => policy.RequireRole("webtech-admin"));
   }
   
   /// <summary>
   /// Add Cors Policy to the WebApplicationBuilder
   /// </summary>
   /// <param name="builder"></param>
   private static void AddCorsPolicy(
      WebApplicationBuilder builder
   ) {
   //    builder.Services.AddCors(options => {
   //       options.AddDefaultPolicy(policy => {
   //          policy
   //             .WithOrigins("http://localhost:5001")
   //             .AllowAnyMethod()
   //             .AllowAnyHeader();
   //       });
   //    });

      builder.Services.AddCors(options => {
         options.AddPolicy("AllowSpecificOrigin",
            policy => policy
               .WithOrigins("http://localhost:5001")
               .AllowAnyHeader()
               .AllowAnyMethod());
      });
   
      // builder.Services.AddCors(options => {
      //    options.AddDefaultPolicy(policy => {
      //       policy
      //         .AllowAnyOrigin()
      //         .AllowAnyMethod()
      //         .AllowAnyHeader();
      //    });
      //});
   }
   
   /// <summary>
   /// Use Swagger Middleware
   /// </summary>
   /// <param name="app">WebApplication</param>
   private static void UseSwaggerConfiguration(
      WebApplication app
   ) {
      var provider =
         app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
      app.UseSwagger();
      app.UseSwaggerUI(options => {
         foreach (var description in provider.ApiVersionDescriptions) {
            options.SwaggerEndpoint(
               $"/swagger/{description.GroupName}/swagger.json",
               description.GroupName.ToUpperInvariant());
         }
      });
   }
}

public class LowerCaseNamingPolicy : JsonNamingPolicy {
   public override string ConvertName(string name) =>
      name.ToLower();
}


/*
 *
 *using System;
   using System.IO;
   using System.Linq;
   using System.Security.Claims;
   using System.Text;
   using System.Text.Json;
   using System.Text.Json.Serialization;
   using System.Threading.Tasks;
   using Asp.Versioning;
   using Asp.Versioning.ApiExplorer;
   using BankingApi.Core.Misc;
   using BankingApi.Di;
   using BankingClient.Utils;
   using Microsoft.AspNetCore.Authentication.JwtBearer;
   using Microsoft.AspNetCore.Builder;
   using Microsoft.AspNetCore.Http.Features;
   using Microsoft.AspNetCore.HttpLogging;
   using Microsoft.Extensions.Configuration;
   using Microsoft.Extensions.DependencyInjection;
   using Microsoft.Extensions.Hosting;
   using Microsoft.Extensions.Logging;
   using Microsoft.IdentityModel.Tokens;
   using Microsoft.OpenApi.Models;
   namespace BankingApi;
   
   /*
    builder.Services.AddCors siehe ExtensionMethods
    * /
   public class Program {
      
      private static void Main(string[] args) {
         // path for BankingApi images
         var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
         var wwwroot = Path.Combine(home, "Aspire_2025", "WebTech", "BankingApi", "root");
   
         //
         // Configure API
         //
         var builder = WebApplication.CreateBuilder(args);
   
         // path for PeopleApi root folder
         //var wwwroot = Path.Combine(directory, localFolder, "WebTech","BankingApi", "root");
         if (!Directory.Exists(wwwroot)) Directory.CreateDirectory(wwwroot);
         builder.Environment.WebRootPath = wwwroot; // path to images;
   
         // Add Logging Services to DI-Container
         builder.Logging.ClearProviders();
         builder.Logging.AddConsole();
         builder.Logging.AddDebug();
         var logger = builder.Logging.Services.BuildServiceProvider()
            .GetRequiredService<ILogger<Program>>();
         logger.LogInformation("ASP.NET BankingAPI is starting...WebRootPath: {1}", wwwroot);
   
         // Configure DI-Container aka builder.Services:IServiceCollection
         // ---------------------------------------------------------------------
         // add http logging 
         builder.Services.AddHttpLogging(opts =>
            opts.LoggingFields = HttpLoggingFields.All);
   
   
         // add core
         builder.Services.AddCore();
   
         // add Persistence
         builder.Services.AddPersistence(builder.Configuration);
   
         // add Error handling
         builder.Services.AddProblemDetails();
   
         // add Keycloak Authentication and Authorization
         AddKeycloakAuthenticationAndAuthorization(builder, logger);
   
         // add API versioning
         AddApiVersioning(builder, logger);
         // var apiVersionReader = ApiVersionReader.Combine(
         //    new UrlSegmentApiVersionReader(),
         //    new HeaderApiVersionReader("x-api-version")
         //    // new MediaTypeApiVersionReader("x-api-version"),
         //    // new QueryStringApiVersionReader("api-version")
         // );
         // builder.Services.AddApiVersioning(opt => {
         //       opt.DefaultApiVersion = new ApiVersion(1, 0);
         //       opt.AssumeDefaultVersionWhenUnspecified = true;
         //       opt.ReportApiVersions = true;
         //       //          opt.ApiVersionReader = new UrlSegmentApiVersionReader();
         //       opt.ApiVersionReader = apiVersionReader;
         //    })
         //    .AddMvc()
         //    .AddApiExplorer(options => {
         //       options.GroupNameFormat = "'v'VVV";
         //       options.SubstituteApiVersionInUrl = true;
         //    });
   
         // add OpenApi/Swagger
         AddSwagger(builder, logger);
         // builder.Services.AddSwaggerGen(opt => {
         //    var dir = new DirectoryInfo(AppContext.BaseDirectory);
         //    // combine WebApi.Controllers.xml and WebApi.Core.xml
         //    foreach (var file in dir.EnumerateFiles("*.xml")) {
         //       opt.IncludeXmlComments(file.FullName);
         //    }
         //    
         //    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
         //       Name = "Authorization",
         //       Type = SecuritySchemeType.Http,
         //       Scheme = "bearer",
         //       BearerFormat = "JWT",
         //       In = ParameterLocation.Header,
         //       Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'"
         //    });
         //
         //    opt.AddSecurityRequirement(new OpenApiSecurityRequirement { 
         //       {
         //          new OpenApiSecurityScheme {
         //             Reference = new OpenApiReference {
         //                Type = ReferenceType.SecurityScheme, 
         //                Id = "Bearer"
         //             }
         //          },
         //          new string[] {}
         //       }
         //    });
         // });
         // builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
   
         // limit imageFile max size
         builder.Services.Configure<FormOptions>(options => {
            options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10 MB limit
         });
   
         // add Cors 
         builder.Services.AddCors(options => {
            options.AddDefaultPolicy(policy => {
               policy
                  .WithOrigins(
                     "http://localhost:5001")
                  //"http://localhost:5002
                  .AllowAnyMethod()
                  .AllowAnyHeader();
            });
         });
   
         // add controllers
         builder.Services.AddControllers()
            .AddJsonOptions(opt => {
               opt.JsonSerializerOptions.Converters.Add(new IsoDateTimeConverter());
               opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
               opt.JsonSerializerOptions.PropertyNamingPolicy = new LowerCaseNamingPolicy();
            });
   
   
         var app = builder.Build();
   
         //
         // Configure Http Pipeline
         //
   
         // use middleware to handle errors
         if (app.Environment.IsDevelopment())
            app.UseExceptionHandler("/error-development");
         else
            app.UseExceptionHandler("/error");
   
         // API Versioning, OpenAPI/Swagger documentation
         var provider =
            app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
         
         
         if (app.Environment.IsDevelopment()) {
            app.UseSwagger();
            app.UseSwaggerUI(options => {
               foreach (var description in provider.ApiVersionDescriptions) {
                  options.SwaggerEndpoint(
                     $"/swagger/{description.GroupName}/swagger.json",
                     description.GroupName.ToUpperInvariant());
               }
            });
         }
         
         app.UseCors();
         
         app.UseAuthentication();
         app.UseAuthorization();
         
         app.UseRouting();
   
         app.MapControllers();
         app.Run();
      }
   
      private static void AddApiVersioning(
         WebApplicationBuilder builder,
         ILogger<Program> logger
      ) {
         
         logger.LogInformation("AddApiVersioning: {1}, {2}", "UrlSegment","Header");
         
         var apiVersionReader = ApiVersionReader.Combine(
            new UrlSegmentApiVersionReader(),
            new HeaderApiVersionReader("x-api-version")
            // new MediaTypeApiVersionReader("x-api-version"),
            // new QueryStringApiVersionReader("api-version")
         );
   
         builder.Services.AddApiVersioning(opt => {
               opt.DefaultApiVersion = new ApiVersion(1, 0);
               opt.AssumeDefaultVersionWhenUnspecified = true;
               opt.ReportApiVersions = true;
               opt.ApiVersionReader = apiVersionReader;
            })
            .AddMvc()
            .AddApiExplorer(options => {
               options.GroupNameFormat = "'v'VVV";
               options.SubstituteApiVersionInUrl = true;
            });
      }
      
      /// <summary>
      /// Add Swagger to the WebApplicationBuilder
      /// </summary>
      /// <param name="builder">WebApplicationBuilder</param>
      /// <param name="logger">Logger</param>
      private static void AddSwagger(
         WebApplicationBuilder builder,
         ILogger<Program> logger
      ) {
         logger.LogInformation("AddSwagger: {1}", "SwaggerSecurity with JWR Bearer");
         
         builder.Services.AddSwaggerGen(opt => {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            foreach (var file in dir.EnumerateFiles("*.xml")) {
               opt.IncludeXmlComments(file.FullName);
            }
   
            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
               Name = "Authorization",
               Type = SecuritySchemeType.Http,
               Scheme = "bearer",
               BearerFormat = "JWT",
               In = ParameterLocation.Header,
               Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'"
            });
   
            opt.AddSecurityRequirement(new OpenApiSecurityRequirement {
               {
                  new OpenApiSecurityScheme {
                     Reference = new OpenApiReference {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                     }
                  },
                  new string[] {}
               }
            });
            
            // Add swagger document for every API version discovered
            var provider = builder.Services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
            foreach (var description in provider.ApiVersionDescriptions) {
               opt.SwaggerDoc(description.GroupName, new OpenApiInfo {
                  Title = "BankingApi",
                  Description = description.IsDeprecated ? 
                     "This API version has been deprecated." : 
                     "Prinzipbeispiel für ein Bankkonto",
                  Version = description.ApiVersion.ToString(),
               });
            }
         });
         //builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
      }
      
      
      /// <summary>
      /// Add Keycloak Authentication and Authorization to the WebApplicationBuilder
      /// </summary>
      /// <param name="builder">WebApplicationBuilder</param>
      /// <param name="logger">Logger</param>
      private static void AddKeycloakAuthenticationAndAuthorization(
         WebApplicationBuilder builder,
         ILogger<Program> logger
      ) {
         // Read Keycloak settings from configuration
         var keycloakSettings = builder.Configuration.GetSection("Keycloak");
   
         logger.LogInformation("Keycloak:Authority: {1}",keycloakSettings["Authority"]);
         logger.LogInformation("Keycloak:ClientId: {1}", keycloakSettings["ClientId"]);
         logger.LogInformation("Keycloak:ClientSecret: {1}", keycloakSettings["ClientSecret"]);
         logger.LogInformation("Keycloak:Roles: {1}", keycloakSettings["Roles"]);
   
         
         // 1. Add Authentication services
         builder.Services.AddAuthentication(options => {
               options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
               options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(bearerOptions => {
               bearerOptions.RequireHttpsMetadata = false; // we are using HTTP
               bearerOptions.Authority = keycloakSettings["Authority"];
               bearerOptions.Audience = keycloakSettings["ClientId"];
               bearerOptions.TokenValidationParameters = new TokenValidationParameters {
                  ValidateAudience = true,
                  ValidAudiences = new[] { keycloakSettings["ClientId"] },
                  ValidateIssuer = true,
                  ValidIssuer = keycloakSettings["Authority"],
                  ValidateLifetime = true,
                  RequireSignedTokens = true,
                  ClockSkew = TimeSpan.Zero // Adjust if necessary
               };
   
               // 2. Map the realm roles from Keycloak's token to .NET roles
               //    Keycloak places roles under realm_access: { roles: [] }
               bearerOptions.Events = new JwtBearerEvents {
                  OnTokenValidated = context => {
                     if (context.Principal?.Identity is ClaimsIdentity identity) {
                        // Keycloak puts roles in realm_access -> roles array.
                        // Example: "realm_access": { "roles": ["webtech-admin", "webtech-user"] }
                        var realmAccessClaim = identity.FindFirst("realm_access");
                        var realmAccessJson = realmAccessClaim?.Value;
                        if (realmAccessJson is not null) {
                           var roles = JsonDocument.Parse(realmAccessJson)
                              .RootElement
                              .GetProperty("roles")
                              .EnumerateArray()
                              .Select(r => r.GetString());
                           foreach (var role in roles) {
                              if (!string.IsNullOrWhiteSpace(role)) {
                                 identity.AddClaim(new Claim(ClaimTypes.Role, role));
                                 logger.LogInformation("Keycloak:Roles: {1}", role);
                              }
                           }
                        }
                     }
                     return Task.CompletedTask;
                  }
               };
            });
   
         // 3. Add Authorization roles 
         builder.Services.AddAuthorizationBuilder()
            .AddPolicy("UserPolicy", policy => policy.RequireRole("webtech-user"))
            .AddPolicy("AdminPolicy", policy => policy.RequireRole("webtech-admin"));
      }
      
      /// <summary>
      /// Add Cors Policy to the WebApplicationBuilder
      /// </summary>
      /// <param name="builder"></param>
      private static void AddCorsPolicy(
         WebApplicationBuilder builder,
         ILogger<Program> logger
      ) {
         builder.Services.AddCors(options => {
            options.AddDefaultPolicy(policy => {
               policy
                  .WithOrigins("http://localhost:5001")
                  .AllowAnyMethod()
                  .AllowAnyHeader();
            });
         });
         
         //builder.Services.AddCors(options => {
         //   options.AddDefaultPolicy(policy => {
         //      policy
         //        .AllowAnyOrigin()
         //        .AllowAnyMethod()
         //        .AllowAnyHeader();
         //   });
         //});
      }
   }
   
   public class LowerCaseNamingPolicy : JsonNamingPolicy {
      public override string ConvertName(string name) =>
         name.ToLower();
   }
 *
 *
 * 
 */
