using System.Net.Http.Headers;
using BankingClient.Dto;
using BankingClient.Services;
using Microsoft.AspNetCore.Components.Forms;
namespace BankingClient.Pages.Person;

public class HandleImage(
   ImagesService ImagesService,
   ILogger<HandleImage> Logger
) {
   
   public async Task<PersonDto?> Upload(
      IBrowserFile selectedFile,
      PersonDto _personDto
   ) {
      // multipart content
      using var content = new MultipartFormDataContent();
      content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
      
      // image as Formfile 
      await using var stream = selectedFile.OpenReadStream(maxAllowedSize: 20_000_000);
      StreamContent streamContent = new StreamContent(stream);
      streamContent.Headers.ContentType = new MediaTypeHeaderValue(selectedFile.ContentType);
      content.Add(streamContent, "file", selectedFile.Name);
        
      
      Logger.LogInformation("Uploading content: {0}", streamContent.Headers.ContentType);   
      // upload image as multipart content
      var imageUrl = await ImagesService.PostImage(content);
      Logger.LogInformation("Uploaded image file URL: {0}", imageUrl);
    
      if (imageUrl != null) {
         // handle success
         _personDto = _personDto with {
            LocalImageUrl = selectedFile.Name, // local file url
            ImageUrl = imageUrl // remote file url
         };
         Logger.LogInformation("{0}", _personDto);

         return _personDto;
      }
      // handle failure 
      return null;
   }
   
   public async Task<PersonDto?> DeleteImageFile(
      PersonDto _personDto
   ) {
      // delete the old image file - if exists
      if(_personDto.ImageUrl != null) {
         // delete the old image file
         if (await ImagesService.DeleteImage(_personDto.ImageUrl)) {
            // Handle success
            _personDto = _personDto with {
               LocalImageUrl = null,
               ImageUrl = null
            };
            return _personDto;
         } 
         // handle failure
         return null;
      } 
      // do nothing
      return _personDto;
   }
   
}