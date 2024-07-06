using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Net.Myzuc.UtilLib
{
    public sealed class WrapperStream<InputType,OutputType> : Stream where InputType : Stream where OutputType : Stream
    {
        public readonly InputType? Input;
        public readonly OutputType? Output;
        public override bool CanRead => Input?.CanRead ?? false;
        public override bool CanWrite => Output?.CanWrite ?? false;
        public override bool CanSeek => false;
        public override long Length => throw new System.NotSupportedException();
        public override long Position { get => throw new System.NotSupportedException(); set => throw new System.NotSupportedException(); }
        public WrapperStream(InputType? input, OutputType? output)
        {
            Input = input;
            Output = output;
        }
        public WrapperStream<OutputType, InputType> Invert()
        {
            return new(Output, Input);
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            Contract.Requires(CanRead);
            return Input!.Read(buffer, offset, count);
        }
        public override int Read(Span<byte> buffer)
        {
            Contract.Requires(CanRead);
            return Input!.Read(buffer);
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            Contract.Requires(CanWrite);
            Output!.Write(buffer, offset, count);
        }
        public override void Write(ReadOnlySpan<byte> buffer)
        {
            Contract.Requires(CanWrite);
            Output!.Write(buffer);
        }
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            Contract.Requires(CanRead);
            return Input!.ReadAsync(buffer, offset, count, cancellationToken);
        }
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            Contract.Requires(CanRead);
            return Input!.ReadAsync(buffer, cancellationToken);
        }
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            Contract.Requires(CanWrite);
            return Output!.WriteAsync(buffer, offset, count, cancellationToken);
        }
        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            Contract.Requires(CanWrite);
            return Output!.WriteAsync(buffer, cancellationToken);
        }
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
        {
            Contract.Requires(CanRead);
            return Input!.BeginRead(buffer, offset, count, callback, state);
        }
        public override int EndRead(IAsyncResult asyncResult)
        {
            Contract.Requires(CanRead);
            return Input!.EndRead(asyncResult);
        }
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
        {
            Contract.Requires(CanWrite);
            return Output!.BeginWrite(buffer, offset, count, callback, state);
        }
        public override void EndWrite(IAsyncResult asyncResult)
        {
            Contract.Requires(CanWrite);
            Output!.EndWrite(asyncResult);
        }
        public override int ReadByte()
        {
            Contract.Requires(CanRead);
            return Input!.ReadByte();
        }
        public override void WriteByte(byte value)
        { 
            Contract.Requires(CanWrite);
            Output!.WriteByte(value);
        }
        public override void CopyTo(Stream destination, int bufferSize)
        {
            Contract.Requires(CanRead);
            Input!.CopyTo(destination, bufferSize);
        }
        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            Contract.Requires(CanRead);
            return Input!.CopyToAsync(destination, bufferSize, cancellationToken);
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
            Input?.Flush();
            Output?.Flush();
        }
        public override async Task FlushAsync(CancellationToken cancellationToken)
        {
            if (Input is not null) await Input.FlushAsync(cancellationToken);
            if (Output is not null) await Output.FlushAsync(cancellationToken);
        }
        public override void Close()
        {
            Input?.Close();
            Output?.Close();
        }
        protected override void Dispose(bool disposing)
        {
            Input?.Dispose();
            Output?.Dispose();
        }
        public async override ValueTask DisposeAsync()
        {
            if (Input is not null) await Input.DisposeAsync();
            if (Output is not null) await Output.DisposeAsync();
        }
    }
}
