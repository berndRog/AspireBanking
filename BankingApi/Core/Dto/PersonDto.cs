using System;
namespace BankingApi.Core.Dto;

/// <summary>
/// PersonDto 
/// </summary>
public record PersonDto(
   Guid Id,
   string Firstname,
   string Lastname,
   string? Email,
   string? Phone,
   string? LocalImageUrl,
   string? ImageUrl
);