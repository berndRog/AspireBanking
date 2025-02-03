using System;
using BankingApi.Core.DomainModel.NullEntities;
namespace BankingApi.Core.DomainModel.Entities; 

public class Beneficiary: AEntity{
   
   #region properties
   public override Guid Id { get; init; } = Guid.NewGuid();
   // name of the credit account owner
   public string FirstName { get; init; } = string.Empty;  
   public string LastName { get; init; } = string.Empty;  
   // transfer money to credit account via Iban
   public string Iban { get; init; } = string.Empty;

   // Navigation-Property Beneficiary  --> Account [1]
   // Debit account
   public Guid AccountId{ get; set; } = NullAccount.Instance.Id;  
   #endregion
}