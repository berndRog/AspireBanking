using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
namespace BankingClient.Ui.Models;

public class PersonCreateModel {

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
   
   [Required]
   [MinLength(6, ErrorMessage = "Nutzername muss mindestens 6 Zeichen lang sein.")]
   public string Username { get; set; } = string.Empty;

   [Required]
   [MinLength(8, ErrorMessage = "Passwort muss mindestens 8 Zeichen lang sein.")]
   [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*+-?&_]{8,}$", 
      ErrorMessage = "Passwort muss einen Groß-/und Kleinbucnstanen, eine Ziffer und eins der Zeichen $!%*+-?&_ enthalten.")]
   public string Password { get; set; } = string.Empty;
   
   [Required]
   [Compare("Password", ErrorMessage = "Passwort stimmt nicht überein.")]
   public string ConfirmedPassword { get; set; } = string.Empty;
   
}

public class DachPhoneNumberAttribute : ValidationAttribute {
   protected override ValidationResult IsValid(object? value, ValidationContext validationContext) {
      if (value == null) return ValidationResult.Success!;
      
      // 0049 1234 / 1234-1234
      // +49 (0) 1234 / 1234-1234
      // 01234 / 1234-1234
      // 001 123477 12 342
      
      var phoneNumber = value.ToString();
      if (string.IsNullOrWhiteSpace(phoneNumber)) return ValidationResult.Success!;

//    var regex = new Regex(@"^(\+49|0049|+43|0043|+41|0041)(\s?\(0\))?\s?\d{4}\s?\/?\s?\d{4,5}(\s?-\s?\d{1,4})?$");
      var regex = new Regex(@"^(\+49|0049|\+43|0043|\+41|0041|0)[1-9][0-9\s/-]{5,}$");

      if (regex.IsMatch(phoneNumber)) return ValidationResult.Success!;
      return new ValidationResult("Die Telefonnummer ist ungültig für die D-A-CH Region.");
   }
}