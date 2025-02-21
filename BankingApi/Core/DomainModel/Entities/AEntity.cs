using System;
namespace BankingApi.Core.DomainModel.Entities;
public abstract class AEntity {
   public abstract Guid Id { get; init;  }
}