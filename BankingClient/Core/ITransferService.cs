using BankingClient.Core.Dto;
namespace BankingClient.Core;

public interface ITransferService {
   Task<ResultData<IEnumerable<TransferDto>?>> GetByAccountId(Guid accountId);
   Task<ResultData<TransferDto?>> SendTransfer(TransferDto transferDto, Guid accountId);
}