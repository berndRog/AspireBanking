using System;
using System.Threading.Tasks;
using BankingApi.Core.DomainModel.Entities;
namespace BankingApi.Core.UseCases; 
public class UseCasesTransfer(
   IAccountsRepository accountsRepository,
   IBeneficiariesRepository beneficiariesRepository,
   ITransfersRepository transfersRepository,
   ITransactionsRepository transactionsRepository,
   IDataContext dataContext
): IUseCasesTransfer {
   
   private (Transaction debit, Transaction credit) CreateTransactions(DateTime date, double amount) {
      var debit = new Transaction();
      var credit = new Transaction();
      // Lastschrift
      debit.Set(date: date);
      debit.Set(amount: -amount);
      // Gutschrift
      credit.Set(date: date);
      credit.Set(amount);
      return (debit, credit);
   }
   
   public async Task<ResultData<Transfer>> SendMoneyAsync(
      Guid accountDebitId,         // Account from which the transfer is made (debit) 
      Transfer transfer            // Transfer data transferDto
   ){
      try {
         // check if debit account exists (Lastschrift)
         var accountDebit = await accountsRepository.FindByIdAsync(accountDebitId);
         // check if the benificiary exits
         Beneficiary? beneficiary = null;
         if (transfer.BeneficiaryId != null) {
            var beneficiaryId = (Guid)transfer.BeneficiaryId!; // convert nullable to non-nullable
            beneficiary = await beneficiariesRepository.FindByIdAsync(beneficiaryId);
         }
         // check if credit account exists (Gutschrift)
         Account? accountCredit = null;
         if (beneficiary?.Iban != null) {
            string beneficiaryIban = beneficiary.Iban!; // convert nullable to non-nullable
            accountCredit = await accountsRepository.FindByAsync(a => a.Iban == beneficiaryIban);
         }
         
         // return errorcodes when accountDebit, accountCredit or beneficiary is null
         if(accountDebit == null)
            return new Error<Transfer>(404,"Debit Account for the transfer doesn't exist.");
         if(accountCredit == null)
            return new Error<Transfer>(404, "Credit Account (Iban) for the transfer doesn't exist."); 
         if(beneficiary == null) 
            return new Error<Transfer>(404, "Beneficiary for the transfer doesn't exist.");
         if(transfer.BeneficiaryId == null) 
            return new Error<Transfer>(404, "BeneficiaryId for the transfer not given.");
         if(await transfersRepository.FindByIdAsync(transfer.Id) != null)
            return new Error<Transfer>(409, "Transfer already exists.");
         
         // set accountDebitId for transfer
         transfer.Set(accountId: accountDebitId);
         // add transfer to debit account and beneficiary
         accountDebit.Add(transfer, beneficiary);
         
         // create transactions
         var (transactionDebit, transactionCredit) = 
            CreateTransactions(transfer.Date, transfer.Amount);
         // add transaction to debit account (Lastschrift)
         accountDebit.Add(transactionDebit, transfer);
         // add transaction to credit account (Gutschrift)     
         accountCredit.Add(transactionCredit, transfer);

         // save to transfers-/transactionsRepository and write to database
         transfersRepository.Add(transfer);
         transactionsRepository.Add(transactionDebit);
         transactionsRepository.Add(transactionCredit);
         await dataContext.SaveAllChangesAsync();

         return new Success<Transfer>(201, transfer);
         
      } catch (Exception e) {
         return new Error<Transfer>(500, e.Message);
      }
   }

   public async Task<ResultData<Transfer>> ReverseMoneyAsync(
      Guid originalTransferId,
      Transfer reverseTransfer
   ) {
      
      // check if original transfer exists
      var originalTransfer = await transfersRepository.FindByIdAsync(originalTransferId);
      if(originalTransfer == null)
         return new Error<Transfer>(404,"Reverse Money: Original transfer doesn't exist."); 
      
      // check if debit account exists (Lastschrift -> Gutschrift)
      var accountDebit = await accountsRepository.FindByIdAsync(originalTransfer.AccountId);
      if(accountDebit == null)
         return new Error<Transfer>(404,"Debit Account for the transfer doesn't exist."); 

      // check if beneficiary exists      
      if(originalTransfer.BeneficiaryId == null)
         return new Error<Transfer>(400,"Reverse Money: BeneficiaryId is null");
      var beneficiaryId = (Guid) originalTransfer.BeneficiaryId!; // convert nullable to non-nullable
      var beneficiary = await beneficiariesRepository.FindByIdAsync(beneficiaryId);
      if(beneficiary == null) 
         return new Error<Transfer>(400,"Beneficiary for the transfer doesn't exist.");
      
      // check if credit account exists (Gutschrift -> Lastschrift)
      var accountCredit = await accountsRepository.FindByAsync(a => a.Iban == beneficiary.Iban);
      if(accountCredit == null)
         return new Error<Transfer>(404,"Credit Account (Iban) for the transfer doesn't exist.");
      
      // transactionDebit + transactionCredit should not be null 
      // var originalTransactions =
      //    await transactionsRepository.SelectByTransferIdAsync(originalTransferId);
      // if (originalTransactions.Count() != 2)
      //    return new Error<Transfer>(404,"Reverse Money: Original transactions are not valid.");
      // var originalTransactionDebit = originalTransactions.FirstOrDefault(t => t.Amount < 0.0);
      // if(originalTransactionDebit == null)          
      //    return new Error<Transfer>(404, "Reverse Money: Original debit transaction not found."); 
      // var originalTransactionCredit = originalTransactions.FirstOrDefault(t => t.Amount >= 0.0);
      // if(originalTransactionCredit == null) 
      //    return new Error<Transfer>(404, "Reverse Money: Original credit transaction not found.");     

      // Create transfer and two transactions
      reverseTransfer.Set(-originalTransfer.Amount);
      Transaction reverseTransactionDebit = new ();
      reverseTransactionDebit.Set(amount: +originalTransfer.Amount);
      Transaction reverseTransactionCredit = new();
      reverseTransactionCredit.Set(amount: -originalTransfer.Amount);
      
      // and add transfer to account
      accountDebit.Add(reverseTransfer, beneficiary);
      // Create transactionFrom (Original debit) - Lastschrift
      accountDebit.Add(reverseTransactionDebit, reverseTransfer);
      // Create transactionTo (Credit)  - Gutschrift     
      accountCredit.Add(reverseTransactionCredit, reverseTransfer);       
      
      transfersRepository.Add(reverseTransfer);             
      transactionsRepository.Add(reverseTransactionDebit);            
      transactionsRepository.Add(reverseTransactionCredit);      
      await dataContext.SaveAllChangesAsync();     

      return new Success<Transfer>(0,reverseTransfer);
   }
}