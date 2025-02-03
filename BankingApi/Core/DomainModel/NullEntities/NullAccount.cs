using System;
using BankingApi.Core.DomainModel.Entities;

namespace BankingApi.Core.DomainModel.NullEntities;
// https://jonskeet.uk/csharp/singleton.html
public sealed class NullAccount: Account {  
   // Singleton Skeet Version 4
   public static NullAccount Instance { get; } = new ();
   static NullAccount() { }
   private NullAccount() { Id = Guid.Empty;  }
}