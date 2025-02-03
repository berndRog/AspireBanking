using BankingClient.Dto;
using BankingClient.Services;
using Microsoft.AspNetCore.Components;
namespace BankingClient.Pages.Accounts;

public partial class AccountTransactions {
   [Inject] public required IAccountService AccountService { get; set; }
   [Inject] public required ITransactionService TransactionService { get; set; }

   private IEnumerable<TransactionDto>? _transactions;
   private IEnumerable<AccountDto>? _accounts;
   private DateTime? _startDate;
   private DateTime? _endDate;
   private bool _showTransactions;
   private Guid _selectedAccountId;
   private int _selectedYear;

   protected override async Task OnInitializedAsync() {
      // Fetch accounts for logged-in owner
      Guid ownerId = new Guid("10000000-0000-0000-0000-000000000000");
      // Get the ID of the logged-in owner, e.g., from authentication service
      _accounts = await AccountService.GetAllAccountsByOwner(ownerId);
      if (_accounts != null) _selectedAccountId = _accounts.First().Id;
   }

   protected async Task ShowTransactions() {
      if (_startDate.HasValue && _endDate.HasValue && _accounts != null) {
         _transactions = await TransactionService.GetTransactionsOfAccountById(
            _selectedAccountId, _startDate.Value.ToString("yyyy-MM-dd"), _endDate.Value.ToString("yyyy-MM-dd"));
      }
      else if (_accounts != null) {
         _transactions = await TransactionService.GetYearlyTransactionsByAccount(
            _selectedAccountId, _selectedYear);
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