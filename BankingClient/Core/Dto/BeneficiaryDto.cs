﻿namespace BankingClient.Core.Dto;

/// <summary>
/// BenecifiaryDto (Empfänger von Überweisungen)
/// </summary>
public record BeneficiaryDto(
   Guid Id,
   string FirstName,
   string LastName,
   string Iban,
   Guid AccountId
);