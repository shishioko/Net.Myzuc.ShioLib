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
                await source.ReadAsync(buffer.AsMemory(0, request));
                await client.WriteAsync(buffer.AsMemory(0, request));
            }
        }
        private readonly Stream Stream;
        private byte[] Buffer;
        private int BufferPosition;
        private int BufferRequests = 0;
        private readonly ushort BufferSize;
        private readonly int BufferCount;
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new System.NotSupportedException();
        public override long Position { get => throw new System.NotSupportedException(); set => throw new System.NotSupportedException(); }
        public OnDemandStream(Stream stream, ushort bufferSize, int bufferCount)
        {
            Stream = stream;
            BufferSize = bufferSize;
            BufferCount = bufferCount;
            Buffer = new byte[BufferSize];
            BufferPosition = BufferSize;
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
            while (BufferRequests < BufferCount)
            {
                await Stream.WriteU16Async(BufferSize);
                BufferRequests++;
            }
            if (Buffer.Length <= BufferPosition)
            {
                await Stream.ReadAsync(Buffer, cancellationToken);
                BufferPosition = 0;
                BufferRequests--;
            }
            int length = int.Min(buffer.Length, Buffer.Length - BufferPosition);
            Buffer.AsMemory(BufferPosition, length).CopyTo(buffer);
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
