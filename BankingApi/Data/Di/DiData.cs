using System;
using BankingApi.Core;
using BankingApi.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace BankingApi.Data.Di;

public static class DiData {
   public static void AddDatabase(
      this IServiceCollection services,
      IConfiguration configuration,
      bool isTest = false
   ){
      services.AddScoped<IOwnersRepository, OwnersRepository>();
      services.AddScoped<IAccountsRepository, AccountsRepository>();
      services.AddScoped<IBeneficiariesRepository, BeneficiariesRepository>();
      services.AddScoped<ITransfersRepository, TransfersRepository>();
      services.AddScoped<ITransactionsRepository, TransactionsRepository>();
      
      // Add DbContext (Database) to DI-Container
      var (useDatabase, dataSource) = DataContext.EvalDatabaseConfiguration(configuration);
      
      switch (useDatabase) {
         case "LocalDb":
         case "SqlServer":
            services.AddDbContext<IDataContext, DataContext>(options => 
               options.UseSqlServer(dataSource)
            );
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