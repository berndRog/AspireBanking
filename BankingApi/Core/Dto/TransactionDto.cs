using System;
namespace BankingApi.Core.Dto;

/// <summary>
/// TransactionDto (Buchung)
/// </summary>
public record TransactionDto(
   Guid Id,
   DateTime Date,
   double Amount,
   Guid? AccountId = default,
   Guid? TransferId = default
);