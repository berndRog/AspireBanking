using BankingClient.Dto;
namespace BankingClient.Services;

public interface ITransactionService {
   Task<IEnumerable<TransactionDto>?> GetTransactionsOfAccountById(Guid accountId, string start, string end);
   Task<IEnumerable<TransactionDto>?> GetYearlyTransactionsByAccount(Guid accountId, int year);
}