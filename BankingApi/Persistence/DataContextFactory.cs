// using System;
// using System.IO;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Design;
// using Microsoft.Extensions.Configuration;
//
// namespace BankingApi.Persistence;
// public class DataContextFactory : IDesignTimeDbContextFactory<DbContext> {
//
//    public DbContext CreateDbContext(string[] args) {
//       // Nuget:  Microsoft.Extensions.Configuration
//       //       + Microsoft.Extensions.Configuration.Json
//       var configuration = new ConfigurationBuilder()
//                          .SetBasePath(Directory.GetCurrentDirectory())
//                          .AddJsonFile("appSettings.json", false)
//                          .Build();
//
//       var (useDatabase, dataSource) = 
//          DataContext.EvalDatabaseConfiguration(configuration);
//       
//       var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
//       switch (useDatabase) {
//          case "LocalDb":
//             Console.WriteLine("--> LocalDb");
//             Console.WriteLine($"--> ConnectionsString {dataSource}");
//             optionsBuilder.UseSqlServer(dataSource);
//             break;
//          case "SqlServer":
//             Console.WriteLine("--> SQLServer");
//             Console.WriteLine($"--> ConnectionsString {dataSource}");
//             optionsBuilder.UseSqlServer(dataSource);
//             break;
//          // https://zetbit.tech/categories/asp-dot-net-core/41/setup-mysql-connection-dot-net-7-asp-net-core
//          case "MariaDb":
//             // Console.WriteLine("--> MariaDb");
//             // Console.WriteLine($"--> ConnectionsString {dataSource}");
//             // var databaseVersion = MariaDbServerVersion.AutoDetect(dataSource);
//             // Console.WriteLine($"--> AutoDetect {databaseVersion}");
//             // var version = new MariaDbServerVersion(databaseVersion);
//             // Console.WriteLine("--> Version {version.Version}");
//             // optionsBuilder.UseMySql(dataSource, version);
//             break;
//          case "Postgres":
//             Console.WriteLine("--> Postgres latest");
//             Console.WriteLine($"--> ConnectionsString {dataSource}");
//             optionsBuilder.UseNpgsql(dataSource);
//             break;
//          case "Sqlite":
//             Console.WriteLine("--> Sqlite");
//             Console.WriteLine($"--> ConnectionsString {dataSource}");
//             optionsBuilder.UseSqlite(dataSource);
//             break;
//          default:
//             throw new Exception($"appsettings.json UseDatabase {useDatabase} not available");
//       }
//       return new DataContext(optionsBuilder.Options);
//    }
// }