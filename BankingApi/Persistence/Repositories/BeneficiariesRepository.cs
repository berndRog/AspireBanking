using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BankingApi.Core;
using BankingApi.Core.DomainModel.Entities;
using Microsoft.EntityFrameworkCore;
[assembly: InternalsVisibleTo("BankingApiTest")]
namespace BankingApi.Persistence.Repositories;
internal class BeneficiariesRepository(
   DataContext dataContext
) : AGenericRepository<Beneficiary>(dataContext), IBeneficiariesRepository {
   
   
   public async Task<Beneficiary?> FindByOwnersNameAsync(string firstName, string lastName) {
      return await _typeDbSet
         .FirstOrDefaultAsync(b => b.FirstName == firstName && b.LastName == lastName);


      // return await _typeDbSet
      //    .Where(b => EF.Functions.Like(b.FirstName+" "+b.LastName, "%"+compare.Trim()+"%"))
      //    .ToListAsync(); 
   }

   public async Task<IEnumerable<Beneficiary>> SelectByAccountIdAsync(Guid accountId){
      return await _typeDbSet
         .Where(b => b.AccountId == accountId)
         .ToListAsync();         
   }
}