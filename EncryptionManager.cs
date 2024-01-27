using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using System.Web;
using System.Windows.Interop;
using System.Globalization;

namespace ChatClient
{
    public class EncryptionManager
    {
        private static readonly byte[] Key = Convert.FromBase64String("AAECAwQFBgcICQoLDA0ODw==");
        private static byte[] IV = new byte[16];

        public static byte[] Encrypt(string plainText)
        {
            byte[] src = Encoding.UTF8.GetBytes(plainText);
            using (Aes aesAlg = Aes.Create())
            {
                GenerateIV();
                aesAlg.KeySize = 128;
                aesAlg.BlockSize = 128;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.Zeros;
                aesAlg.Key = Key;
                aesAlg.IV = (byte[])IV.Clone();

                using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                {
                    byte[] plaintextBytes = Encoding.UTF8.GetBytes(plainText);

                    // Encrypt the plaintext
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(plaintextBytes, 0, plaintextBytes.Length);

                    // Combine IV and ciphertext for transmission
                    byte[] result = new byte[aesAlg.IV.Length + encryptedBytes.Length];
                    Array.Copy(aesAlg.IV, result, aesAlg.IV.Length);
                    Array.Copy(encryptedBytes, 0, result, aesAlg.IV.Length, encryptedBytes.Length);

                    return result;
                }
            }
        }

        public static string Decrypt(string hexEncryptedString)
        {
            hexEncryptedString = hexEncryptedString.Replace(" ", "+");
            byte[] src = Convert.FromBase64String(hexEncryptedString);
            using (Aes aesAlg = Aes.Create())
            {
                GetIVFromEncryptedData(src);
                aesAlg.KeySize = 128;
                aesAlg.BlockSize = 128;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.Zeros;
                aesAlg.Key = Key;
                aesAlg.IV = (byte[])IV.Clone();
                
                

                using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                {
                    byte[] decryptedText = decryptor.TransformFinalBlock(src, 16, src.Length - 16);

                    return Encoding.UTF8.GetString(decryptedText).Trim('0'); // Trim trailing zeros added during padding
                }
            }
            
        }

        private static void GenerateIV()
        {
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(IV);
            }
        }

        private static void GetIVFromEncryptedData(byte[] encryptedData)
        {
            Array.Copy(encryptedData, 0, IV, 0, IV.Length);
        }
    }
}
