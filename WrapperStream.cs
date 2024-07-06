using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Net.Myzuc.ShioLib
{
    public class WrapperStream<InputType, OutputType> : Stream where InputType : Stream where OutputType : Stream
    {
        public InputType Input;
        public OutputType Output;
        public override bool CanRead => Input.CanRead;
        public override bool CanWrite => Output.CanWrite;
        public override bool CanSeek => false;
        public override bool CanTimeout => Input.CanTimeout || Output.CanTimeout;
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
            return Input.Read(buffer, offset, count);
        }
        public override int Read(Span<byte> buffer)
        {
            return Input.Read(buffer);
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            Output.Write(buffer, offset, count);
        }
        public override void Write(ReadOnlySpan<byte> buffer)
        {
            Output.Write(buffer);
        }
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return Input.ReadAsync(buffer, offset, count, cancellationToken);
        }
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return Input.ReadAsync(buffer, cancellationToken);
        }
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return Output.WriteAsync(buffer, offset, count, cancellationToken);
        }
        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return Output.WriteAsync(buffer, cancellationToken);
        }
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
        {
            return Input.BeginRead(buffer, offset, count, callback, state);
        }
        public override int EndRead(IAsyncResult asyncResult)
        {
            return Input.EndRead(asyncResult);
        }
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
        {
            return Output.BeginWrite(buffer, offset, count, callback, state);
        }
        public override void EndWrite(IAsyncResult asyncResult)
        {
            Output.EndWrite(asyncResult);
        }
        public override int ReadByte()
        {
            return Input.ReadByte();
        }
        public override void WriteByte(byte value)
        { 
            Output.WriteByte(value);
        }
        public override void CopyTo(Stream destination, int bufferSize)
        {
            Input.CopyTo(destination, bufferSize);
        }
        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            return Input.CopyToAsync(destination, bufferSize, cancellationToken);
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
            Input.Flush();
            Output.Flush();
        }
        public override async Task FlushAsync(CancellationToken cancellationToken)
        {
            await Input.FlushAsync(cancellationToken);
            await Output.FlushAsync(cancellationToken);
        }
        public override void Close()
        {
            Input.Close();
            Output.Close();
        }
        protected override void Dispose(bool disposing)
        {
            Input.Dispose();
            Output.Dispose();
        }
        public async override ValueTask DisposeAsync()
        {
            await Input.DisposeAsync();
            await Output.DisposeAsync();
        }
    }
}
