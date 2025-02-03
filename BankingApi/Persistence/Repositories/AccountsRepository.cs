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

internal class AccountsRepository(
   DataContext dataContext
) : AGenericRepository<Account>(dataContext), IAccountsRepository {
   
   public async Task<IEnumerable<Account>> SelectByOwnerIdAsync(
      Guid ownerId
   ) {
      var query = _typeDbSet.AsQueryable() // IQueryable<Account>
         .Where(a => a.OwnerId == ownerId);
      return await query.ToListAsync();
   }
   
   public async Task<Account?> FindByIdJoinAsync(
      Guid id,
      bool joinOwner,
      bool joinBeneficiaries,
      bool joinTransfers,
      bool joinTransactions
   ) {
      var query = _typeDbSet.AsQueryable();  // IQueryable<Account>
      query = query.Where(a => a.Id == id);     
      if (joinOwner) query = query.Include(a => a.Owner);
      if (joinBeneficiaries) query = query.Include(a => a.Beneficiaries);
      if (joinTransfers) query = query.Include(a => a.Transfers).ThenInclude(t => t.Transactions);
      if (joinTransactions) query = query.Include(a => a.Transactions);
      query = query.AsSplitQuery();
      return await query.FirstOrDefaultAsync();
   }
   

}