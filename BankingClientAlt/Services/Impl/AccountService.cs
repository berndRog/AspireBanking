using System.Text.Json;
using BankingClient.Dto;
using BankingClient.ErrorHandling;
namespace BankingClient.Services.Impl;

public class AccountService(
   HttpClient httpClient,
   IConfiguration configuration,
   ILogger<AccountService> logger,
   ResponseErrors responseErrors,
   JsonSerializerOptions jsonOptions
): BaseWebService<AccountService>(httpClient, configuration, logger, responseErrors, jsonOptions), 
   IAccountService {
   
   // GET: /banking/v2/owners/{ownerid}/accounts Get all accounts of owner with given ID
   public async Task<IEnumerable<AccountDto>?> GetAllAccountsByOwner(
      Guid ownerId
   ) => await GetAllAsync<AccountDto>($"/owners/{ownerId}/accounts");
   
   // GET: /banking/v2/accounts/{id} Get account by ID
   public async Task<AccountDto?> GetAccountById(
      Guid accountId
   ) => await GetAsync<AccountDto>($"accounts/{accountId}");

   // GET: /banking/v2/accounts/{iban} Find Account by IBAN field
   public async Task<AccountDto?> GetAccountByIban(
      string iban
   ) => await GetAsync<AccountDto>($"accounts/{iban}");
   
   // POST: /banking/v2/owners/{ownerId}/accounts Create a new account. 
   public async Task<AccountDto?> PostAccount(
      Guid ownerId, AccountDto accountDto
   ) => await PostAsync<AccountDto>($"owners/{ownerId}/accounts", accountDto);
   
}