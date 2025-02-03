using System;
using BankingApi.Core;
using BankingApi.Core.DomainModel;
using BankingApi.Persistence;
using BankingApi.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace BankingApi.Di;

public static class DiPersistence {
   public static void AddPersistence(
      this IServiceCollection services,
      IConfiguration configuration,
      bool isTest = false
   ){

      services.AddScoped<IOwnersRepository, OwnersRepository>();
      services.AddScoped<IAccountsRepository, AccountsRepository>();
      services.AddScoped<IBeneficiariesRepository, BeneficiariesRepository>();
      services.AddScoped<ITransfersRepository, TransfersRepository>();
      services.AddScoped<ITransactionsRepository, TransactionsRepository>();
      services.AddScoped<ImagesRepository, ImagesRepositoryImpl>();
      
      services.AddScoped<IPeopleRepository, PeopleRepository>();
      
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
            // services.AddDbContext<CDbContext>(options => 
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
   }
}