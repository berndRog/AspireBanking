using BankingClient.Core;
using BankingClient.Core.Dto;
using BankingClient.Services;
using BankingClient.Ui.Models;
using BankingClient.Ui.Pages.Account;
using Microsoft.AspNetCore.Components;

namespace BankingClient.Ui.Pages.Transaction;

public partial class TransactionsList(
   IAccountService accountService,
   ITransactionService transactionService,
   UserStateHolder userStateHolder,
   NavigationManager navigationManager,
   ILogger<AccountsList> logger
): ComponentBase {
   
   [Parameter] public Guid AccountId { get; set; }
   
   private OwnerDto? _ownerDto = null;
   private AccountDto? _accountDto = null;
   private string? _errorMessage = null;
   
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
      logger.LogInformation("TransactionList: StateHasChangeg()");
      StateHasChanged();

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
            logger.LogInformation("TransactionList: StateHasChangeg()");
            StateHasChanged();
            break;
         case ResultData<AccountDto?>.Error error:
            _errorMessage = error.Exception.Message;
            return;
      }
      StateHasChanged();
      
   }
   
   protected override async Task OnAfterRenderAsync(bool firstRender)
   {
      if (firstRender)
      {
         StateHasChanged(); // Nochmals UI-Update auslösen

         logger.LogInformation("OnAfterRenderAsync: Rendering TransactionsList Component.");
         logger.LogInformation("OwnerDto: {0}", _ownerDto);
         logger.LogInformation("AccountDto: {0}", _accountDto);
         logger.LogInformation("TransactionModel: {0}", _transactionModel);
      }
   }
    
   
   private TransactionModel _transactionModel = new ();

   private void HandleStartEndDate(TransactionModel model) {
      Console.WriteLine($"Kontoauszug für den Zeitraum {model.Start:dd.MM.yyyy} - {model.End:dd.MM.yyyy}");
      // Hier kannst du die weitere Verarbeitung des Modells durchführen
   }
}