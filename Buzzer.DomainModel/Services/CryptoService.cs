using System;
using System.Security.Cryptography;
using System.Text;

namespace Buzzer.DomainModel.Services
{
   public static class CryptoService
   {
      public static string GetHash(string text)
      {
         using (SHA256 sha256 = SHA256Managed.Create())
         {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            byte[] hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
         }
      }
   }
}