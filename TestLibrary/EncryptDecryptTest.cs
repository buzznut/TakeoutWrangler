//  <@$&< copyright begin >&$@> 24FE144C2255E2F7CCB65514965434A807AE8998C9C4D01902A628F980431C98:20241017.A:2025:7:1:14:38
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// Copyright © 2024-2025 Stewart A. Nutter - All Rights Reserved.
// No warranty is implied or given.
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// <@$&< copyright end >&$@>

using System.Security.Cryptography;

namespace TestLibrary;

[TestClass]
public class EncryptDecryptTest
{
    [TestMethod]
    public void TestMethodMovie()
    {
        string filePath = @"C:\Users\sanut\OneDrive\Pictures\2024_12\PXL_20241213_201714342.mp4";

        string tmpEncryptPath = null;
        string tmpDecryptPath = null;
        try
        {
            tmpEncryptPath = DoEncrypt(filePath, "testpassword", "movie", out string originalExtension, out DateTime originalDate);

            try
            {
                tmpDecryptPath = DoDecrypt(tmpEncryptPath, out string originalExtensionTest, out DateTime originalDateTest);

                Assert.AreEqual(originalExtension, originalExtensionTest, "The original and decrypted file extensions should match.");
                Assert.AreEqual(originalDate, originalDateTest, "The original and decrypted file dates should match.");

                // compare the original file with the decrypted file
                using (FileStream originalStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (FileStream decryptedStream = new FileStream(tmpDecryptPath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] originalBuffer = new byte[4096];
                        byte[] decryptedBuffer = new byte[4096];
                        int originalBytesRead;
                        int decryptedBytesRead;
                        do
                        {
                            originalBytesRead = originalStream.Read(originalBuffer, 0, originalBuffer.Length);
                            decryptedBytesRead = decryptedStream.Read(decryptedBuffer, 0, decryptedBuffer.Length);
                            Assert.AreEqual(originalBytesRead, decryptedBytesRead, "The number of bytes read from the original and decrypted files should match.");
                            Assert.IsTrue(originalBuffer.Take(originalBytesRead).SequenceEqual(decryptedBuffer.Take(decryptedBytesRead)), "The contents of the original and decrypted files should match.");
                        }
                        while (originalBytesRead > 0);
                    }
                }
            }
            finally
            {
                if (File.Exists(tmpDecryptPath))
                {
                    File.Delete(tmpDecryptPath);
                }
            }
        }
        finally
        {
        }
    }

    private string DoDecrypt(string tmpEncryptPath, out string originalExtension, out DateTime originalDate)
    {
        using (FileStream inputStream = new FileStream(tmpEncryptPath, FileMode.Open, FileAccess.ReadWrite))
        {
            string tempPath = Path.Combine(Path.GetTempPath(), "test_decrypt" + ".tmp");
            using (FileStream outputStream = new FileStream(tempPath, FileMode.Create, FileAccess.ReadWrite))
            {
                string password = "testPassword";
                CommonLibrary.FileEncryption.DecryptFile(inputStream, outputStream, password, out originalExtension, out originalDate);
                return tempPath;
            }
        }
    }

    private string DoEncrypt(string filePath, string v, string name, out string originalExtension, out DateTime originalDate)
    {
        using (FileStream inputStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            string tempPath = Path.Combine(Path.GetTempPath(), $"{name}.twl");
            using (FileStream outputStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
            {
                string password = "testPassword";
                originalExtension = Path.GetExtension(filePath);
                originalDate = DateTime.Now;

                CommonLibrary.FileEncryption.EncryptFile(inputStream, outputStream, password, originalExtension, originalDate);
            }

            return tempPath;
        }
    }

    [TestMethod]
    public void TestMethodPicture()
    {
        string filePath = @"C:\Users\sanut\OneDrive\Pictures\2024_12\PXL_20241214_194939029.jpg";

        string tmpEncryptPath = null;
        string tmpDecryptPath = null;

        try
        {
            tmpEncryptPath = DoEncrypt(filePath, "testPassword", "picture", out string originalExtension, out DateTime originalDate);
            tmpDecryptPath = DoDecrypt(tmpEncryptPath, out string originalExtensionTest, out DateTime originalDateTest);

            Assert.AreEqual(originalExtension, originalExtensionTest, "The original and decrypted file extensions should match.");
            Assert.AreEqual(originalDate, originalDateTest, "The original and decrypted file dates should match.");

            // compare the original file with the decrypted file
            using (FileStream originalStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (FileStream decryptedStream = new FileStream(tmpDecryptPath, FileMode.Open, FileAccess.Read))
                {
                    byte[] originalBuffer = new byte[4096];
                    byte[] decryptedBuffer = new byte[4096];
                    int originalBytesRead;
                    int decryptedBytesRead;
                    do
                    {
                        originalBytesRead = originalStream.Read(originalBuffer, 0, originalBuffer.Length);
                        decryptedBytesRead = decryptedStream.Read(decryptedBuffer, 0, decryptedBuffer.Length);
                        Assert.AreEqual(originalBytesRead, decryptedBytesRead, "The number of bytes read from the original and decrypted files should match.");
                        Assert.IsTrue(originalBuffer.Take(originalBytesRead).SequenceEqual(decryptedBuffer.Take(decryptedBytesRead)), "The contents of the original and decrypted files should match.");
                    }
                    while (originalBytesRead > 0);
                }
            }
        }
        finally
        {
            if (File.Exists(tmpDecryptPath))
            {
                File.Delete(tmpDecryptPath);
            }
        }
    }
}
