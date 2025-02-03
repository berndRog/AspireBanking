using AutoMapper;
using BankingApi.Core.Dto;
using BankingApi.Core.Mapping;
using FluentAssertions;
using Xunit;
namespace BankingApiTest.Core.Dto;

public class TransferDtoUt {

   private readonly Seed _seed;
   
   public TransferDtoUt() {
      var config = new MapperConfiguration(config =>
         config.AddProfile(new MappingProfile())
      );
      var mapper = new Mapper(config);
      _seed = new Seed();
   }
   
   [Fact]
   public void CtorAndGetterUt() {
      // Act
      var actual = new TransferDto(
         Id: _seed.Transfer1.Id,
         Description: _seed.Transfer1.Description,
         Date: _seed.Transfer1.Date,
         Amount: _seed.Transfer1.Amount,
         AccountId: _seed.Account1.Id,
         BeneficiaryId : _seed.Beneficiary8.Id
      );
      // Assert
      actual.Should().NotBeNull() .And. BeOfType<TransferDto>();
      actual.Id.Should().Be(_seed.Transfer1.Id);
      actual.Description.Should().Be(_seed.Transfer1.Description);
      actual.Date.Should().Be(_seed.Transfer1.Date);
      actual.Amount.Should().Be(_seed.Transfer1.Amount);
      actual.AccountId.Should().Be(_seed.Account1.Id);
      actual.BeneficiaryId.Should().Be(_seed.Beneficiary8.Id);
   }
}