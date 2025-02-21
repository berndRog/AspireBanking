using System;
using BankingApi.Core.DomainModel.NullEntities;
namespace BankingApi.Core.DomainModel.Entities;

public class Transaction: AEntity   {

   // properties
   public override Guid Id { get; init; } = Guid.NewGuid();
   public DateTime Date { get; private set; } =  DateTime.UtcNow;
   public double Amount { get; private set; } = 0.0;
   // navigation property Transaction --> Account [1]  
   public Account? Account { get; private set; } = null;
   public Guid? AccountId { get; private set; } = null;
   // navigation property Transaction --> Transfer [0..1]  
   public Transfer? Transfer { get; private set; } = null;
   public Guid? TransferId { get; private set; } = null;
   // setter
   public void Set(DateTime date) =>
      Date = date;
   public void Set(double amount) =>
      Amount = amount;
   
   // ctor   
   public Transaction() {}
   public Transaction(Guid id, DateTime date, double amount, Account? account, Transfer? transfer) {
      Id = id;
      Date = date;
      Amount = amount;
      Account = account;
      AccountId = account?.Id ?? null;
      Transfer = transfer;
      TransferId = transfer?.Id ?? null;
   }
   // mapping dto -> entity
   public Transaction(Guid id, DateTime date, double amount, Guid? accountId, Guid? transferId) {
      Id = id;
      Date = date;
      Amount = amount;
      Account = null;
      AccountId = accountId;
      Transfer = null;
      TransferId = transferId;
   }
   
   // methods
   public void Set(Account account, Transfer transfer){
      Account = account;
      AccountId = account.Id;
      Transfer = transfer;
      TransferId = transfer.Id;
      // Date = transfer.Date;
      // Amount = transfer.Amount;
   }


}