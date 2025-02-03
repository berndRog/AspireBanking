using System.Text.Json;
using BankingClient.Dto;
using BankingClient.ErrorHandling;
using BankingClient.Services.Impl;
namespace BankingClient.Services;

public class PeopleService(
   HttpClient httpClient,
   IConfiguration configuration,
   ILogger<PeopleService> logger,
   ResponseErrors responseErrors,
   JsonSerializerOptions jsonOptions
) : BaseWebService<PeopleService>(httpClient, configuration, logger, responseErrors, jsonOptions),
   IPeopleService {

   // Get Person /banking/v2/people: Get all persons
   public async Task<IEnumerable<PersonDto>> GetAllPersons() =>
      await GetAllAsync<PersonDto>($"people");


   // Get Person /banking/v2/people/{personid}: Get person by Id
   public async Task<PersonDto?> GetPersonById(Guid id) =>
      await GetAsync<PersonDto>($"people/{id}");


   // Post Person /banking/v2/people: Create an owner
   public async Task<PersonDto?> PostPerson(PersonDto personDto) =>
      await PostAsync<PersonDto>("people", personDto);


   // Put Person /banking/v2/people/{personId}: Update an owner
   public async Task<PersonDto?> PutPerson(PersonDto personDto) =>
      await PutAsync<PersonDto>($"people/{personDto.Id}", personDto);


   // Delete Person /banking/v2/people/{personId}: Delete an owner
   public async Task<bool> DeletePerson(Guid personId) =>
      await DeleteAsync($"people/{personId}");
}
      