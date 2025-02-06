using BankingClient.Core;
using BankingClient.Core.Dto;
using BankingClient.Services;
using BankingClient.Ui.Models;
using Microsoft.AspNetCore.Components;
namespace BankingClient.Ui.Pages.Transfer;

public partial class TransferSend(
   IAccountService accountService,
   IBeneficiaryService beneficiaryService,
   ITransferService transferService,
   UserStateHolder userStateHolder,
   NavigationManager navigationManager,
   ILogger<TransferCreate> logger
) : ComponentBase {
   [Parameter] public required Guid AccountId { get; set; }
   [Parameter] public required Guid BeneficiaryId { get; set; }

   private OwnerDto? _ownerDto = null;
   private AccountDto? _accountDto = null;
   private BeneficiaryDto? _beneficiaryDto = null;
   private TransferDto? _transferDto = null;
   private string? _errorMessage = null;
   private string? _warnMessage = null;
   private bool _isTransferSuccessful = false;

   private readonly TransferCreateModel _transferModel = new();
   
   protected override async Task OnInitializedAsync() {
      if (!userStateHolder.IsAuthenticated) {
         _errorMessage = "Kontoinhaber ist nicht angemeldet!";
         return;
      }
      // the actual owner of the account
      _ownerDto = userStateHolder.OwnerDto!;
      
      // get account and beneficiary from url
      switch (await accountService.GetById(AccountId)) {
         case ResultData<AccountDto?>.Success sucess:
            logger.LogInformation("TransferSend: GetAccountById: {1}", sucess.Data);
            _accountDto = sucess.Data!;
            break;
         case ResultData<AccountDto?>.Error error:
            _errorMessage = $"Problem mit dem Sender-Konto\nerror.Exception.Message";
            return;
      }
      switch (await beneficiaryService.GetById(BeneficiaryId)) {
         case ResultData<BeneficiaryDto?>.Success sucess:
            logger.LogInformation("TransferDo: GetBeneficiaryById: {1}", sucess.Data);
            _beneficiaryDto = sucess.Data!;
            break;
         case ResultData<BeneficiaryDto?>.Error error:
            _errorMessage = $"Problem mit dem Zahlungsempfänger \n{error.Exception.Message}";
            return;
      }
   }
   
   
   private async Task HandleSubmit() {
      Console.WriteLine($"Amount: {_transferModel.Amount}, Reason: {_transferModel.TransferReason}");
      
      // Do transfer
      if( _accountDto == null) {
         _errorMessage = "Sender-Konto ist unbekannt.";
         return;
      }
      if( _beneficiaryDto == null) {
         _errorMessage = "Zahlungsempfänger ist unbekannt.";
         return;
      }
      
      // do the money transfer
      var accountId = _accountDto!.Id;
      var beneficiaryId = _beneficiaryDto!.Id;
      var transferDto = new TransferDto {
         Amount = _transferModel.Amount,
         Description = _transferModel.TransferReason,
         Date = _transferModel.TransferDate,
         AccountId = accountId,
         BeneficiaryId = beneficiaryId
      };
      
      switch (await transferService.SendTransfer(transferDto, accountId)) {
         case ResultData<TransferDto?>.Success sucess:
            logger.LogInformation("TransferSend: SendTransfer: {1}", sucess.Data);
            _transferDto = sucess.Data!;
            
            _isTransferSuccessful = true;
            _warnMessage = $"Überweisung erfolgreich\n" +
               $"{_transferDto.Amount} € an {_beneficiaryDto.FirstName} {_beneficiaryDto.LastName} {_beneficiaryDto.Iban}";
            break;
         case ResultData<TransferDto?>.Error error:
            _errorMessage = $"Problem mit der Überweisung\n{error.Exception.Message}";
            return;
      }
   }

   private void LeaveForm() {
      Console.WriteLine("Leave form called.");
   }

   private void CancelOperation() {
      Console.WriteLine("Cancel operation called.");
   }

}