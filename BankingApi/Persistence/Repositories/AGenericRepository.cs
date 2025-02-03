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

internal abstract class AGenericRepository<T> : IGenericRepository<T>
   where T : AEntity {

   #region fields
   protected readonly DataContext _dbContext;
   protected readonly DbSet<T>    _typeDbSet;
   #endregion
   
   #region ctor
   protected AGenericRepository(
      DataContext dataContext
   ){
      _dbContext = dataContext;
      _typeDbSet = dataContext.Set<T>();
   }
   #endregion

   #region methods
   /// <summary>
   /// Select all items asynchronously
   /// </summary>
   /// <param name="withTracking">== false -> NoTracking, i.e. items are not loaded into the repository</param>
   /// <returns>IEnumerable{T}</returns>
   public virtual async Task<IEnumerable<T>> SelectAsync(bool withTracking = false) {
      IQueryable<T> query = _typeDbSet;
      if(!withTracking) query = query.AsNoTracking();
      return await query.ToListAsync(); 
   }

   /// <summary>
   /// Find an item by Id asynchronously
   /// </summary>
   /// <param name="id">Guid</param>
   /// <returns>T?</returns>
   public virtual async Task<T?> FindByIdAsync(Guid id) =>
      await _typeDbSet.FirstOrDefaultAsync(item => item.Id == id);
   
   /// <summary>
   /// Filter items by LINQ expression asynchronously
   /// </summary>
   /// <param name="predicate">LINQ expression tree used as filter</param>
   /// <returns>IEnumerable</returns>
   public virtual async Task<IEnumerable<T>> FilterByAsync(Expression<Func<T, bool>> predicate) =>
      await _typeDbSet.Where(predicate).ToListAsync();

   /// <summary>
   /// Find an item by LINQ expression asynchronously
   /// </summary>
   /// <param name="predicate">LINQ expression tree used as filter</param>
   /// <returns>T?</returns>
   public virtual async Task<T?> FindByAsync(Expression<Func<T, bool>> predicate) =>
      await _typeDbSet.FirstOrDefaultAsync(predicate);

   /// <summary>
   /// Write an item to the repository
   /// </summary>
   /// <param name="item">item to add</param>
   public void Add(T item) =>
      _typeDbSet.Add(item);

   /// <summary>
   /// Add a range of items to the repository
   /// </summary>
   /// <param name="items">items to add</param>
   public virtual void AddRange(IEnumerable<T> items) =>
      _typeDbSet.AddRange(items);

   /// <summary>
   /// Update an exiting item asynchronously, item with item.id must exist
   /// </summary>
   /// <param name="item">Item to update</param>
   /// <exception cref="ApplicationException">item with given id not found</exception>
   public async Task UpdateAsync(T item){
      var foundItem = await _typeDbSet.FirstOrDefaultAsync(i => i.Id == item.Id)
         ?? throw new ApplicationException("Update failed, item not found");
      _dbContext.Entry(foundItem).CurrentValues.SetValues(item);
      _dbContext.Entry(foundItem).State = EntityState.Modified;
   }
   
   /// <summary>
   /// Remove an item from the repository
   /// </summary>
   /// <param name="item">item to remove</param>
   public virtual void Remove(T item){
      _typeDbSet.Remove(item);
   }
   #endregion
}
