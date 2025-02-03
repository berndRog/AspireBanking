using System.Net.Http.Headers;
using BankingClient.Dto;
using BankingClient.Model;
using BankingClient.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
namespace BankingClient.Pages.Person;

public partial class PersonInput {
  
  private readonly PersonInputModel _personInput = new ();
  private PersonDto _personDto = new (
    Id: Guid.NewGuid(),
    Firstname: string.Empty,
    Lastname: string.Empty,
    Email: string.Empty,
    Phone: null,
    LocalImageUrl: null,
    ImageUrl: null
  ); 
  private UserDto _userDto = new (
    Id: Guid.NewGuid(),
    Username: string.Empty,
    Password: string.Empty,
    PersonId: Guid.Empty
  ); 
  
  private IBrowserFile? selectedFile = null;
  private string? _imageBase64;
  private string _errorMessage = string.Empty;  
  private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

  [Inject] private ILogger<PersonInput> Logger { get; set; } = null!;
  [Inject] private HandleImage HandleImage { get; set; } = null!;
  [Inject] private IPeopleService PeopleService { get; set; } = null!;
  [Inject] private ImagesService ImagesService { get; set; } = null!;
  [Inject] private NavigationManager NavigationManager { get; set; } = null!;
  
  // Handle the file selection for a person image
  private async Task HandleFileSelected(InputFileChangeEventArgs e) {
    selectedFile = e.File;
    Logger.LogInformation("image file selected: " + selectedFile.Name);
    
    using var stream = selectedFile.OpenReadStream(maxAllowedSize: 10_000_000);
    using var memoryStream = new MemoryStream();
    await stream.CopyToAsync(memoryStream);
    var base64 = Convert.ToBase64String(memoryStream.ToArray());
    _imageBase64 = $"data:{selectedFile.ContentType};base64,{base64}";
  }
    
  private async Task HandleSubmit() {
    Logger.LogError("!!! HandleSubmit !!");
    PersonDto? personDtoResult = null;
    
    // Handle the form submission
    try {
        // update personDto with input data
        _personDto = _personDto with {
          Firstname = _personInput.Firstname,
          Lastname = _personInput.Lastname,
          Email = _personInput.Email,
          Phone = _personInput.Phone
        };

        // upload image file as multipart content
        // --------------------------------------
        Logger.LogInformation("selectedFile: {1} {2}", selectedFile?.Name, selectedFile?.ContentType);
        if (selectedFile != null) {
          personDtoResult = await HandleImage.Upload(selectedFile, _personDto);
          if(personDtoResult == null) {
            _errorMessage = "Fehler beim Upload image file";
            Logger.LogError(_errorMessage);
          //NavigationManager.NavigateTo("/index");
            return;
          }
          _personDto = personDtoResult;
        }
        
        // create the person (POST)
        // ------------------------
        personDtoResult = await PeopleService.PostPerson(_personDto);
        if (personDtoResult == null) {
          // Handle failure
          _errorMessage = "Post personDto fails";
          Logger.LogError(_errorMessage);
          //NavigationManager.NavigateTo("/index");
          return;
        }
        _personDto = personDtoResult;
    } catch (Exception ex) {
      _errorMessage = ex.Message;
      Logger.LogError(_errorMessage);
      return;
    } 
    
    // create person successfully -> create user 
    // if (_personDto != null) {
    //   Logger.LogInformation("Person created successful");
    //   // add input data to userDto
    //   _userDto = _userDto with { 
    //     Username = _personInput.Username,
    //     Password = _personInput.Password,
    //     PersonId = personDtoResult.Id
    //   };
    //   try {
    //     var userResult = await AuthService.Register(_userDto);
    //     if (userResult) {
    //       Logger.LogInformation("User registered successful");
    //       //NavigationManager.NavigateTo("/index");
    //     }
    //     else {
    //       _errorMessage = "Registrieren ist fehlgeschlagen.";
    //       Logger.LogError(_errorMessage);
    //
    //       //await PeopleService.DeletePerson(personResult.Id);
    //     }
    //   } catch (Exception ex) {
    //     _errorMessage = "Person konnte nicht erstellt werden.";
    //     Logger.LogError(_errorMessage);
    //   }
    // }
    NavigationManager.NavigateTo("/people");
    
  }

  private async Task<bool> UploadImageFile() {
    // multipart content
    using var content = new MultipartFormDataContent();
    // image as Formfile 
    await using var stream = selectedFile.OpenReadStream(maxAllowedSize: 20_000_000);
    var fileContent = new StreamContent(stream);
    fileContent.Headers.ContentType = new MediaTypeHeaderValue(selectedFile.ContentType);
    content.Add(fileContent, "file", selectedFile.Name);
          
    // upload image as multipart content
    var imageUrl = await ImagesService.PostImage(content);
    Logger.LogInformation("Uploaded image file URL: {1}", imageUrl);
    
    if (imageUrl != null) {
      // Handle success
      _personDto = _personDto with {
        LocalImageUrl = selectedFile.Name, // local file url
        ImageUrl = imageUrl // remote file url
      };
      return true;
    }
    // Handle failure
    _errorMessage = "Uploading the image file fails";
    Logger.LogError(_errorMessage);
    NavigationManager.NavigateTo("/index");
    return false;
  }

  // leave the form
  private void LeaveForm() {
    NavigationManager.NavigateTo("/person");
  }

  // cancel the operation
  private void CancelOperation() {
    _cancellationTokenSource.Cancel();
    NavigationManager.NavigateTo("/index");
  }  
}
