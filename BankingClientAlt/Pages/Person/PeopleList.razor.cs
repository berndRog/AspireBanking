using BankingClient.Dto;
using BankingClient.Services;
using Microsoft.AspNetCore.Components;
namespace BankingClient.Pages.Person;

public partial class PeopleList {
   private IEnumerable<PersonDto> _personenDto = [];
   private string? _errorMessage = null;

   [Inject] private ILogger<PeopleList> Logger { get; set; } = null!;
   [Inject] private IPeopleService PeopleService { get; set; } = null!;
   [Inject] private NavigationManager NavigationManager { get; set; } = null!;
   
   protected override async Task OnInitializedAsync() {
      try {
         //throw new Exception("Test Exception");
         _personenDto = await PeopleService.GetAllPersons();
      }
      catch (Exception e) {
         _errorMessage = e.Message;
      }
   }

   private void LoadPerson(PersonDto personDto) {
      Logger.LogInformation("LoadPerson {personDto}", personDto);
      NavigationManager.NavigateTo($"/person/{personDto.Id}");
   }
}