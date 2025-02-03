using System.Text.Json;
using BankingClient.Core;
using BankingClient.Core.Dto;
namespace BankingClient.Services;

public class TransferService(
   IHttpClientFactory httpClientFactory,
   IConfiguration configuration,
   JsonSerializerOptions jsonOptions,
   CancellationTokenSource cancellationTokenSource,
   ILogger<TransferService> logger
) : BaseWebService<TransferService>(
   httpClientFactory: httpClientFactory,
   configuration: configuration,
   jsonOptions: jsonOptions,   
   cancellationTokenSource: cancellationTokenSource,
   logger: logger
), ITransferService {
   // Get Transfer by accountID
   public async Task<ResultData<IEnumerable<TransferDto>?>> GetByAccountId(
      Guid accountId
   ) => await GetAllAsync<TransferDto>($"accounts/{accountId}/transfers");

   // Send Transfer 
   public async Task<ResultData<TransferDto?>> SendTransfer(TransferDto transferDto, Guid accountId) =>
      await PostAsync<TransferDto>($"accounts/{accountId}/transfers", transferDto);
}