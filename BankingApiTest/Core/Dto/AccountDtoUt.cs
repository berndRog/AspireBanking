using BankingApi.Core.Dto;
using FluentAssertions;
using Xunit;
namespace BankingApiTest.Core.Dto;
public class AccountDtoUt {
   private readonly Seed _seed;

   public AccountDtoUt(){ _seed = new Seed(); }

   #region Account
   [Fact]
   public void CtorAndGetterUt(){
      // Arrange
      // Act
      var actual = new AccountDto(
         Id: _seed.Account1.Id,
         Iban: _seed.Account1.Iban,
         Balance: _seed.Account1.Balance,
         OwnerId: _seed.Account1.OwnerId
      );
      // Assert
      actual.Should().NotBeNull();
      actual.Should().BeOfType<AccountDto>();
      actual.Id.Should().Be(_seed.Account1.Id);
      actual.Iban.Should().Be(_seed.Account1.Iban);
      actual.Balance.Should().Be(_seed.Account1.Balance);
      actual.OwnerId.Should().Be(_seed.Account1.OwnerId);
   }
   #endregion
}