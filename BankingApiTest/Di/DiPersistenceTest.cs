using System;
using System.IO;
using BankingApi.Core;
using BankingApi.Di;
using BankingApi.Persistence;
using BankingApi.Persistence.Repositories;
using BankingApiTest.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
namespace BankingApiTest.Di;

public static class DiTestPersistence {

   public static void AddPersistenceTest(
      this IServiceCollection services
   ) {

      // Configuration
      // Nuget:  Microsoft.Extensions.Configuration
      //       + Microsoft.Extensions.Configuration.Json
      var configuration = new ConfigurationBuilder()
         .SetBasePath(Directory.GetCurrentDirectory())
         .AddJsonFile("appsettingsTest.json", false)
         .Build();
      services.AddSingleton<IConfiguration>(configuration);
      
      // Logging
      // Nuget:  Microsoft.Extensions.Logging
      //       + Microsoft.Extensions.Logging.Configuration
      //       + Microsoft.Extensions.Logging.Debug
      var logging = configuration.GetSection("Logging");
      services.AddLogging(builder => {
         builder.ClearProviders();
         builder.AddConfiguration(logging);
         builder.AddDebug();
      });
      
      // UseCases, Mapper ...
      services.AddCore();
      
      // Repository, Database ...
      services.AddSingleton<IOwnersRepository, OwnersRepository>();
      services.AddSingleton<IAccountsRepository, AccountsRepository>();      
      services.AddSingleton<IBeneficiariesRepository, BeneficiariesRepository>();     
      services.AddSingleton<ITransfersRepository, TransfersRepository>();
      services.AddSingleton<ITransactionsRepository, TransactionsRepository>();
     
      services.AddSingleton<IPeopleRepository, PeopleRepository>();

      
      services.AddSingleton<IDataContext, DataContext>();
      
      // Add DbContext (Database) to DI-Container
      var (useDatabase, dataSource) = DataContext.EvalDatabaseConfiguration(configuration);
      
      switch (useDatabase) {
         case "LocalDb":
         case "SqlServer":
            services.AddDbContext<IDataContext, DataContext>(options => 
               options.UseSqlServer(dataSource)
            );
            break;
         case "MariaDb":
//          var databaseVersion = MariaDbServerVersion.AutoDetect(dataSource)
//          var version = new MariaDbServerVersion(databaseVersion);
            // services.AddDbContext<IDataContext, DataContext>(options => 
            //    options.UseMySql(dataSource, version)
            // );
            break;
         case "Postgres":
            services.AddDbContext<IDataContext, DataContext>((_, options) =>
                  options.UseNpgsql(dataSource) //, b => b.UseNodaTime())
            );
            break;
         case "Sqlite":
            services.AddDbContext<IDataContext, DataContext>(options => 
               options.UseSqlite(dataSource)
            );
            break;
         default:
            throw new Exception("appsettings.json UseDatabase not available");
      }
      services.AddScoped<ArrangeTest>();
      services.AddScoped<Seed>();
   }
}