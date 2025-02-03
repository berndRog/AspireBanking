using System;
namespace BankingApi.Core.DomainModel.Entities;

public class User: AEntity {
   public override Guid Id { get; init; } = Guid.NewGuid();
   public string Username { get; set; } = string.Empty;
   public string Hashed { get; set; } = string.Empty;
   public string Salted { get; set; } = string.Empty;
   public Guid PersonId { get; set; } = Guid.Empty;
}