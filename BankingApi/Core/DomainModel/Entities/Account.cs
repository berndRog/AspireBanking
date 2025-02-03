using System;
using System.Collections.Generic;
using BankingApi.Core.DomainModel.NullEntities;
using BankingApi.Core.Dto;
using BankingApi.Core.Misc;
namespace BankingApi.Core.DomainModel.Entities;

public class Account: AEntity {
   
   #region fields
   private readonly string _iban = string.Empty;
   #endregion
   
   #region properties
   public override Guid Id { get; init; } = Guid.NewGuid();
   public string Iban { 
      get => _iban;
      init => _iban = Utils.CheckIban(value);
   }
   public double Balance { get; set; } 
   
   // Navigation-Property Account --> Owner [1]  
   public Owner Owner{ get; set; } = NullOwner.Instance;
   public Guid OwnerId{ get; set; } = NullOwner.Instance.Id;
   
   // Navigation-Property Account --> Beneficiary [0..*]  
   public IList<Beneficiary> Beneficiaries { get; } = new List<Beneficiary>();
   // Navigation-Property Account --> Transfer [0..*] 
   public IList<Transfer> Transfers { get; } = new List<Transfer>();
   // Navigation-Property Account --> Transaction [0..*] 
   public IList<Transaction> Transactions { get; } = new List<Transaction>();
   #endregion
   
   #region ctor
   public Account() {}
   public Account(AccountDto dto) {
      Id = dto.Id;
      Iban = Utils.CheckIban(dto.Iban);
      Balance = dto.Balance;
      OwnerId = dto.OwnerId;
   }
   #endregion

   
   #region methods Beneficiaries
   public void Add(Beneficiary beneficiary){     
//    beneficiary.Account = this;
      // debit account
      beneficiary.AccountId = Id;
      Beneficiaries.Add(beneficiary);
   }
   #endregion

   #region methods Transfers
   // Add transfer to account and to beneficiary
   public void Add(Transfer transfer, Beneficiary beneficiary) {
      // set transfer properties
      transfer.Account = this;
      transfer.AccountId = Id;   
      // add transfer to account
      Transfers.Add(transfer);
      
      // set beneficiary properties      
      transfer.Beneficiary = beneficiary;
      transfer.BeneficiaryId = beneficiary.Id;
   }
   #endregion
   
   #region methods Transaction
   // Add transactions to accounts
   public void Add(Transaction transaction, Transfer transfer){
      
      // set transaction properties
      transaction.Account = this;
      transaction.AccountId = this.Id;
      transaction.Transfer = transfer;
      transaction.TransferId = transfer.Id;
      transaction.Date = transfer.Date;
      
      // add transaction to transfer
      transfer.Add(transaction);
      // add transaction to account
      Transactions.Add(transaction);
      
      // change account balance
      Balance += transaction.Amount;
   }
   #endregion   
}