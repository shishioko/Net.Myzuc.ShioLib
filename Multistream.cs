using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Net.Myzuc.UtilLib
{
    /// <summary>
    /// Representative of a connection to a Multistream server.
    /// </summary>
    public sealed class MultistreamConnection : IDisposable, IAsyncDisposable
    {
        private bool Disposed = false;
        private Stream Stream;
        private readonly SemaphoreSlim Sync = new(1, 1);
        private readonly SemaphoreSlim SyncWrite = new(1, 1);
        private readonly Dictionary<Guid, ChannelStream> Streams = [];
        /// <summary>
        /// Fired whenever a <see cref="Net.Myzuc.UtilLib.ChannelStream"/> is opened.
        /// </summary>
        public event Func<ChannelStream, Task> OnRequest = (ChannelStream stream) => Task.CompletedTask;
        /// <summary>
        /// Fired once after disposal of the <see cref="Net.Myzuc.Multistream.Client.MultistreamConnection"/> has finished.
        /// </summary>
        public event Func<Task> OnDisposed = () => Task.CompletedTask;
        public MultistreamConnection(Stream stream)
        {
            Stream = stream;
        }
        /// <summary>
        /// Closes all <see cref="Net.Myzuc.UtilLib.ChannelStream"/> and disposes the underlying <see cref="System.Net.Sockets.Socket"/> asynchronously.
        /// </summary>
        /// <returns></returns>
        public async ValueTask DisposeAsync()
        {
            if (Disposed) return;
            Disposed = true;
            await Stream.DisposeAsync();
            Sync.Dispose();
            SyncWrite.Dispose();
            foreach (ChannelStream stream in Streams.Values) await stream.DisposeAsync();
            await OnDisposed();
        }
        /// <summary>
        /// Closes all <see cref="Net.Myzuc.UtilLib.ChannelStream"/> and disposes the underlying <see cref="System.Net.Sockets.Socket"/> synchronously.
        /// </summary>
        /// <returns></returns>
        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
            Stream.Dispose();
            Sync.Dispose();
            SyncWrite.Dispose();
            foreach (ChannelStream stream in Streams.Values) stream.Dispose();
            OnDisposed().Wait();
        }
        /// <summary>
        /// Opens a new <see cref="Net.Myzuc.UtilLib.ChannelStream"/> on the <see cref="Net.Myzuc.Multistream.Client.MultistreamConnection"/> asynchronously.
        /// </summary>
        /// <returns>The newly opened <see cref="Net.Myzuc.UtilLib.ChannelStream"/></returns>
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
        /// Opens a new <see cref="Net.Myzuc.UtilLib.ChannelStream"/> on the <see cref="Net.Myzuc.Multistream.Client.MultistreamConnection"/> synchronously.
        /// </summary>
        /// <returns>The newly opened <see cref="Net.Myzuc.UtilLib.ChannelStream"/></returns>
        public ChannelStream Open()
        {
            return OpenAsync().Result;
        }
        private async Task InitializeAsync()
        {
            Version localVersion = Assembly.GetExecutingAssembly().GetName().Version ?? new(0, 0, 0, 0);
            await Stream.WriteS32VAsync(localVersion.Major);
            await Stream.WriteS32VAsync(localVersion.Minor);
            await Stream.WriteS32VAsync(localVersion.MajorRevision);
            await Stream.WriteS32VAsync(localVersion.MinorRevision);
            if (localVersion != new Version(await Stream.ReadS32VAsync(), await Stream.ReadS32VAsync(), await Stream.ReadS32VAsync(), await Stream.ReadS32VAsync()))
            {
                await DisposeAsync();
                throw new NotSupportedException("Version mismatch");
            }
            using RSA rsa = RSA.Create();
            rsa.KeySize = 2048;
            await Stream.WriteU8AAsync(rsa.ExportRSAPublicKey(), SizePrefix.S32V, rsa.KeySize / 8 + 128);
            byte[] secret = rsa.Decrypt(await Stream.ReadU8AAsync(SizePrefix.S32V), RSAEncryptionPadding.Pkcs1);
            using Aes aes = Aes.Create();
            aes.Mode = CipherMode.CFB;
            aes.BlockSize = 128;
            aes.FeedbackSize = 8;
            aes.KeySize = 256;
            aes.Key = secret;
            aes.IV = secret[..16];
            aes.Padding = PaddingMode.PKCS7;
            Stream = new WrapperStream<CryptoStream, CryptoStream>(new(Stream, aes.CreateDecryptor(), CryptoStreamMode.Read), new(Stream, aes.CreateEncryptor(), CryptoStreamMode.Write));
            _ = ReceiveAsync();
        }
        private async Task ReceiveAsync()
        {
            try
            {
                while (!Disposed)
                {
                    Guid streamId = await Stream.ReadGuidAsync();
                    byte[] data = await Stream.ReadU8AAsync(SizePrefix.U16, 1024);
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
                    await Stream.ReadU8AAsync(((16 - ((18 + data.Length) % 16)) & 15) + 16);
                }
            }
            catch (Exception)
            {

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
                        int size = int.Min(data.Length - i, i + 1024);
                        await Stream.WriteU8AAsync(data[i..size], SizePrefix.U16, 1024);
                        await Stream.WriteU8AAsync(new byte[((16 - ((18 + size) % 16)) & 15) + 16]);
                        SyncWrite.Release();
                    }
                    if (complete) break;
                }
                if (!stream.Reader!.Completion.IsCompleted) await stream.DisposeAsync();
                await Sync.WaitAsync();
                Streams.Remove(streamId);
                Sync.Release();
            }
            catch (Exception)
            {
                await DisposeAsync();
            }
        }
    }
}
