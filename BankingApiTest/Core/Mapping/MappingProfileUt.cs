using System;
using AutoMapper;
using FluentAssertions;
using BankingApi.Core.DomainModel.Entities;
using BankingApi.Core.Dto;
using BankingApi.Core.Mapping;
using Xunit;

namespace BankingApiTest.Core.Mapping;

public class MappingProfileUt {
   private readonly Seed _seed;
   private readonly IMapper _mapper;

   public MappingProfileUt() {
      var config = new MapperConfiguration(config =>
         config.AddProfile(new MappingProfile())
      );
      _mapper = new Mapper(config);
      _seed = new Seed();
      _seed.InitAccounts().InitBeneficiaries().InitTransfersTransactions();
   }

   [Fact]
   public void Owner2OwnerDtoUt() {
      // Arrange
      // Act
      var actualDto = _mapper.Map<OwnerDto>(_seed.Owner1);
      // Assert
      actualDto.Should().NotBeNull().And
         .BeOfType<OwnerDto>();

      actualDto.Id.Should().Be(_seed.Owner1.Id);
      actualDto.FirstName.Should().Be(_seed.Owner1.FirstName);
      actualDto.Email.Should().Be(_seed.Owner1.Email);
   }

   [Fact]
   public void OwnerDto2OwnerUt() {
      // Arrange
      var ownerDto = _mapper.Map<OwnerDto>(_seed.Owner1);
      // Act
      var actualOwner = _mapper.Map<Owner>(ownerDto);
      // Assert
      actualOwner.Should().NotBeNull().And
         .BeOfType<Owner>();
      actualOwner.Id.Should().Be(_seed.Owner1.Id);
      actualOwner.FirstName.Should().Be(_seed.Owner1.FirstName);
      actualOwner.Email.Should().Be(_seed.Owner1.Email);
   }

   [Fact]
   public void Account2AccountDtoUt() {
      // Arrange
      // Act
      var actualDto = _mapper.Map<AccountDto>(_seed.Account1);
      // Assert
      actualDto.Should().NotBeNull().And
         .BeOfType<AccountDto>();
      actualDto.Id.Should().Be(_seed.Account1.Id);
      actualDto.Iban.Should().Be(_seed.Account1.Iban);
      actualDto.Balance.Should().Be(_seed.Account1.Balance);
   }

   [Fact]
   public void AccountDto2AccountUt() {
      // Arrange
      var accountDto = _mapper.Map<AccountDto>(_seed.Account1);
      // Act
      var actual = _mapper.Map<Account>(accountDto);
      // Assert
      actual.Should().NotBeNull().And
         .BeOfType<Account>();
      actual.Id.Should().Be(_seed.Account1.Id);
      actual.Iban.Should().Be(_seed.Account1.Iban);
      actual.Balance.Should().Be(_seed.Account1.Balance);
   }

   [Fact]
   public void Beneficiary2BeneficiaryDtoUt() {
      // Arrange
      // Act
      var actualDto = _mapper.Map<BeneficiaryDto>(_seed.Beneficiary1);
      // Assert
      actualDto.Should().NotBeNull().And
         .BeOfType<BeneficiaryDto>();
      actualDto.Id.Should().Be(_seed.Beneficiary1.Id);
      actualDto.FirstName.Should().Be(_seed.Beneficiary1.FirstName);
      actualDto.LastName.Should().Be(_seed.Beneficiary1.LastName);
      actualDto.Iban.Should().Be(_seed.Beneficiary1.Iban);
      actualDto.AccountId.Should().Be(_seed.Beneficiary1.AccountId);
   }

   [Fact]
   public void BeneficiaryDto2BeneficiaryUt() {
      // Arrange
      var beneficiaryDto = _mapper.Map<BeneficiaryDto>(_seed.Beneficiary1);
      // Act
      var actual = _mapper.Map<Beneficiary>(beneficiaryDto);
      // Assert
      actual.Should().NotBeNull().And
         .BeOfType<Beneficiary>();
      actual.Id.Should().Be(_seed.Beneficiary1.Id);
      actual.FirstName.Should().Be(_seed.Beneficiary1.FirstName);
      actual.LastName.Should().Be(_seed.Beneficiary1.LastName);
      actual.Iban.Should().Be(_seed.Beneficiary1.Iban);
      actual.AccountId.Should().Be(_seed.Beneficiary1.AccountId);
   }

   [Fact]
   public void Transfer2TransferDtoUt() {
      // Arrange
      // Act
      var actualDto = _mapper.Map<TransferDto>(_seed.Transfer1);
      // Assert
      actualDto.Should().NotBeNull().And
         .BeOfType<TransferDto>();
      actualDto.Id.Should().Be(_seed.Transfer1.Id);
      actualDto.Description.Should().Be(_seed.Transfer1.Description);
      actualDto.Date.Should().Be(_seed.Transfer1.Date);
      actualDto.Amount.Should().Be(_seed.Transfer1.Amount);
      actualDto.AccountId.Should().Be(_seed.Transfer1.AccountId);
      if (_seed.Transfer1.BeneficiaryId != null)
         actualDto.BeneficiaryId.Should().Be((Guid)_seed.Transfer1.BeneficiaryId);
   }

   [Fact]
   public void TransferDto2TransferUt() {
      // Arrange
      var transferDto = _mapper.Map<TransferDto>(_seed.Transfer1);
      // Act
      var actual = _mapper.Map<Transfer>(transferDto);
      // Assert
      actual.Should().NotBeNull().And
         .BeOfType<Transfer>();
      actual.Id.Should().Be(_seed.Transfer1.Id);
      actual.Description.Should().Be(_seed.Transfer1.Description);
      actual.Date.Should().Be(_seed.Transfer1.Date);
      actual.Amount.Should().Be(_seed.Transfer1.Amount);
      actual.AccountId.Should().Be(_seed.Transfer1.AccountId);
      actual.BeneficiaryId.Should().Be(_seed.Transfer1.BeneficiaryId);
   }

   [Fact]
   public void Transaction2TransactionDtoUt() {
      // Arrange
      // Act
      var actualDto = _mapper.Map<TransactionDto>(_seed.Transaction1);
      // Assert
      actualDto.Should().NotBeNull().And
         .BeOfType<TransactionDto>();
      actualDto.Id.Should().Be(_seed.Transaction1.Id);
      actualDto.Date.Should().Be(_seed.Transaction1.Date);
      actualDto.AccountId.Should().Be(_seed.Transfer1.AccountId);
   }

   [Fact]
   public void TransactionDto2TransactionUt() {
      // Arrange
      var transactionDto = _mapper.Map<TransactionDto>(_seed.Transaction1);
      // Act
      var actual = _mapper.Map<Transaction>(transactionDto);
      // Assert
      actual.Should().NotBeNull().And
         .BeOfType<Transaction>();
      actual.Id.Should().Be(_seed.Transaction1.Id);
      actual.AccountId.Should().Be(_seed.Transfer1.AccountId);
   }
   
}