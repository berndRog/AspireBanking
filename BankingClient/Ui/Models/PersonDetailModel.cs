using System.ComponentModel.DataAnnotations;
namespace BankingClient.Ui.Models;

public class PersonDetailModel {

   [Required]
   [MinLength(2, ErrorMessage = "Vorname must mindestens 2 Zeichen lang sein.")]
   [MaxLength(50, ErrorMessage = "Vorname darf maximal 50 Zeichen lang sein.")]
   public string Firstname { get; set; } = string.Empty;

   [Required]
   [MinLength(2, ErrorMessage = "Nachname must mindestens 2 Zeichen lang sein.")]
   [MaxLength(50, ErrorMessage = "Nachname darf maximal 50 Zeichen lang sein.")]
   public string Lastname { get; set; } = string.Empty;

   [Required] 
   [EmailAddress(ErrorMessage = "Email-Adresse ist ungültig.")] 
   public string Email { get; set; } = string.Empty;

   [DachPhoneNumber]
   public string? Phone { get; set; } = null;

   
}

