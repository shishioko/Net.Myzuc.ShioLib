using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Net.Myzuc.ShioLib
{
    public class AesCfbStream : WrapperStream<Stream, Stream>
    {
        public static async Task<byte[]> ExchangeKeyAsync(Stream stream, bool host, int aesKeySize = 32)
        {
            using RSA rsa = RSA.Create();
            rsa.KeySize = 2048;
            if (host)
            {
                await stream.WriteU8AAsync(rsa.ExportRSAPublicKey(), SizePrefix.S32V, rsa.KeySize / 8 + 32);
                byte[] secret = RandomNumberGenerator.GetBytes(aesKeySize);
                await stream.WriteU8AAsync(rsa.Encrypt(secret, RSAEncryptionPadding.Pkcs1), SizePrefix.S32V, rsa.KeySize / 8);
                return secret;
            }
            else
            {
                rsa.ImportRSAPublicKey(await stream.ReadU8AAsync(SizePrefix.S32V, rsa.KeySize / 8 + 32), out _);
                return rsa.Decrypt(await stream.ReadU8AAsync(SizePrefix.S32V, rsa.KeySize / 8), RSAEncryptionPadding.Pkcs1);
            }
        }
        public static byte[] ExchangeKey(Stream stream, bool host, int aesKeySize = 32)
        {
            using RSA rsa = RSA.Create();
            rsa.KeySize = 2048;
            if (host)
            {
                stream.WriteU8A(rsa.ExportRSAPublicKey(), SizePrefix.S32V, rsa.KeySize / 8 + 32);
                byte[] secret = RandomNumberGenerator.GetBytes(aesKeySize);
                stream.WriteU8A(rsa.Encrypt(secret, RSAEncryptionPadding.Pkcs1), SizePrefix.S32V, rsa.KeySize / 8);
                return secret;
            }
            else
            {
                rsa.ImportRSAPublicKey(stream.ReadU8A(SizePrefix.S32V, rsa.KeySize / 8 + 32), out _);
                return rsa.Decrypt(stream.ReadU8A(SizePrefix.S32V, rsa.KeySize / 8), RSAEncryptionPadding.Pkcs1);
            }
        }
        public AesCfbStream(Stream stream, byte[] secret, byte[] vector) : base(Null, Null)
        {
            using Aes aes = Aes.Create();
            aes.Mode = CipherMode.CFB;
            aes.BlockSize = 128;
            aes.FeedbackSize = 8;
            aes.KeySize = secret.Length * 8;
            aes.Key = secret;
            aes.IV = vector;
            aes.Padding = PaddingMode.None;
            Input = stream;
            Input = new CryptoStream(stream, aes.CreateDecryptor(), CryptoStreamMode.Read);
            Output = new CryptoStream(stream, aes.CreateEncryptor(), CryptoStreamMode.Write);
        }
    }
}
