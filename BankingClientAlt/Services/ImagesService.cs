namespace BankingClient.Services;

public interface ImagesService {
   Task<bool> FindImage(string imageUrl);
   Task<(byte[]?, string, string)> GetImage(string imageUrl);
   Task<string?> PostImage(MultipartFormDataContent content);
   Task<bool> DeleteImage(string imageUrl);
}