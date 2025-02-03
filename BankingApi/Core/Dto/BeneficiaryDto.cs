using System;
namespace BankingApi.Core.Dto;

/// <summary>
/// BenecifiaryDto (Empfänger von Überweisungen)
/// </summary>
public record BeneficiaryDto(
   Guid Id,
   string LastName,
   string FirstName,
   string Iban,
   Guid AccountId = default
);