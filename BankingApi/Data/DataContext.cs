using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BankingApi.Core;
using BankingApi.Core.DomainModel.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
[assembly: InternalsVisibleTo("BankingApiTest")]
namespace BankingApi.Data; 
internal class DataContext(
   DbContextOptions<DataContext> options
) : DbContext(options), IDataContext {

   #region fields
   private ILogger<DataContext>? _logger;
   #endregion
   
   #region properties
   // Note that DbContext caches the instance of DbSet returned from the
   // Set method so that each of these properties will return the same
   // instance every time it is called.
   public DbSet<Owner> Owners => Set<Owner>(); // call to a method, not a field 
   public DbSet<Account> Accounts => Set<Account>();
   public DbSet<Beneficiary> Beneficiaries => Set<Beneficiary>();
   public DbSet<Transfer> Transfers => Set<Transfer>();
   public DbSet<Transaction> Transactions => Set<Transaction>();
   #endregion
   
   #region methods
   public async Task<bool> SaveAllChangesAsync() {
      
      // log repositories before transfer to the database
      _logger?.LogInformation("\n{output}",ChangeTracker.DebugView.LongView);
      
      // save all changes to the database, returns the number of rows affected
      var result = await SaveChangesAsync();
      
      // log repositories after transfer to the database
      _logger?.LogInformation("SaveChanges {result}",result);
      _logger?.LogInformation("\n{output}",ChangeTracker.DebugView.LongView);
      return result > 0;
   }
   
   public void ClearChangeTracker() =>
      ChangeTracker.Clear();

   public void LogChangeTracker(string text) =>
      _logger?.LogInformation("{Text}\n{Tracker}", text, ChangeTracker.DebugView.LongView);
   #endregion
   
   #region override
   protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
      var loggerFactory = LoggerFactory.Create(builder => {
         builder.AddDebug();
      });
      _logger = loggerFactory.CreateLogger<DataContext>();
      
      if (!optionsBuilder.IsConfigured) {
         // Configure logging
         optionsBuilder
            .UseLoggerFactory(loggerFactory)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
      }
      
      loggerFactory.Dispose();
      
      optionsBuilder.EnableSensitiveDataLogging(false);
      
   }
   
   protected override void OnModelCreating(ModelBuilder modelBuilder) {
      base.OnModelCreating(modelBuilder);
      
      //
      // PROPERTY CONFIGURATION
      //
      
      // UTC for DateTime
      modelBuilder.Entity<Transfer>(e => {
         e.Property(e => e.Date)
            .HasConversion(
               v => v, // to UTC before saving
               v => DateTime.SpecifyKind(v, DateTimeKind.Utc)); // set as UTC when loading
      });
      modelBuilder.Entity<Transaction>(entity => {
         entity.Property(e => e.Date)
            .HasConversion(
               v => v, // to UTC before saving
               v => DateTime.SpecifyKind(v, DateTimeKind.Utc)); // set as UTC when loading
      });
/*      
      // Owners
      modelBuilder.Entity<Owner>(e => {
         e.ToTable("Owners");          // tablename Owners
         e.HasKey(owner => owner.Id);  // primary key
         e.Property(owner => owner.Id) // primary key has type Guid
            .ValueGeneratedNever();         // and should never be gerated by DB  
         e.Property(owner => owner.FirstName).HasMaxLength(64);
         e.Property(owner => owner.Email).HasMaxLength(128);
      });
      // Accounts
      modelBuilder.Entity<Account>(e => {
         e.ToTable("Accounts");
         e.HasKey(account => account.Id);
         e.Property(account => account.Id).ValueGeneratedNever();
         e.Property(account => account.Iban).HasMaxLength(32);
      });
      // Beneficiaries
      modelBuilder.Entity<Beneficiary>(e => {
         e.ToTable("Beneficiaries");
         e.HasKey(beneficiary => beneficiary.Id);
         e.Property(beneficiary => beneficiary.Id).ValueGeneratedNever();
         e.Property(beneficiary => beneficiary.FirstName).HasMaxLength(64);
         e.Property(beneficiary => beneficiary.Iban).HasMaxLength(32);
      });
      // Transfers
      modelBuilder.Entity<Transfer>(e => {
         e.ToTable("Transfers");
         e.HasKey(transfer => transfer.Id);
         e.Property(transfer => transfer.Id).ValueGeneratedNever();
      });
      // Transactions
      modelBuilder.Entity<Transaction>(e => {
         e.ToTable("Transactions");
         e.HasKey(transaction => transaction.Id);
         e.Property(transaction => transaction.Id).ValueGeneratedNever();
      });
      
      //
      // RELATIONS
      //
      // Owner [1] <--> [0..*] Account One-to-Many
      // -------------------------------------------
      // Either
      // modelBuilder.Entity<Owner>(e => {
      //    e.HasMany(owner => owner.Accounts)     // Owner   --> Account [0..*]
      //       .WithOne(account => account.Owner)  // Account --> Owner   [1]
      //       .HasForeignKey(account => account.OwnerId) // Fk in Account
      //       .HasPrincipalKey(owner => owner.Id) // Pk in Owner
      //       .OnDelete(DeleteBehavior.Cascade)
      //       .IsRequired();
      //    e.Navigation(onwer => onwer.Accounts); // Navigation property
      // });
      //Or
      modelBuilder.Entity<Account>(e => {
         e.HasOne(account => account.Owner)      // Account --> Owner   [1]
            .WithMany(owner => owner.Accounts)   // Owner   --> Account [0..*]
            .HasForeignKey(account => account.OwnerId) // Fk in Account
            .HasPrincipalKey(owner => owner.Id)        // Pk in Owner
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
         e.Navigation(account => account.Owner); // Navigation property
      });
      //
      // Account [1] <--> [0..*] Beneficiaries  One-To-Many
      // ---------------------------------------------------
      // Either 
      // modelBuilder.Entity<Account>(e => {
      //    e.HasMany(account => account.Beneficiaries) // Account    --> Beneficiary [0..*]
      //       .WithOne()                               // Beneficiary --> Account not modelled 
      //       .HasForeignKey(beneficiary => beneficiary.AccountId) // Fk in Beneficiary
      //       .HasPrincipalKey(account => account.Id)              // Pk in Account
      //       .OnDelete(DeleteBehavior.Cascade)
      //       .IsRequired();
      //    e.Navigation(account => account.Beneficiaries); // Navigation property
      // });
      // Or
      modelBuilder.Entity<Beneficiary>(e => {
         e.HasOne<Account>(account => null)             // Beneficiary --> Account not modelled            
            .WithMany(account => account.Beneficiaries) // Account     --> Beneficiary [0..*]
            .HasForeignKey(beneficiary => beneficiary.AccountId) // Fk in Beneficiary
            .HasPrincipalKey(account => account.Id)              // Pk in Account
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
         //e.Navigation(beneficiary => beneficiary.Account); // no Navigation property
      });
      //
      // Account [1] <--> [0..*] Transfers One-to-Many
      // ----------------------------------------------  
      // Either
      // modelBuilder.Entity<Account>(e => {
      //    e.HasMany(account => account.Transfers)   // Account    --> Transfers [0..*]
      //       .WithOne(transfer => transfer.Account) // Transfer   --> Account   [1]
      //       .HasForeignKey(transfer => transfer.AccountId) // Fk in Transfer
      //       .HasPrincipalKey(account => account.Id)        // Pk in Account
      //       .OnDelete(DeleteBehavior.Cascade)
      //       .IsRequired();
      //    e.Navigation(account => account.Transfers); // Navigation property
      // });
      // Or
      modelBuilder.Entity<Transfer>(e => {
         e.HasOne(transfer => transfer.Account)     // Transfer --> Account [1]
            .WithMany(account => account.Transfers) // Account --> Transfer [0..*]
            .HasForeignKey(transfer => transfer.AccountId) // Fk in Transfer
            .HasPrincipalKey(account => account.Id)        // Pk in Account
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(true);
         e.Navigation(transfer => transfer.Account); // Navigation property
      });
      //
      // Account [1] <--> [0..*] Transactions One-To-Many
      // ------------------------------------------------
      // Either
      // modelBuilder.Entity<Account>(e => {
      //    e.HasMany(account => account.Transactions)
      //       .WithOne(transaction => transaction.Account)
      //       .HasForeignKey(transaction => transaction.AccountId)
      //       .HasPrincipalKey(account => account.Id)
      //       .OnDelete(DeleteBehavior.Cascade)
      //       .IsRequired();
      //    e.Navigation(account => account.Transactions);
      // });
      // Or 
      modelBuilder.Entity<Transaction>(e => {
         e.HasOne(transaction => transaction.Account) // Transaction --> Account [1]
            .WithMany(account => account.Transactions)
            .HasForeignKey(transaction => transaction.AccountId)
            .HasPrincipalKey(account => account.Id)
            .IsRequired(false);
         e.Navigation(transaction => transaction.Account);
      });
      //
      // Transfer [--] <--> [0..1] Beneficiary
      // ------------------------------------
      modelBuilder.Entity<Transfer>(e => {
         e.HasOne(transfer => transfer.Beneficiary) // Transfer    --> Beneficiary [0..1]
            .WithMany()                             // Beneficiary --> Transfer [--]
            .HasForeignKey(transfer => transfer.BeneficiaryId) // Fk in Transfer
            .HasPrincipalKey(beneficiary => beneficiary.Id)    // Pk in Beneficiary 
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);
         e.Navigation(transfer => transfer.Beneficiary);
      });
      //
      // Transaction [0..*] <--> [0..1] Transfer
      // ---------------------------------------
      modelBuilder.Entity<Transaction>(e => {
         e.HasOne(transaction => transaction.Transfer)
            .WithMany(transfer => transfer.Transactions)
            .HasForeignKey(transaction => transaction.TransferId)
            .HasPrincipalKey(transfer => transfer.Id)
            .IsRequired(false)
            .HasPrincipalKey(transfer => transfer.Id)
            .OnDelete(DeleteBehavior.NoAction);
         e.Navigation(transaction => transaction.Transfer);
      });
*/
   }
   #endregion
   
   #region static methods
// "UseDatabase": "Sqlite",
// "ConnectionStrings": {
//    "LocalDb": "WebApi04",
//    "SqlServer": "Server=localhost,2433; Database=WebApi04; User=sa; Password=P@ssword_geh1m;",
//    "Sqlite": "WebApi04"
// },
   public static (string useDatabase, string dataSource) EvalDatabaseConfiguration(
      IConfiguration configuration
   ) {

      var useDatabase = configuration.GetSection("UseDatabase").Value ??
         throw new Exception("UseDatabase is not available");

      // read connection string from appsettings.json
      var connectionString = configuration.GetSection("ConnectionStrings")[useDatabase]
         ?? throw new Exception("ConnectionStrings is not available"); 
      
      // /users/documents/WebApi
      var localFolder = configuration.GetSection("LocalFolder").Value ??
         throw new Exception("LocalFolder is not available");
      var directory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
      var pathDocuments = Path.Combine(directory, localFolder, "WebTech", "BankingApi");
      
      Directory.CreateDirectory(pathDocuments);
      
      switch (useDatabase) {
         case "LocalDb":
            var dbFile = $"{Path.Combine(pathDocuments, connectionString)}.mdf";
            var dataSourceLocalDb =
               $"Data Source = (LocalDB)\\MSSQLLocalDB; " +
               $"Initial Catalog = {connectionString}; Integrated Security = True; " +
               $"AttachDbFileName = {dbFile};";
            Console.WriteLine($"....: EvalDatabaseConfiguration: LocalDb {dataSourceLocalDb}");
            Console.WriteLine();
            return (useDatabase, dataSourceLocalDb);

         case "SqlServer":
            return (useDatabase, connectionString);

         case "Sqlite":
            var dataSourceSqlite =
               "Data Source=" + Path.Combine(pathDocuments, connectionString) + ".db";
            Console.WriteLine($"....: EvalDatabaseConfiguration: Sqlite {dataSourceSqlite}");
            Console.WriteLine();

            return (useDatabase, dataSourceSqlite);
         default:
            throw new Exception("appsettings.json Problems with database configuration");
      }
   }
   #endregion
   
}