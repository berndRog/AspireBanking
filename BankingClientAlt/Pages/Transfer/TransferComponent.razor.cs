using BankingClient.Dto;
using BankingClient.Services;
using Microsoft.AspNetCore.Components;
namespace BankingClient.Pages.Transfer;

public partial class TransferComponent
{
   
    #region fields

    private TransferDto? _transfer = new TransferDto(); 
    private IEnumerable<BeneficiaryDto>? _beneficiaries;
    private IEnumerable<AccountDto>? _accounts;
    private Dictionary<Guid, IEnumerable<BeneficiaryDto>> _allBeneficiariesOfAccounts = new();

    #endregion

    #region injections

    [Inject] public ITransferService? TransferService { get; set; }
    [Inject] public IBeneficiaryService? BeneficiaryService { get; set; }
    [Inject] public IAccountService? AccountService { get; set; }
    [Inject] public UserStateHolder? UserStateContainer { get; set; }

    #endregion

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
        _accounts = await AccountService.GetAllAccountsByOwner(UserStateContainer.OwnerDto.Id);
        foreach (var account in _accounts)
        {
            IEnumerable<BeneficiaryDto> beneficiaries = await BeneficiaryService.GetAllBeneficiariesByAccount(account.Id);
            if (beneficiaries == null)
            {
                continue; 
            }
            _allBeneficiariesOfAccounts.Add(account.Id, beneficiaries); 
        }
        HandleFirstSelectChange();
    }

    private async Task HandleSubmit()
    {
        if (_transfer != null)
        {
            var response = TransferService.SendTransfer(_transfer, SelectedAccountId);
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