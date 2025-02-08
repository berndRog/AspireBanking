using System.ComponentModel.DataAnnotations;
namespace BankingClient.Ui.Models;

public class TransactionModel {
   [Required(ErrorMessage = "Anfangsdatum ist erforderlich.")]
   public DateTime Start { get; set; } = DateTime.Now;

   [Required(ErrorMessage = "Enddatum ist erforderlich.")]
   public DateTime End { get; set; } = DateTime.Now;
}