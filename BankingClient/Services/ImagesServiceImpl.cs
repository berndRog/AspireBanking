using System.Text.Json;
using System.Web;
using BankingClient.Core;
using Microsoft.AspNetCore.Authorization;
namespace BankingClient.Services;

[Authorize(Policy = "User")]
public class ImagesServiceImpl(
   IHttpClientFactory httpClientFactory,
   IConfiguration configuration,
   JsonSerializerOptions jsonOptions,
   CancellationTokenSource cancellationTokenSource,
   ILogger<ImagesServiceImpl> logger
) : ImagesService {
   
   private readonly HttpClient _httpClient = httpClientFactory.CreateClient("BankingApi");
   private readonly CancellationToken _cancellationToken = cancellationTokenSource.Token;
   
   // Get image by {imageUrl}
   public async Task<bool> FindImage(string imageUrl) {
      try {
         var uriBuilder = new UriBuilder();
         var query = HttpUtility.ParseQueryString(uriBuilder.Query);
         query["imageUrl"] = imageUrl;
         uriBuilder.Query = query.ToString();
         var url = uriBuilder.ToString();
         
         logger.LogInformation("FindImage: " + url);
         var response = await _httpClient.GetAsync(url, _cancellationToken);

         if (response.IsSuccessStatusCode) return true;
         //await errorHandler.Handle<ImagesServiceImpl>(response, logger);
      }
      catch (Exception e) {
         //errorHandler.Handle(e);
      }
      return false;
   }

   /// <summary>
   /// Get Image, return byte[], content type and fileName
   /// </summary>
   /// <param name="imageUrl"></param>
   /// <returns>Task{ byte[], string, string}</returns>
   public async Task<(byte[]?, string, string)> GetImage(string imageUrl) {
      try {
         var response = await _httpClient.GetAsync("{imageUrl}, _cancellationToken);");
         if (response.IsSuccessStatusCode) {
            var result = await response.Content.ReadAsStringAsync();
            var (fileBytes, fileContentType, fileName) =
               JsonSerializer.Deserialize<(byte[], string, string)>(result, jsonOptions);
            return (fileBytes, fileContentType, fileName);
         }
         //errorHandler.Handle(response, logger);
      }
      catch (Exception e) {
         //errorHandler.Handle(e);
      }
      return (null, string.Empty, string.Empty);
   }

   /// <summary>
   /// Post image /imageFiles: Upload an image file
   /// </summary>
   /// <param name="content">Multipart Content</param>
   /// <returns>imageUrl:String?</returns>
   public async Task<string?> PostImage(MultipartFormDataContent content) {
      try {
         var baseUrl = configuration["Api:BaseUrl"];
         var url = $"{baseUrl}/images";
         
         var response = await _httpClient.PostAsync(url, content, _cancellationToken);
         if (response.IsSuccessStatusCode) {
            return await response.Content.ReadAsStringAsync(_cancellationToken);
         }
         
         //await errorHandler.Handle(response, logger);
      }
      catch (Exception e) {
         //errorHandler.Handle(e);
      }
      return null;
   }

   /// <summary>
   /// Delete image file with {imageUrl}
   /// </summary>
   /// <param name="imageUrl"></param>
   /// <returns></returns>
   /// <exception cref="NotImplementedException"></exception>
   public async Task<bool> DeleteImage(string imageUrl) {
      try {
         var response = await _httpClient.DeleteAsync(imageUrl, _cancellationToken);
         if (response.IsSuccessStatusCode) return true;
         
         //await errorHandler.Handle(response, logger);
      }
      catch (Exception e) {
         //errorHandler.Handle(e);
      }
      return false;
   }
}