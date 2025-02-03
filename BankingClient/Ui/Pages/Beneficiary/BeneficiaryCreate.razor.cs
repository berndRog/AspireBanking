using BankingClient.Core.Dto;
using BankingClient.Ui.Models;
namespace BankingClient.Ui.Pages.Beneficiary;

public partial class BeneficiaryCreate(
   ) {

   
   private readonly BeneficiaryCreateModel _beneficiaryCreate = new();
   private string? _errorMessage = null;

   private void HandleSubmit() {
      // Implementiere die Logik für das Absenden des Formulars
   }

   private void LeaveForm() {
      // Implementiere die Navigation zurück
   }

   private void CancelOperation() {
      // Implementiere die Abbruch-Logik
   }
   
}