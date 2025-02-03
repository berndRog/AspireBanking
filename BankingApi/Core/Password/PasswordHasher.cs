using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
namespace BankingApi.Core.Password;

public static class PasswordHasher {
   
   // Hash a password with a random salt
   // returns the salt and the hash
   public static (string, string) Hash(string password) {
      // generate a 128-bit salt using a secure PRNG 
      byte[] salt = new byte[128 / 8];
      using (var rng = RandomNumberGenerator.Create()) {
         rng.GetBytes(salt);
      }
      
      // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
      byte[] key = KeyDerivation.Pbkdf2(
         password: password,
         salt: salt,
         prf: KeyDerivationPrf.HMACSHA256,
         iterationCount: 10000,
         numBytesRequested: 256 / 8
      );

      // return the salt and the key
      string salted = Convert.ToBase64String(salt);
      string hashed = Convert.ToBase64String(key);
      return (hashed, salted);
   }

   public static bool Verify(string password, string hashedPassword, string salted) {
   
      byte[] salt = Convert.FromBase64String(salted);
      var key = KeyDerivation.Pbkdf2(
         password: password,
         salt: salt,
         prf: KeyDerivationPrf.HMACSHA256,
         iterationCount: 10000,
         numBytesRequested: 256 / 8
      );

      string hashed = Convert.ToBase64String(key);
      
      return hashed == hashedPassword;
   }
}