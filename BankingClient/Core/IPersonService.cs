using BankingClient.Core.Dto;
namespace BankingClient.Core;

public interface IPersonService {
   Task<ResultData<IEnumerable<PersonDto>?>> GetAllPersons();
   Task<ResultData<PersonDto?>> GetPersonById(Guid id);
   Task<ResultData<PersonDto?>> PostPerson(PersonDto personDto);
   Task<ResultData<PersonDto?>> PutPerson(PersonDto personDto);
   Task<ResultData<object?>> DeletePerson(Guid personId);
}