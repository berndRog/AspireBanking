using BankingClient.Dto;
namespace BankingClient.Services;


public interface ITransferService
{
   Task<IEnumerable<TransferDto>> GetTransfersByAccountId(Guid accountId);
   Task<TransferDto?> SendTransfer(TransferDto transferDto, Guid accountId);
}