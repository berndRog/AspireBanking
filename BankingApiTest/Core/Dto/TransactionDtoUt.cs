using AutoMapper;
using BankingApi.Core.Dto;
using BankingApi.Core.Mapping;
using FluentAssertions;
using Xunit;
namespace BankingApiTest.Core.Dto;

public class TransactionDtoUt   {
  
   private readonly Seed _seed;
   
   public TransactionDtoUt() {
      var config = new MapperConfiguration(config =>
         config.AddProfile(new MappingProfile())
      );
      var mapper = new Mapper(config);
      _seed = new Seed();
   }
   
   [Fact]
   public void CtorAndGetterUt() {
      // Act
      var transfer = _seed.Transfer1;
      
      var actualDebit = new TransactionDto(
         Id: _seed.Transaction1.Id,
         Date: transfer.Date,
         Amount: -transfer.Amount,
         AccountId: transfer.Account.Id,
         TransferId : transfer.Id
      );
      // Assert
      actualDebit.Should().NotBeNull() .And. BeOfType<TransactionDto>();
      actualDebit.Id.Should().Be(_seed.Transaction1.Id);
      actualDebit.Date.Should().Be(transfer.Date);
      actualDebit.Amount.Should().Be(-transfer.Amount);
      actualDebit.AccountId.Should().Be(transfer.Account.Id);
      actualDebit.TransferId.Should().Be(transfer.Id);
   }
}