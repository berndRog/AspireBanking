using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BankingApi.Core.DomainModel.Entities;
namespace BankingApi.Core; 
public interface ITransactionsRepository: IGenericRepository<Transaction> {
   Task<IEnumerable<Transaction>> SelectByTransferIdAsync(Guid transferId);
   Task<IEnumerable<Transaction>> FilterByAccountIdAsync(
      Guid accountId,
      Expression<Func<Transaction, bool>>? predicate
   );
}