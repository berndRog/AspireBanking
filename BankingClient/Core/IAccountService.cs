using BankingClient.Core.Dto;
namespace BankingClient.Core;

public interface IAccountService {
   Task<ResultData<IEnumerable<AccountDto>?>> GetAll();
   Task<ResultData<IEnumerable<AccountDto>?>> GetAllByOwner(Guid ownerId);
   Task<ResultData<AccountDto?>> GetById(Guid accountId);
   Task<ResultData<AccountDto?>> GetByIban(string iban);
   Task<ResultData<AccountDto?>> Post(Guid ownerId, AccountDto accountDto);
}