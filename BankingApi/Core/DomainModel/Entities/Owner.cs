using System;
using System.Collections.Generic;
using BankingApi.Core.DomainModel.NullEntities;
namespace BankingApi.Core.DomainModel.Entities;

public class Owner: AEntity {

   public override Guid Id { get; init; } = Guid.NewGuid();
   public string FirstName { get; private set; } = string.Empty;
   public string LastName { get; private set; } = string.Empty;
   public string? Email { get; private set; } = null;
   public string? UserName { get; private set; } = null;
   public string? UserId { get; private set; } = null;
   // Navigation-Property Owner --> Account [0..*]  
   public IList<Account> Accounts { get; } = new List<Account>();
   
   public Owner() { }
   
   public Owner(Guid id, string firstName, string lastName,
      string? email, string? userName, string? userId) {
      Id = id;
      FirstName = firstName;
      LastName = lastName;
      Email = email;
      UserName = userName;
      UserId = userId;
   }
   
   public void Update(Owner updOwner) {
      FirstName = updOwner.FirstName;
      LastName = updOwner.LastName;
      if(updOwner.Email != null) Email = updOwner.Email;
      if(updOwner.UserName != null) UserName = updOwner.UserName;
      if(updOwner.UserId != null) UserId = updOwner.UserId;
   } 
   
   public void Add(Account account) {
      if (account.Owner is not NullOwner)
         throw new ApplicationException("Account is already asigned to another owner");
      account.Update(this);
      Accounts.Add(account);
   }   
   public void Remove(Account account) {
      // account.Owner = null;
   // account.OwnerId = null;
      Accounts.Remove(account);
   }   

}