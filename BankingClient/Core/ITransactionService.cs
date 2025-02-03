using BankingClient.Core.Dto;
namespace BankingClient.Core;

public interface ITransactionService {
   Task<ResultData<IEnumerable<TransactionDto>?>> GetByAccountId(Guid accountId, string start, string end);
   Task<ResultData<IEnumerable<TransactionDto>?>> GetYearlyByAccountId(Guid accountId, int year);
}