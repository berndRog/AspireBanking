using BankingClient.Core;
using BankingClient.Core.Dto;
using BankingClient.Services;
using Microsoft.AspNetCore.Components;
namespace BankingClient.Ui.Components.notused;
public partial class ProfileComponent {

    
    #region fields
    [Inject] private UserStateHolder? UserStateService { get; set; }

    private OwnerDto? owner; 
    
    #endregion
    
    #region parameter
    [Parameter]
    public string name {get;set;}
    [Parameter]
    public string surname {get;set;}
    #endregion
    
    #region methods
    
    protected override void OnInitialized()
    {
        owner = UserStateService.OwnerDto;
    }
    private void HandleSubmit() {

    } 
   #endregion




}