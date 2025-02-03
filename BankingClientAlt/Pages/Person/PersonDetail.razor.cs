using BankingClient.Dto;
using BankingClient.Model;
using BankingClient.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
namespace BankingClient.Pages.Person;

public partial class PersonDetail {
  
  private readonly PersonDetailModel _personDetail = new ();
  
  private PersonDto? _personDto; 
    
  private IBrowserFile? selectedFile = null;
  private string? _imageUrl;
  private string? _imageBase64;
  private string _errorMessage = string.Empty;  
  private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

  [Parameter] public Guid Id { get; set; }
  
  [Inject] private ILogger<PersonDetail> Logger { get; set; } = null!;
  [Inject] private HandleImage HandleImage { get; set; } = null!;
  [Inject] private IPeopleService PeopleService { get; set; } = null!;
  [Inject] private ImagesService ImagesService { get; set; } = null!;
  [Inject] private NavigationManager NavigationManager { get; set; } = null!;

  // Handle the file selection for a person image
  private async Task HandleFileSelected(InputFileChangeEventArgs e) {
    selectedFile = e.File;
    Logger.LogInformation("image file selected: " + selectedFile.Name);
    await using var stream = selectedFile.OpenReadStream(maxAllowedSize: 10_000_000);
    using var memoryStream = new MemoryStream();
    await stream.CopyToAsync(memoryStream);
    var base64 = Convert.ToBase64String(memoryStream.ToArray());
    _imageBase64 = $"data:{selectedFile.ContentType};base64,{base64}";
  }
  
  protected override async Task OnInitializedAsync() {
    try {
      _personDto = await PeopleService.GetPersonById(Id);
      Logger.LogInformation("OnInitialize() GetById {1}", _personDto);
      if(_personDto == null) {
        _errorMessage = "Person nicht gefunden";
        Logger.LogError(_errorMessage);
        return;
      }

      if (_personDto.ImageUrl != null) {
        //var (image, contentType, fileName) = await ImagesService.GetImage(_personDto.ImageUrl);
        //var base64 = Convert.ToBase64String(image);
        _imageBase64 = _personDto.ImageUrl;
      }
      _personDetail.Firstname = _personDto.Firstname;
      _personDetail.Lastname = _personDto.Lastname;
      _personDetail.Email = _personDto.Email;
      _personDetail.Phone = _personDto.Phone;
    } catch (Exception ex) {
      _errorMessage = ex.Message;
      Logger.LogError(_errorMessage);
    }
  }
  
  private async Task HandleSubmit() {
    PersonDto? personDtoResult = null;
    
    // Handle the form submission
    try {
      _personDto = _personDto with {
        Firstname = _personDetail.Firstname,
        Lastname = _personDetail.Lastname,
        Email = _personDetail.Email,
        Phone = _personDetail.Phone
      };

      // upload image file as multipart content
      // --------------------------------------
      if (selectedFile != null) {
        // delete the old image file
        _personDto = await HandleImage.DeleteImageFile(_personDto);
        Logger.LogInformation("after delete image: {0}", _personDto);
        if(_personDto == null) {
          _errorMessage = "Fehler beim Löschen des alten Fotos";
          Logger.LogError(_errorMessage);
          return;
        }
        // upload the new image file
        Logger.LogInformation("before upload image: {0}", _personDto);
        _personDto = await HandleImage.Upload(selectedFile, _personDto);
        Logger.LogInformation("after upload image: {0}", _personDto);
        if(_personDto == null) {
            _errorMessage = "Fehler beim Upload des Fotos";
            Logger.LogError(_errorMessage);
        //  NavigationManager.NavigateTo("/index");
            return;
        };        
      }
    
      // Update the person
      _personDto = await PeopleService.PutPerson(_personDto);
      if(_personDto == null) {
        _errorMessage = "Fehler beim Update der Person";
        Logger.LogError(_errorMessage);
        return;
      }
      
      NavigationManager.NavigateTo("/personen");
    } catch (Exception ex) {
      _errorMessage = ex.Message;
    }
  }

  // leave the form
  private void LeaveForm() {
    NavigationManager.NavigateTo("/personen");
  }

  // cancel the operation
  private void CancelOperation() {
    _cancellationTokenSource.Cancel();
  }  
}