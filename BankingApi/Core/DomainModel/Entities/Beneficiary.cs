using System;
using BankingApi.Core.DomainModel.NullEntities;
namespace BankingApi.Core.DomainModel.Entities; 

public class Beneficiary: AEntity{
   
   // properties
   public override Guid Id { get; init; } = Guid.NewGuid();
   // name of the credit account owner
   public string FirstName { get; private set; } = string.Empty;
   public string LastName { get; private set; } = string.Empty;  
   // transfer money to credit account via Iban
   public string Iban { get; private set; } = string.Empty;
   // Navigation-Property Beneficiary  --> Account [1]
   // Debit account
   public Guid AccountId { get; private set; } = Guid.Empty;  
 
   // ctor
   public Beneficiary() {}
   public Beneficiary(Guid id, string firstName, string lastName, string iban, Guid accountId) {
      Id = id;
      FirstName = firstName;
      LastName = lastName;
      Iban = iban;
      AccountId = accountId;
   }
   
   // methods
   public void Add(Account account) {
      //_account = account;
      AccountId = account.Id;
   }
   
}