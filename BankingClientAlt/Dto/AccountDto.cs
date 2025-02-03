namespace BankingClient.Dto;

/// <summary>
/// AccountDto (Bankkonto)
/// </summary>
public record AccountDto(
   Guid Id,
   string Iban,
   double Balance,
   Guid OwnerId
);
