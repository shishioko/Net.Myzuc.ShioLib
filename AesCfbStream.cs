using System.IO;
using System.Security.Cryptography;

namespace Net.Myzuc.UtilLib
{
    public sealed class AesCfbStream
    {
        public readonly Stream Stream;
        public AesCfbStream(Stream stream, byte[] secret, byte[] vector)
        {
            using Aes aes = Aes.Create();
            aes.Mode = CipherMode.CFB;
            aes.BlockSize = 128;
            aes.FeedbackSize = 8;
            aes.KeySize = secret.Length * 8;
            aes.Key = secret;
            aes.IV = vector;
            aes.Padding = PaddingMode.None;
            Stream = new WrapperStream<CryptoStream, CryptoStream>(new(Stream, aes.CreateDecryptor(), CryptoStreamMode.Read), new(Stream, aes.CreateEncryptor(), CryptoStreamMode.Write));
        }
    }
}
