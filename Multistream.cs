using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Net.Myzuc.ShioLib
{
    /// <summary>
    /// Representative of a connection to a Multistream server.
    /// </summary>
    public sealed class MultiStream : IDisposable, IAsyncDisposable
    {
        private bool KeepOpen;
        private bool Disposed = false;
        private readonly Stream Stream;
        private readonly SemaphoreSlim Sync = new(1, 1);
        private readonly SemaphoreSlim SyncWrite = new(1, 1);
        private readonly Dictionary<Guid, ChannelStream> Streams = [];
        /// <summary>
        /// Fired whenever a <see cref="ChannelStream"/> is opened.
        /// </summary>
        public event Func<ChannelStream, Task> OnRequest = (ChannelStream stream) => Task.CompletedTask;
        /// <summary>
        /// Fired once after disposal of the <see cref="MultiStream"/> has finished.
        /// </summary>
        public event Func<Task> OnDisposed = () => Task.CompletedTask;
        /// <summary>
        /// Creates a new <see cref="MultiStream"/> on top of a <see cref="System.IO.Stream"/>
        /// </summary>
        /// <param name="stream">The underlying<see cref="System.IO.Stream"/></param>
        /// <param name="keepOpen">Whether to keep the underlying <see cref="System.IO.Stream"/> open after disposal</param>
        public MultiStream(Stream stream, bool keepOpen)
        {
            Stream = stream;
            KeepOpen = keepOpen;
            _ = ReceiveAsync();
        }
        /// <summary>
        /// Closes all <see cref="ChannelStream"/> and conditionally disposes the underlying <see cref="System.IO.Stream"/> asynchronously.
        /// </summary>
        /// <returns></returns>
        public async ValueTask DisposeAsync()
        {
            if (Disposed) return;
            Disposed = true;
            if (!KeepOpen) await Stream.DisposeAsync();
            Sync.Dispose();
            SyncWrite.Dispose();
            foreach (ChannelStream stream in Streams.Values) await stream.DisposeAsync();
            await OnDisposed();
        }
        /// <summary>
        /// Closes all <see cref="ChannelStream"/> and conditionally disposes the underlying <see cref="System.Net.Sockets.Socket"/> synchronously.
        /// </summary>
        /// <returns></returns>
        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
            if (!KeepOpen) Stream.Dispose();
            Sync.Dispose();
            SyncWrite.Dispose();
            foreach (ChannelStream stream in Streams.Values) stream.Dispose();
            OnDisposed().Wait();
        }
        /// <summary>
        /// Opens a new <see cref="ChannelStream"/> on the <see cref="MultiStream"/> asynchronously.
        /// </summary>
        /// <returns>The newly opened <see cref="ChannelStream"/></returns>
        public async Task<ChannelStream> OpenAsync()
        {
            await Sync.WaitAsync();
            Guid streamId = Guid.NewGuid();
            while (Streams.ContainsKey(streamId)) streamId = Guid.NewGuid();
            (ChannelStream userStream, ChannelStream appStream) = ChannelStream.CreatePair();
            Streams.Add(streamId, appStream);
            _ = SendAsync(streamId, appStream);
            Sync.Release();
            return userStream;
        }
        /// <summary>
        /// Opens a new <see cref="ChannelStream"/> on the <see cref="MultiStream"/> synchronously.
        /// </summary>
        /// <returns>The newly opened <see cref="ChannelStream"/></returns>
        public ChannelStream Open()
        {
            return OpenAsync().Result;
        }
        private async Task ReceiveAsync()
        {
            try
            {
                while (!Disposed)
                {
                    Guid streamId = await Stream.ReadGuidAsync();
                    byte[] data = await Stream.ReadU8AAsync(SizePrefix.U32V, 1024);
                    await Sync.WaitAsync();
                    if (!Streams.TryGetValue(streamId, out ChannelStream? stream))
                    {
                        (ChannelStream userStream, ChannelStream appStream) = ChannelStream.CreatePair();
                        Streams.Add(streamId, appStream);
                        _ = SendAsync(streamId, appStream);
                        await OnRequest(userStream);
                        stream = appStream;
                    }
                    await stream!.WriteAsync(data);
                    if (data.Length <= 0)
                    {
                        stream.Writer!.Complete();
                        Streams.Remove(streamId);
                    }
                    Sync.Release();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

            }
            finally
            {
                await DisposeAsync();
            }
        }
        private async Task SendAsync(Guid streamId, ChannelStream stream)
        {
            try
            {
                while (!Disposed)
                {
                    bool complete = !await stream.Reader!.WaitToReadAsync();
                    byte[] data = complete ? [] : await stream.Reader!.ReadAsync();
                    if (!complete && data.Length == 0) continue;
                    for (int i = 0; i < data.Length; i += 1024)
                    {
                        await SyncWrite.WaitAsync();
                        await Stream.WriteGuidAsync(streamId);
                        int size = int.Min(data.Length - i, 1024);
                        await Stream.WriteU8AAsync(data[i..(i + size)], SizePrefix.U32V, 1024);
                        SyncWrite.Release();
                    }
                    if (complete) break;
                }
                if (!stream.Reader!.Completion.IsCompleted) await stream.DisposeAsync();
                await Sync.WaitAsync();
                Streams.Remove(streamId);
                Sync.Release();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await DisposeAsync();
            }
        }
    }
}
