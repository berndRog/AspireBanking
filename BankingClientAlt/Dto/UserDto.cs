namespace BankingClient.Dto;
public record UserDto(
   Guid Id,
   string Username,
   string Password,
   Guid PersonId
);