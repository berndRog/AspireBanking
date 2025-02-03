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
internal class TransactionsRepository(
   DataContext dataContext
// ILogger<TransactionsRepository> logger
): AGenericRepository<Transaction>(dataContext), ITransactionsRepository {
   
   public async Task<IEnumerable<Transaction>> FilterByAccountIdAsync(
      Guid accountId,
      Expression<Func<Transaction, bool>>? predicate
   ) {
      var query = _typeDbSet.AsQueryable()  // IQueryable<Transaction>
         .Where(t => t.AccountId == accountId).AsQueryable()
         .AsSplitQuery();
      if(predicate is not null) query = query
         .Where(predicate)
         .AsSingleQuery();
      return await query.ToListAsync();
   }
   
   public async Task<IEnumerable<Transaction>> SelectByTransferIdAsync(Guid transferId) {
      return await _typeDbSet
         .Where(transaction => transaction.TransferId == transferId)
         .ToListAsync();
   }
   
   public async Task DeleteAsync(Guid id) {
      var transaction = await _typeDbSet.FindAsync(id) 
         ?? throw new Exception("Transaction DeleteById: Id not found");
      _typeDbSet.Remove(transaction);
   }
}