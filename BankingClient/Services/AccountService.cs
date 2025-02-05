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
   
   // Get all accounts 
   [Authorize(Policy = "AdminPolicy")]
   public async Task<ResultData<IEnumerable<AccountDto>?>> GetAll() =>
      await GetAllAsync<AccountDto>($"accounts");
   
   // GET all accounts of owner by Id
   [Authorize(Policy = "CombinedPolicy")]
   public async Task<ResultData<IEnumerable<AccountDto>?>> GetAllByOwner(
      Guid ownerId
   ) => await GetAllAsync<AccountDto>($"owners/{ownerId}/accounts");

   // GET account by Id
   [Authorize(Policy = "CombinedPolicy")]
   public async Task<ResultData<AccountDto?>> GetById(
      Guid accountId
   ) => await GetAsync<AccountDto>($"accounts/{accountId}");

   // GET account by IBAN 
   [Authorize(Policy = "CombinedPolicy")]
   public async Task<ResultData<AccountDto?>> GetByIban(
      string iban
   ) => await GetAsync<AccountDto>($"accounts/iban/{iban}");

   // POST (create) a new account
   [Authorize(Policy = "AdminPolicy")]
   public async Task<ResultData<AccountDto?>> Post(
      Guid ownerId, AccountDto accountDto
   ) => await PostAsync($"owners/{ownerId}/accounts", accountDto);
}