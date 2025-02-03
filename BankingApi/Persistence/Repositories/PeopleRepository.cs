using BankingApi.Core;
using BankingApi.Core.DomainModel.Entities;
namespace BankingApi.Persistence.Repositories;

internal class PeopleRepository(
   DataContext dataContext
) : AGenericRepository<Person>(dataContext), IPeopleRepository {
   
}