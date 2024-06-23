using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Net.Myzuc.UtilLib
{
    public sealed class ChannelStream : Stream
    {
        public static (ChannelStream a, ChannelStream b) CreatePair()
        {
            Channel<byte[]> a = Channel.CreateUnbounded<byte[]>(new() { SingleReader = false, SingleWriter = false }); 
            Channel<byte[]> b = Channel.CreateUnbounded<byte[]>(new() { SingleReader = false, SingleWriter = false });
            return (new(a.Reader, b.Writer), new(b.Reader, a.Writer));
        }
        public readonly ChannelReader<byte[]>? Reader;
        public readonly ChannelWriter<byte[]>? Writer;
        private byte[] LastRead = Array.Empty<byte>();
        private int LastReadPosition = 0;
        public override bool CanRead => Reader is not null;
        public override bool CanSeek => false;
        public override bool CanWrite => Writer is not null;
        public override long Length => throw new System.NotSupportedException();
        public override long Position { get => throw new System.NotSupportedException(); set => throw new System.NotSupportedException(); }
        public ChannelStream(ChannelReader<byte[]>? reader, ChannelWriter<byte[]>? writer)
        {
            Reader = reader;
            Writer = writer;
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
            WriteAsync(buffer, offset, count).Wait();
        }
        public override void Write(ReadOnlySpan<byte> buffer)
        {
            WriteAsync(new ReadOnlyMemory<byte>(buffer.ToArray())).AsTask().Wait();
        }
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            Contract.Requires(CanRead);
            if (LastRead.Length <= LastReadPosition)
            {
                if (Reader!.Completion.IsCompleted && Reader!.Count == 0) return 0;
                do LastRead = await Reader!.ReadAsync(cancellationToken);
                while (LastRead.Length <= 0);
                LastReadPosition = 0;
            }
            int length = int.Min(count, LastRead.Length - LastReadPosition);
            Array.Copy(LastRead, LastReadPosition, buffer, offset, length);
            LastReadPosition += length;
            return length;
        }
        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            Contract.Requires(CanRead);
            if (LastRead.Length <= LastReadPosition)
            {
                if (Reader!.Completion.IsCompleted && Reader!.Count == 0) return 0;
                do LastRead = await Reader!.ReadAsync(cancellationToken);
                while (LastRead.Length == 0);
                LastReadPosition = 0;
            }
            int length = int.Min(buffer.Length, LastRead.Length - LastReadPosition);
            LastRead.AsSpan(LastReadPosition, length).CopyTo(buffer.Span);
            LastReadPosition += length;
            return length;
        }
        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            Contract.Requires(CanWrite);
            await Writer!.WriteAsync(buffer[offset..(offset+count)], cancellationToken);
        }
        public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            await Writer!.WriteAsync(buffer.ToArray(), cancellationToken);
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
            Contract.Requires(CanWrite);
            Writer!.Complete();
        }
        protected override void Dispose(bool disposing)
        {
            Writer?.Complete();
        }
    }
}
