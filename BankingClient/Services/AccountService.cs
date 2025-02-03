using System.Text.Json;
using BankingClient.Core;
using BankingClient.Core.Dto;
using Microsoft.AspNetCore.Authorization;
namespace BankingClient.Services;

public class AccountService(
   IHttpClientFactory httpClientFactory,
   IConfiguration configuration,
   JsonSerializerOptions jsonOptions,
   CancellationTokenSource cancellationTokenSource,
   ILogger<AccountService> logger
) : BaseWebService<AccountService>(
   httpClientFactory: httpClientFactory,
   configuration: configuration,
   jsonOptions: jsonOptions,
   cancellationTokenSource: cancellationTokenSource,
   logger: logger
), IAccountService {
   
   
   // GET all accounts of owner by Id
   public async Task<ResultData<IEnumerable<AccountDto>?>> GetAllByOwner(
      Guid ownerId
   ) => await GetAllAsync<AccountDto>($"owners/{ownerId}/accounts");

   // GET account by Id
   public async Task<ResultData<AccountDto?>> GetById(
      Guid accountId
   ) => await GetAsync<AccountDto>($"accounts/{accountId}");

   // GET account by IBAN 
   public async Task<ResultData<AccountDto?>> GetByIban(
      string iban
   ) => await GetAsync<AccountDto>($"accounts/iban/{iban}");

   // POST (create) a new account
   [Authorize(Policy = "Admin")]
   public async Task<ResultData<AccountDto?>> Post(
      Guid ownerId, AccountDto accountDto
   ) => await PostAsync($"owners/{ownerId}/accounts", accountDto);
}