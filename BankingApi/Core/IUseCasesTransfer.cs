using System;
using System.Threading.Tasks;
using BankingApi.Core.DomainModel.Entities;
namespace BankingApi.Core; 
public interface IUseCasesTransfer {

   Task<ResultData<Transfer>> SendMoneyAsync(
      Guid accountDebitId,    // Account from which the transfer is made (debit) 
      Transfer transfer
   );
   
   Task<ResultData<Transfer>> ReverseMoneyAsync(
      Guid originalTransferId,          
      Transfer reverseTransfer
   );
}