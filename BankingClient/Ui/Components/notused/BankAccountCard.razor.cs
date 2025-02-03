using Microsoft.AspNetCore.Components;
namespace BankingClient.Ui.Components.notused;

public partial class BankAccountCard {
   [Parameter] public string AccountNumber { get; set; } = string.Empty;
   [Parameter] public double AccountBalance { get; set; }
   private bool showTransactions = false;
   private List<string> Transactions = new();

   private async Task ToggleTransactions() {
      if (!showTransactions) {
         // Hier sollten Sie den Code zum Abrufen der letzten Transaktionen von Ihrer API hinzufügen.
         // Verwenden Sie beispielsweise HttpClient, um die Daten abzurufen und die "Transactions" Liste zu aktualisieren.

         // Beispielcode (ersetzen Sie dies durch den tatsächlichen API-Aufruf):
         Transactions = await FetchTransactionsFromAPI();
      }
      showTransactions = !showTransactions;
   }

   private async Task<List<string>> FetchTransactionsFromAPI() {
      // Hier sollte der tatsächliche API-Aufruf erfolgen, um Transaktionen abzurufen.
      // Ersetzen Sie dies durch Ihren eigenen Code zur Datenabfrage.
      // Zum Beispiel:
      // var response = await httpClient.GetFromJsonAsync<List<string>>("API-URL-hier");
      // return response;

      // In diesem Beispiel wird statische Daten zurückgegeben.
      return new List<string> {
         "Transaktion 1: -100 EUR",
         "Transaktion 2: +200 EUR",
         "Transaktion 3: -50 EUR"
      };
   }
}