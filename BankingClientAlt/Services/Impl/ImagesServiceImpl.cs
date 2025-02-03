using System.Text.Json;
using System.Web;
using BankingClient.ErrorHandling;
using BankingClient.Services.Impl;
namespace BankingClient.Services;

public class ImagesServiceImpl(
   HttpClient httpClient,
   IConfiguration configuration,
   ILogger<ImagesServiceImpl> logger,
   ResponseErrors responseErrors,
   JsonSerializerOptions jsonOptions
) : ImagesService {
   
   // Get image by {imageUrl}
   public async Task<bool> FindImage(string imageUrl) {
      try {
         var uriBuilder = new UriBuilder();
         var query = HttpUtility.ParseQueryString(uriBuilder.Query);
         query["imageUrl"] = imageUrl;
         uriBuilder.Query = query.ToString();
         var url = uriBuilder.ToString();
         
         logger.LogInformation("FindImage: " + url);
         var response = await httpClient.GetAsync(url);

         if (response.IsSuccessStatusCode) return true;
         await responseErrors.Handle<ImagesServiceImpl>(response, logger);
      }
      catch (Exception e) {
         responseErrors.Handle(e);
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
         var response = await httpClient.GetAsync("{imageUrl}");
         if (response.IsSuccessStatusCode) {
            var result = await response.Content.ReadAsStringAsync();
            var (fileBytes, fileContentType, fileName) =
               JsonSerializer.Deserialize<(byte[], string, string)>(result, jsonOptions);
            return (fileBytes, fileContentType, fileName);
         }
         responseErrors.Handle(response, logger);
      }
      catch (Exception e) {
         responseErrors.Handle(e);
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
         
         var response = await httpClient.PostAsync(url, content);
         if (response.IsSuccessStatusCode) {
            return await response.Content.ReadAsStringAsync();
         }
         
         await responseErrors.Handle(response, logger);
      }
      catch (Exception e) {
         responseErrors.Handle(e);
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
         var response = await httpClient.DeleteAsync(imageUrl);
         if (response.IsSuccessStatusCode) return true;
         
         await responseErrors.Handle(response, logger);
      }
      catch (Exception e) {
         responseErrors.Handle(e);
      }
      return false;
   }
}