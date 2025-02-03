using BankingClient.Core;
using BankingClient.Core.Dto;
using Microsoft.AspNetCore.Components;
namespace BankingClient.Ui.Pages.notused;

public partial class TransferOverview(
   ITransferService transferService,
   ILogger<TransferOverview> logger
) {
   
   [Parameter] public Guid AccountId { get; set; }
   
   
   private IEnumerable<TransferDto>? _transferDtos = null;
   private Guid _selectedAccountId;
   private string? _errorMessage = null;

   
   protected override async Task OnInitializedAsync() {
      if (AccountId == Guid.Empty) {
         return;
      }
      await base.OnInitializedAsync();
      await GetTransfers();
   }

   private async Task GetTransfers() {
      var resultData = await transferService.GetByAccountId(AccountId);
      switch (resultData) {
         case ResultData<IEnumerable<TransferDto>?>.Success sucess:
            logger.LogInformation("TransferOverview: GetByAccountId Suceess: {1}", sucess.Data);
            _transferDtos = sucess.Data!;
            break;
         case ResultData<IEnumerable<TransferDto>?>.Error error:
            logger.LogError("TransferOverview: GetByAccountId Error: {1}", error.Exception.Message);
            _errorMessage = error.Exception.Message;
            break;
      }
   }

   private async Task UpdateTransferHistory() {
      await GetTransfers();
   }

   private async Task HandleSelectedAccountIdChanged(Guid newAccountId) {
      _selectedAccountId = newAccountId;
      await UpdateTransferHistory();
   }

   
}