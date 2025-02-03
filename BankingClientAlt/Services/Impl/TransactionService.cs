using System.Net.Http.Json;
using System.Text.Json;
using BankingClient.Dto;
using BankingClient.ErrorHandling;
namespace BankingClient.Services.Impl;

public class TransactionService(
   HttpClient httpClient,
   IConfiguration configuration,
   ILogger<TransactionService> logger,
   ResponseErrors responseErrors,
   JsonSerializerOptions jsonOptions
): BaseWebService<TransactionService>(httpClient, configuration, logger, responseErrors, jsonOptions), 
   ITransactionService {
   
   private readonly HttpClient _httpClient = httpClient;
   private readonly IConfiguration _configuration = configuration;
   private readonly JsonSerializerOptions _jsonOptions = jsonOptions;

   // GET: /banking/v2/accounts/{accountId}/transactions/filter List transactions of an account by accountId and time interval start to end
   public async Task<IEnumerable<TransactionDto>?> GetTransactionsOfAccountById(
      Guid accountId, string start, string end
   ) => await GetAllAsync<TransactionDto>($"accounts/{accountId}/transactions/filter?start={start}&end={end}");
   
   //
   // {
   //    try {
   //       var baseUrl = _configuration["Api:BaseUrl"];
   //       var url = $"{baseUrl}/accounts/{accountId}/transactions/filter?start={start}&end={end}";
   //       var response = await _httpClient.GetAsync(url);
   //
   //       if (response.IsSuccessStatusCode) {
   //          return await response.Content.ReadFromJsonAsync<IEnumerable<TransactionDto>>(_jsonOptions)
   //             ?? new List<TransactionDto>();
   //       }
   //       var message = await response.Content.ReadAsStringAsync();
   //       throw new Exception($"Http Status Code: {response.StatusCode} message: {message}");
   //    }
   //    catch (Exception e) {
   //       Console.WriteLine(e);
   //       throw;
   //    }
   // }

   // GET: /banking/v2/accounts/{accountId}/transactions/filter List transactions of an account by accountId and time interval start to end
   public async Task<IEnumerable<TransactionDto>?> GetYearlyTransactionsByAccount(Guid accountId, int year) {
      try {
         var baseUrl = _configuration["Api:BaseUrl"];
         var url = $"{baseUrl}/accounts/{accountId}/transactions/filter?start={year}-01-01&end={year}-12-31";
         var response = await _httpClient.GetAsync(url);

         if (response.IsSuccessStatusCode) {
            return await response.Content.ReadFromJsonAsync<IEnumerable<TransactionDto>>(_jsonOptions)
               ?? new List<TransactionDto>();
         }

         var message = await response.Content.ReadAsStringAsync();
         throw new Exception($"Http Status Code: {response.StatusCode} message: {message}");
      }
      catch (Exception e) {
         Console.WriteLine(e);
         throw;
      }
   }
}