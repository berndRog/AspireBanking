using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankingApi.Core.DomainModel.Entities;
namespace BankingApi.Core; 
public interface IAccountsRepository: IGenericRepository<Account> {

   Task<IEnumerable<Account>> SelectByOwnerIdAsync(Guid ownerId);
      
   Task<Account?> FindByIdJoinAsync(
      Guid id,
      bool joinOwner = false, 
      bool joinBeneficiaries = false,
      bool joinTransfers = false,
      bool joinTransactions = false
   );

}