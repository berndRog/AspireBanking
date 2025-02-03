using System;
using FluentAssertions;
using BankingApi.Core.DomainModel.Entities;
using BankingApi.Core.Misc;
using Xunit;

namespace BankingApiTest.Core.DomainModel.Entities;
public class TransactionUt {

   private readonly Seed _seed;
   
   public TransactionUt() {
      _seed = new Seed();
   }
   
   [Fact]
   public void Ctor() {
      // Arrange
      // Act
      var actual = new Transaction();
      // Assert
      actual.Should().NotBeNull();
      actual.Should().BeOfType<Transaction>();
   }

   [Fact]
   public void ObjectInitializerUt() {
      // Arrange
      var id = new Guid("00000001-0000-0000-0000-000000000000");
      var modified = DateTime.UtcNow;
      const double amount = 100.0;           
      // Act
      var actual = new Transaction {
         Id = id,         
         Amount = amount,
         Account = _seed.Account1,
         AccountId = _seed.Account1.Id,
         Transfer = _seed.Transfer1,
         TransferId = _seed.Transfer1.Id
      };
      // Assert
      actual.Should().NotBeNull();
      actual.Should().BeOfType<Transaction>();
      actual.Id.Should().Be(id);
      actual.Amount.Should().Be(amount);
      actual.Account.Should().BeEquivalentTo(_seed.Account1);
      actual.AccountId.Should().Be((Guid)_seed.Account1.Id);
      actual.Transfer.Should().BeEquivalentTo(_seed.Transfer1);
      actual.TransferId.Should().Be(_seed.Transfer1.Id);
   }
   [Fact]
   public void GetterUt() {
      // Arrange
      var id = new Guid("00000001-0000-0000-0000-000000000000");
      var modified = DateTime.Now;
      const double amount = 100.0;
      var transaction = new Transaction {
         Id = id,
         Amount = amount,
         Account = _seed.Account1,
         AccountId = _seed.Account1.Id,
         Transfer = _seed.Transfer1,
         TransferId = _seed.Transfer1.Id
      };
      // Act
      var actualId = transaction.Id;
      var actualAmount = transaction.Amount;
      var actualAccount = transaction.Account;
      var actualAccountId = transaction.AccountId;
      var actualTransfer = transaction.Transfer;
      var actualTransferId = transaction.TransferId;
      // Assert
      actualId.Should().Be(id);
      actualAmount.Should().Be(amount);
      actualAccount.Should().BeEquivalentTo(_seed.Account1);
      actualAccountId.Should().Be(_seed.Account1.Id);
      actualTransfer.Should().BeEquivalentTo(_seed.Transfer1);
      actualTransferId.Should().Be(_seed.Transfer1.Id);
   }
   
   [Fact]
   public void SetUt() {
      // Arrange
      var id = new Guid("00000001-0000-0000-0000-000000000000");
      var transaction = new Transaction {
         Id = id,
         Amount = _seed.Transfer1.Amount,
         Date = _seed.Transfer1.Date
      };
      // Act
      transaction.Set(_seed.Account1, _seed.Transfer1, true);  // Debit
      // Assert
      transaction.Id.Should().Be(id);
      transaction.Amount.Should().Be(-(_seed.Transfer1.Amount));
      transaction.Date.Should().Be(_seed.Transfer1.Date);
      transaction.Account.Should().BeEquivalentTo(_seed.Account1);
      transaction.AccountId.Should().Be(_seed.Account1.Id);
      transaction.Transfer.Should().BeEquivalentTo(_seed.Transfer1);
      transaction.TransferId.Should().Be(_seed.Transfer1.Id);
   }

   
   [Fact]
   public void AddDebitCreditUt(){
      // Arrange
      _seed.InitAccounts().InitBeneficiaries();
      var accountDebit = _seed.Account1;
      var accountCredit = _seed.Account6;
      var beneficiary = _seed.Beneficiary1;
      var transfer = _seed.Transfer1;
      var transactionDebit = new Transaction();
      var transactionCredit = new Transaction();
      // Act
      accountDebit.Add(transfer, beneficiary);
      transactionDebit.Set(accountDebit, transfer, true);
      transactionCredit.Set(accountCredit, transfer, false);
      // Assert      
      transactionDebit.Amount.Should().Be(-(transfer.Amount));
      transactionDebit.AccountId.Should().Be(accountDebit.Id);
      transactionDebit.Account.Id.Should().Be(accountDebit.Id);
      transactionDebit.TransferId.Should().Be(transfer.Id);
      transactionDebit.Transfer?.Id.Should().Be(transfer.Id);

      transactionCredit.Amount.Should().Be(transfer.Amount);
      transactionCredit.AccountId.Should().Be(accountCredit.Id);
      transactionCredit.Account.Id.Should().Be(accountCredit.Id);
      transactionCredit.TransferId.Should().Be(transfer.Id);
      transactionCredit.Transfer?.Id.Should().Be(transfer.Id);
   }
   
   [Fact]
   public void ReverseDebitCreditUt(){
      // Arrange
      _seed.InitAccounts().InitBeneficiaries();
      var accountDebit = _seed.Account1;
      var accountCredit = _seed.Account6;
      var beneficiary = _seed.Beneficiary1;
      var originalTransfer = _seed.Transfer1;
      var originalTransactionDebit = _seed.Transaction1;
      var originalTransactionCredit = _seed.Transaction2;
      accountDebit.Add(originalTransfer, beneficiary);
      originalTransactionDebit.Set(accountDebit, originalTransfer, true);
      originalTransactionCredit.Set(accountCredit, originalTransfer, false);
      // Act
      var reverseTransfer = new Transfer{
         Description = "Rückbuchung",
         Amount = -originalTransfer.Amount,
         Date = DateTime.UtcNow
      };

      var reverseTransactionDebit = new Transaction();
      var reverseTransactionCredit = new Transaction();
      accountDebit.Add(reverseTransfer, beneficiary);
      reverseTransactionDebit.Set(accountDebit, reverseTransfer, false);
      reverseTransactionCredit.Set(accountCredit, reverseTransfer, true);
      
      // Assert      
      reverseTransactionDebit.Date.Should().Be(reverseTransfer.Date);
      reverseTransactionDebit.Amount.Should().Be(reverseTransfer.Amount);
      reverseTransactionDebit.AccountId.Should().Be(accountDebit.Id);
      reverseTransactionDebit.Account.Id.Should().Be(accountDebit.Id);
      reverseTransactionDebit.TransferId.Should().Be(reverseTransfer.Id);
      reverseTransactionDebit.Transfer?.Id.Should().Be(reverseTransfer.Id);
      
      reverseTransactionCredit.Date.Should().Be(reverseTransfer.Date);
      reverseTransactionCredit.Amount.Should().Be(-(reverseTransfer.Amount));
      reverseTransactionCredit.AccountId.Should().Be(accountCredit.Id);
      reverseTransactionCredit.Account.Id.Should().Be(accountCredit.Id);
      reverseTransactionCredit.TransferId.Should().Be(reverseTransfer.Id);
      reverseTransactionCredit.Transfer?.Id.Should().Be(reverseTransfer.Id);
   }
}