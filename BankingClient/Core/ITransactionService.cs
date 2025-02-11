using BankingClient.Core.Dto;
namespace BankingClient.Core;

public interface ITransactionService {
   //Task<ResultData<IEnumerable<TransactionDto>?>> FilterByAccountId(Guid accountId, string start, string end);
   Task<ResultData<IEnumerable<TransactionListItemDto>?>> FilterListItemsByAccountId(Guid accountId, string start, string end);
}