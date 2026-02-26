using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace StockManagement.Business.Constants.Services
{
    public class EncryptionService
    {
        private readonly byte[] _key;

        public EncryptionService(IConfiguration configuration)
        {
            var keyBase64 = configuration["Encryption:Key"]
                           ?? throw new InvalidOperationException("Encryption:Key read error!");

            if (string.IsNullOrWhiteSpace(keyBase64) || keyBase64.Length < 44)
                throw new InvalidOperationException("Encryption:Undefineted Key!");

            _key = Convert.FromBase64String(keyBase64);
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;

            using AesGcm aes = new(_key);
            byte[] nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
            RandomNumberGenerator.Fill(nonce);

            byte[] plaintextBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] ciphertext = new byte[plaintextBytes.Length];
            byte[] tag = new byte[AesGcm.TagByteSizes.MaxSize];

            aes.Encrypt(nonce, plaintextBytes, ciphertext, tag);

            byte[] result = new byte[nonce.Length + ciphertext.Length + tag.Length];
            Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
            Buffer.BlockCopy(ciphertext, 0, result, nonce.Length, ciphertext.Length);
            Buffer.BlockCopy(tag, 0, result, nonce.Length + ciphertext.Length, tag.Length);

            return Convert.ToBase64String(result);
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return cipherText;

            try
            {
                byte[] fullCipher = Convert.FromBase64String(cipherText);

                using AesGcm aes = new(_key);
                byte[] nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
                byte[] ciphertext = new byte[fullCipher.Length - nonce.Length - AesGcm.TagByteSizes.MaxSize];
                byte[] tag = new byte[AesGcm.TagByteSizes.MaxSize];

                Buffer.BlockCopy(fullCipher, 0, nonce, 0, nonce.Length);
                Buffer.BlockCopy(fullCipher, nonce.Length, ciphertext, 0, ciphertext.Length);
                Buffer.BlockCopy(fullCipher, nonce.Length + ciphertext.Length, tag, 0, tag.Length);

                byte[] decrypted = new byte[ciphertext.Length];
                aes.Decrypt(nonce, ciphertext, tag, decrypted);

                return Encoding.UTF8.GetString(decrypted);
            }
            catch
            {
                return "[Non-encripted key]";
            }
        }
    }
}
