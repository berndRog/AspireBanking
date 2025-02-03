using BankingApi.Core.Dto;
using FluentAssertions;
using Xunit;
namespace BankingApiTest.Core.Dto;
public class BeneficiaryDtoUt {
   private readonly Seed _seed = new();

   [Fact]
   public void CtorAndGetterUt(){
      // Arrange
      // Act
      var actual = new BeneficiaryDto(
         Id: _seed.Beneficiary1.Id,
         FirstName: _seed.Beneficiary1.FirstName,
         LastName: _seed.Beneficiary1.LastName,
         Iban:  _seed.Beneficiary1.Iban,
         AccountId: _seed.Account8.Id
      );
      // Assert
      actual.Should().NotBeNull();
      actual.Should().BeOfType<BeneficiaryDto>();
      actual.Id.Should().Be(_seed.Beneficiary1.Id);
      actual.FirstName.Should().Be(_seed.Beneficiary1.FirstName);
      actual.LastName.Should().Be(_seed.Beneficiary1.LastName);
      actual.Iban.Should().Be(_seed.Beneficiary1.Iban);
      actual.AccountId.Should().Be(_seed.Account8.Id);
   }

}