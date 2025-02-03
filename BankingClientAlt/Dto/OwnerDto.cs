namespace BankingClient.Dto;

/// <summary>
/// OwnerDto (Kontoinhaber)
/// </summary>
public record OwnerDto(
   Guid Id,
   string FirstName,
   string LastName,
   DateTime Birthdate,
   string? Email,
   string? ImagePath,
   string? UserId  // must be encrypted
);
