using System;
namespace BankingApi.Core.DomainModel.Entities;
public abstract class AEntity {
   #region properties
   public abstract Guid Id { get; init; }
   #endregion
}