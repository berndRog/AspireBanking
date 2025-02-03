using System.ComponentModel.DataAnnotations;
namespace BankingClient.Model;

public class LoginModel {
   [Required]
   [MinLength(6, ErrorMessage = "Nutzername muss mindestens 6 Zeichen lang sein.")]
   public string Username { get; set; } = string.Empty;

   [Required]
   [MinLength(8, ErrorMessage = "Passwort muss mindestens 8 Zeichen lang sein.")]
   [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*+-?&_]{8,}$", 
      ErrorMessage = "Passwort muss einen Groß-/und Kleinbucnstanen, eine Ziffer und eins der Zeichen $!%*+-?&_ enthalten.")]
   public string Password { get; set; } = string.Empty;
   
}