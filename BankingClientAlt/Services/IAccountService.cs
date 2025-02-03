using BankingClient.Dto;
namespace BankingClient.Services;

public interface IAccountService {
   Task<IEnumerable<AccountDto>?> GetAllAccountsByOwner(Guid ownerId);
   Task<AccountDto?> GetAccountById(Guid accountId);
   Task<AccountDto?> GetAccountByIban(string iban);
   Task<AccountDto?> PostAccount(Guid ownerId, AccountDto accountDto);
}