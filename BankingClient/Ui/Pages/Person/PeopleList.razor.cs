using BankingClient.Core;
using BankingClient.Core.Dto;
using BankingClient.Services;
using Microsoft.AspNetCore.Components;
namespace BankingClient.Ui.Pages.Person;

public partial class PeopleList(
   IPersonService personService,
   NavigationManager navigationManager,
   ILogger<PeopleList> logger
) {
   private IEnumerable<PersonDto> _personDtos = [];
   private string? _errorMessage = null;

   
   protected override async Task OnInitializedAsync() {
      var resultData = await personService.GetAllPersons();
      switch (resultData) {
         case ResultData<IEnumerable<PersonDto>?>.Success sucess:
            logger.LogInformation("PeopleList: GetPeople Suceess: {1}", sucess.Data!.Count());
            _personDtos = sucess.Data!;
            break;
         case ResultData<IEnumerable<PersonDto>?>.Error error:
            logger.LogError("PeopleList: GetPeople Error: {Error}", error.Exception.Message);
            _errorMessage = error.Exception.Message;
            break;
      }
   }

   private void LoadPerson(PersonDto personDto) {
      logger.LogInformation("LoadPerson {personDto}", personDto);
      navigationManager.NavigateTo($"/person/{personDto.Id}");
   }
}