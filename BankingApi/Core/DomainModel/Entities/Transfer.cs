using System;
using System.Collections.Generic;
using System.Linq;
using BankingApi.Core.DomainModel.NullEntities;
namespace BankingApi.Core.DomainModel.Entities;

public class Transfer: AEntity   {
   
   #region properties
   public override Guid Id { get; init; } = Guid.NewGuid();
   public DateTime Date {get; set; } = DateTime.UtcNow;
   public string Description { get; set; } = string.Empty;
   public double Amount{ get; set; }  // Amount < 0 -> Reverse SendMoney
   
   // Navigation-Property Transfer --> Account [1]  
   public Account Account { get; set; } = NullAccount.Instance;
   public Guid AccountId { get; set; } = Guid.Empty;
   
   // Navigation-Property Transfer --> Beneficiary [0..1]  (nullable)
   public Beneficiary? Beneficiary { get; set; }
   public Guid? BeneficiaryId { get; set; }

   // Navigation-Property Transfer --> Transactions [0..2]
   public IList<Transaction> Transactions { get; } = new List<Transaction>();
   #endregion
   
   #region methods
   public void Add(Transaction transaction) =>
      Transactions.Add(transaction);

   public Transaction? RemoveTransaction() {
      var transaction = Transactions.FirstOrDefault(t => t.Amount < 0);  // Debit
      if(transaction == null) return null;
      transaction.Transfer = null;
      transaction.TransferId = null;
      return transaction;
   }
   #endregion
}