//  <@$&< copyright begin >&$@> 24FE144C2255E2F7CCB65514965434A807AE8998C9C4D01902A628F980431C98:20241017.A:2025:7:24:7:12
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// Copyright © 2024-2025 Stewart A. Nutter - All Rights Reserved.
// No warranty is implied or given.
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// <@$&< copyright end >&$@>

using System.Security.Cryptography;
using Newtonsoft.Json;

namespace CommonLibrary;

public class FileEncryption
{
    public static void EncryptFile(Stream inputStream, Stream outputStream, string password, string originalExtension, DateTime originalDate)
    {
        Dictionary<string, object> metadata = new Dictionary<string, object>
        {
            { "Ext", originalExtension },
            { "Date", originalDate }
        };

        // Generate a random salt
        byte[] salt = RandomNumberGenerator.GetBytes(16);

        // Get an iterations value
        int iterations = GenerateRandomIterations();

        using (Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA3_512))
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key.GetBytes(32); // 256-bit key
                aes.IV = key.GetBytes(16);  // 128-bit IV

                string jsonMetadata = JsonConvert.SerializeObject(metadata);
                outputStream.WriteString(jsonMetadata);

                // Write the salt to the beginning of the file
                outputStream.Write(salt, 0, salt.Length);
                salt = null; // Clear the salt from memory after use

                // write the iteration count
                outputStream.WriteInt(key.IterationCount); // Write the iteration count

                // write the encrypted data
                using (CryptoStream cryptoStream = new CryptoStream(outputStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    inputStream.CopyTo(cryptoStream);
                }
            }
        }
    }

    public static void DecryptFile(Stream inputStream, Stream outputStream, string password, out string originalExtension, out DateTime originalDate)
    {
        string jsonMetadata = inputStream.ReadString(); // Read the metadata JSON string
        Dictionary<string, object> metadata = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonMetadata);
        if (metadata == null)
        {
            throw new InvalidOperationException("Metadata is missing or invalid.");
        }

        originalExtension = string.Empty;
        originalDate = DateTime.MinValue;

        if (metadata.TryGetValue("Ext", out object extObj) && extObj is string ext)
        {
            originalExtension = ext;
        }

        if (metadata.TryGetValue("Date", out object dateObj) && dateObj is DateTime date)
        {
            originalDate = date;
        }

        // Read the salt from the beginning of the file
        byte[] salt = new byte[16];
        inputStream.ReadExactly(salt);

        int iterations = inputStream.ReadInt(); // Read the iteration count

        // Derive the key and IV from the password and salt
        using (Rfc2898DeriveBytes keyDerivation = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA3_512))
        {

            salt = null; // Clear the salt from memory after use
            iterations = 0; // Clear the iterations after use
            password = string.Empty; // Clear the password from memory after use

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyDerivation.GetBytes(32); // 256-bit key
                aes.IV = keyDerivation.GetBytes(16);  // 128-bit IV

                // Decrypt the file
                using (CryptoStream cryptoStream = new CryptoStream(inputStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    cryptoStream.CopyTo(outputStream);
                }
            }
        }
    }

    public static byte[] GenerateRandomBytes(int length)
    {
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            byte[] bytes = new byte[length];
            rng.GetBytes(bytes);
            return bytes;
        }
    }

    public static int GenerateRandomIterations()
    {
        int min = 90000;
        int max = 120000;

        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            byte[] randomNumber = new byte[4];
            rng.GetBytes(randomNumber);
            int value = BitConverter.ToInt32(randomNumber, 0);
            return Math.Abs(value % (max - min)) + min;
        }
    }
}
