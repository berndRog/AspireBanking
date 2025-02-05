using System.Net.Http.Json;
using System.Text.Json;
using BankingClient.Core;
namespace BankingClient.Services;

public abstract class BaseWebService<TService> (
   WebServiceOptions<TService> options
) where TService : class  {
   // the base address is http://localhost:5000/banking/v3/
   protected readonly HttpClient _httpClient = options.HttpClientFactory.CreateClient("BankingApi");
   protected readonly JsonSerializerOptions _jsonOptions = options.JsonOptions;
   protected readonly CancellationToken _cancellationToken = options.CancellationTokenSource.Token;
   protected readonly ILogger<TService> _logger = options.Logger;
   
   protected Task<ResultData<IEnumerable<TDto>?>> GetAllAsync<TDto>(string path) where TDto : class {
      return SendRequestAsync<IEnumerable<TDto>>(
         () => _httpClient.GetAsync(path, _cancellationToken));
   }

   protected Task<ResultData<TDto?>> GetAsync<TDto>(string path) where TDto : class {
      return SendRequestAsync<TDto>(
         () => _httpClient.GetAsync(path, _cancellationToken));
   }

   protected Task<ResultData<TDto?>> PostAsync<TDto>(string path, TDto dto) where TDto : class {
      return SendRequestAsync<TDto>(
         () => _httpClient.PostAsJsonAsync(path, dto, _jsonOptions, _cancellationToken));
   }

   protected Task<ResultData<TDto?>> PutAsync<TDto>(string path, TDto dto) where TDto : class {
      return SendRequestAsync<TDto>(
         () => _httpClient.PutAsJsonAsync(path, dto, _jsonOptions, _cancellationToken));
   }

   protected Task<ResultData<object?>> DeleteAsync(string path) {
      return SendRequestAsync<object>(
         () => _httpClient.DeleteAsync(path, _cancellationToken));
   }

   private async Task<ResultData<TResponse?>> SendRequestAsync<TResponse>(
      Func<Task<HttpResponseMessage>> httpRequest // lambda function
   ) where TResponse : class {
      try {
         
         // send the request
         var response = await httpRequest();
         
         // throw an exception if the response is not successful
         response.EnsureSuccessStatusCode();
         
         // if the response is successful, return the response
         var data = await response.Content
            .ReadFromJsonAsync<TResponse>(_jsonOptions, _cancellationToken);
         return new ResultData<TResponse?>.Success(data);
      }
      
      // catch different exception type an return ResultData.Error()
      catch (HttpRequestException e) when (e.StatusCode.HasValue) {
         _logger.LogError(e, "HTTP error {StatusCode}", e.StatusCode);
         return new ResultData<TResponse?>.Error(
            new Exception($"HTTP error {e.StatusCode}: {e.Message}"));
      }
      catch (OperationCanceledException e) {
         _logger.LogWarning("Request was canceled: {e}",e.Message);
         return new ResultData<TResponse?>.Error(
            new Exception($"Request was canceled: {e.Message}."));
      }
      catch (Exception e) {
         var location = $"Error in {nameof(TService)}:SendRequestAsync():";
         _logger.LogError("{0} {1} ", location, e.Message);
         return new ResultData<TResponse?>.Error(
            new Exception($"{location} {e.Message}"));
      }
   }
   
   // version before the code was simplified
   // protected async Task<IEnumerable<TDto>> GetAllAsync<TDto>(string path) where TDto : class  {
   //    try {
   //       var baseUrl = configuration["Api:BaseUrl"];
   //       var url = $"{baseUrl}/{path}";
   //       var response = await httpClient.GetAsync(url);
   //
   //       if (response.IsSuccessStatusCode) {
   //          return await response.Content.ReadFromJsonAsync<IEnumerable<TDto>>(jsonOptions)
   //             ?? new List<TDto>();
   //       }
   //       var message = errorHandler.Handle(response, logger);
   //       throw new Exception($"Http Status Code: {response.StatusCode} message: {message}");
   //    }
   //    catch (Exception e) {
   //       var message = $"Error in {nameof(T)}:GetAllAsync():";
   //       logger.LogError(e, "{0} {1} ", message, e.Message);
   //       throw new Exception(message + e.Message);
   //    }
   // }
   //
   // protected async Task<TDto?> GetAsync<TDto>(string path) where TDto : class {
   //    try {
   //       var baseUrl = configuration["Api:BaseUrl"];
   //       var url = $"{baseUrl}/{path}";
   //       var response = await httpClient.GetAsync(url);
   //
   //       if (response.IsSuccessStatusCode) {
   //          return await response.Content.ReadFromJsonAsync<TDto>(jsonOptions);
   //       }
   //       var message = await errorHandler.Handle<T>(response, logger);
   //       throw new Exception($"Http Status Code: {response.StatusCode} message: {message}");
   //    }
   //    catch (Exception e) {
   //       var message = $"Error in {nameof(T)}:GetAsync():";
   //       logger.LogError(e, "{0} {1} ", message, e.Message);
   //       throw new Exception(message + e.Message);
   //    }
   // }
   //
   // protected async Task<TDto?> PostAsync<TDto>(string path, TDto dto) where TDto : class  {
   //    try {
   //       var baseUrl = configuration["Api:BaseUrl"];
   //       var url = $"{baseUrl}/{path}";
   //       var response = await httpClient.PostAsJsonAsync(url, dto, jsonOptions);
   //
   //       if (response.IsSuccessStatusCode) {
   //          return await response.Content.ReadFromJsonAsync<TDto>(jsonOptions);
   //       }
   //       var message = await errorHandler.Handle<T>(response, logger);
   //       throw new Exception($"Http Status Code: {response.StatusCode} message: {message}");
   //    }
   //    catch (Exception e) {
   //       var message = $"Error in {nameof(T)}:PostAsync():";
   //       logger.LogError(e, "{0} {1} ", message, e.Message);
   //       throw new Exception(message + e.Message);
   //    }
   // }
   //
   // protected async Task<TDto?> PutAsync<TDto>(string path, TDto dto) where TDto : class  {
   //    try {
   //       var baseUrl = configuration["Api:BaseUrl"];
   //       var url = $"{baseUrl}/{path}";
   //       var response = await httpClient.PutAsJsonAsync(url, dto, jsonOptions);
   //
   //       if (response.IsSuccessStatusCode) {
   //          return await response.Content.ReadFromJsonAsync<TDto>(jsonOptions);
   //       }
   //       var message =  await errorHandler.Handle<T>(response, logger);
   //       throw new Exception($"Http Status Code: {response.StatusCode} message: {message}");
   //    }
   //    catch (Exception e) {
   //       var message = $"Error in {nameof(T)}:PutAsync():";
   //       logger.LogError(e, "{0} {1} ", message, e.Message);
   //       throw new Exception(message + e.Message);
   //    }
   // }
   //
   // protected async Task<bool> DeleteAsync(string path)  {
   //    try {
   //       var baseUrl = configuration["Api:BaseUrl"];
   //       var url = $"{baseUrl}/{path}";
   //       var response = await httpClient.DeleteAsync(url);
   //
   //       if (response.IsSuccessStatusCode) return true;
   //       
   //       var message = await errorHandler.Handle<T>(response, logger);
   //       throw new Exception($"Http Status Code: {response.StatusCode} message: {message}");
   //    }
   //    catch (Exception e) {
   //       var message = $"Error in {nameof(T)}:PutAsync():";
   //       logger.LogError(e, "{0} {1} ", message, e.Message);
   //       throw new Exception(message + e.Message);
   //    }
   // }
}