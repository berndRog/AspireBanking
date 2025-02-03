using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BankingApi.Core;
using BankingApi.Core.DomainModel.Entities;
using Microsoft.EntityFrameworkCore;
[assembly: InternalsVisibleTo("BankingApiTest")]
namespace BankingApi.Persistence.Repositories;
internal class OwnersRepository(
   DataContext dataContext
) : AGenericRepository<Owner>(dataContext), IOwnersRepository {
   
   public async Task<bool?> ExistsByUserNameAsync(string userName) {
      var owner =  await _typeDbSet.FirstOrDefaultAsync(o => o.UserName == userName);
      return (owner != null);
   }

   public async Task<Owner?> FindByUserNameAsync(string userName) =>
      await _typeDbSet.FirstOrDefaultAsync(o => o.UserName == userName);
   

   public async Task<Owner?> FindByUserIdAsync(string userId) =>
      await _typeDbSet.FirstOrDefaultAsync(o => o.UserId == userId);      
   
   public async Task<IEnumerable<Owner>> LikeNameByAsync(string name) =>
      await _typeDbSet
         .Where(o => EF.Functions.Like(o.FirstName+" "+o.LastName, "%"+name.Trim()+"%"))
         .ToListAsync(); 
   
   public async Task<Owner?> FindByIdJoinAsync(Guid id, bool join) {
      var query = _typeDbSet.AsQueryable(); // IQueryable<Owner>
      if (join) query = query.Include(o => o.Accounts).AsSingleQuery();
      return await query.FirstOrDefaultAsync(o => o.Id == id);
   }
   
   public async Task<Owner?> FindByJoinAsync(
      Expression<Func<Owner, bool>> predicate,
      bool join
   ) {
      var query = _typeDbSet.AsQueryable(); // IQueryable<Owner>
      if (join) query = query.Include(o => o.Accounts).AsSingleQuery();
      return await query.FirstOrDefaultAsync(predicate);
   }
   
   // When applying Include to load related data, you can add enumerable operations
   // to the included collection navigation, which allows for filtering and sorting
   // of the results.
   // Supported operations are: Where, OrderBy, OrderByDescending,
   // ThenBy, ThenByDescending, Skip, and Take.
   // SELECT owner.*, accounts.*
   // FROM(
   //    SELECT *
   //    FROM Owners AS o
   //    WHERE o.Id = id
   //    LIMIT 1
   // ) AS owner
   // LEFT JOIN(
   //   SELECT *
   //   FROM Accounts AS "a"
   //   WHERE a.OwnerId = id
   // ) AS accounts
   // ON owner.Id = accounts.OwnerId
   public async Task<Owner?> FindByIdJoin2Async(
      Guid id,
      bool joinAccounts
   ) {
      // Filtered include
      if (joinAccounts) {
         return await _typeDbSet.Include(owner => 
               owner.Accounts.Where(account => account.OwnerId == id))
                             .FirstOrDefaultAsync(o => o.Id == id);
      }
      return await _typeDbSet.FirstOrDefaultAsync(o => o.Id == id);
   }
}