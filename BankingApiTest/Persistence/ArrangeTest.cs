using System;
using System.Threading.Tasks;
using BankingApi.Core;
using BankingApi.Core.DomainModel.Entities;

namespace BankingApiTest.Persistence;
public class ArrangeTest(
   IOwnersRepository ownersRepository,
   IAccountsRepository accountsRepository,
   IBeneficiariesRepository beneficiariesRepository,
   ITransfersRepository transfersRepository,
   ITransactionsRepository transactionsRepository,
   IDataContext dataContext,
   IUseCasesTransfer useCasesTransfer
) {
   
   public async Task OwnerWith1AccountAsync(Seed seed){
      // Arrange
      // Owner owner = new() {
      //    Id = new Guid("10000000-0000-0000-0000-000000000000"),
      //    FirstName = "Erika Mustermann",
      //    Birthdate = new DateTime(1988, 2, 1).ToUtcDateTime(),
      //    Email = "erika.mustermann@t-online.de"
      // }; // _seed.Owner1
      // Account account = new() {
      //    Id = new Guid("01000000-0000-0000-0000-000000000000"),
      //    Iban = "DE10 10000000 0000000001",
      //    Balance = 2100.0
      // }; // _seed.Account1
      // owner.Add(account); // DomainModel
      // accountsRepository.Add(account);
      // await dataContext.SaveAllChangesAsync();
      seed.Owner1.Add(seed.Account1);
      accountsRepository.Add(seed.Account1);
      await dataContext.SaveAllChangesAsync();
      dataContext.ClearChangeTracker();
   }

   public async Task OwnerWith2AccountAsync(Seed seed){
      seed.Owner1.Add(seed.Account1);
      seed.Owner1.Add(seed.Account2);
      ownersRepository.Add(seed.Owner1);
      accountsRepository.Add(seed.Account1);
      accountsRepository.Add(seed.Account2);
      await dataContext.SaveAllChangesAsync();
   }

   public async Task OwnersAsync(Seed seed){
      ownersRepository.AddRange(seed.Owners);
      await dataContext.SaveAllChangesAsync();
      dataContext.ClearChangeTracker();
   }

   public async Task Owner1WithAccountsAsync(Seed seed) {
      seed.InitAccountsForOwner1();
      //ownersRepository.Add(seed.Owner1);
      accountsRepository.Add(seed.Account1);
      accountsRepository.Add(seed.Account2);
      await dataContext.SaveAllChangesAsync();
      dataContext.ClearChangeTracker();
   }

   public async Task Owner1WithAccountsAndBenficiariesAsync(Seed seed) {
      seed.InitAccountsForOwner1();
      seed.InitBeneficiariesForAccoutsOfOwner1();
      ownersRepository.Add(seed.Owner1);
      accountsRepository.Add(seed.Account1);
      accountsRepository.Add(seed.Account2);
      beneficiariesRepository.Add(seed.Beneficiary1);
      beneficiariesRepository.Add(seed.Beneficiary2);
      beneficiariesRepository.Add(seed.Beneficiary3);
      beneficiariesRepository.Add(seed.Beneficiary4);
      await dataContext.SaveAllChangesAsync();
      dataContext.ClearChangeTracker();
   }
   
   public async Task OwnersWithAccountsAsync(Seed seed){
      seed.InitAccounts();
      ownersRepository.AddRange(seed.Owners);  
      accountsRepository.AddRange(seed.Accounts);
      dataContext.LogChangeTracker("OwnersWithAccountsAsync()");
      await dataContext.SaveAllChangesAsync();
      dataContext.ClearChangeTracker();
   }
   
   public async Task OwnersWithAccountsAndBeneficiariesAsync(Seed seed){
      seed.InitAccounts();
      seed.InitBeneficiaries();

      ownersRepository.AddRange(seed.Owners);
      accountsRepository.AddRange(seed.Accounts);
      beneficiariesRepository.AddRange(seed.Beneficiaries);
      await dataContext.SaveAllChangesAsync();
      dataContext.ClearChangeTracker();
   }
   
   public async Task PrepareTest1(Seed seed){
      
      // add owner1, owner5 to ownersRepository
      ownersRepository.Add(seed.Owner1);
      ownersRepository.Add(seed.Owner5);

      // add account1, account6 to accountsRepository
      seed.Owner1.Add(seed.Account1);
      seed.Owner5.Add(seed.Account6);
      accountsRepository.Add(seed.Account1);
      accountsRepository.Add(seed.Account6);

      // add beneficiary1 to beneficiariesRepository
      seed.Account1.Add(seed.Beneficiary1);
      beneficiariesRepository.Add(seed.Beneficiary1);

      // save to database
      await dataContext.SaveAllChangesAsync();
      dataContext.ClearChangeTracker();
   }
   
   public async Task SendMoneyTest1(Seed seed){
      ownersRepository.Add(seed.Owner1);
      ownersRepository.Add(seed.Owner5);

      seed.Owner1.Add(seed.Account1);
      seed.Owner5.Add(seed.Account6);
      accountsRepository.Add(seed.Account1);
      accountsRepository.Add(seed.Account6);

      seed.Account1.Add(seed.Beneficiary1);
      beneficiariesRepository.Add(seed.Beneficiary1);
      await dataContext.SaveAllChangesAsync();
      
      seed.Transfer1.Account = seed.Account1;
      seed.Transfer1.AccountId = seed.Account1.Id;
      seed.Transfer1.Beneficiary = seed.Beneficiary1;
      seed.Transfer1.BeneficiaryId = seed.Beneficiary1.Id;

      var result = await useCasesTransfer.SendMoneyAsync(
         accountDebitId:   seed.Account1.Id,
         transfer:         seed.Transfer1
      ); 
      if(result is Error<Transfer>) throw new Exception(result.Message);
      dataContext.ClearChangeTracker();
   }

   public async Task ExampleAsync(Seed seed){
      ownersRepository.AddRange(seed.Owners);
      await dataContext.SaveAllChangesAsync();

      seed.InitAccounts();
      accountsRepository.AddRange(seed.Accounts);
      await dataContext.SaveAllChangesAsync();
//    ShowRepository("InitAccounts");
      
      seed.InitBeneficiaries();
      beneficiariesRepository.AddRange(seed.Beneficiaries);
      await dataContext.SaveAllChangesAsync();
//    ShowRepository("InitBeneficiaries");
      
      seed.InitTransfersTransactions();
      transfersRepository.AddRange(seed.Transfers);
      transactionsRepository.AddRange(seed.Transactions);
      await dataContext.SaveAllChangesAsync();
//    ShowRepository("InitTransfersTransactions");
      dataContext.ClearChangeTracker();      
   }
}