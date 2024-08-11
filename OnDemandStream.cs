using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace Net.Myzuc.ShioLib
{
    public sealed class OnDemandStream : Stream
    {
        public static async Task ProvideAsync(Stream client, Stream source)
        {
            byte[] buffer = new byte[ushort.MaxValue];
            while (true)
            {
                ushort request = await client.ReadU16Async();
                while (request > 0)
                {
                    int read = await source.ReadAsync(buffer.AsMemory(0, request));
                    if (read <= 0) return;
                    await client.WriteAsync(buffer.AsMemory(0, read));
                    request -= (ushort)read;
                }
            }
        }
        private readonly Stream Stream;
        private byte[] Buffer;
        private int BufferPosition = 0;
        private int BufferSize = 0;
        private int BufferRequests = 0;
        private readonly int BufferCount;
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new System.NotSupportedException();
        public override long Position { get => throw new System.NotSupportedException(); set => throw new System.NotSupportedException(); }
        public OnDemandStream(Stream stream, ushort bufferSize, int bufferCount)
        {
            Stream = stream;
            BufferCount = bufferCount;
            Buffer = new byte[BufferPosition = BufferSize = bufferSize];
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            return ReadAsync(buffer, offset, count).Result;
        }
        public override int Read(Span<byte> buffer)
        {
            byte[] data = new byte[buffer.Length];
            int read = ReadAsync(data, 0, buffer.Length).Result;
            data.AsSpan().CopyTo(buffer);
            return read;
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
        public override void Write(ReadOnlySpan<byte> buffer)
        {
            throw new NotSupportedException();
        }
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return ReadAsync(buffer.AsMemory(offset, count), cancellationToken).AsTask();
        }
        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (BufferSize <= 0) return 0;
            while (BufferRequests < BufferCount)
            {
                await Stream.WriteU16Async((ushort)Buffer.Length);
                BufferRequests++;
            }
            if (BufferSize <= BufferPosition)
            {
                BufferSize = await Stream.ReadAsync(Buffer, cancellationToken);
                if (BufferSize <= 0) return 0;
                BufferPosition = 0;
                BufferRequests--;
            }
            int length = int.Min(buffer.Length, BufferSize - BufferPosition);
            Buffer.AsMemory(BufferPosition, length).CopyTo(buffer);
            BufferPosition += length;
            return length;
        }
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
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
            return;
        }
        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        public override void Close()
        {
            Stream.Close();
        }
        protected override void Dispose(bool disposing)
        {
            Stream.Dispose();
        }
    }
}
