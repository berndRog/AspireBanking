using BankingClient.Dto;
using BankingClient.Services;
using Microsoft.AspNetCore.Components;
namespace BankingClient.Pages.Owner;

public partial class OwnerOverview
{
    [Inject] private IOwnerService OwnerService { get; set; }
    [Inject] private UserStateHolder UserStateServive { get; set; }
    
    private OwnerDto? _ownerDto;
    
    // Erika
    private Guid testUserId = new Guid("10000000-0000-0000-0000-000000000000");
//    private Guid testUserId = new Guid("30000000-0000-0000-0000-000000000000");
    
    protected override async Task OnInitializedAsync()
    {
        //_userStateContainer.OnStateChange += StateHasChanged;
        //_ownerDto = await OwnerService.GetOwnerById(testUserId);
        //_userStateContainer.SetOwner(_ownerDto);
    }
}