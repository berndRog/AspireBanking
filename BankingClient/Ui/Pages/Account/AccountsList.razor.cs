using BankingClient.Core;
using BankingClient.Core.Dto;
using BankingClient.Services;
using Microsoft.AspNetCore.Components;
namespace BankingClient.Ui.Pages.Account;

public partial class AccountsList(
    IAccountService accountService,
    UserStateHolder userStateHolder,
    NavigationManager navigationManager,
    ILogger<AccountsList> logger
): ComponentBase {
    private List<AccountDto> _accountDtos = [];
    private string? _errorMessage = null;

    protected override async Task OnInitializedAsync() {
        if (!userStateHolder.IsAuthenticated) {
            _errorMessage = "Admin ist nicht angemeldet!";
            return;
        }

        switch (await accountService.GetAll()) {
            case ResultData<IEnumerable<AccountDto>?>.Success sucess:
                logger.LogInformation("OwnerList: GetAll");
                _accountDtos = sucess.Data!.OrderBy(a => a.Iban).ToList();
                break;
            case ResultData<IEnumerable<AccountDto>?>.Error error:
                _errorMessage = error.Exception.Message;
                return;
        }
    }
    
    private void OpenAccount(Guid accountId) {
        logger.LogInformation("OwnerList: nav: /accounts/{1}", accountId);
        navigationManager.NavigateTo($"/accounts/{accountId}");
    }
}