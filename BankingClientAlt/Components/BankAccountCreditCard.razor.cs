using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
namespace BankingClient.Components;

public partial class BankAccountCreditCard 
{
    [Parameter]
    public string AccountNumber { get; set; } = string.Empty;
    
    [Parameter]
    public double AccountBalance { get; set; }


    [Parameter] public EventCallback<MouseEventArgs> ShowTransactionsCallback { get; set; }
    [Parameter] public EventCallback<MouseEventArgs> NewTransferCallback { get; set; }
}