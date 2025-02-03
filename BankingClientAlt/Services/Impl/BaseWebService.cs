using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BankingClient.ErrorHandling;
namespace BankingClient.Services.Impl;

public abstract class BaseWebService<T> (
   HttpClient httpClient,
   IConfiguration configuration,
   ILogger<T> logger,
   ResponseErrors responseErrors,
   JsonSerializerOptions jsonOptions
) where T : class  {
   
   
   protected Task<IEnumerable<TDto>?> GetAllAsync<TDto>(string path) where TDto : class {
      var baseUrl = configuration["Api:BaseUrl"];
      var url = $"{baseUrl}/{path}";
      return SendRequestAsync<IEnumerable<TDto>>(() => httpClient.GetAsync(url));
   }

   protected Task<TDto?> GetAsync<TDto>(string path) where TDto : class {
      var baseUrl = configuration["Api:BaseUrl"];
      var url = $"{baseUrl}/{path}";
      return SendRequestAsync<TDto>(() => httpClient.GetAsync(url));
   }

   protected Task<TDto?> PostAsync<TDto>(string path, TDto dto) where TDto : class {
      var baseUrl = configuration["Api:BaseUrl"];
      var url = $"{baseUrl}/{path}";
      return SendRequestAsync<TDto>(() => httpClient.PostAsJsonAsync(url, dto, jsonOptions));
   }

   protected Task<TDto?> PutAsync<TDto>(string path, TDto dto) where TDto : class {
      var baseUrl = configuration["Api:BaseUrl"];
      var url = $"{baseUrl}/{path}";
      return SendRequestAsync<TDto>(() => httpClient.PutAsJsonAsync(url, dto, jsonOptions));
   }

   protected async Task<bool> DeleteAsync(string path) {
      var baseUrl = configuration["Api:BaseUrl"];
      var url = $"{baseUrl}/{path}";
      var response = await SendRequestAsync<object>(() => httpClient.DeleteAsync(url));
      return response != null;
   }

   private async Task<TResponse?> SendRequestAsync<TResponse>(
      Func<Task<HttpResponseMessage>> httpRequest
   ) where TResponse : class {
      try {
         var response = await httpRequest();

         if (response.IsSuccessStatusCode) {
            return await response.Content.ReadFromJsonAsync<TResponse>(jsonOptions);
         }

         await responseErrors.Handle<T>(response, logger);
         throw new Exception();
      }
      catch (Exception e) {
         var message = $"Error in {nameof(T)}:SendRequestAsync():";
         logger.LogError(e, "{0} {1} ", message, e.Message);
         throw new Exception(message + e.Message);
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
   //       var message = responseErrors.Handle(response, logger);
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
   //       var message = await responseErrors.Handle<T>(response, logger);
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
   //       var message = await responseErrors.Handle<T>(response, logger);
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
   //       var message =  await responseErrors.Handle<T>(response, logger);
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
   //       var message = await responseErrors.Handle<T>(response, logger);
   //       throw new Exception($"Http Status Code: {response.StatusCode} message: {message}");
   //    }
   //    catch (Exception e) {
   //       var message = $"Error in {nameof(T)}:PutAsync():";
   //       logger.LogError(e, "{0} {1} ", message, e.Message);
   //       throw new Exception(message + e.Message);
   //    }
   // }
   
   
   
}