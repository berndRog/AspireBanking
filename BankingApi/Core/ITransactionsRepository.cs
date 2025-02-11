using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BankingApi.Core.DomainModel.Entities;
using BankingApi.Core.Dto;
namespace BankingApi.Core; 
public interface ITransactionsRepository: IGenericRepository<Transaction> {
   
   Task<IEnumerable<Transaction>> SelectByTransferIdAsync(Guid transferId);

   Task<IEnumerable<Transaction>> FilterByAccountIdAsync(
      Guid accountId,
      Expression<Func<Transaction, bool>>? predicate
   );
   
   Task<IEnumerable<TransactionListItemDto>> FilterListItemsByAccountIdAsync(
      Guid accountId,
      Expression<Func<Transaction, bool>>? predicate
   );
}