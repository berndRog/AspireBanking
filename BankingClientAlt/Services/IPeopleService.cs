using BankingClient.Dto;
namespace BankingClient.Services;

public interface IPeopleService {
   Task<IEnumerable<PersonDto>> GetAllPersons();
   Task<PersonDto?> GetPersonById(Guid id);
   Task<PersonDto?> PostPerson(PersonDto personDto);
   Task<PersonDto?> PutPerson(PersonDto personDto);
   Task<bool> DeletePerson(Guid personId);
}