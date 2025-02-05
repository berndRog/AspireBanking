using BankingClient.Core;
using BankingClient.Core.Dto;
using BankingClient.Services;
using Microsoft.AspNetCore.Components;
namespace BankingClient.Ui.Components.notused;

public partial class TransferComponent(
    ITransferService transferService,
    IBeneficiaryService beneficiaryService,
    IAccountService accountService,
    UserStateHolder userStateHolder
){
    
    private TransferDto? _transfer = new TransferDto(); 
    private IEnumerable<BeneficiaryDto>? _beneficiaries;
    private IEnumerable<AccountDto>? _accounts;
    private Dictionary<Guid, IEnumerable<BeneficiaryDto>> _allBeneficiariesOfAccounts = new();
    
    #region parameters

    [Parameter]
    public Guid SelectedAccountId 
    {
        get { return _transfer.AccountId;}
        set
        {
            if (_transfer.AccountId != value)
            {
                _transfer.AccountId = value; 
                HandleFirstSelectChange();
            }

        }
    }
    
    [Parameter]
    public Guid SelectedBeneficiaryId
    {
        get { return _transfer.BeneficiaryId; }
        set
        {
            _transfer.BeneficiaryId = value;
        }
    }

    
    // two way binding to parent component transferoverview
    [Parameter]
    public EventCallback<Guid> OnTransfer { get; set; }

 
    #endregion

    #region methods
    protected override async Task OnInitializedAsync()
    {
        /* rg
        _accounts = await accountService.GetAllByOwner(userStateHolder.OwnerDto.Id);
        foreach (var account in _accounts)
        {
            IEnumerable<BeneficiaryDto> beneficiaries = await beneficiaryService.GetAllBeneficiariesByAccount(account.Id);
            if (beneficiaries == null)
            {
                continue; 
            }
            _allBeneficiariesOfAccounts.Add(account.Id, beneficiaries); 
        }
        HandleFirstSelectChange();
        */
    }

    private async Task HandleSubmit()
    {
        if (_transfer != null)
        {
            var response = transferService.SendTransfer(_transfer, SelectedAccountId);
            await OnTransfer.InvokeAsync(_transfer.AccountId);
        }
        
    }
    
    private void HandleFirstSelectChange()
    {
        if (_allBeneficiariesOfAccounts.TryGetValue(_transfer.AccountId, out var value))
        {
            _beneficiaries = value;
        }
        StateHasChanged();
    }
    #endregion
    
}