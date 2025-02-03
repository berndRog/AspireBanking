using System;
using BankingApi.Core.DomainModel.Entities;
using FluentAssertions;
using Xunit;

namespace BankingApiTest.Core.DomainModel.Entities;
public class TransferUt {

   private readonly Seed _seed;
   
   public TransferUt() {
      _seed = new Seed();
   }
   
   [Fact]
   public void Ctor() {
      // Arrange
      // Act
      var actual = new Transfer();
      // Assert
      actual.Should().NotBeNull();
      actual.Should().BeOfType<Transfer>();
   }

   [Fact]
   public void ObjectInitializerUt() {
      // Arrange
      _seed.Account8.Add(_seed.Beneficiary1);
      var id = new Guid("00000001-0000-0000-0000-000000000000");
      var modified = DateTime.UtcNow;
      const string description = "Überweisung an Erika";
      const double amount = 100.0;           
      // Act
      var actual = new Transfer {
         Id = id,
         Description = description,
         Date = modified,
         Amount = amount,
         Account = _seed.Account1,
         AccountId = _seed.Account1.Id,
         Beneficiary = _seed.Beneficiary1,
         BeneficiaryId = _seed.Beneficiary1.Id
      };
      // Assert
      actual.Should().NotBeNull();
      actual.Should().BeOfType<Transfer>();
      actual.Id.Should().Be(id);
      actual.Description.Should().Be(description);
      actual.Date.Should().Be(modified);
      actual.Amount.Should().Be(amount);
      actual.Account.Should().BeEquivalentTo(_seed.Account1);
      actual.AccountId.Should().Be(_seed.Account1.Id);
      actual.Beneficiary.Should().BeEquivalentTo(_seed.Beneficiary1);
      actual.BeneficiaryId.Should().Be(_seed.Beneficiary1.Id);
   }
   [Fact]
   public void GetterUt() {
      // Arrange
      _seed.Account8.Add(_seed.Beneficiary1);
      var id = new Guid("00000001-0000-0000-0000-000000000000");
      var modified = DateTime.UtcNow;
      const string description = "Überweisung an Erika";
      const double amount = 100.0;  
      var transfer = new Transfer {
         Id = id,
         Description = description,
         Date = modified,
         Amount = amount,
         Account = _seed.Account8,
         AccountId = _seed.Account8.Id,
         Beneficiary = _seed.Beneficiary1,
         BeneficiaryId = _seed.Beneficiary1.Id
      };
      // Act
      var actualId = transfer.Id;
      var actualDescription = transfer.Description;
      var actualDate = transfer.Date;
      var actualAmount = transfer.Amount;
      var actualAccount = transfer.Account;
      var actualAccountId = transfer.AccountId;
      var actualBeneficiary = transfer.Beneficiary;
      var actualBeneficiaryId = transfer.BeneficiaryId;
      // Assert
      actualId.Should().Be(id);
      actualDescription.Should().Be(description);

      actualDate.Should().Be(modified);
      actualAmount.Should().Be(amount);
      actualAccount.Should().BeEquivalentTo(_seed.Account8);
      actualAccountId.Should().Be(_seed.Account8.Id);
      actualBeneficiary.Should().BeEquivalentTo(_seed.Beneficiary1);
      actualBeneficiaryId.Should().Be(_seed.Beneficiary1.Id);
   }
   [Fact]
   public void SetterUt() {
      // Arrange
      _seed.Account8.Add(_seed.Beneficiary1);
      var id = new Guid("00000001-0000-0000-0000-000000000000");
      var modified = DateTime.UtcNow;
      const string description = "Überweisung an Erika";
      const double amount = 100.0;
      var account = _seed.Account8;
      var accountId = _seed.Account8.Id;
      var beneficiary = _seed.Beneficiary1;
      var beneficiaryId = _seed.Beneficiary1.Id;
      // Act
      var actual = new Transfer();
      actual.Description = description;
      actual.Date = modified;
      actual.Amount = amount;
      actual.Account = account;
      actual.AccountId = accountId;
      actual.Beneficiary = beneficiary;
      actual.BeneficiaryId = beneficiaryId;
      // Assert
      actual.Should().NotBeNull();
      actual.Should().BeOfType<Transfer>();
      actual.Description.Should().Be(description);
      actual.Date.Should().Be(modified);
      actual.Amount.Should().Be(amount);
      actual.Account.Should().BeEquivalentTo(account);
      actual.AccountId.Should().Be(accountId);
      actual.Beneficiary.Should().BeEquivalentTo(_seed.Beneficiary1);
      actual.BeneficiaryId.Should().Be(beneficiaryId);
   }
}