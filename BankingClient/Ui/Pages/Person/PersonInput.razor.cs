using System.Net.Http.Headers;
using BankingClient.Core;
using BankingClient.Core.Dto;
using BankingClient.Services;
using BankingClient.Ui.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
namespace BankingClient.Ui.Pages.Person;

public partial class PersonInput(
  HandleImage handleImage,
  IPersonService personService,
  ImagesService imagesService,
  NavigationManager navigationManager,
  ILogger<PersonInput> logger
) {

  private string? _imageBase64;
  private string _errorMessage = string.Empty;  
  private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

  
  private readonly PersonCreateModel _personCreate = new ();
  
  private PersonDto _personDto = new (
    Id: Guid.NewGuid(),
    Firstname: string.Empty,
    Lastname: string.Empty,
    Email: string.Empty,
    Phone: null,
    LocalImageUrl: null,
    ImageUrl: null
  );    
  
  
  private IBrowserFile? _selectedFile;

  private async Task HandleFileSelected(InputFileChangeEventArgs? e) {
    try {
      if (e?.File == null) {
        _errorMessage = "Keine Datei ausgewählt!";
        return;
      }
      _selectedFile = e.File;
      const long maxFileSize = 10_000_000; // 10MB Limit

      await using var stream = _selectedFile.OpenReadStream(maxFileSize);
      using var memoryStream = new MemoryStream();
      await stream.CopyToAsync(memoryStream);

      var base64 = Convert.ToBase64String(memoryStream.ToArray());
      _imageBase64 = $"data:{_selectedFile.ContentType};base64,{base64}";
    }
    catch (Exception ex) {
      _errorMessage = $"Fehler beim Laden des Bildes: {ex.Message}";
    }
  }

    
  private async Task HandleSubmit() {
    logger.LogError("!!! HandleSubmit !!");
    PersonDto? personDtoResult = null;
    
    // Handle the form submission
    try {
        // update personDto with input data
        _personDto = _personDto with {
          Firstname = _personCreate.Firstname,
          Lastname = _personCreate.Lastname,
          Email = _personCreate.Email,
          Phone = _personCreate.Phone
        };

        // upload image file as multipart content
        // --------------------------------------
        logger.LogInformation("_selectedFile: {1} {2}", _selectedFile?.Name, _selectedFile?.ContentType);
        if (_selectedFile != null) {
          personDtoResult = await handleImage.Upload(_selectedFile, _personDto);
          if(personDtoResult == null) {
            _errorMessage = "Fehler beim Upload image file";
            logger.LogError(_errorMessage);
          //navigationManager.NavigateTo("/index");
            return;
          }
          _personDto = personDtoResult;
        }
        
        // create the person (POST)
        var resultData = await personService.PostPerson(_personDto);
        switch (resultData) {
          case ResultData<PersonDto?>.Success sucess:
            logger.LogInformation("PersonDetail: PostPerson Suceess: {1}", sucess.Data);
            personDtoResult = sucess.Data!;
            break;
          case ResultData<PersonDto?>.Error error:
            logger.LogError("PersonDetail: PostPerson Error: {1}", error.Exception.Message);
            _errorMessage = error.Exception.Message;
            break;
        }
        if (personDtoResult == null) {
          _errorMessage = "Post personDto fails";
          return;
        }
        _personDto = personDtoResult;
        
    } catch (Exception ex) {
      _errorMessage = ex.Message;
      logger.LogError(_errorMessage);
      return;
    } 
    
    // create person successfully -> create user 
    // if (_personDto != null) {
    //   logger.LogInformation("Person created successful");
    //   // add input data to userDto
    //   _userDto = _userDto with { 
    //     Username = _personCreate.Username,
    //     Password = _personCreate.Password,
    //     PersonId = personDtoResult.Id
    //   };
    //   try {
    //     var userResult = await AuthService.Register(_userDto);
    //     if (userResult) {
    //       logger.LogInformation("User registered successful");
    //       //navigationManager.NavigateTo("/index");
    //     }
    //     else {
    //       _errorMessage = "Registrieren ist fehlgeschlagen.";
    //       logger.LogError(_errorMessage);
    //
    //       //await personService.DeletePerson(personResult.Id);
    //     }
    //   } catch (Exception ex) {
    //     _errorMessage = "Person konnte nicht erstellt werden.";
    //     logger.LogError(_errorMessage);
    //   }
    // }
    navigationManager.NavigateTo("/people");
    
  }

  private async Task<bool> UploadImageFile() {
    // multipart content
    using var content = new MultipartFormDataContent();
    // image as Formfile 
    await using var stream = _selectedFile.OpenReadStream(maxAllowedSize: 20_000_000);
    var fileContent = new StreamContent(stream);
    fileContent.Headers.ContentType = new MediaTypeHeaderValue(_selectedFile.ContentType);
    content.Add(fileContent, "file", _selectedFile.Name);
          
    // upload image as multipart content
    var imageUrl = await imagesService.PostImage(content);
    logger.LogInformation("Uploaded image file URL: {1}", imageUrl);
    
    if (imageUrl != null) {
      // Handle success
      _personDto = _personDto with {
        LocalImageUrl = _selectedFile.Name, // local file url
        ImageUrl = imageUrl // remote file url
      };
      return true;
    }
    // Handle failure
    _errorMessage = "Uploading the image file fails";
    logger.LogError(_errorMessage);
    navigationManager.NavigateTo("/index");
    return false;
  }

  // leave the form
  private void LeaveForm() {
    navigationManager.NavigateTo("/person");
  }

  // cancel the operation
  private void CancelOperation() {
    _cancellationTokenSource.Cancel();
    navigationManager.NavigateTo("/index");
  }  
}
