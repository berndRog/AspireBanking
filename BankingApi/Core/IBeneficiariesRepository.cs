using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankingApi.Core.DomainModel.Entities;
namespace BankingApi.Core; 
public interface IBeneficiariesRepository: IGenericRepository<Beneficiary> {
   Task<Beneficiary?> FindByOwnersNameAsync(string firstName, string lastName);
   Task<IEnumerable<Beneficiary>> SelectByAccountIdAsync(Guid accountId);
}
