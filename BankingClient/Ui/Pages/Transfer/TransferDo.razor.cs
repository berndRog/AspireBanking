using BankingClient.Core;
using BankingClient.Core.Dto;
using BankingClient.Services;
using Microsoft.AspNetCore.Components;
namespace BankingClient.Ui.Pages.Transfer;

public partial class TransferDo(
   IAccountService accountService,
      IBeneficiaryService beneficiaryService,
   ITransactionService transactionService,
      ITransferService transferService,
   UserStateHolder userStateHolder,
      NavigationManager navigationManager,
   ILogger<TransferCreate> logger
   ): ComponentBase {

      [Parameter] public required Guid AccountId { get; set; }

   private OwnerDto? _ownerDto = null;
   private AccountDto? _accountDto = null;
   private List<AccountDto> _accountDtos = [];
   private List<BeneficiaryDto>? _beneficiaryDtos = null;
   private string? _errorMessage = null;
   
}