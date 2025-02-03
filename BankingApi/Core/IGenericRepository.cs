using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BankingApi.Core.DomainModel.Entities;
namespace BankingApi.Core; 

public interface IGenericRepository<T> where T : AEntity {
   
   // read from database?
   Task<IEnumerable<T>> SelectAsync  (bool withTracking = false);
   Task<T?>             FindByIdAsync(Guid id);
   Task<IEnumerable<T>> FilterByAsync(Expression<Func<T, bool>> predicate);
   Task<T?>             FindByAsync  (Expression<Func<T, bool>> predicate); 
   
   // write to in-memory repository
   void                 Add          (T item);
   void                 AddRange     (IEnumerable<T> items);
   Task                 UpdateAsync  (T item);
   void                 Remove       (T item);
}