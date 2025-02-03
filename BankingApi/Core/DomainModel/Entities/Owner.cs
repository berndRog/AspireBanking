using System;
using System.Collections.Generic;
using BankingApi.Core.DomainModel.NullEntities;
namespace BankingApi.Core.DomainModel.Entities;

public class Owner: AEntity {
   #region properties
   public override Guid Id { get; init; } = Guid.NewGuid();
   public string FirstName { get; set; } = string.Empty;
   public string LastName { get; set; } = string.Empty;
   public string? Email { get; set; } = string.Empty;
   public string? UserName { get; set; } = null;
   public string? UserId { get; set; } = null;
   public string? UserRole { get; set; } = null;
   
   // Navigation-Property Owner --> Account [0..*]  
   public IList<Account> Accounts { get; } = new List<Account>();
   #endregion
   
   #region methods
   public void Update(Owner updOwner) {
      FirstName = updOwner.FirstName;
      LastName = updOwner.LastName;
      if(updOwner.Email != null) Email = updOwner.Email;
      if(updOwner.UserName != null) UserName = updOwner.UserName;
      if(updOwner.UserId != null) UserId = updOwner.UserId;
      if(updOwner.UserRole != null) UserRole = updOwner.UserRole;
   } 
   
   public void Add(Account account) {
      if (account.Owner is not NullOwner)
         throw new ApplicationException("Account is already asigned to another owner");
      account.Owner = this;
      account.OwnerId = Id;
      Accounts.Add(account);
   }   
   public void Remove(Account account) {
      // account.Owner = null;
   // account.OwnerId = null;
      Accounts.Remove(account);
   }   
   #endregion
}