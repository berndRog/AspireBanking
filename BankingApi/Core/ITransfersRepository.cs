using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankingApi.Core.DomainModel.Entities;
namespace BankingApi.Core; 
public interface ITransfersRepository: IGenericRepository<Transfer> {
   
   Task<IEnumerable<Transfer>> SelectByAccountIdAsync(Guid accountId);
   Task<IEnumerable<Transfer>> SelectByBeneficiaryIdAsync(Guid beneficiaryId);
   
}