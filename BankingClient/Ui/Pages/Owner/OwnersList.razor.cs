using BankingClient.Core;
using BankingClient.Core.Dto;
using BankingClient.Services;
using Microsoft.AspNetCore.Components;
namespace BankingClient.Ui.Pages.Owner;

public partial class OwnersList(
    IOwnerService ownerService,
    IAccountService accountService,
    UserStateHolder userStateHolder,
    NavigationManager navigationManager,
    ILogger<OwnerDetail> logger
) {
    private List<OwnerDto> _ownerDtos = [];
    private string? _errorMessage = null;

    protected override async Task OnInitializedAsync() {
        
        if (!userStateHolder.IsAuthenticated) {
            _errorMessage = "Admin ist nicht angemeldet!";
            return;
        }

        var resultData = await ownerService.GetAll();
        switch (resultData) {
            case ResultData<IEnumerable<OwnerDto>?>.Success sucess:
                logger.LogInformation("OwnerList: GetAll");
                _ownerDtos = sucess.Data!.ToList();
                break;
            case ResultData<IEnumerable<OwnerDto>?>.Error error:
                _errorMessage = error.Exception.Message;
                return;
        }
    }
}