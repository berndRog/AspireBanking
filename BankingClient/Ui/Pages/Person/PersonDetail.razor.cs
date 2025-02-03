using BankingClient.Core;
using BankingClient.Core.Dto;
using BankingClient.Services;
using BankingClient.Ui.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
namespace BankingClient.Ui.Pages.Person;

public partial class PersonDetail(
  HandleImage handleImage,
  IPersonService personService,
  ImagesService imagesService,
  NavigationManager navigationManager,
  ILogger<PersonDetail> logger
) {

  [Parameter] public Guid Id { get; set; }
  
  private readonly PersonDetailModel _personDetail = new ();
  private PersonDto? _personDto; 
    
  private IBrowserFile? selectedFile = null;
  private string? _imageUrl;
  private string? _imageBase64;
  private string _errorMessage = string.Empty;  
  private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
  
  // Handle the file selection for a person image
  private async Task HandleFileSelected(InputFileChangeEventArgs e) {
    selectedFile = e.File;
    logger.LogInformation("image file selected: " + selectedFile.Name);
    await using var stream = selectedFile.OpenReadStream(maxAllowedSize: 10_000_000);
    using var memoryStream = new MemoryStream();
    await stream.CopyToAsync(memoryStream);
    var base64 = Convert.ToBase64String(memoryStream.ToArray());
    _imageBase64 = $"data:{selectedFile.ContentType};base64,{base64}";
  }
  
  protected override async Task OnInitializedAsync() {
    try {
      var resultData = await personService.GetPersonById(Id);
      switch (resultData) {
        case ResultData<PersonDto?>.Success sucess:
          logger.LogInformation("PersonDetail: GetPersonById Suceess: {1}", sucess.Data);
          _personDto = sucess.Data!;
          break;
        case ResultData<PersonDto?>.Error error:
          logger.LogError("PersonDetail: GetPersonById Error: {1}", error.Exception.Message);
          _errorMessage = error.Exception.Message;
          break;
      }

      if (_personDto!.ImageUrl != null) {
        //var (image, contentType, fileName) = await imagesService.GetImage(_personDto.ImageUrl);
        //var base64 = Convert.ToBase64String(image);
        _imageBase64 = _personDto.ImageUrl;
      }
      _personDetail.Firstname = _personDto.Firstname;
      _personDetail.Lastname = _personDto.Lastname;
      _personDetail.Email = _personDto.Email;
      _personDetail.Phone = _personDto.Phone;
    } catch (Exception ex) {
      _errorMessage = ex.Message;
      logger.LogError(_errorMessage);
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
        _personDto = await handleImage.DeleteImageFile(_personDto);
        logger.LogInformation("after delete image: {0}", _personDto);
        if(_personDto == null) {
          _errorMessage = "Fehler beim Löschen des alten Fotos";
          logger.LogError(_errorMessage);
          return;
        }
        // upload the new image file
        logger.LogInformation("before upload image: {0}", _personDto);
        _personDto = await handleImage.Upload(selectedFile, _personDto);
        logger.LogInformation("after upload image: {0}", _personDto);
        if(_personDto == null) {
            _errorMessage = "Fehler beim Upload des Fotos";
            logger.LogError(_errorMessage);
        //  navigationManager.NavigateTo("/index");
            return;
        };        
      }
    
      // Update the person
      var resultData = await personService.PutPerson(_personDto);
      switch (resultData) {
        case ResultData<PersonDto?>.Success sucess:
          logger.LogInformation("PersonDetail: PutPerson Suceess: {1}", sucess.Data);
          _personDto = sucess.Data!;
          break;
        case ResultData<PersonDto?>.Error error:
          logger.LogError("PersonDetail: PutPerson Error: {1}", error.Exception.Message);
          _errorMessage = error.Exception.Message;
          break;
      }
      navigationManager.NavigateTo("/personen");
    } catch (Exception ex) {
      _errorMessage = ex.Message;
    }
  }

  // leave the form
  private void LeaveForm() {
    navigationManager.NavigateTo("/personen");
  }

  // cancel the operation
  private void CancelOperation() {
    _cancellationTokenSource.Cancel();
  }  
}