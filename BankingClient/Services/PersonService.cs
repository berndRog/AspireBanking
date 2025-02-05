using System.Text.Json;
using BankingClient.Core;
using BankingClient.Core.Dto;
namespace BankingClient.Services;

public class PersonService(
   WebServiceOptions<PersonService> options
): BaseWebService<PersonService>(options), IPersonService {
   
   // GET  all persons
   public async Task<ResultData<IEnumerable<PersonDto>?>> GetAllPersons() =>
      await GetAllAsync<PersonDto>($"people");
   
   // GET person by Id
   public async Task<ResultData<PersonDto?>> GetPersonById(Guid id) =>
      await GetAsync<PersonDto>($"people/{id}");
   
   // POST (create) a person
   public async Task<ResultData<PersonDto?>> PostPerson(PersonDto personDto) =>
      await PostAsync("people", personDto);
   
   // PUT (update) a person
   public async Task<ResultData<PersonDto?>> PutPerson(PersonDto personDto) =>
      await PutAsync($"people/{personDto.Id}", personDto);
   
   // Delete Person 
   public async Task<ResultData<object?>> DeletePerson(Guid personId) =>
      await DeleteAsync($"people/{personId}");
}
      