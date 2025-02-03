using System;
namespace BankingApi.Core.Dto;

/// <summary>
/// OwnerDto (Kontoinhaber)
/// </summary>
public record OwnerDto(
   Guid Id,
   string FirstName,
   string LastName,
   string? Email,
   string? UserName,
   string? UserId,
   string? UserRole
); 
   

