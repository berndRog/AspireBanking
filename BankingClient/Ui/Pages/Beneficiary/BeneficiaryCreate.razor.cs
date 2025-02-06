using BankingClient.Core;
using BankingClient.Core.Dto;
using BankingClient.Ui.Models;
using Microsoft.AspNetCore.Components;
namespace BankingClient.Ui.Pages.Beneficiary;

public partial class BeneficiaryCreate(
   IOwnerService ownerService,
   IAccountService accountService,
   IBeneficiaryService beneficiaryService,
   NavigationManager navigationManager,
   ILogger<BeneficiaryCreate> logger
): ComponentBase {
   
   [Parameter] public required Guid AccountId { get; set; }
   
   private readonly BeneficiaryCreateModel _beneficiaryCreate = new();
   private string? _errorMessage = null;

   private OwnerDto _ownerDto = null;
   private AccountDto? _accountDto = null;
   private BeneficiaryDto? _beneficiaryDto = null;
   
   private async Task HandleSubmit() {
      
      logger.LogInformation("BeneficiaryCreate: HandleSubmit() {1} {2} {3}", 
         _beneficiaryCreate.FirstName, _beneficiaryCreate.LastName, _beneficiaryCreate.Iban);
      
      switch (await ownerService.GetByName($"{_beneficiaryCreate.FirstName} {_beneficiaryCreate.LastName}")) {
         case ResultData<IEnumerable<OwnerDto?>>.Success success:
            logger.LogInformation("BeneficiaryCreate: GetByName() success");
            if(success.Data!.Count() == 1) {
               _ownerDto = success.Data.FirstOrDefault();               
            } 
            else if(success.Data!.Count() > 1) 
               _errorMessage = $"Es gibt mehr einen Owner {success.Data}";
            break;
         case ResultData<IEnumerable<OwnerDto>?>.Error error:
            _errorMessage = error.Exception.Message;
            return;
      }
      
      switch (await accountService.GetByIban(_beneficiaryCreate.Iban)) {
         case ResultData<AccountDto?>.Success sucess:
            logger.LogInformation("BeneficiaryCreate: GetAccountByIban: {1}", sucess.Data);
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

      switch (await beneficiaryService.Post(AccountId, _beneficiaryDto)) {
         case ResultData<BeneficiaryDto?>.Success sucess:
            logger.LogInformation("BeneficiaryCreate: PostBeneficiary: {1}", _beneficiaryDto);
            break;
         case ResultData<BeneficiaryDto?>.Error error:
            _errorMessage = error.Exception.Message;
            return;
      }
      navigationManager.NavigateTo($"/accounts/{AccountId}");
   
   }

   private void LeaveForm() {
      // Implementiere die Navigation zurück
      
   }

   private void CancelOperation() {
      // Implementiere die Abbruch-Logik
   }
   
}