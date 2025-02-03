using System;
using System.Globalization;
using TimeZoneConverter;
namespace BankingApi.Core.Misc; 
public static class Utils {
   
   public static string CheckIban(string? iban) {
      //"DEkk BBBB BBBB CCCC CCCC CC"
      if (iban != null) {
         iban = iban.Replace(" ", "").ToUpper();
         if (iban.Length == 22) return iban;
      }
      // if iban is not valid, create a new one
      var random = new Random();
             //     DEkk                           BBBB                    BBBB
             return "DE" + Digits2(random) + " " + Digits4(random) + " " + Digits4(random) + " " + 
             // CCCC                    CCCC                   CC
                Digits4(random) + " " + Digits4(random) + " " + Digits2(random); 
   }

   private static string Digits2(Random random) =>
      random.Next(1, 99).ToString("D2");
   private static string Digits4(Random random) =>
      random.Next(0, 1000).ToString("D4") + " ";
  
   public static string ToIso8601TzString(this DateTime dateTime, TimeZoneInfo tz) {
      var formatString = "yyyy-MM-dd'T'HH:mm:sszzz"; 
      var dateTimeOffset = new DateTimeOffset(dateTime, tz.GetUtcOffset(dateTime));
      return dateTimeOffset.ToString("o");
   }
   
   public static DateTime AsUniversalTime(this DateTime dateTime) =>
      DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
   
   public static (bool, DateTime?, string?) EvalDateTime(string? date) {
      // 1234567890
      // YYYY-MM-dd
      if(date == null || date.Length < 10) 
         return (true, null, $"EvalDateTime: Date format not accepted: {date}");

      if (!DateTime.TryParseExact(
             s: date,
             format: "yyyy-MM-dd",
             provider: CultureInfo.InvariantCulture,
             style: DateTimeStyles.AdjustToUniversal,
             result: out var dateTime)
      ) {
         return (true, null, $"EvalDateTime: Date format not accepted: {date}");
      }
      return (false, dateTime.ToUniversalTime(), null);
   }
   
   public static string As8(this Guid guid) => guid.ToString()[..8];

   public static string ConvertToWindowsTimezone(string ianaTimeZone) {
      try {
         return TZConvert.IanaToWindows(ianaTimeZone);
      }
      catch (TimeZoneNotFoundException ex) {
         return $"Undefined TimeZone: {ex.Message}";
      }
      catch (InvalidTimeZoneException ex) {
         return $"Error in TimeZone: {ex.Message}";
      } 
   }

}
   
 