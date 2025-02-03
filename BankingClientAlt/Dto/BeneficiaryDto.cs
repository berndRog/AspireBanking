namespace BankingClient.Dto;

/// <summary>
/// BenecifiaryDto (Empfänger von Überweisungen)
/// </summary>
public record BeneficiaryDto(
   Guid Id,
   string Name,
   string Iban,
   Guid AccountId
);