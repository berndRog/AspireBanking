using System.Text.Json;
using BankingClient.Dto;
using BankingClient.ErrorHandling;
using Microsoft.AspNetCore.Components;
namespace BankingClient.Services.Impl;

public class TransferService(
    HttpClient httpClient,
    IConfiguration configuration,
    ILogger<TransferService> logger,
    ResponseErrors responseErrors,
    JsonSerializerOptions jsonOptions
): BaseWebService<TransferService>(httpClient, configuration, logger, responseErrors, jsonOptions), 
     ITransferService {
    
    // Get Transfer by accountID
    public async Task<IEnumerable<TransferDto>> GetTransfersByAccountId(Guid accountId) =>
        await GetAllAsync<TransferDto>($"/accounts/{accountId}/transfers");
    
    // Send Transfer 
    public async Task<TransferDto?> SendTransfer(TransferDto transferDto, Guid accountId) =>
        await PostAsync<TransferDto>($"accounts/{accountId}/transfers", transferDto);
}