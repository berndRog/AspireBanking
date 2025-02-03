using System;
using System.Collections.Generic;
using System.Net.Mime;
using BankingApi.Core.Misc;
namespace BankingApi.Core.DomainModel.Entities;

public class Person : AEntity {
   public override Guid Id { get; init; } = Guid.NewGuid();
   public string FirstName { get; set; } = string.Empty;
   public string LastName { get; set; } = string.Empty;
   public string? Email { get; set; } = null;
   public string? Phone { get; set; } = null;
   public string? localImageUrl { get; set; } = null;
   public string? imageUrl { get; set; } = null;   
}