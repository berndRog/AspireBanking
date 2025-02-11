using BankingClient.Core;
using BankingClient.Core.Dto;
using BankingClient.Services;
using BankingClient.Ui.Models;
using BankingClient.Ui.Pages.Account;
using Microsoft.AspNetCore.Components;

namespace BankingClient.Ui.Pages.Transaction;

public partial class TransactionsList(
   IOwnerService ownerService,
   IAccountService accountService,
   ITransferService transferService,
   ITransactionService transactionService,
   UserStateHolder userStateHolder,
   NavigationManager navigationManager,
   ILogger<TransactionsList> logger
): ComponentBase {
   
   [Parameter] public Guid AccountId { get; set; }
   
   private OwnerDto? _ownerDto = null;
   private AccountDto? _accountDto = null;
   private List<TransactionListItemDto> _transactionListItemDtos = new();
   private string? _errorMessage = null;
   private TransactionStartEndModel _transactionStartEndModel = new ();
   
   protected override async Task OnInitializedAsync() {
      if (!userStateHolder.IsAuthenticated) {
         _errorMessage = "Admin ist nicht angemeldet!";
         return;
      }
      if(userStateHolder.OwnerDto == null) {
         _errorMessage = "Kontoinhaber ist nicht korrekt angemeldet!";
         return;
      }
      _ownerDto = userStateHolder.OwnerDto!;

      logger.LogInformation("TransactionsList: {1} {2} AccountId: {3}", 
         _ownerDto?.FirstName, _ownerDto?.LastName, AccountId);
      switch (await accountService.GetById(AccountId)) {
         case ResultData<AccountDto?>.Success sucess:
            logger.LogInformation("TransactionList: GetAccountById: {1}", sucess.Data);
            _accountDto = sucess.Data!;
            if(_accountDto.OwnerId != _ownerDto.Id) {
               _errorMessage = "Kontozugriff fehlgeschlagen"; 
               return;
            }
            break;
         case ResultData<AccountDto?>.Error error:
            _errorMessage = error.Exception.Message;
            return;
      }

      // last six months as default
      _transactionStartEndModel.Start = DateTime.Now.AddMonths(-6);
      _transactionStartEndModel.End = DateTime.Now;
      
      await FetchTransactions();
   }

   private async Task FetchTransactions() {

      // convert into UTC Iso format
      var startIso = _transactionStartEndModel.Start.ToUniversalTime().ToString("o");
      var endIso = _transactionStartEndModel.End.ToUniversalTime().ToString("o");
      
      logger.LogInformation("TransactionList: GetByAccountId: {1} {2} {3}", 
         AccountId, startIso, endIso);
      
      switch (await transactionService.FilterListItemsByAccountId(AccountId, startIso, endIso)) {
         case ResultData<IEnumerable<TransactionListItemDto>?>.Success sucess:
            logger.LogInformation("TransactionList: GetByAccountId");
            _transactionListItemDtos = sucess.Data!.OrderByDescending(t => t.Date).ToList();
            break;
         case ResultData<IEnumerable<TransactionListItemDto>?>.Error error:
            _errorMessage = error.Exception.Message;
            return;
      }
      

      logger.LogInformation("TransactionList: StateHasChanged()");

      StateHasChanged();
   }
   
   private async Task HandleStartEndDate(TransactionStartEndModel startEndModel) {
      await FetchTransactions();
   }
}