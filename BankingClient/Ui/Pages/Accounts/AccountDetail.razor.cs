using BankingClient.Core;
using BankingClient.Core.Dto;
using BankingClient.Services;
using Microsoft.AspNetCore.Components;
namespace BankingClient.Ui.Pages.Accounts;

public partial class AccountDetail(
   IAccountService accountService,
   IBeneficiaryService beneficiaryService,
   UserStateHolder userStateHolder,
   NavigationManager navigationManager,
   ILogger<AccountDetail> logger
) {
   [Parameter] public required string Iban { get; set; }

   private OwnerDto? _ownerDto = null;
   private AccountDto? _accountDto = null;
   private List<AccountDto> _accountDtos = [];
   private List<BeneficiaryDto>? _beneficiaryDtos = null;
   private string? _errorMessage = null;

   protected override async Task OnInitializedAsync() {
      
      logger.LogInformation("AccountDetail: OnInitializedAsync Iban: {1}",Iban);
      if (!userStateHolder.IsAuthenticated) {
         _errorMessage = "Kontoinhaber ist nicht angemeldet!";
         return;
      }
      if(userStateHolder.OwnerDto == null) {
         _errorMessage = "Kontoinhaber ist nicht korrekt angemeldet!";
         return;
      }
      _ownerDto = userStateHolder.OwnerDto!;
      
      var resultAccount = await accountService.GetByIban(Iban);
      switch (resultAccount) {
         case ResultData<AccountDto?>.Success sucess:
            logger.LogInformation("AccountDetail: GetByIban: {1}", sucess.Data);
            _accountDto = sucess.Data!;
            userStateHolder.AccountDto = _accountDto;
            _accountDtos.Add(_accountDto);
            break;
         case ResultData<AccountDto?>.Error error:
            _errorMessage = error.Exception.Message;
            return;
      }
   }
   
   private void HandleTransfers() {
      logger.LogInformation("AccountDetail: HandleTransfers navigate to /accounts/{1}/transfers/create", _accountDto!.Id);
      navigationManager.NavigateTo($"/accounts/{_accountDto!.Id}/transfers/create");
   }
   
   private void HandleTransactions() {
      logger.LogInformation("AccountDetail: HandleTransactions navigate to /accounts/{1}/transactions/create", _accountDto!.Id);
      navigationManager.NavigateTo($"/accounts/{_accountDto!.Id}/transctions/create");
   }


   private void LeaveForm() {
      navigationManager.NavigateTo("/home");
   }
   
}