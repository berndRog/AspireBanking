using System.Collections.Generic;
using System.Threading.Tasks;
using BankingApi.Core.DomainModel.Entities;
using FluentAssertions;
using Xunit;
namespace BankingApiTest.Persistence.Repositories;
[Collection(nameof(SystemTestCollectionDefinition))]
public class BeneficiariesRepositoryUt : BaseRepositoryUt{
   
   private void ShowRepository(string text) {
#if DEBUG
      _dataContext.LogChangeTracker(text);
#endif
   }

   #region methods beneficiary <-> account   
   [Fact]
   public async Task FindById() {
      // Arrange
      await _arrangeTest.OwnersWithAccountsAndBeneficiariesAsync(_seed);
      // Act 
      var actual = await _beneficiariesRepository.FindByIdAsync(_seed.Beneficiary2.Id);
      // Assert
      ShowRepository("FindById");
      actual.Should()
         .NotBeNull().And
         .BeEquivalentTo(_seed.Beneficiary2);
   }

   [Fact]
   public async Task AddBeneficiary(){
      // Arrange
      await _arrangeTest.OwnersWithAccountsAsync(_seed);
      // Act
      _seed.Account1.Add(_seed.Beneficiary1);
      _beneficiariesRepository.Add(_seed.Beneficiary1);
      await _dataContext.SaveAllChangesAsync();
      // Assert
      var actual = await _beneficiariesRepository.FindByIdAsync(_seed.Beneficiary1.Id);
      actual.Should().BeEquivalentTo(_seed.Beneficiary1);      
   }
   
   [Fact]
   public async Task SelectByAccountId() {
      // Arrange
      await _arrangeTest.OwnersWithAccountsAndBeneficiariesAsync(_seed);
      var expected = new List<Beneficiary> { _seed.Beneficiary1, _seed.Beneficiary2 };
      // Act 
      var actual = await _beneficiariesRepository.SelectByAccountIdAsync(_seed.Account1.Id);
      // Assert
      ShowRepository("SelectByAccountId");
      actual.Should()
         .NotBeNull().And
         .BeEquivalentTo(expected);
   }
   #endregion
}