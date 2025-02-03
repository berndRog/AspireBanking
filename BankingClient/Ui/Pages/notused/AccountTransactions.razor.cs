using BankingClient.Core;
using BankingClient.Core.Dto;
using Microsoft.AspNetCore.Components;
namespace BankingClient.Ui.Pages.notused;

public partial class AccountTransactions(
   IAccountService accountService,
   ITransactionService transactionService,
   ILogger<AccountTransactions> logger
) {
   
   [Parameter] public Guid AccountId { get; set; }
   
   private IEnumerable<TransactionDto>? _transactions;
   private IEnumerable<AccountDto>? _accounts;
   private AccountDto? _account;
   private DateTime? _startDate;
   private DateTime? _endDate;
   private bool _showTransactions;
   private Guid _selectedAccountId;
   private int _selectedYear;
   private string? _errorMessage = null;

   protected override async Task OnInitializedAsync() {
      var resultData = await accountService.GetById(AccountId);
      switch (resultData) {
         case ResultData<AccountDto?>.Success sucess:
            logger.LogInformation("AccountTransaction: GetById Suceess: {1}", sucess.Data);
            _account = sucess.Data!;
            break;
         case ResultData<AccountDto?>.Error error:
            logger.LogError("AccountTransaction: GetnById Error: {1}", error.Exception.Message);
            _errorMessage = error.Exception.Message;
            break;
      } 
   }

   private async Task ShowTransactions() {
      if (_startDate.HasValue && _endDate.HasValue) {
         var start = _startDate.Value.ToString("yyyy-MM-dd");
         var end = _endDate.Value.ToString("yyyy-MM-dd");
         var resultData = await transactionService.GetByAccountId(AccountId, start, end);
         switch (resultData) {
            case ResultData<IEnumerable<TransactionDto>?>.Success sucess:
               logger.LogInformation("AccountTransaction: GetByAccountId Suceess: {1}", sucess.Data);
               _transactions = sucess.Data!;
               break;
            case ResultData<IEnumerable<TransactionDto>?>.Error error:
               logger.LogError("AccountTransaction: GetByAccountId Error: {1}", error.Exception.Message);
               _errorMessage = error.Exception.Message;
               break;
         } 
      }
      else {
         var resultData  = await transactionService.GetYearlyByAccountId(_selectedAccountId, _selectedYear);
         switch (resultData) {
            case ResultData<IEnumerable<TransactionDto>?>.Success sucess:
               logger.LogInformation("AccountTransaction: GetByAccountId Suceess: {1}", sucess.Data);
               _transactions = sucess.Data!;
               break;
            case ResultData<IEnumerable<TransactionDto>?>.Error error:
               logger.LogError("AccountTransaction: GetByAccountId Error: {1}", error.Exception.Message);
               _errorMessage = error.Exception.Message;
               break;
         } 
      }

      // Rerender page
      StateHasChanged();
      _showTransactions = true;
   }

   private void UpdateStartDate(ChangeEventArgs args) {
      if (args.Value != null && DateTime.TryParse(args.Value.ToString(), out DateTime result)) {
         _startDate = result;
      }
   }

   private void UpdateEndDate(ChangeEventArgs args) {
      if (args.Value != null && DateTime.TryParse(args.Value.ToString(), out DateTime result)) {
         _endDate = result;
      }
   }
}