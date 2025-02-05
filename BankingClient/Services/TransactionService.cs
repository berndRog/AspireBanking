using System.Text.Json;
using BankingClient.Core;
using BankingClient.Core.Dto;
namespace BankingClient.Services;

public class TransactionService(
   WebServiceOptions<TransactionService> options
): BaseWebService<TransactionService>(options), ITransactionService {

   // GET a list of transactions of an account by accountId and time interval start to end
   public async Task<ResultData<IEnumerable<TransactionDto>?>> GetByAccountId(
      Guid accountId, 
      string start, 
      string end
   ) => await GetAllAsync<TransactionDto>($"accounts/{accountId}/transactions/filter?start={start}&end={end}");
   
   // GET a list of transactions of an account by accountId and time interval start to end
   public async Task<ResultData<IEnumerable<TransactionDto>?>> GetYearlyByAccountId(
      Guid accountId, 
      int year
   ) => await GetAllAsync<TransactionDto>($"accounts/{accountId}/transactions/filter?start={year}-01-01&end={year}-12-31");
   
   //  GET: /banking/v2/accounts/{accountId}/transactions/filter List transactions of an account by accountId and time interval start to end
   // public async Task<IEnumerable<TransactionDto>?> GetYearlyByAccountId(
   //    Guid accountId, string start, string end
   // ){
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
   // public async Task<IEnumerable<TransactionDto>?> GetYearlyByAccountId(Guid accountId, int year) {
   //    try {
   //       var baseUrl = _configuration["Api:BaseUrl"];
   //       var url = $"{baseUrl}/accounts/{accountId}/transactions/filter?start={year}-01-01&end={year}-12-31";
   //       var response = await _httpClient.GetAsync(url);
   //
   //       if (response.IsSuccessStatusCode) {
   //          return await response.Content.ReadFromJsonAsync<IEnumerable<TransactionDto>>(_jsonOptions)
   //             ?? new List<TransactionDto>();
   //       }
   //
   //       var message = await response.Content.ReadAsStringAsync();
   //       throw new Exception($"Http Status Code: {response.StatusCode} message: {message}");
   //    }
   //    catch (Exception e) {
   //       Console.WriteLine(e);
   //       throw;
   //    }
   // }
}