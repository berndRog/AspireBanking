using System;
namespace BankingApi.Core.Dto;

/// <summary>
/// AccountDto (Bankkonto)
/// </summary>
public record AccountDto(
   Guid Id,
   string Iban,
   double Balance,
   Guid OwnerId = default
) {
   public AccountDto() : this(Guid.Empty, string.Empty, 0.0) { 
     Iban = Iban.Replace(" ", "").ToUpper();
   } 
}
