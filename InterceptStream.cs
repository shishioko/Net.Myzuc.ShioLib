using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Net.Myzuc.ShioLib
{
    public class InterceptStream<T> : Stream where T : Stream
    {
        private readonly T Stream;
        private readonly Stream Input;
        private readonly Stream Output;
        public override bool CanRead => Stream.CanRead;
        public override bool CanWrite => Stream.CanWrite;
        public override bool CanSeek => false;
        public override bool CanTimeout => Stream.CanTimeout;
        public override long Length => throw new System.NotSupportedException();
        public override long Position { get => throw new System.NotSupportedException(); set => throw new System.NotSupportedException(); }
        public InterceptStream(T stream, Stream input, Stream output)
        {
            Stream = stream;
            Input = input;
            Output = output;
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            int read = Stream.Read(buffer, offset, count);
            Input.Write(buffer, offset, read);
            return read;
        }
        public override int Read(Span<byte> buffer)
        {
            int read = Stream.Read(buffer);
            Input.Write(buffer.Slice(0, read));
            return read;
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            Output.Write(buffer, offset, count);
            Stream.Write(buffer, offset, count);
        }
        public override void Write(ReadOnlySpan<byte> buffer)
        {
            Output.Write(buffer);
            Stream.Write(buffer);
        }
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            int read = await Stream.ReadAsync(buffer, offset, count, cancellationToken);
            await Input.WriteAsync(buffer, offset, read);
            return read;
        }
        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            int read = await Stream.ReadAsync(buffer, cancellationToken);
            await Input.WriteAsync(buffer.Slice(0, read));
            return read;
        }
        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            await Output.WriteAsync(buffer, offset, count, cancellationToken);
            await Stream.WriteAsync(buffer, offset, count, cancellationToken);
        }
        public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            await Output.WriteAsync(buffer, cancellationToken);
            await Stream.WriteAsync(buffer, cancellationToken);
        }
        public override int ReadByte()
        {
            int read = Stream.ReadByte();
            if (read >= byte.MinValue && read <= byte.MaxValue) Input.WriteByte((byte)read);
            return read;
        }
        public override void WriteByte(byte value)
        {
            Output.WriteByte(value);
            Stream.WriteByte(value);
        }
        public override void CopyTo(Stream destination, int bufferSize)
        {
            byte[] buffer = new byte[bufferSize];
            int read = Stream.Read(buffer, 0, buffer.Length);
            Input.Write(buffer, 0, read);
            Stream.Write(buffer, 0, read);
        }
        public override async Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[bufferSize];
            int read = await Stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
            await Input.WriteAsync(buffer, 0, read, cancellationToken);
            await Stream.WriteAsync(buffer, 0, read, cancellationToken);
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
            Stream.Flush();
        }
        public override async Task FlushAsync(CancellationToken cancellationToken)
        {
            await Stream.FlushAsync(cancellationToken);
            await Stream.FlushAsync(cancellationToken);
        }
        public override void Close()
        {
            Stream.Close();
            Stream.Close();
        }
        protected override void Dispose(bool disposing)
        {
            Stream.Dispose();
            Stream.Dispose();
        }
        public async override ValueTask DisposeAsync()
        {
            await Stream.DisposeAsync();
            await Stream.DisposeAsync();
        }
    }
}
