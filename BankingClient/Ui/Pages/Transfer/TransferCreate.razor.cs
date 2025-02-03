using BankingClient.Core;
using BankingClient.Core.Dto;
using BankingClient.Ui.Pages.Owner;
using BankingClient.Services;
using Microsoft.AspNetCore.Components;
namespace BankingClient.Ui.Pages.Transfer;

public partial class TransferCreate(
   IAccountService accountService,
   IBeneficiaryService beneficiaryService,
   ITransactionService transactionService,
   ITransferService transferService,
   UserStateHolder userStateHolder,
   NavigationManager navigationManager,
   ILogger<TransferCreate> logger
) {

   [Parameter] public required Guid AccountId { get; set; }

   private OwnerDto? _ownerDto = null;
   private AccountDto? _accountDto = null;
   private List<AccountDto> _accountDtos = [];
   private List<BeneficiaryDto>? _beneficiaryDtos = null;
   private string? _errorMessage = null;
   
   
   protected override async Task OnInitializedAsync() {

      //logger.LogInformation("OwnerDetail: OnInitializedAsync Id: {1}", Id);
      if (!userStateHolder.IsAuthenticated) {
         _errorMessage = "Kontoinhaber ist nicht angemeldet!";
         return;
      }
      _ownerDto = userStateHolder.OwnerDto!;
      _accountDto = userStateHolder.AccountDto;
      if (AccountId != _accountDto?.Id) {
         var resultAccount = await accountService.GetById(AccountId);
         switch (resultAccount) {
            case ResultData<AccountDto?>.Success sucess:
               logger.LogInformation("TransferCreate: GetAccountById: {1}", sucess.Data);
               _accountDto = sucess.Data!;
               break;
            case ResultData<AccountDto?>.Error error:
               _errorMessage = error.Exception.Message;
               return;
         }  
      }
      
      var resultBeneficiaries = await beneficiaryService.GetAllBeneficiariesByAccount(_accountDto!.Id);
      switch (resultBeneficiaries) {
         case ResultData<IEnumerable<BeneficiaryDto>?>.Success sucess:
            logger.LogInformation("AccountDetail: GetBeneficiariesByAccountId: {1}", sucess.Data);
            _beneficiaryDtos = sucess.Data!.ToList();
            break;
         case ResultData<IEnumerable<BeneficiaryDto>?>.Error error:
            _errorMessage = error.Exception.Message;
            return;
      }
      
   }
   
   private void OpenBeneficiary(Guid id) {
      logger.LogInformation("AccountDetail: nav: /beneficiries/{1}", id);
      //    navigationManager.NavigateTo($"/accounts/iban/{accountIban}");
   }

}