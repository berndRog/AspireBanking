using System;
using System.Collections.Generic;
using BankingApi.Core.DomainModel.NullEntities;
using BankingApi.Core.Misc;
namespace BankingApi.Core.DomainModel.Entities;

public class Account: AEntity {
   // fields
   private readonly string _iban = string.Empty;
   
   // properties
   public override Guid Id { get; init; } = Guid.NewGuid();
   public string Iban { 
      get => _iban;
      init => _iban = Utils.CheckIban(value);
   }
   public double Balance { get; private set; } = 0.0;
   // navigation property Account --> Owner [1]  
   public Owner Owner { get; private set; } = NullOwner.Instance;
   public Guid OwnerId { get; private set; } = NullOwner.Instance.Id;
   // navigation property  Account --> Beneficiary [0..*]  
   public ICollection<Beneficiary> Beneficiaries { get; } = new List<Beneficiary>();
   // navigation property  Account --> Transfer [0..*] 
   public IList<Transfer> Transfers { get; } = new List<Transfer>();
   // navigation property  Account --> Transaction [0..*] 
   public IList<Transaction> Transactions { get; } = new List<Transaction>();
   
   // ctor
   public Account() {}
   
   public Account(Guid id, string iban, double balance, Owner owner) {
      Id = id;
      Iban = iban;
      Balance = balance;
      Owner = owner;
      OwnerId = owner.Id;
   }
   // mapping dto -> entity
   public Account(Guid id, string iban, double balance, Guid ownerId) {
      Id = id;
      Iban = iban;
      Balance = balance;
      Owner = NullOwner.Instance;
      OwnerId = ownerId;
   }
   
   // methods Owner
   public void Update(Owner owner) {
      if (owner is NullOwner)
         throw new ApplicationException("Owner is null");
      Owner = owner;
      OwnerId = owner.Id;
   }
   
   // methods Beneficiaries
   public void Add(Beneficiary beneficiary){     
      // debit account
      beneficiary.Add(account: this);
      Beneficiaries.Add(beneficiary);
   }

   // methods Transfers
   // Add transfer to account and to beneficiary
   public void Add(Transfer transfer, Beneficiary beneficiary) {
      transfer.Add(account: this);
      Transfers.Add(transfer);
      transfer.Add(beneficiary: beneficiary);
   }
   
   #region methods Transaction
   // Add transactions to accounts
   public void Add(Transaction transaction, Transfer transfer){
      // set transaction account and transfer in transaction
      transaction.Set(account: this, transfer: transfer);
      // add transaction to transfer
      transfer.Add(transaction);
      // add transaction to account
      Transactions.Add(transaction);
      
      // change account balance
      Balance += transaction.Amount;
   }
   #endregion   
}