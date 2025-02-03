using System.Threading.Tasks;
namespace BankingApi.Core;

public interface ImagesRepository {
   Task<(byte[], string, string)> LoadImageFile(string filePath, string contentType);
   Task<string?> StoreImageFile(string root, byte[] fileContext, string fileExt);
}