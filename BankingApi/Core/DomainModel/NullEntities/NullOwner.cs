using System;
using BankingApi.Core.DomainModel.Entities;
using BankingApi.Core.Misc;
namespace BankingApi.Core.DomainModel.NullEntities;
// https://jonskeet.uk/csharp/singleton.html

public sealed class NullOwner: Owner {  
   // Singleton Skeet Version 4
   public static NullOwner Instance { get; } = new ();
   static NullOwner() { }
   private NullOwner() { }
}