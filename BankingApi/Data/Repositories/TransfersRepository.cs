using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BankingApi.Core;
using BankingApi.Core.DomainModel.Entities;
using Microsoft.EntityFrameworkCore;
[assembly: InternalsVisibleTo("BankingApiTest")]
namespace BankingApi.Data.Repositories;
internal class TransfersRepository(
   DataContext dataContext
): AGenericRepository<Transfer>(dataContext), ITransfersRepository {

   public async Task<IEnumerable<Transfer>> SelectByAccountIdAsync(Guid accountId) 
      => await _typeDbSet.Include(transfer => transfer.Transactions)
         .Where(transfer => transfer.AccountId == accountId)
         .ToListAsync();
   
   public async Task<IEnumerable<Transfer>> SelectByBeneficiaryIdAsync(Guid beneficiaryId) 
      => await _typeDbSet.Include(transfer => transfer.Transactions)
         .Where(transfer => transfer.BeneficiaryId == beneficiaryId)
         .ToListAsync();
}