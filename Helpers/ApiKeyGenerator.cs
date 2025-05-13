using System;
using System.Security.Cryptography;
using System.Text;

public static class ApiKeyGenerator
{
    public static string GenerateApiKey()
    {
        const int keyLength = 32; // Adjust length as needed
        using (var rng = RandomNumberGenerator.Create())
        {
            var bytes = new byte[keyLength];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}