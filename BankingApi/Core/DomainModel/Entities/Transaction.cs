using System;
namespace BankingApi.Core.DomainModel.Entities;

public class Transaction: AEntity   {
   
   #region properties
   public override Guid Id { get; init; } = Guid.NewGuid();
   public DateTime Date { get; set; } = DateTime.UtcNow;
   public double Amount{ get; set; }
   
   // Navigation-Property Transaction --> Account [1]  
   public Account? Account { get; set; }
   public Guid? AccountId { get; set; }
      
   // Navigation-Property Transaction --> Transfer [0..1]  
   public Transfer? Transfer { get; set; }
   public Guid? TransferId { get; set; }
   #endregion
   
   #region methods
   public void Set(Account account, Transfer transfer, bool isDebit = true){
      Account = account;
      AccountId = account.Id;
      Transfer = transfer;
      TransferId = transfer.Id;
      Date = transfer.Date;
      Amount = isDebit ? -transfer.Amount : transfer.Amount;
   }
   #endregion
}