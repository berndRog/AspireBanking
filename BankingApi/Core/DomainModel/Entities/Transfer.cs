using System;
using System.Collections.Generic;
using System.Linq;
using BankingApi.Core.DomainModel.NullEntities;
namespace BankingApi.Core.DomainModel.Entities;

public class Transfer: AEntity   {
   
   // properties
   public override Guid Id { get; init; } = Guid.NewGuid();
   public DateTime Date { get; private set; } = DateTime.UtcNow;
   public string Description { get; private set; } = string.Empty;
   public double Amount { get; private set; } = 0.0;  // Amount < 0 -> Reverse SendMoney
   // navigation property Transfer --> Account [1]  
   public Account Account { get; private set; } = NullAccount.Instance;
   public Guid AccountId { get; private set; } = NullAccount.Instance.Id;
   // navigation property Transfer --> Beneficiary [0..1]  (nullable)
   public Beneficiary? Beneficiary { get; private set; } = null;
   public Guid? BeneficiaryId { get; private set; } = null;
   // navigationc property Transfer --> Transactions [0..2]
   public ICollection<Transaction> Transactions { get; } = [];
   public void Set(double amount) =>
      Amount = amount;
   
   public void Set(Guid accountId) =>
      AccountId = accountId;
   
   // ctor
   public Transfer() { }
   public Transfer(Guid id, DateTime date, string description, double amount, Account account, Beneficiary? beneficiary) {
      Id = id;
      Date = date;
      Description = description;
      Amount = amount;
      Account = account;
      AccountId = account.Id;
      Beneficiary = beneficiary;
      BeneficiaryId = beneficiary?.Id;
   }
   // mapping dto -> entity
   public Transfer(Guid id, DateTime date, string description, double amount, Guid accountId, Guid? beneficiaryId) {
      Id = id;
      Date = date;
      Description = description;
      Amount = amount;
      Account = NullAccount.Instance;
      AccountId = accountId;
      Beneficiary = null;
      BeneficiaryId = beneficiaryId;
   }
   
   // methods
   public void Add(Account account) {
      Account = account;
      AccountId = account.Id;
   }
   public void Add(Beneficiary? beneficiary) {
      Beneficiary = beneficiary;
      BeneficiaryId = beneficiary?.Id;
   }
   
   public void Add(Transaction transaction) =>
      Transactions.Add(transaction);
   
   public void Remove() {
      Beneficiary = null;
      BeneficiaryId = Guid.Empty;
   }
   
}