using System;
using System.Threading.Tasks;
using BankingApi.Core.DomainModel.Entities;
using BankingApi.Core.Misc;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Xunit;
namespace BankingApiTest.Persistence.Repositories;
[Collection(nameof(SystemTestCollectionDefinition))]
public class AccountsRepositoryUt: BaseRepositoryUt {
   
   #region Owners <-> Accounts   
   private EquivalencyAssertionOptions<Account> ExcludeReferences(
      EquivalencyAssertionOptions<Account> options
   ){
      options.Excluding(account => account.Owner);
      options.Excluding(account => account.Beneficiaries);
      options.Excluding(account => account.Transfers);
      options.Excluding(account => account.Transactions);
      return options;
   }

   [Fact]
   public async Task AddUt() {
      // Arrange
      Owner owner = new(){
         Id = new Guid("10000000-0000-0000-0000-000000000000"),
         FirstName = "Erika Mustermann",
         Email = "erika.mustermann@t-online.de"
      };  
      Account account = new(){
         Id = new Guid("01000000-0000-0000-0000-000000000000"),
         Iban = "DE10 10000000 0000000001",
         Balance = 2100.0
      };
      // Act 
      owner.Add(account);        // DomainModel
      _accountsRepository.Add(account);
      await _dataContext.SaveAllChangesAsync();
      // Assert
      _dataContext.ClearChangeTracker(); // clear repository cache
      var actual = await _accountsRepository.FindByIdAsync(account.Id);
      actual.Should()
         .NotBeNull().And
         .BeEquivalentTo(account, options => options.Excluding(a => a.Owner));
   }
   
   [Fact]
   public async Task FindByIdUt() {
      // Arrange Owner1 with Account 1
      await _arrangeTest.OwnerWith1AccountAsync(_seed);  // repository cache is cleared
      // Act 
      // no join operation -> i.e. no references loaded from database
      var actual = await _accountsRepository.FindByIdAsync(_seed.Account1.Id);
      // Assert
      _dataContext.LogChangeTracker("FindbyId");
      actual.Should().BeEquivalentTo(_seed.Account1, options => options.Excluding(a => a.Owner));
   }
   [Fact]
   public async Task FindByAsynUt() { 
      // Arrange
      await _arrangeTest.OwnersWithAccountsAsync(_seed); // repository cache is cleared
      // Act 
      var actual =  
         await _accountsRepository.FindByAsync(o => o.Iban.Contains("DE201000"));
      // Assert
      _dataContext.LogChangeTracker("FindbyIban");
      actual.Should().BeEquivalentTo(_seed.Account3, options => options.Excluding(a => a.Owner));
   }
   #endregion
   
   #region Owners <-> Accounts <-> Beneficiaries + Transfers/Transactions
   private EquivalencyAssertionOptions<Account> ExcludeCyclicReferences(
      EquivalencyAssertionOptions<Account> options
   ){
      options.Excluding(account => account.Owner);
      options.Excluding(account => account.Beneficiaries);
      options.Excluding(account => account.Transfers);
      options.Excluding(account => account.Transactions);
      options.IgnoringCyclicReferences();
      return options;
   }
   
   [Fact]
   public async Task SelectByOwnerIdUt() {
      // Arrange
      await _arrangeTest.ExampleAsync(_seed);
      var expected = _seed.Owner1.Accounts;
      // Act
      var actual = await _accountsRepository.SelectByOwnerIdAsync(_seed.Owner1.Id);

      // Assert
      _dataContext.LogChangeTracker("SelectByOwnerId");
      actual.Should()
         .BeEquivalentTo(expected, ExcludeCyclicReferences);
   }
   
   [Fact]
   public async Task FindByIdJoinUt() {
      // Arrange
      await _arrangeTest.ExampleAsync(_seed);
      // Act
      var actual = await _accountsRepository.FindByIdJoinAsync(_seed.Account1.Id, true, true, true, true);

      // Assert
      _dataContext.LogChangeTracker("FindByIdJoin");
      actual.Should()
          .BeEquivalentTo( _seed.Account1, ExcludeCyclicReferences);
   }
   #endregion

}