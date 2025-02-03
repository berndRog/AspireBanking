using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BankingApi.Core.DomainModel.Entities;
namespace BankingApi.Core; 

public interface IOwnersRepository: IGenericRepository<Owner> {
   Task<bool?> ExistsByUserNameAsync(string userName);
   Task<Owner?> FindByUserNameAsync(string userName);
   Task<Owner?> FindByUserIdAsync(string userId);
   Task<IEnumerable<Owner>> LikeNameByAsync(string name);
   Task<Owner?> FindByIdJoinAsync(Guid id, bool join);
   Task<Owner?> FindByJoinAsync(
      Expression<Func<Owner, bool>> predicate,
      bool join
   );
}