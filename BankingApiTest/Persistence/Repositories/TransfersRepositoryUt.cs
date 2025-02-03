using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankingApi.Core.DomainModel.Entities;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Xunit;
namespace BankingApiTest.Persistence.Repositories;
[Collection(nameof(SystemTestCollectionDefinition))]
public  class TransfersRepositoryUt: BaseRepositoryUt {

   private void ShowRepository(string text){
#if DEBUG
      _dataContext.LogChangeTracker(text);
#endif
   }
   
   private EquivalencyAssertionOptions<Transfer> ExcludeReferences(
      EquivalencyAssertionOptions<Transfer> options
   ) {
      options.Excluding(transfer => transfer.Account);
      options.Excluding(transfer => transfer.Beneficiary);
      options.For(transfer => transfer.Transactions).Exclude(transaction => transaction.Account);
      options.IgnoringCyclicReferences();
      return options;
   }
   
   [Fact]
   public async Task SelectByAccountIdAsyncUt() {
      // Arrange
      await _arrangeTest.ExampleAsync(_seed);
      var expected = new List<Transfer>{ _seed.Transfer1, _seed.Transfer2 };
      
      // Act  without reference objects
      var actual = 
         await _transfersRepository.SelectByAccountIdAsync(_seed.Account1.Id);
      // Assert
      ShowRepository("SelectByAccountIdAsync");
      actual.Should()
         .NotBeNull().And
         .HaveCount(2).And
         .BeEquivalentTo(expected, ExcludeReferences);
   }
   
   [Fact]
   public async Task AddUt() {
      // Arrange
      await _arrangeTest.OwnersWithAccountsAndBeneficiariesAsync(_seed);
      var account = await _accountsRepository.FindByIdAsync(_seed.Account1.Id) ??
         throw new Exception($"Account {_seed.Account1.Id} doesn't exists."); 
      var beneficiary = await _beneficiariesRepository.FindByIdAsync(_seed.Beneficiary1.Id) ??
         throw new Exception($"Beneficiary {_seed.Beneficiary1.Id} doesn't exists.");
      var transfer = _seed.Transfer1;
      // Act
      account.Add(transfer, beneficiary);
      _transfersRepository.Add(transfer);
      await _dataContext.SaveAllChangesAsync();
      // Assert
      var actual = await _transfersRepository.FindByIdAsync(_seed.Transfer1.Id);
      actual.Should().BeEquivalentTo(_seed.Transfer1);
   }
}