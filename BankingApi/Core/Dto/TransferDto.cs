using System;
namespace BankingApi.Core.Dto;

/// <summary>
/// TransferDto (Überweisung an einen Empfänger)
/// </summary>
public record TransferDto (
   Guid Id,
   DateTime Date,
   string Description,
   double Amount,
   Guid AccountId = default,
   Guid? BeneficiaryId = null
);