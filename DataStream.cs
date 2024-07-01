using System;
using System.Buffers.Binary;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Net.Myzuc.UtilLib
{
    /// <summary>
    /// Provides methods for reading and writing various data formats from any big endian stream.
    /// </summary>
    public sealed class DataStream<T> : IDisposable, IAsyncDisposable where T : Stream
    {
        public readonly T Stream;
        private readonly bool KeepOpen;
        private readonly DataStream<Stream> Normal;
        public DataStream(T stream, bool keepOpen = false)
        {
            Stream = stream;
            KeepOpen = keepOpen;
            if (this is DataStream<Stream> normal) Normal = normal;
            else Normal = new(Stream, KeepOpen);
        }
        public void Dispose()
        {
            if (!KeepOpen) Stream.Dispose();
        }
        public async ValueTask DisposeAsync()
        {
            if (!KeepOpen) await Stream.DisposeAsync();
        }

        #region U8
        public async Task<byte[]> ReadU8AAsync(int size)
        {
            byte[] data = new byte[size];
            int position = 0;
            while (position < size)
            {
                int read = await Stream.ReadAsync(data.AsMemory(position, size - position));
                position += read;
                if (read == 0) throw new EndOfStreamException();
            }
            return data;
        }
        public async Task WriteU8AAsync(byte[] data)
        {
            await Stream.WriteAsync(data);
        }
        public async Task<byte[]> ReadU8AAsync(SizePrefix prefix)
        {
            return await ReadU8AAsync(await prefix.ReadAsync(Normal));
        }
        public async Task WriteU8AAsync(byte[] data, SizePrefix prefix)
        {
            await prefix.WriteAsync(Normal, data.Length);
            await WriteU8AAsync(data);
        }
        public async Task<byte> ReadU8Async()
        {
            return (await ReadU8AAsync(1))[0];
        }
        public async Task WriteU8Async(byte data)
        {
            await Stream.WriteAsync(new byte[] { data });
        }
        public byte[] ReadU8A(int size)
        {
            byte[] data = new byte[size];
            int position = 0;
            while (position < size)
            {
                int read = Stream.Read(data, position, size - position);
                position += read;
                if (read == 0) throw new EndOfStreamException();
            }
            return data;
        }
        public void WriteU8A(byte[] data)
        {
            Stream.Write(data);
        }
        public byte[] ReadU8A(SizePrefix prefix)
        {
            return ReadU8A(prefix.ReadSync(Normal));
        }
        public void WriteU8A(byte[] data, SizePrefix prefix)
        {
            prefix.WriteSync(Normal, data.Length);
            WriteU8A(data);
        }
        public byte ReadU8()
        {
            return (ReadU8A(1))[0];
        }
        public void WriteU8(byte data)
        {
            Stream.Write([data]);
        }
        #endregion
        #region S8
        public async Task<sbyte[]> ReadS8AAsync(int size)
        {
            return MemoryMarshal.Cast<byte, sbyte>(await ReadU8AAsync(size)).ToArray();
        }
        public async Task WriteS8AAsync(sbyte[] data)
        {
            await Stream.WriteAsync(MemoryMarshal.AsBytes(data.AsSpan()).ToArray());
        }
        public async Task<sbyte[]> ReadS8AAsync(SizePrefix prefix)
        {
            return MemoryMarshal.Cast<byte, sbyte>(await ReadU8AAsync(await prefix.ReadAsync(Normal))).ToArray();
        }
        public async Task WriteS8AAsync(sbyte[] data, SizePrefix prefix)
        {
            await prefix.WriteAsync(Normal, data.Length);
            await Stream.WriteAsync(MemoryMarshal.AsBytes(data.AsSpan()).ToArray());
        }
        public async Task<sbyte> ReadS8Async()
        {
            return (await ReadS8AAsync(1))[0];
        }
        public async Task WriteS8Async(sbyte data)
        {
            await Stream.WriteAsync(MemoryMarshal.AsBytes<sbyte>(new sbyte[] { data }).ToArray());
        }
        public sbyte[] ReadS8A(int size)
        {
            return MemoryMarshal.Cast<byte, sbyte>(ReadU8A(size)).ToArray();
        }
        public void WriteS8A(sbyte[] data)
        {
            Stream.Write(MemoryMarshal.AsBytes(data.AsSpan()).ToArray());
        }
        public sbyte[] ReadS8A(SizePrefix prefix)
        {
            return MemoryMarshal.Cast<byte, sbyte>(ReadU8A(prefix.ReadSync(Normal))).ToArray();
        }
        public void WriteS8A(sbyte[] data, SizePrefix prefix)
        {
            prefix.WriteSync(Normal, data.Length);
            Stream.Write(MemoryMarshal.AsBytes(data.AsSpan()).ToArray());
        }
        public sbyte ReadS8()
        {
            return (ReadS8A(1))[0];
        }
        public void WriteS8(sbyte data)
        {
            Stream.Write(MemoryMarshal.AsBytes<sbyte>(new sbyte[] { data }).ToArray());
        }
        #endregion
        #region U16
        public async Task<ushort[]> ReadU16AAsync(int size)
        {
            byte[] buffer = await ReadU8AAsync(size * sizeof(ushort));
            ushort[] data = new ushort[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadUInt16BigEndian(buffer.AsSpan()[(i * sizeof(ushort))..]);
            }
            return data;
        }
        public async Task WriteU16AAsync(ushort[] data)
        {
            byte[] buffer = new byte[data.Length * sizeof(ushort)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteUInt16BigEndian(buffer.AsSpan()[(i * sizeof(ushort))..], data[i]);
            }
            await Stream.WriteAsync(buffer);
        }
        public async Task<ushort[]> ReadU16AAsync(SizePrefix prefix)
        {
            int size = await prefix.ReadAsync(Normal);
            byte[] buffer = await ReadU8AAsync(size * sizeof(ushort));
            ushort[] data = new ushort[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadUInt16BigEndian(buffer.AsSpan()[(i * sizeof(ushort))..]);
            }
            return data;
        }
        public async Task WriteU16AAsync(ushort[] data, SizePrefix prefix)
        {
            await prefix.WriteAsync(Normal, data.Length);
            byte[] buffer = new byte[data.Length * sizeof(ushort)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteUInt16BigEndian(buffer.AsSpan()[(i * sizeof(ushort))..], data[i]);
            }
            await Stream.WriteAsync(buffer);
        }
        public async Task<ushort> ReadU16Async()
        {
            byte[] buffer = await ReadU8AAsync(sizeof(ushort));
            return BinaryPrimitives.ReadUInt16BigEndian(buffer);
        }
        public async Task WriteU16Async(ushort data)
        {
            byte[] buffer = new byte[sizeof(ushort)];
            BinaryPrimitives.WriteUInt16BigEndian(buffer, data);
            await Stream.WriteAsync(buffer);
        }
        public ushort[] ReadU16A(int size)
        {
            byte[] buffer = ReadU8A(size * sizeof(ushort));
            ushort[] data = new ushort[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadUInt16BigEndian(buffer.AsSpan()[(i * sizeof(ushort))..]);
            }
            return data;
        }
        public void WriteU16A(ushort[] data)
        {
            byte[] buffer = new byte[data.Length * sizeof(ushort)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteUInt16BigEndian(buffer.AsSpan()[(i * sizeof(ushort))..], data[i]);
            }
            Stream.Write(buffer);
        }
        public ushort[] ReadU16A(SizePrefix prefix)
        {
            int size = prefix.ReadSync(Normal);
            byte[] buffer = ReadU8A(size * sizeof(ushort));
            ushort[] data = new ushort[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadUInt16BigEndian(buffer.AsSpan()[(i * sizeof(ushort))..]);
            }
            return data;
        }
        public void WriteU16A(ushort[] data, SizePrefix prefix)
        {
            prefix.WriteSync(Normal, data.Length);
            byte[] buffer = new byte[data.Length * sizeof(ushort)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteUInt16BigEndian(buffer.AsSpan()[(i * sizeof(ushort))..], data[i]);
            }
            Stream.Write(buffer);
        }
        public ushort ReadU16()
        {
            byte[] buffer = ReadU8A(sizeof(ushort));
            return BinaryPrimitives.ReadUInt16BigEndian(buffer);
        }
        public void WriteU16(ushort data)
        {
            byte[] buffer = new byte[sizeof(ushort)];
            BinaryPrimitives.WriteUInt16BigEndian(buffer, data);
            Stream.Write(buffer);
        }
        #endregion
        #region S16
        public async Task<short[]> ReadS16AAsync(int size)
        {
            byte[] buffer = await ReadU8AAsync(size * sizeof(short));
            short[] data = new short[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadInt16BigEndian(buffer.AsSpan()[(i * sizeof(short))..]);
            }
            return data;
        }
        public async Task WriteS16AAsync(short[] data)
        {
            byte[] buffer = new byte[data.Length * sizeof(short)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteInt16BigEndian(buffer.AsSpan()[(i * sizeof(short))..], data[i]);
            }
            await Stream.WriteAsync(buffer);
        }
        public async Task<short[]> ReadS16AAsync(SizePrefix prefix)
        {
            int size = await prefix.ReadAsync(Normal);
            byte[] buffer = await ReadU8AAsync(size * sizeof(short));
            short[] data = new short[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadInt16BigEndian(buffer.AsSpan()[(i * sizeof(short))..]);
            }
            return data;
        }
        public async Task WriteS16AAsync(short[] data, SizePrefix prefix)
        {
            await prefix.WriteAsync(Normal, data.Length);
            byte[] buffer = new byte[data.Length * sizeof(short)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteInt16BigEndian(buffer.AsSpan()[(i * sizeof(short))..], data[i]);
            }
            await Stream.WriteAsync(buffer);
        }
        public async Task<short> ReadS16Async()
        {
            byte[] buffer = await ReadU8AAsync(sizeof(short));
            return BinaryPrimitives.ReadInt16BigEndian(buffer);
        }
        public async Task WriteS16Async(short data)
        {
            byte[] buffer = new byte[sizeof(short)];
            BinaryPrimitives.WriteInt16BigEndian(buffer, data);
            await Stream.WriteAsync(buffer);
        }
        public short[] ReadS16A(int size)
        {
            byte[] buffer = ReadU8A(size * sizeof(short));
            short[] data = new short[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadInt16BigEndian(buffer.AsSpan()[(i * sizeof(short))..]);
            }
            return data;
        }
        public void WriteS16A(short[] data)
        {
            byte[] buffer = new byte[data.Length * sizeof(short)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteInt16BigEndian(buffer.AsSpan()[(i * sizeof(short))..], data[i]);
            }
            Stream.Write(buffer);
        }
        public short[] ReadS16A(SizePrefix prefix)
        {
            int size = prefix.ReadSync(Normal);
            byte[] buffer = ReadU8A(size * sizeof(short));
            short[] data = new short[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadInt16BigEndian(buffer.AsSpan()[(i * sizeof(short))..]);
            }
            return data;
        }
        public void WriteS16A(short[] data, SizePrefix prefix)
        {
            prefix.WriteSync(Normal, data.Length);
            byte[] buffer = new byte[data.Length * sizeof(short)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteInt16BigEndian(buffer.AsSpan()[(i * sizeof(short))..], data[i]);
            }
            Stream.Write(buffer);
        }
        public short ReadS16()
        {
            byte[] buffer = ReadU8A(sizeof(short));
            return BinaryPrimitives.ReadInt16BigEndian(buffer);
        }
        public void WriteS16(short data)
        {
            byte[] buffer = new byte[sizeof(short)];
            BinaryPrimitives.WriteInt16BigEndian(buffer, data);
            Stream.Write(buffer);
        }
        #endregion
        #region U32
        public async Task<uint[]> ReadU32AAsync(int size)
        {
            byte[] buffer = await ReadU8AAsync(size * sizeof(uint));
            uint[] data = new uint[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadUInt32BigEndian(buffer.AsSpan()[(i * sizeof(uint))..]);
            }
            return data;
        }
        public async Task WriteU32AAsync(uint[] data)
        {
            byte[] buffer = new byte[data.Length * sizeof(uint)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteUInt32BigEndian(buffer.AsSpan()[(i * sizeof(uint))..], data[i]);
            }
            await Stream.WriteAsync(buffer);
        }
        public async Task<uint[]> ReadU32AAsync(SizePrefix prefix)
        {
            int size = await prefix.ReadAsync(Normal);
            byte[] buffer = await ReadU8AAsync(size * sizeof(uint));
            uint[] data = new uint[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadUInt32BigEndian(buffer.AsSpan()[(i * sizeof(uint))..]);
            }
            return data;
        }
        public async Task WriteU32AAsync(uint[] data, SizePrefix prefix)
        {
            await prefix.WriteAsync(Normal, data.Length);
            byte[] buffer = new byte[data.Length * sizeof(uint)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteUInt32BigEndian(buffer.AsSpan()[(i * sizeof(uint))..], data[i]);
            }
            await Stream.WriteAsync(buffer);
        }
        public async Task<uint> ReadU32Async()
        {
            byte[] buffer = await ReadU8AAsync(sizeof(uint));
            return BinaryPrimitives.ReadUInt32BigEndian(buffer);
        }
        public async Task WriteU32Async(uint data)
        {
            byte[] buffer = new byte[sizeof(uint)];
            BinaryPrimitives.WriteUInt32BigEndian(buffer, data);
            await Stream.WriteAsync(buffer);
        }
        public uint[] ReadU32A(int size)
        {
            byte[] buffer = ReadU8A(size * sizeof(uint));
            uint[] data = new uint[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadUInt32BigEndian(buffer.AsSpan()[(i * sizeof(uint))..]);
            }
            return data;
        }
        public void WriteU32A(uint[] data)
        {
            byte[] buffer = new byte[data.Length * sizeof(uint)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteUInt32BigEndian(buffer.AsSpan()[(i * sizeof(uint))..], data[i]);
            }
            Stream.Write(buffer);
        }
        public uint[] ReadU32A(SizePrefix prefix)
        {
            int size = prefix.ReadSync(Normal);
            byte[] buffer = ReadU8A(size * sizeof(uint));
            uint[] data = new uint[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadUInt32BigEndian(buffer.AsSpan()[(i * sizeof(uint))..]);
            }
            return data;
        }
        public void WriteU32A(uint[] data, SizePrefix prefix)
        {
            prefix.WriteSync(Normal, data.Length);
            byte[] buffer = new byte[data.Length * sizeof(uint)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteUInt32BigEndian(buffer.AsSpan()[(i * sizeof(uint))..], data[i]);
            }
            Stream.Write(buffer);
        }
        public uint ReadU32()
        {
            byte[] buffer = ReadU8A(sizeof(uint));
            return BinaryPrimitives.ReadUInt32BigEndian(buffer);
        }
        public void WriteU32(uint data)
        {
            byte[] buffer = new byte[sizeof(uint)];
            BinaryPrimitives.WriteUInt32BigEndian(buffer, data);
            Stream.Write(buffer);
        }
        #endregion
        #region S32
        public async Task<int[]> ReadS32AAsync(int size)
        {
            byte[] buffer = await ReadU8AAsync(size * sizeof(int));
            int[] data = new int[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadInt32BigEndian(buffer.AsSpan()[(i * sizeof(int))..]);
            }
            return data;
        }
        public async Task WriteS32AAsync(int[] data)
        {
            byte[] buffer = new byte[data.Length * sizeof(int)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteInt32BigEndian(buffer.AsSpan()[(i * sizeof(int))..], data[i]);
            }
            await Stream.WriteAsync(buffer);
        }
        public async Task<int[]> ReadS32AAsync(SizePrefix prefix)
        {
            int size = await prefix.ReadAsync(Normal);
            byte[] buffer = await ReadU8AAsync(size * sizeof(int));
            int[] data = new int[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadInt32BigEndian(buffer.AsSpan()[(i * sizeof(int))..]);
            }
            return data;
        }
        public async Task WriteS32AAsync(int[] data, SizePrefix prefix)
        {
            await prefix.WriteAsync(Normal, data.Length);
            byte[] buffer = new byte[data.Length * sizeof(int)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteInt32BigEndian(buffer.AsSpan()[(i * sizeof(int))..], data[i]);
            }
            await Stream.WriteAsync(buffer);
        }
        public async Task<int> ReadS32Async()
        {
            byte[] buffer = await ReadU8AAsync(sizeof(int));
            return BinaryPrimitives.ReadInt32BigEndian(buffer);
        }
        public async Task WriteS32Async(int data)
        {
            byte[] buffer = new byte[sizeof(int)];
            BinaryPrimitives.WriteInt32BigEndian(buffer, data);
            await Stream.WriteAsync(buffer);
        }
        public int[] ReadS32A(int size)
        {
            byte[] buffer = ReadU8A(size * sizeof(int));
            int[] data = new int[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadInt32BigEndian(buffer.AsSpan()[(i * sizeof(int))..]);
            }
            return data;
        }
        public void WriteS32A(int[] data)
        {
            byte[] buffer = new byte[data.Length * sizeof(int)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteInt32BigEndian(buffer.AsSpan()[(i * sizeof(int))..], data[i]);
            }
            Stream.Write(buffer);
        }
        public int[] ReadS32A(SizePrefix prefix)
        {
            int size = prefix.ReadSync(Normal);
            byte[] buffer = ReadU8A(size * sizeof(int));
            int[] data = new int[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadInt32BigEndian(buffer.AsSpan()[(i * sizeof(int))..]);
            }
            return data;
        }
        public void WriteS32A(int[] data, SizePrefix prefix)
        {
            prefix.WriteSync(Normal, data.Length);
            byte[] buffer = new byte[data.Length * sizeof(int)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteInt32BigEndian(buffer.AsSpan()[(i * sizeof(int))..], data[i]);
            }
            Stream.Write(buffer);
        }
        public int ReadS32()
        {
            byte[] buffer = ReadU8A(sizeof(int));
            return BinaryPrimitives.ReadInt32BigEndian(buffer);
        }
        public void WriteS32(int data)
        {
            byte[] buffer = new byte[sizeof(int)];
            BinaryPrimitives.WriteInt32BigEndian(buffer, data);
            Stream.Write(buffer);
        }
        #endregion
        #region U64
        public async Task<ulong[]> ReadU64AAsync(int size)
        {
            byte[] buffer = await ReadU8AAsync(size * sizeof(ulong));
            ulong[] data = new ulong[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadUInt64BigEndian(buffer.AsSpan()[(i * sizeof(ulong))..]);
            }
            return data;
        }
        public async Task WriteU64AAsync(ulong[] data)
        {
            byte[] buffer = new byte[data.Length * sizeof(ulong)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteUInt64BigEndian(buffer.AsSpan()[(i * sizeof(ulong))..], data[i]);
            }
            await Stream.WriteAsync(buffer);
        }
        public async Task<ulong[]> ReadU64AAsync(SizePrefix prefix)
        {
            int size = await prefix.ReadAsync(Normal);
            byte[] buffer = await ReadU8AAsync(size * sizeof(ulong));
            ulong[] data = new ulong[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadUInt64BigEndian(buffer.AsSpan()[(i * sizeof(ulong))..]);
            }
            return data;
        }
        public async Task WriteU64AAsync(ulong[] data, SizePrefix prefix)
        {
            await prefix.WriteAsync(Normal, data.Length);
            byte[] buffer = new byte[data.Length * sizeof(ulong)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteUInt64BigEndian(buffer.AsSpan()[(i * sizeof(ulong))..], data[i]);
            }
            await Stream.WriteAsync(buffer);
        }
        public async Task<ulong> ReadU64Async()
        {
            byte[] buffer = await ReadU8AAsync(sizeof(ulong));
            return BinaryPrimitives.ReadUInt64BigEndian(buffer);
        }
        public async Task WriteU64Async(ulong data)
        {
            byte[] buffer = new byte[sizeof(ulong)];
            BinaryPrimitives.WriteUInt64BigEndian(buffer, data);
            await Stream.WriteAsync(buffer);
        }
        public ulong[] ReadU64A(int size)
        {
            byte[] buffer = ReadU8A(size * sizeof(ulong));
            ulong[] data = new ulong[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadUInt64BigEndian(buffer.AsSpan()[(i * sizeof(ulong))..]);
            }
            return data;
        }
        public void WriteU64A(ulong[] data)
        {
            byte[] buffer = new byte[data.Length * sizeof(ulong)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteUInt64BigEndian(buffer.AsSpan()[(i * sizeof(ulong))..], data[i]);
            }
            Stream.Write(buffer);
        }
        public ulong[] ReadU64A(SizePrefix prefix)
        {
            int size = prefix.ReadSync(Normal);
            byte[] buffer = ReadU8A(size * sizeof(ulong));
            ulong[] data = new ulong[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadUInt64BigEndian(buffer.AsSpan()[(i * sizeof(ulong))..]);
            }
            return data;
        }
        public void WriteU64A(ulong[] data, SizePrefix prefix)
        {
            prefix.WriteSync(Normal, data.Length);
            byte[] buffer = new byte[data.Length * sizeof(ulong)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteUInt64BigEndian(buffer.AsSpan()[(i * sizeof(ulong))..], data[i]);
            }
            Stream.Write(buffer);
        }
        public ulong ReadU64()
        {
            byte[] buffer = ReadU8A(sizeof(ulong));
            return BinaryPrimitives.ReadUInt64BigEndian(buffer);
        }
        public void WriteU64(ulong data)
        {
            byte[] buffer = new byte[sizeof(ulong)];
            BinaryPrimitives.WriteUInt64BigEndian(buffer, data);
            Stream.Write(buffer);
        }
        #endregion
        #region S64
        public async Task<long[]> ReadS64AAsync(int size)
        {
            byte[] buffer = await ReadU8AAsync(size * sizeof(long));
            long[] data = new long[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadInt64BigEndian(buffer.AsSpan()[(i * sizeof(long))..]);
            }
            return data;
        }
        public async Task WriteS64AAsync(long[] data)
        {
            byte[] buffer = new byte[data.Length * sizeof(long)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteInt64BigEndian(buffer.AsSpan()[(i * sizeof(long))..], data[i]);
            }
            await Stream.WriteAsync(buffer);
        }
        public async Task<long[]> ReadS64AAsync(SizePrefix prefix)
        {
            int size = await prefix.ReadAsync(Normal);
            byte[] buffer = await ReadU8AAsync(size * sizeof(long));
            long[] data = new long[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadInt64BigEndian(buffer.AsSpan()[(i * sizeof(long))..]);
            }
            return data;
        }
        public async Task WriteS64AAsync(long[] data, SizePrefix prefix)
        {
            await prefix.WriteAsync(Normal, data.Length);
            byte[] buffer = new byte[data.Length * sizeof(long)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteInt64BigEndian(buffer.AsSpan()[(i * sizeof(long))..], data[i]);
            }
            await Stream.WriteAsync(buffer);
        }
        public async Task<long> ReadS64Async()
        {
            byte[] buffer = await ReadU8AAsync(sizeof(long));
            return BinaryPrimitives.ReadInt64BigEndian(buffer);
        }
        public async Task WriteS64Async(long data)
        {
            byte[] buffer = new byte[sizeof(long)];
            BinaryPrimitives.WriteInt64BigEndian(buffer, data);
            await Stream.WriteAsync(buffer);
        }
        public long[] ReadS64A(int size)
        {
            byte[] buffer = ReadU8A(size * sizeof(long));
            long[] data = new long[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadInt64BigEndian(buffer.AsSpan()[(i * sizeof(long))..]);
            }
            return data;
        }
        public void WriteS64A(long[] data)
        {
            byte[] buffer = new byte[data.Length * sizeof(long)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteInt64BigEndian(buffer.AsSpan()[(i * sizeof(long))..], data[i]);
            }
            Stream.Write(buffer);
        }
        public long[] ReadS64A(SizePrefix prefix)
        {
            int size = prefix.ReadSync(Normal);
            byte[] buffer = ReadU8A(size * sizeof(long));
            long[] data = new long[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadInt64BigEndian(buffer.AsSpan()[(i * sizeof(long))..]);
            }
            return data;
        }
        public void WriteS64A(long[] data, SizePrefix prefix)
        {
            prefix.WriteSync(Normal, data.Length);
            byte[] buffer = new byte[data.Length * sizeof(long)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteInt64BigEndian(buffer.AsSpan()[(i * sizeof(long))..], data[i]);
            }
            Stream.Write(buffer);
        }
        public long ReadS64()
        {
            byte[] buffer = ReadU8A(sizeof(long));
            return BinaryPrimitives.ReadInt64BigEndian(buffer);
        }
        public void WriteS64(long data)
        {
            byte[] buffer = new byte[sizeof(long)];
            BinaryPrimitives.WriteInt64BigEndian(buffer, data);
            Stream.Write(buffer);
        }
        #endregion
        #region F32
        public async Task<float[]> ReadF32AAsync(int size)
        {
            byte[] buffer = await ReadU8AAsync(size * sizeof(float));
            float[] data = new float[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadSingleBigEndian(buffer.AsSpan()[(i * sizeof(float))..]);
            }
            return data;
        }
        public async Task WriteF32AAsync(float[] data)
        {
            byte[] buffer = new byte[data.Length * sizeof(float)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteSingleBigEndian(buffer.AsSpan()[(i * sizeof(float))..], data[i]);
            }
            await Stream.WriteAsync(buffer);
        }
        public async Task<float[]> ReadF32AAsync(SizePrefix prefix)
        {
            int size = await prefix.ReadAsync(Normal);
            byte[] buffer = await ReadU8AAsync(size * sizeof(float));
            float[] data = new float[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadSingleBigEndian(buffer.AsSpan()[(i * sizeof(float))..]);
            }
            return data;
        }
        public async Task WriteF32AAsync(float[] data, SizePrefix prefix)
        {
            await prefix.WriteAsync(Normal, data.Length);
            byte[] buffer = new byte[data.Length * sizeof(float)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteSingleBigEndian(buffer.AsSpan()[(i * sizeof(float))..], data[i]);
            }
            await Stream.WriteAsync(buffer);
        }
        public async Task<float> ReadF32Async()
        {
            byte[] buffer = await ReadU8AAsync(sizeof(float));
            return BinaryPrimitives.ReadSingleBigEndian(buffer);
        }
        public async Task WriteF32Async(float data)
        {
            byte[] buffer = new byte[sizeof(float)];
            BinaryPrimitives.WriteSingleBigEndian(buffer, data);
            await Stream.WriteAsync(buffer);
        }
        public float[] ReadF32A(int size)
        {
            byte[] buffer = ReadU8A(size * sizeof(float));
            float[] data = new float[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadSingleBigEndian(buffer.AsSpan()[(i * sizeof(float))..]);
            }
            return data;
        }
        public void WriteF32A(float[] data)
        {
            byte[] buffer = new byte[data.Length * sizeof(float)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteSingleBigEndian(buffer.AsSpan()[(i * sizeof(float))..], data[i]);
            }
            Stream.Write(buffer);
        }
        public float[] ReadF32A(SizePrefix prefix)
        {
            int size = prefix.ReadSync(Normal);
            byte[] buffer = ReadU8A(size * sizeof(float));
            float[] data = new float[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadSingleBigEndian(buffer.AsSpan()[(i * sizeof(float))..]);
            }
            return data;
        }
        public void WriteF32A(float[] data, SizePrefix prefix)
        {
            prefix.WriteSync(Normal, data.Length);
            byte[] buffer = new byte[data.Length * sizeof(float)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteSingleBigEndian(buffer.AsSpan()[(i * sizeof(float))..], data[i]);
            }
            Stream.Write(buffer);
        }
        public float ReadF32()
        {
            byte[] buffer = ReadU8A(sizeof(float));
            return BinaryPrimitives.ReadSingleBigEndian(buffer);
        }
        public void WriteF32(float data)
        {
            byte[] buffer = new byte[sizeof(float)];
            BinaryPrimitives.WriteSingleBigEndian(buffer, data);
            Stream.Write(buffer);
        }
        #endregion
        #region F64
        public async Task<double[]> ReadF64AAsync(int size)
        {
            byte[] buffer = await ReadU8AAsync(size * sizeof(double));
            double[] data = new double[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadDoubleBigEndian(buffer.AsSpan()[(i * sizeof(double))..]);
            }
            return data;
        }
        public async Task WriteF64AAsync(double[] data)
        {
            byte[] buffer = new byte[data.Length * sizeof(double)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteDoubleBigEndian(buffer.AsSpan()[(i * sizeof(double))..], data[i]);
            }
            await Stream.WriteAsync(buffer);
        }
        public async Task<double[]> ReadF64AAsync(SizePrefix prefix)
        {
            int size = await prefix.ReadAsync(Normal);
            byte[] buffer = await ReadU8AAsync(size * sizeof(double));
            double[] data = new double[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadDoubleBigEndian(buffer.AsSpan()[(i * sizeof(double))..]);
            }
            return data;
        }
        public async Task WriteF64AAsync(double[] data, SizePrefix prefix)
        {
            await prefix.WriteAsync(Normal, data.Length);
            byte[] buffer = new byte[data.Length * sizeof(double)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteDoubleBigEndian(buffer.AsSpan()[(i * sizeof(double))..], data[i]);
            }
            await Stream.WriteAsync(buffer);
        }
        public async Task<double> ReadF64Async()
        {
            byte[] buffer = await ReadU8AAsync(sizeof(double));
            return BinaryPrimitives.ReadDoubleBigEndian(buffer);
        }
        public async Task WriteF64Async(double data)
        {
            byte[] buffer = new byte[sizeof(double)];
            BinaryPrimitives.WriteDoubleBigEndian(buffer, data);
            await Stream.WriteAsync(buffer);
        }
        public double[] ReadF64A(int size)
        {
            byte[] buffer = ReadU8A(size * sizeof(double));
            double[] data = new double[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadDoubleBigEndian(buffer.AsSpan()[(i * sizeof(double))..]);
            }
            return data;
        }
        public void WriteF64A(double[] data)
        {
            byte[] buffer = new byte[data.Length * sizeof(double)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteDoubleBigEndian(buffer.AsSpan()[(i * sizeof(double))..], data[i]);
            }
            Stream.Write(buffer);
        }
        public double[] ReadF64A(SizePrefix prefix)
        {
            int size = prefix.ReadSync(Normal);
            byte[] buffer = ReadU8A(size * sizeof(double));
            double[] data = new double[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadDoubleBigEndian(buffer.AsSpan()[(i * sizeof(double))..]);
            }
            return data;
        }
        public void WriteF64A(double[] data, SizePrefix prefix)
        {
            prefix.WriteSync(Normal, data.Length);
            byte[] buffer = new byte[data.Length * sizeof(double)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteDoubleBigEndian(buffer.AsSpan()[(i * sizeof(double))..], data[i]);
            }
            Stream.Write(buffer);
        }
        public double ReadF64()
        {
            byte[] buffer = ReadU8A(sizeof(double));
            return BinaryPrimitives.ReadDoubleBigEndian(buffer);
        }
        public void WriteF64(double data)
        {
            byte[] buffer = new byte[sizeof(double)];
            BinaryPrimitives.WriteDoubleBigEndian(buffer, data);
            Stream.Write(buffer);
        }
        #endregion

        #region U8V
        public async Task<byte> ReadU8VAsync()
        {
            byte data = 0;
            int position = 0;
            while (true)
            {
                byte current = await ReadU8Async();
                data |= (byte)((current & 127U) << position);
                if ((current & 128) == 0) return data;
                position += 7;
                if (position >= sizeof(ushort) * 8) throw new ProtocolViolationException();
            }
        }
        public async Task<int> WriteU8VAsync(byte data)
        {
            int size = 0;
            do
            {
                byte current = data;
                data >>= 7;
                if (data != 0) current |= 128;
                else current &= 127;
                await WriteU8Async(current);
                size++;
            }
            while (data != 0);
            return size;
        }
        public byte ReadU8V()
        {
            byte data = 0;
            int position = 0;
            while (true)
            {
                byte current = ReadU8();
                data |= (byte)((current & 127U) << position);
                if ((current & 128) == 0) return data;
                position += 7;
                if (position >= sizeof(ushort) * 8) throw new ProtocolViolationException();
            }
        }
        public int WriteU8V(byte data)
        {
            int size = 0;
            do
            {
                byte current = data;
                data >>= 7;
                if (data != 0) current |= 128;
                else current &= 127;
                WriteU8(current);
                size++;
            }
            while (data != 0);
            return size;
        }
        #endregion
        #region S8V
        public async Task<sbyte> ReadS8VAsync()
        {
            return (sbyte)await ReadU8VAsync();
        }
        public async Task<int> WriteS8VAsync(sbyte data)
        {
            int size = 0;
            do
            {
                byte current = (byte)data;
                data >>= 7;
                if (data != 0) current |= 128;
                else current &= 127;
                await WriteU8Async(current);
                size++;
            }
            while (data != 0);
            return size;
        }
        public sbyte ReadS8V()
        {
            return (sbyte)ReadU8V();
        }
        public int WriteS8V(sbyte data)
        {
            int size = 0;
            do
            {
                byte current = (byte)data;
                data >>= 7;
                if (data != 0) current |= 128;
                else current &= 127;
                WriteU8(current);
                size++;
            }
            while (data != 0);
            return size;
        }
        #endregion
        #region U16V
        public async Task<ushort> ReadU16VAsync()
        {
            ushort data = 0;
            int position = 0;
            while (true)
            {
                byte current = await ReadU8Async();
                data |= (ushort)((current & 127U) << position);
                if ((current & 128) == 0) return data;
                position += 7;
                if (position >= sizeof(ushort) * 8) throw new ProtocolViolationException();
            }
        }
        public async Task<int> WriteU16VAsync(ushort data)
        {
            int size = 0;
            do
            {
                byte current = (byte)data;
                data >>= 7;
                if (data != 0) current |= 128;
                else current &= 127;
                await WriteU8Async(current);
                size++;
            }
            while (data != 0);
            return size;
        }
        public ushort ReadU16V()
        {
            ushort data = 0;
            int position = 0;
            while (true)
            {
                byte current = ReadU8();
                data |= (ushort)((current & 127U) << position);
                if ((current & 128) == 0) return data;
                position += 7;
                if (position >= sizeof(ushort) * 8) throw new ProtocolViolationException();
            }
        }
        public int WriteU16V(ushort data)
        {
            int size = 0;
            do
            {
                byte current = (byte)data;
                data >>= 7;
                if (data != 0) current |= 128;
                else current &= 127;
                WriteU8(current);
                size++;
            }
            while (data != 0);
            return size;
        }
        #endregion
        #region S16V
        public async Task<short> ReadS16VAsync()
        {
            return (short)await ReadU16VAsync();
        }
        public async Task<int> WriteS16VAsync(short data)
        {
            int size = 0;
            do
            {
                byte current = (byte)data;
                data >>= 7;
                if (data != 0) current |= 128;
                else current &= 127;
                await WriteU8Async(current);
                size++;
            }
            while (data != 0);
            return size;
        }
        public short ReadS16V()
        {
            return (short)ReadU16V();
        }
        public int WriteS16V(short data)
        {
            int size = 0;
            do
            {
                byte current = (byte)data;
                data >>= 7;
                if (data != 0) current |= 128;
                else current &= 127;
                WriteU8(current);
                size++;
            }
            while (data != 0);
            return size;
        }
        #endregion
        #region U32V
        public async Task<uint> ReadU32VAsync()
        {
            uint data = 0;
            int position = 0;
            while (true)
            {
                byte current = await ReadU8Async();
                data |= (current & 127U) << position;
                if ((current & 128) == 0) return data;
                position += 7;
                if (position >= sizeof(uint) * 8) throw new ProtocolViolationException();
            }
        }
        public async Task<int> WriteU32VAsync(uint data)
        {
            int size = 0;
            do
            {
                byte current = (byte)data;
                data >>= 7;
                if (data != 0) current |= 128;
                else current &= 127;
                await WriteU8Async(current);
                size++;
            }
            while (data != 0);
            return size;
        }
        public uint ReadU32V()
        {
            uint data = 0;
            int position = 0;
            while (true)
            {
                byte current = ReadU8();
                data |= (current & 127U) << position;
                if ((current & 128) == 0) return data;
                position += 7;
                if (position >= sizeof(uint) * 8) throw new ProtocolViolationException();
            }
        }
        public int WriteU32V(uint data)
        {
            int size = 0;
            do
            {
                byte current = (byte)data;
                data >>= 7;
                if (data != 0) current |= 128;
                else current &= 127;
                WriteU8(current);
                size++;
            }
            while (data != 0);
            return size;
        }
        #endregion
        #region S32
        public async Task<int> ReadS32VAsync()
        {
            return (int)await ReadU32VAsync();
        }
        public async Task<int> WriteS32VAsync(int data)
        {
            int size = 0;
            do
            {
                byte current = (byte)data;
                data >>= 7;
                if (data != 0) current |= 128;
                else current &= 127;
                await WriteU8Async(current);
                size++;
            }
            while (data != 0);
            return size;
        }
        public int ReadS32V()
        {
            return (int)ReadU32V();
        }
        public int WriteS32V(int data)
        {
            int size = 0;
            do
            {
                byte current = (byte)data;
                data >>= 7;
                if (data != 0) current |= 128;
                else current &= 127;
                WriteU8(current);
                size++;
            }
            while (data != 0);
            return size;
        }
        #endregion
        #region U64V
        public async Task<ulong> ReadU64VAsync()
        {
            ulong data = 0;
            int position = 0;
            while (true)
            {
                byte current = await ReadU8Async();
                data |= (current & 127U) << position;
                if ((current & 128) == 0) return data;
                position += 7;
                if (position >= 64) throw new ProtocolViolationException();
            }
        }
        public async Task<int> WriteU64VAsync(ulong data)
        {
            int size = 0;
            do
            {
                byte current = (byte)data;
                data >>= 7;
                if (data != 0) current |= 128;
                else current &= 127;
                await WriteU8Async(current);
                size++;
            }
            while (data != 0);
            return size;
        }
        public ulong ReadU64V()
        {
            ulong data = 0;
            int position = 0;
            while (true)
            {
                byte current = ReadU8();
                data |= (current & 127U) << position;
                if ((current & 128) == 0) return data;
                position += 7;
                if (position >= 64) throw new ProtocolViolationException();
            }
        }
        public int WriteU64V(ulong data)
        {
            int size = 0;
            do
            {
                byte current = (byte)data;
                data >>= 7;
                if (data != 0) current |= 128;
                else current &= 127;
                WriteU8(current);
                size++;
            }
            while (data != 0);
            return size;
        }
        #endregion
        #region S64
        public async Task<long> ReadS64VAsync()
        {
            return (long)await ReadU64VAsync();
        }
        public async Task<int> WriteS64VAsync(long data)
        {
            int size = 0;
            do
            {
                byte current = (byte)data;
                data >>= 7;
                if (data != 0) current |= 128;
                else current &= 127;
                await WriteU8Async(current);
                size++;
            }
            while (data != 0);
            return size;
        }
        public long ReadS64V()
        {
            return (long)ReadU64V();
        }
        public int WriteS64V(long data)
        {
            int size = 0;
            do
            {
                byte current = (byte)data;
                data >>= 7;
                if (data != 0) current |= 128;
                else current &= 127;
                WriteU8(current);
                size++;
            }
            while (data != 0);
            return size;
        }
        #endregion

        #region Bool
        public async Task<bool> ReadBoolAsync()
        {
            return (await ReadU8Async()) != 0;
        }
        public async Task WriteBoolAsync(bool data)
        {
            await Stream.WriteAsync(MemoryMarshal.AsBytes<bool>(new bool[] { data }).ToArray());
        }
        public bool ReadBool()
        {
            return (ReadU8()) != 0;
        }
        public void WriteBool(bool data)
        {
            Stream.Write(MemoryMarshal.AsBytes<bool>(new bool[] { data }).ToArray());
        }
        #endregion
        #region Guid
        public async Task<Guid> ReadGuidAsync()
        {
            byte[] buffer = await ReadU8AAsync(16);
            if (BitConverter.IsLittleEndian) buffer = [buffer[3], buffer[2], buffer[1], buffer[0], buffer[5], buffer[4], buffer[7], buffer[6], buffer[8], buffer[9], buffer[10], buffer[11], buffer[12], buffer[13], buffer[14], buffer[15]];
            return MemoryMarshal.Cast<byte, Guid>(buffer)[0];
        }
        public async Task WriteGuidAsync(Guid data)
        {
            byte[] buffer = MemoryMarshal.AsBytes([data]).ToArray();
            if (!BitConverter.IsLittleEndian)
            {
                await Stream.WriteAsync(buffer);
                return;
            }
            await Stream.WriteAsync(new byte[] { buffer[3], buffer[2], buffer[1], buffer[0], buffer[5], buffer[4], buffer[7], buffer[6], buffer[8], buffer[9], buffer[10], buffer[11], buffer[12], buffer[13], buffer[14], buffer[15] });
        }
        public Guid ReadGuid()
        {
            byte[] buffer = ReadU8A(16);
            if (BitConverter.IsLittleEndian) buffer = [buffer[3], buffer[2], buffer[1], buffer[0], buffer[5], buffer[4], buffer[7], buffer[6], buffer[8], buffer[9], buffer[10], buffer[11], buffer[12], buffer[13], buffer[14], buffer[15]];
            return MemoryMarshal.Cast<byte, Guid>(buffer)[0];
        }
        public void WriteGuid(Guid data)
        {
            byte[] buffer = MemoryMarshal.AsBytes([data]).ToArray();
            if (!BitConverter.IsLittleEndian)
            {
                Stream.Write(buffer);
                return;
            }
            Stream.Write([buffer[3], buffer[2], buffer[1], buffer[0], buffer[5], buffer[4], buffer[7], buffer[6], buffer[8], buffer[9], buffer[10], buffer[11], buffer[12], buffer[13], buffer[14], buffer[15]]);
        }
        #endregion

        #region String
        public async Task<string> ReadStringAsync(SizePrefix prefix, Encoding? encoding = null)
        {
            return (encoding ?? Encoding.UTF8).GetString(await ReadU8AAsync(await prefix.ReadAsync(Normal)));
        }
        public async Task WriteStringAsync(string data, SizePrefix prefix, Encoding? encoding = null)
        {
            byte[] buffer = (encoding ?? Encoding.UTF8).GetBytes(data);
            await prefix.WriteAsync(Normal, buffer.Length);
            await WriteU8AAsync(buffer);
        }
        public string ReadString(SizePrefix prefix, Encoding? encoding = null)
        {
            return (encoding ?? Encoding.UTF8).GetString(ReadU8A(prefix.ReadSync(Normal)));
        }
        public void WriteString(string data, SizePrefix prefix, Encoding? encoding = null)
        {
            byte[] buffer = (encoding ?? Encoding.UTF8).GetBytes(data);
            prefix.WriteSync(Normal, buffer.Length);
            WriteU8A(buffer);
        }
        #endregion
    }
}
