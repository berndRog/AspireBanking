using BankingClient.Core;
using BankingClient.Core.Dto;
using BankingClient.Ui.Models;
using Microsoft.AspNetCore.Components;
namespace BankingClient.Ui.Pages.Beneficiary;

public partial class BeneficiaryCreate(
   IAccountService accountService,
   IBeneficiaryService beneficiaryService,
   NavigationManager navigationManager,
   ILogger<BeneficiaryCreate> logger
): ComponentBase {
   
   [Parameter] public required Guid AccountId { get; set; }

   
   private readonly BeneficiaryCreateModel _beneficiaryCreate = new();
   private string? _errorMessage = null;

   private AccountDto? _accountDto = null;
   private BeneficiaryDto? _beneficiaryDto = null;
   
   private async Task HandleSubmit() {
      var resultAccount = await accountService.GetByIban(_beneficiaryCreate.Iban);
      switch (resultAccount) {
         case ResultData<AccountDto?>.Success sucess:
            logger.LogInformation("AccountDetail: GetByIban: {1}", sucess.Data);
            _accountDto = sucess.Data!;
            break;
         case ResultData<AccountDto?>.Error error:
            _errorMessage = error.Exception.Message;
            return;
      }
      _beneficiaryDto = new BeneficiaryDto(
         Id: Guid.NewGuid(),
         FirstName: _beneficiaryCreate.FirstName,
         LastName: _beneficiaryCreate.LastName,
         Iban: _beneficiaryCreate.Iban,
         AccountId: AccountId
      );
      logger.LogInformation("BeneficiaryCreate: PostBeneficiary: {1}", _beneficiaryDto);
      beneficiaryService.PostBeneficiary(AccountId, _beneficiaryDto);
      navigationManager.NavigateTo($"/accounts/{AccountId}");
   
   }

   private void LeaveForm() {
      // Implementiere die Navigation zurück
      
   }

   private void CancelOperation() {
      // Implementiere die Abbruch-Logik
   }
   
}