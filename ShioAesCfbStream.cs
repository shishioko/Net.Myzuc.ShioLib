using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Net.Myzuc.ShioLib
{
    public sealed class ShioAesCfbStream : Stream
    {
        public readonly Stream Stream;
        private readonly Aes Aes;
        private readonly bool KeepOpen;
        private readonly byte[] ReadVector = new byte[16];
        private readonly byte[] WriteVector = new byte[16];
        private int ReadVectorPosition = 0;
        private int WriteVectorPosition = 0;
        public override bool CanRead => Stream.CanRead;
        public override bool CanWrite => Stream.CanWrite;
        public override bool CanSeek => false;
        public override bool CanTimeout => Stream.CanTimeout;
        public override long Length => throw new System.NotSupportedException();
        public override long Position { get => throw new System.NotSupportedException(); set => throw new System.NotSupportedException(); }
        public ShioAesCfbStream(Stream stream, byte[] secret, byte[] vector, bool keepOpen)
        {
            Stream = stream;
            Buffer.BlockCopy(vector, 0, ReadVector, 0, 16);
            Buffer.BlockCopy(vector, 0, WriteVector, 0, 16);
            Aes = Aes.Create();
            Aes.BlockSize = 128;
            Aes.KeySize = secret.Length * 8;
            Aes.Key = secret;
            Aes.Mode = CipherMode.ECB;
            Aes.Padding = PaddingMode.None;
            KeepOpen = keepOpen;
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            int read = Stream.Read(buffer, offset, count);
            if (read <= 0) return read;
            for (int i = 0, size; i < read; i += size)
            {
                size = int.Min(16 - (ReadVectorPosition &= 15), read - i);
                if (ReadVectorPosition == 0) Buffer.BlockCopy(Aes.EncryptEcb(ReadVector, PaddingMode.None), 0, ReadVector, 0, 16);
                for (int e = 0; e < size; e++)
                {
                    ReadVector[ReadVectorPosition] ^= buffer[offset + i + e] ^= ReadVector[ReadVectorPosition++];
                }
            }
            return read;
        }
        public override int Read(Span<byte> buffer)
        {
            int read = Stream.Read(buffer);
            if (read <= 0) return read;
            for (int i = 0, size; i < read; i += size)
            {
                size = int.Min(16 - (ReadVectorPosition &= 15), read - i);
                if (ReadVectorPosition == 0) Buffer.BlockCopy(Aes.EncryptEcb(ReadVector, PaddingMode.None), 0, ReadVector, 0, 16);
                for (int e = 0; e < size; e++)
                {
                    ReadVector[ReadVectorPosition] ^= buffer[i + e] ^= ReadVector[ReadVectorPosition++];
                }
            }
            return read;
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (count <= 0) return;
            for (int i = 0, size; i < count; i += size)
            {
                size = int.Min(16 - (WriteVectorPosition &= 15), count - i);
                if (WriteVectorPosition == 0) Buffer.BlockCopy(Aes.EncryptEcb(WriteVector, PaddingMode.None), 0, WriteVector, 0, 16);
                for (int e = 0; e < size; e++)
                {
                    WriteVector[WriteVectorPosition++] ^= buffer[offset + i + e];
                }
                Stream.Write(WriteVector, WriteVectorPosition - size, size);
            }
        }
        public override void Write(ReadOnlySpan<byte> buffer)
        {
            if (buffer.Length <= 0) return;
            for (int i = 0, size; i < buffer.Length; i += size)
            {
                size = int.Min(16 - (WriteVectorPosition &= 15), buffer.Length - i);
                if (WriteVectorPosition == 0) Buffer.BlockCopy(Aes.EncryptEcb(WriteVector, PaddingMode.None), 0, WriteVector, 0, 16);
                for (int e = 0; e < size; e++)
                {
                    WriteVector[WriteVectorPosition++] ^= buffer[i + e];
                }
                Stream.Write(WriteVector, WriteVectorPosition - size, size);
            }
        }
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken = default)
        {
            int read = await Stream.ReadAsync(buffer.AsMemory(offset, count), cancellationToken);
            if (read <= 0) return read;
            for (int i = 0, size; i < read; i += size)
            {
                size = int.Min(16 - (ReadVectorPosition &= 15), read - i);
                if (ReadVectorPosition == 0) Buffer.BlockCopy(Aes.EncryptEcb(ReadVector, PaddingMode.None), 0, ReadVector, 0, 16);
                for (int e = 0; e < size; e++)
                {
                    ReadVector[ReadVectorPosition] ^= buffer[offset + i + e] ^= ReadVector[ReadVectorPosition++];
                }
            }
            return read;
        }
        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            int read = await Stream.ReadAsync(buffer, cancellationToken);
            if (read <= 0) return read;
            for (int i = 0, size; i < read; i += size)
            {
                size = int.Min(16 - (ReadVectorPosition &= 15), read - i);
                if (ReadVectorPosition == 0) Buffer.BlockCopy(Aes.EncryptEcb(ReadVector, PaddingMode.None), 0, ReadVector, 0, 16);
                for (int e = 0; e < size; e++)
                {
                    ReadVector[ReadVectorPosition] ^= buffer.Span[i + e] ^= ReadVector[ReadVectorPosition++];
                }
            }
            return read;
        }
        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken = default)
        {
            if (count <= 0) return;
            for (int i = 0, size; i < count; i += size)
            {
                size = int.Min(16 - (WriteVectorPosition &= 15), count - i);
                if (WriteVectorPosition == 0) Buffer.BlockCopy(Aes.EncryptEcb(WriteVector, PaddingMode.None), 0, WriteVector, 0, 16);
                for (int e = 0; e < size; e++)
                {
                    WriteVector[WriteVectorPosition++] ^= buffer[offset + i + e];
                }
                await Stream.WriteAsync(WriteVector.AsMemory(WriteVectorPosition - size, size), cancellationToken);
            }
        }
        public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (buffer.Length <= 0) return;
            for (int i = 0, size; i < buffer.Length; i += size)
            {
                size = int.Min(16 - (WriteVectorPosition &= 15), buffer.Length - i);
                if (WriteVectorPosition == 0) Buffer.BlockCopy(Aes.EncryptEcb(WriteVector, PaddingMode.None), 0, WriteVector, 0, 16);
                for (int e = 0; e < size; e++)
                {
                    WriteVector[WriteVectorPosition++] ^= buffer.Span[i + e];
                }
                await Stream.WriteAsync(WriteVector.AsMemory(WriteVectorPosition - size, size), cancellationToken);
            }
        }
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new System.NotSupportedException();
        }
        public override void SetLength(long value)
        {
            throw new System.NotSupportedException();
        }
        public override void Flush()
        {
            Stream.Flush();
        }
        public override Task FlushAsync(CancellationToken cancellationToken = default)
        {
            return Stream.FlushAsync(cancellationToken);
        }
        public override void Close()
        {
            Stream.Close();
        }
        protected override void Dispose(bool disposing)
        {
            Aes.Dispose();
            if (!KeepOpen) Stream.Dispose();
        }
        public async override ValueTask DisposeAsync()
        {
            Aes.Dispose();
            if (!KeepOpen) await Stream.DisposeAsync();
        }
    }
}