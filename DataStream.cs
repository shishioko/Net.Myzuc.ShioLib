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
    public static class DataStreamExtension
    {
        #region U8
        public static async Task<byte[]> ReadU8AAsync(this Stream stream, int size)
        {
            byte[] data = new byte[size];
            int position = 0;
            while (position < size)
            {
                int read = await stream.ReadAsync(data.AsMemory(position, size - position));
                position += read;
                if (read == 0) throw new EndOfStreamException();
            }
            return data;
        }
        public static async Task WriteU8AAsync(this Stream stream, byte[] data)
        {
            await stream.WriteAsync(data);
        }
        public static async Task<byte[]> ReadU8AAsync(this Stream stream, SizePrefix prefix, int limit = int.MaxValue)
        {
            return await stream.ReadU8AAsync(await prefix.ReadAsync(stream, limit));
        }
        public static async Task WriteU8AAsync(this Stream stream, byte[] data, SizePrefix prefix, int limit = int.MaxValue)
        {
            await prefix.WriteAsync(stream, limit, data.Length);
            await stream.WriteU8AAsync(data);
        }
        public static async Task<byte> ReadU8Async(this Stream stream)
        {
            return (await stream.ReadU8AAsync(1))[0];
        }
        public static async Task WriteU8Async(this Stream stream, byte data)
        {
            await stream.WriteAsync(new byte[] { data });
        }
        public static byte[] ReadU8A(this Stream stream, int size)
        {
            byte[] data = new byte[size];
            int position = 0;
            while (position < size)
            {
                int read = stream.Read(data, position, size - position);
                position += read;
                if (read == 0) throw new EndOfStreamException();
            }
            return data;
        }
        public static void WriteU8A(this Stream stream, byte[] data)
        {
            stream.Write(data);
        }
        public static byte[] ReadU8A(this Stream stream, SizePrefix prefix, int limit = int.MaxValue)
        {
            return stream.ReadU8A(prefix.ReadSync(stream, limit));
        }
        public static void WriteU8A(this Stream stream, byte[] data, SizePrefix prefix, int limit = int.MaxValue)
        {
            prefix.WriteSync(stream, limit, data.Length);
            stream.WriteU8A(data);
        }
        public static byte ReadU8(this Stream stream)
        {
            return (stream.ReadU8A(1))[0];
        }
        public static void WriteU8(this Stream stream, byte data)
        {
            stream.Write([data]);
        }
        #endregion
        #region S8
        public static async Task<sbyte[]> ReadS8AAsync(this Stream stream, int size)
        {
            return MemoryMarshal.Cast<byte, sbyte>(await stream.ReadU8AAsync(size)).ToArray();
        }
        public static async Task WriteS8AAsync(this Stream stream, sbyte[] data)
        {
            await stream.WriteAsync(MemoryMarshal.AsBytes(data.AsSpan()).ToArray());
        }
        public static async Task<sbyte[]> ReadS8AAsync(this Stream stream, SizePrefix prefix, int limit = int.MaxValue)
        {
            return MemoryMarshal.Cast<byte, sbyte>(await stream.ReadU8AAsync(await prefix.ReadAsync(stream, limit))).ToArray();
        }
        public static async Task WriteS8AAsync(this Stream stream, sbyte[] data, SizePrefix prefix, int limit = int.MaxValue)
        {
            await prefix.WriteAsync(stream, limit, data.Length);
            await stream.WriteAsync(MemoryMarshal.AsBytes(data.AsSpan()).ToArray());
        }
        public static async Task<sbyte> ReadS8Async(this Stream stream)
        {
            return (await stream.ReadS8AAsync(1))[0];
        }
        public static async Task WriteS8Async(this Stream stream, sbyte data)
        {
            await stream.WriteAsync(MemoryMarshal.AsBytes<sbyte>(new sbyte[] { data }).ToArray());
        }
        public static sbyte[] ReadS8A(this Stream stream, int size)
        {
            return MemoryMarshal.Cast<byte, sbyte>(stream.ReadU8A(size)).ToArray();
        }
        public static void WriteS8A(this Stream stream, sbyte[] data)
        {
            stream.Write(MemoryMarshal.AsBytes(data.AsSpan()).ToArray());
        }
        public static sbyte[] ReadS8A(this Stream stream, SizePrefix prefix, int limit = int.MaxValue)
        {
            return MemoryMarshal.Cast<byte, sbyte>(stream.ReadU8A(prefix.ReadSync(stream, limit))).ToArray();
        }
        public static void WriteS8A(this Stream stream, sbyte[] data, SizePrefix prefix, int limit = int.MaxValue)
        {
            prefix.WriteSync(stream, limit, data.Length);
            stream.Write(MemoryMarshal.AsBytes(data.AsSpan()).ToArray());
        }
        public static sbyte ReadS8(this Stream stream)
        {
            return (stream.ReadS8A(1))[0];
        }
        public static void WriteS8(this Stream stream, sbyte data)
        {
            stream.Write(MemoryMarshal.AsBytes<sbyte>(new sbyte[] { data }).ToArray());
        }
        #endregion
        #region U16
        public static async Task<ushort[]> ReadU16AAsync(this Stream stream, int size)
        {
            byte[] buffer = await stream.ReadU8AAsync(size * sizeof(ushort));
            ushort[] data = new ushort[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadUInt16BigEndian(buffer.AsSpan()[(i * sizeof(ushort))..]);
            }
            return data;
        }
        public static async Task WriteU16AAsync(this Stream stream, ushort[] data)
        {
            byte[] buffer = new byte[data.Length * sizeof(ushort)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteUInt16BigEndian(buffer.AsSpan()[(i * sizeof(ushort))..], data[i]);
            }
            await stream.WriteAsync(buffer);
        }
        public static async Task<ushort[]> ReadU16AAsync(this Stream stream, SizePrefix prefix, int limit = int.MaxValue)
        {
            int size = await prefix.ReadAsync(stream, limit);
            byte[] buffer = await stream.ReadU8AAsync(size * sizeof(ushort));
            ushort[] data = new ushort[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadUInt16BigEndian(buffer.AsSpan()[(i * sizeof(ushort))..]);
            }
            return data;
        }
        public static async Task WriteU16AAsync(this Stream stream, ushort[] data, SizePrefix prefix, int limit = int.MaxValue)
        {
            await prefix.WriteAsync(stream, limit, data.Length);
            byte[] buffer = new byte[data.Length * sizeof(ushort)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteUInt16BigEndian(buffer.AsSpan()[(i * sizeof(ushort))..], data[i]);
            }
            await stream.WriteAsync(buffer);
        }
        public static async Task<ushort> ReadU16Async(this Stream stream)
        {
            byte[] buffer = await stream.ReadU8AAsync(sizeof(ushort));
            return BinaryPrimitives.ReadUInt16BigEndian(buffer);
        }
        public static async Task WriteU16Async(this Stream stream, ushort data)
        {
            byte[] buffer = new byte[sizeof(ushort)];
            BinaryPrimitives.WriteUInt16BigEndian(buffer, data);
            await stream.WriteAsync(buffer);
        }
        public static ushort[] ReadU16A(this Stream stream, int size)
        {
            byte[] buffer = stream.ReadU8A(size * sizeof(ushort));
            ushort[] data = new ushort[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadUInt16BigEndian(buffer.AsSpan()[(i * sizeof(ushort))..]);
            }
            return data;
        }
        public static void WriteU16A(this Stream stream, ushort[] data)
        {
            byte[] buffer = new byte[data.Length * sizeof(ushort)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteUInt16BigEndian(buffer.AsSpan()[(i * sizeof(ushort))..], data[i]);
            }
            stream.Write(buffer);
        }
        public static ushort[] ReadU16A(this Stream stream, SizePrefix prefix, int limit = int.MaxValue)
        {
            int size = prefix.ReadSync(stream, limit);
            byte[] buffer = stream.ReadU8A(size * sizeof(ushort));
            ushort[] data = new ushort[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadUInt16BigEndian(buffer.AsSpan()[(i * sizeof(ushort))..]);
            }
            return data;
        }
        public static void WriteU16A(this Stream stream, ushort[] data, SizePrefix prefix, int limit = int.MaxValue)
        {
            prefix.WriteSync(stream, limit, data.Length);
            byte[] buffer = new byte[data.Length * sizeof(ushort)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteUInt16BigEndian(buffer.AsSpan()[(i * sizeof(ushort))..], data[i]);
            }
            stream.Write(buffer);
        }
        public static ushort ReadU16(this Stream stream)
        {
            byte[] buffer = stream.ReadU8A(sizeof(ushort));
            return BinaryPrimitives.ReadUInt16BigEndian(buffer);
        }
        public static void WriteU16(this Stream stream, ushort data)
        {
            byte[] buffer = new byte[sizeof(ushort)];
            BinaryPrimitives.WriteUInt16BigEndian(buffer, data);
            stream.Write(buffer);
        }
        #endregion
        #region S16
        public static async Task<short[]> ReadS16AAsync(this Stream stream, int size)
        {
            byte[] buffer = await stream.ReadU8AAsync(size * sizeof(short));
            short[] data = new short[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadInt16BigEndian(buffer.AsSpan()[(i * sizeof(short))..]);
            }
            return data;
        }
        public static async Task WriteS16AAsync(this Stream stream, short[] data)
        {
            byte[] buffer = new byte[data.Length * sizeof(short)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteInt16BigEndian(buffer.AsSpan()[(i * sizeof(short))..], data[i]);
            }
            await stream.WriteAsync(buffer);
        }
        public static async Task<short[]> ReadS16AAsync(this Stream stream, SizePrefix prefix, int limit = int.MaxValue)
        {
            int size = await prefix.ReadAsync(stream, limit);
            byte[] buffer = await stream.ReadU8AAsync(size * sizeof(short));
            short[] data = new short[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadInt16BigEndian(buffer.AsSpan()[(i * sizeof(short))..]);
            }
            return data;
        }
        public static async Task WriteS16AAsync(this Stream stream, short[] data, SizePrefix prefix, int limit = int.MaxValue)
        {
            await prefix.WriteAsync(stream, limit, data.Length);
            byte[] buffer = new byte[data.Length * sizeof(short)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteInt16BigEndian(buffer.AsSpan()[(i * sizeof(short))..], data[i]);
            }
            await stream.WriteAsync(buffer);
        }
        public static async Task<short> ReadS16Async(this Stream stream)
        {
            byte[] buffer = await stream.ReadU8AAsync(sizeof(short));
            return BinaryPrimitives.ReadInt16BigEndian(buffer);
        }
        public static async Task WriteS16Async(this Stream stream, short data)
        {
            byte[] buffer = new byte[sizeof(short)];
            BinaryPrimitives.WriteInt16BigEndian(buffer, data);
            await stream.WriteAsync(buffer);
        }
        public static short[] ReadS16A(this Stream stream, int size)
        {
            byte[] buffer = stream.ReadU8A(size * sizeof(short));
            short[] data = new short[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadInt16BigEndian(buffer.AsSpan()[(i * sizeof(short))..]);
            }
            return data;
        }
        public static void WriteS16A(this Stream stream, short[] data)
        {
            byte[] buffer = new byte[data.Length * sizeof(short)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteInt16BigEndian(buffer.AsSpan()[(i * sizeof(short))..], data[i]);
            }
            stream.Write(buffer);
        }
        public static short[] ReadS16A(this Stream stream, SizePrefix prefix, int limit = int.MaxValue)
        {
            int size = prefix.ReadSync(stream, limit);
            byte[] buffer = stream.ReadU8A(size * sizeof(short));
            short[] data = new short[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadInt16BigEndian(buffer.AsSpan()[(i * sizeof(short))..]);
            }
            return data;
        }
        public static void WriteS16A(this Stream stream, short[] data, SizePrefix prefix, int limit = int.MaxValue)
        {
            prefix.WriteSync(stream, limit, data.Length);
            byte[] buffer = new byte[data.Length * sizeof(short)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteInt16BigEndian(buffer.AsSpan()[(i * sizeof(short))..], data[i]);
            }
            stream.Write(buffer);
        }
        public static short ReadS16(this Stream stream)
        {
            byte[] buffer = stream.ReadU8A(sizeof(short));
            return BinaryPrimitives.ReadInt16BigEndian(buffer);
        }
        public static void WriteS16(this Stream stream, short data)
        {
            byte[] buffer = new byte[sizeof(short)];
            BinaryPrimitives.WriteInt16BigEndian(buffer, data);
            stream.Write(buffer);
        }
        #endregion
        #region U32
        public static async Task<uint[]> ReadU32AAsync(this Stream stream, int size)
        {
            byte[] buffer = await stream.ReadU8AAsync(size * sizeof(uint));
            uint[] data = new uint[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadUInt32BigEndian(buffer.AsSpan()[(i * sizeof(uint))..]);
            }
            return data;
        }
        public static async Task WriteU32AAsync(this Stream stream, uint[] data)
        {
            byte[] buffer = new byte[data.Length * sizeof(uint)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteUInt32BigEndian(buffer.AsSpan()[(i * sizeof(uint))..], data[i]);
            }
            await stream.WriteAsync(buffer);
        }
        public static async Task<uint[]> ReadU32AAsync(this Stream stream, SizePrefix prefix, int limit = int.MaxValue)
        {
            int size = await prefix.ReadAsync(stream, limit);
            byte[] buffer = await stream.ReadU8AAsync(size * sizeof(uint));
            uint[] data = new uint[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadUInt32BigEndian(buffer.AsSpan()[(i * sizeof(uint))..]);
            }
            return data;
        }
        public static async Task WriteU32AAsync(this Stream stream, uint[] data, SizePrefix prefix, int limit = int.MaxValue)
        {
            await prefix.WriteAsync(stream, limit, data.Length);
            byte[] buffer = new byte[data.Length * sizeof(uint)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteUInt32BigEndian(buffer.AsSpan()[(i * sizeof(uint))..], data[i]);
            }
            await stream.WriteAsync(buffer);
        }
        public static async Task<uint> ReadU32Async(this Stream stream)
        {
            byte[] buffer = await stream.ReadU8AAsync(sizeof(uint));
            return BinaryPrimitives.ReadUInt32BigEndian(buffer);
        }
        public static async Task WriteU32Async(this Stream stream, uint data)
        {
            byte[] buffer = new byte[sizeof(uint)];
            BinaryPrimitives.WriteUInt32BigEndian(buffer, data);
            await stream.WriteAsync(buffer);
        }
        public static uint[] ReadU32A(this Stream stream, int size)
        {
            byte[] buffer = stream.ReadU8A(size * sizeof(uint));
            uint[] data = new uint[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadUInt32BigEndian(buffer.AsSpan()[(i * sizeof(uint))..]);
            }
            return data;
        }
        public static void WriteU32A(this Stream stream, uint[] data)
        {
            byte[] buffer = new byte[data.Length * sizeof(uint)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteUInt32BigEndian(buffer.AsSpan()[(i * sizeof(uint))..], data[i]);
            }
            stream.Write(buffer);
        }
        public static uint[] ReadU32A(this Stream stream, SizePrefix prefix, int limit = int.MaxValue)
        {
            int size = prefix.ReadSync(stream, limit);
            byte[] buffer = stream.ReadU8A(size * sizeof(uint));
            uint[] data = new uint[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadUInt32BigEndian(buffer.AsSpan()[(i * sizeof(uint))..]);
            }
            return data;
        }
        public static void WriteU32A(this Stream stream, uint[] data, SizePrefix prefix, int limit = int.MaxValue)
        {
            prefix.WriteSync(stream, limit, data.Length);
            byte[] buffer = new byte[data.Length * sizeof(uint)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteUInt32BigEndian(buffer.AsSpan()[(i * sizeof(uint))..], data[i]);
            }
            stream.Write(buffer);
        }
        public static uint ReadU32(this Stream stream)
        {
            byte[] buffer = stream.ReadU8A(sizeof(uint));
            return BinaryPrimitives.ReadUInt32BigEndian(buffer);
        }
        public static void WriteU32(this Stream stream, uint data)
        {
            byte[] buffer = new byte[sizeof(uint)];
            BinaryPrimitives.WriteUInt32BigEndian(buffer, data);
            stream.Write(buffer);
        }
        #endregion
        #region S32
        public static async Task<int[]> ReadS32AAsync(this Stream stream, int size)
        {
            byte[] buffer = await stream.ReadU8AAsync(size * sizeof(int));
            int[] data = new int[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadInt32BigEndian(buffer.AsSpan()[(i * sizeof(int))..]);
            }
            return data;
        }
        public static async Task WriteS32AAsync(this Stream stream, int[] data)
        {
            byte[] buffer = new byte[data.Length * sizeof(int)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteInt32BigEndian(buffer.AsSpan()[(i * sizeof(int))..], data[i]);
            }
            await stream.WriteAsync(buffer);
        }
        public static async Task<int[]> ReadS32AAsync(this Stream stream, SizePrefix prefix, int limit = int.MaxValue)
        {
            int size = await prefix.ReadAsync(stream, limit);
            byte[] buffer = await stream.ReadU8AAsync(size * sizeof(int));
            int[] data = new int[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadInt32BigEndian(buffer.AsSpan()[(i * sizeof(int))..]);
            }
            return data;
        }
        public static async Task WriteS32AAsync(this Stream stream, int[] data, SizePrefix prefix, int limit = int.MaxValue)
        {
            await prefix.WriteAsync(stream, limit, data.Length);
            byte[] buffer = new byte[data.Length * sizeof(int)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteInt32BigEndian(buffer.AsSpan()[(i * sizeof(int))..], data[i]);
            }
            await stream.WriteAsync(buffer);
        }
        public static async Task<int> ReadS32Async(this Stream stream)
        {
            byte[] buffer = await stream.ReadU8AAsync(sizeof(int));
            return BinaryPrimitives.ReadInt32BigEndian(buffer);
        }
        public static async Task WriteS32Async(this Stream stream, int data)
        {
            byte[] buffer = new byte[sizeof(int)];
            BinaryPrimitives.WriteInt32BigEndian(buffer, data);
            await stream.WriteAsync(buffer);
        }
        public static int[] ReadS32A(this Stream stream, int size)
        {
            byte[] buffer = stream.ReadU8A(size * sizeof(int));
            int[] data = new int[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadInt32BigEndian(buffer.AsSpan()[(i * sizeof(int))..]);
            }
            return data;
        }
        public static void WriteS32A(this Stream stream, int[] data)
        {
            byte[] buffer = new byte[data.Length * sizeof(int)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteInt32BigEndian(buffer.AsSpan()[(i * sizeof(int))..], data[i]);
            }
            stream.Write(buffer);
        }
        public static int[] ReadS32A(this Stream stream, SizePrefix prefix, int limit = int.MaxValue)
        {
            int size = prefix.ReadSync(stream, limit);
            byte[] buffer = stream.ReadU8A(size * sizeof(int));
            int[] data = new int[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadInt32BigEndian(buffer.AsSpan()[(i * sizeof(int))..]);
            }
            return data;
        }
        public static void WriteS32A(this Stream stream, int[] data, SizePrefix prefix, int limit = int.MaxValue)
        {
            prefix.WriteSync(stream, limit, data.Length);
            byte[] buffer = new byte[data.Length * sizeof(int)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteInt32BigEndian(buffer.AsSpan()[(i * sizeof(int))..], data[i]);
            }
            stream.Write(buffer);
        }
        public static int ReadS32(this Stream stream)
        {
            byte[] buffer = stream.ReadU8A(sizeof(int));
            return BinaryPrimitives.ReadInt32BigEndian(buffer);
        }
        public static void WriteS32(this Stream stream, int data)
        {
            byte[] buffer = new byte[sizeof(int)];
            BinaryPrimitives.WriteInt32BigEndian(buffer, data);
            stream.Write(buffer);
        }
        #endregion
        #region U64
        public static async Task<ulong[]> ReadU64AAsync(this Stream stream, int size)
        {
            byte[] buffer = await stream.ReadU8AAsync(size * sizeof(ulong));
            ulong[] data = new ulong[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadUInt64BigEndian(buffer.AsSpan()[(i * sizeof(ulong))..]);
            }
            return data;
        }
        public static async Task WriteU64AAsync(this Stream stream, ulong[] data)
        {
            byte[] buffer = new byte[data.Length * sizeof(ulong)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteUInt64BigEndian(buffer.AsSpan()[(i * sizeof(ulong))..], data[i]);
            }
            await stream.WriteAsync(buffer);
        }
        public static async Task<ulong[]> ReadU64AAsync(this Stream stream, SizePrefix prefix, int limit = int.MaxValue)
        {
            int size = await prefix.ReadAsync(stream, limit);
            byte[] buffer = await stream.ReadU8AAsync(size * sizeof(ulong));
            ulong[] data = new ulong[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadUInt64BigEndian(buffer.AsSpan()[(i * sizeof(ulong))..]);
            }
            return data;
        }
        public static async Task WriteU64AAsync(this Stream stream, ulong[] data, SizePrefix prefix, int limit = int.MaxValue)
        {
            await prefix.WriteAsync(stream, limit, data.Length);
            byte[] buffer = new byte[data.Length * sizeof(ulong)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteUInt64BigEndian(buffer.AsSpan()[(i * sizeof(ulong))..], data[i]);
            }
            await stream.WriteAsync(buffer);
        }
        public static async Task<ulong> ReadU64Async(this Stream stream)
        {
            byte[] buffer = await stream.ReadU8AAsync(sizeof(ulong));
            return BinaryPrimitives.ReadUInt64BigEndian(buffer);
        }
        public static async Task WriteU64Async(this Stream stream, ulong data)
        {
            byte[] buffer = new byte[sizeof(ulong)];
            BinaryPrimitives.WriteUInt64BigEndian(buffer, data);
            await stream.WriteAsync(buffer);
        }
        public static ulong[] ReadU64A(this Stream stream, int size)
        {
            byte[] buffer = stream.ReadU8A(size * sizeof(ulong));
            ulong[] data = new ulong[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadUInt64BigEndian(buffer.AsSpan()[(i * sizeof(ulong))..]);
            }
            return data;
        }
        public static void WriteU64A(this Stream stream, ulong[] data)
        {
            byte[] buffer = new byte[data.Length * sizeof(ulong)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteUInt64BigEndian(buffer.AsSpan()[(i * sizeof(ulong))..], data[i]);
            }
            stream.Write(buffer);
        }
        public static ulong[] ReadU64A(this Stream stream, SizePrefix prefix, int limit = int.MaxValue)
        {
            int size = prefix.ReadSync(stream, limit);
            byte[] buffer = stream.ReadU8A(size * sizeof(ulong));
            ulong[] data = new ulong[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadUInt64BigEndian(buffer.AsSpan()[(i * sizeof(ulong))..]);
            }
            return data;
        }
        public static void WriteU64A(this Stream stream, ulong[] data, SizePrefix prefix, int limit = int.MaxValue)
        {
            prefix.WriteSync(stream, limit, data.Length);
            byte[] buffer = new byte[data.Length * sizeof(ulong)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteUInt64BigEndian(buffer.AsSpan()[(i * sizeof(ulong))..], data[i]);
            }
            stream.Write(buffer);
        }
        public static ulong ReadU64(this Stream stream)
        {
            byte[] buffer = stream.ReadU8A(sizeof(ulong));
            return BinaryPrimitives.ReadUInt64BigEndian(buffer);
        }
        public static void WriteU64(this Stream stream, ulong data)
        {
            byte[] buffer = new byte[sizeof(ulong)];
            BinaryPrimitives.WriteUInt64BigEndian(buffer, data);
            stream.Write(buffer);
        }
        #endregion
        #region S64
        public static async Task<long[]> ReadS64AAsync(this Stream stream, int size)
        {
            byte[] buffer = await stream.ReadU8AAsync(size * sizeof(long));
            long[] data = new long[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadInt64BigEndian(buffer.AsSpan()[(i * sizeof(long))..]);
            }
            return data;
        }
        public static async Task WriteS64AAsync(this Stream stream, long[] data)
        {
            byte[] buffer = new byte[data.Length * sizeof(long)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteInt64BigEndian(buffer.AsSpan()[(i * sizeof(long))..], data[i]);
            }
            await stream.WriteAsync(buffer);
        }
        public static async Task<long[]> ReadS64AAsync(this Stream stream, SizePrefix prefix, int limit = int.MaxValue)
        {
            int size = await prefix.ReadAsync(stream, limit);
            byte[] buffer = await stream.ReadU8AAsync(size * sizeof(long));
            long[] data = new long[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadInt64BigEndian(buffer.AsSpan()[(i * sizeof(long))..]);
            }
            return data;
        }
        public static async Task WriteS64AAsync(this Stream stream, long[] data, SizePrefix prefix, int limit = int.MaxValue)
        {
            await prefix.WriteAsync(stream, limit, data.Length);
            byte[] buffer = new byte[data.Length * sizeof(long)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteInt64BigEndian(buffer.AsSpan()[(i * sizeof(long))..], data[i]);
            }
            await stream.WriteAsync(buffer);
        }
        public static async Task<long> ReadS64Async(this Stream stream)
        {
            byte[] buffer = await stream.ReadU8AAsync(sizeof(long));
            return BinaryPrimitives.ReadInt64BigEndian(buffer);
        }
        public static async Task WriteS64Async(this Stream stream, long data)
        {
            byte[] buffer = new byte[sizeof(long)];
            BinaryPrimitives.WriteInt64BigEndian(buffer, data);
            await stream.WriteAsync(buffer);
        }
        public static long[] ReadS64A(this Stream stream, int size)
        {
            byte[] buffer = stream.ReadU8A(size * sizeof(long));
            long[] data = new long[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadInt64BigEndian(buffer.AsSpan()[(i * sizeof(long))..]);
            }
            return data;
        }
        public static void WriteS64A(this Stream stream, long[] data)
        {
            byte[] buffer = new byte[data.Length * sizeof(long)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteInt64BigEndian(buffer.AsSpan()[(i * sizeof(long))..], data[i]);
            }
            stream.Write(buffer);
        }
        public static long[] ReadS64A(this Stream stream, SizePrefix prefix, int limit = int.MaxValue)
        {
            int size = prefix.ReadSync(stream, limit);
            byte[] buffer = stream.ReadU8A(size * sizeof(long));
            long[] data = new long[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadInt64BigEndian(buffer.AsSpan()[(i * sizeof(long))..]);
            }
            return data;
        }
        public static void WriteS64A(this Stream stream, long[] data, SizePrefix prefix, int limit = int.MaxValue)
        {
            prefix.WriteSync(stream, limit, data.Length);
            byte[] buffer = new byte[data.Length * sizeof(long)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteInt64BigEndian(buffer.AsSpan()[(i * sizeof(long))..], data[i]);
            }
            stream.Write(buffer);
        }
        public static long ReadS64(this Stream stream)
        {
            byte[] buffer = stream.ReadU8A(sizeof(long));
            return BinaryPrimitives.ReadInt64BigEndian(buffer);
        }
        public static void WriteS64(this Stream stream, long data)
        {
            byte[] buffer = new byte[sizeof(long)];
            BinaryPrimitives.WriteInt64BigEndian(buffer, data);
            stream.Write(buffer);
        }
        #endregion
        #region F32
        public static async Task<float[]> ReadF32AAsync(this Stream stream, int size)
        {
            byte[] buffer = await stream.ReadU8AAsync(size * sizeof(float));
            float[] data = new float[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadSingleBigEndian(buffer.AsSpan()[(i * sizeof(float))..]);
            }
            return data;
        }
        public static async Task WriteF32AAsync(this Stream stream, float[] data)
        {
            byte[] buffer = new byte[data.Length * sizeof(float)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteSingleBigEndian(buffer.AsSpan()[(i * sizeof(float))..], data[i]);
            }
            await stream.WriteAsync(buffer);
        }
        public static async Task<float[]> ReadF32AAsync(this Stream stream, SizePrefix prefix, int limit = int.MaxValue)
        {
            int size = await prefix.ReadAsync(stream, limit);
            byte[] buffer = await stream.ReadU8AAsync(size * sizeof(float));
            float[] data = new float[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadSingleBigEndian(buffer.AsSpan()[(i * sizeof(float))..]);
            }
            return data;
        }
        public static async Task WriteF32AAsync(this Stream stream, float[] data, SizePrefix prefix, int limit = int.MaxValue)
        {
            await prefix.WriteAsync(stream, limit, data.Length);
            byte[] buffer = new byte[data.Length * sizeof(float)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteSingleBigEndian(buffer.AsSpan()[(i * sizeof(float))..], data[i]);
            }
            await stream.WriteAsync(buffer);
        }
        public static async Task<float> ReadF32Async(this Stream stream)
        {
            byte[] buffer = await stream.ReadU8AAsync(sizeof(float));
            return BinaryPrimitives.ReadSingleBigEndian(buffer);
        }
        public static async Task WriteF32Async(this Stream stream, float data)
        {
            byte[] buffer = new byte[sizeof(float)];
            BinaryPrimitives.WriteSingleBigEndian(buffer, data);
            await stream.WriteAsync(buffer);
        }
        public static float[] ReadF32A(this Stream stream, int size)
        {
            byte[] buffer = stream.ReadU8A(size * sizeof(float));
            float[] data = new float[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadSingleBigEndian(buffer.AsSpan()[(i * sizeof(float))..]);
            }
            return data;
        }
        public static void WriteF32A(this Stream stream, float[] data)
        {
            byte[] buffer = new byte[data.Length * sizeof(float)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteSingleBigEndian(buffer.AsSpan()[(i * sizeof(float))..], data[i]);
            }
            stream.Write(buffer);
        }
        public static float[] ReadF32A(this Stream stream, SizePrefix prefix, int limit = int.MaxValue)
        {
            int size = prefix.ReadSync(stream, limit);
            byte[] buffer = stream.ReadU8A(size * sizeof(float));
            float[] data = new float[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadSingleBigEndian(buffer.AsSpan()[(i * sizeof(float))..]);
            }
            return data;
        }
        public static void WriteF32A(this Stream stream, float[] data, SizePrefix prefix, int limit = int.MaxValue)
        {
            prefix.WriteSync(stream, limit, data.Length);
            byte[] buffer = new byte[data.Length * sizeof(float)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteSingleBigEndian(buffer.AsSpan()[(i * sizeof(float))..], data[i]);
            }
            stream.Write(buffer);
        }
        public static float ReadF32(this Stream stream)
        {
            byte[] buffer = stream.ReadU8A(sizeof(float));
            return BinaryPrimitives.ReadSingleBigEndian(buffer);
        }
        public static void WriteF32(this Stream stream, float data)
        {
            byte[] buffer = new byte[sizeof(float)];
            BinaryPrimitives.WriteSingleBigEndian(buffer, data);
            stream.Write(buffer);
        }
        #endregion
        #region F64
        public static async Task<double[]> ReadF64AAsync(this Stream stream, int size)
        {
            byte[] buffer = await stream.ReadU8AAsync(size * sizeof(double));
            double[] data = new double[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadDoubleBigEndian(buffer.AsSpan()[(i * sizeof(double))..]);
            }
            return data;
        }
        public static async Task WriteF64AAsync(this Stream stream, double[] data)
        {
            byte[] buffer = new byte[data.Length * sizeof(double)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteDoubleBigEndian(buffer.AsSpan()[(i * sizeof(double))..], data[i]);
            }
            await stream.WriteAsync(buffer);
        }
        public static async Task<double[]> ReadF64AAsync(this Stream stream, SizePrefix prefix, int limit = int.MaxValue)
        {
            int size = await prefix.ReadAsync(stream, limit);
            byte[] buffer = await stream.ReadU8AAsync(size * sizeof(double));
            double[] data = new double[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadDoubleBigEndian(buffer.AsSpan()[(i * sizeof(double))..]);
            }
            return data;
        }
        public static async Task WriteF64AAsync(this Stream stream, double[] data, SizePrefix prefix, int limit = int.MaxValue)
        {
            await prefix.WriteAsync(stream, limit, data.Length);
            byte[] buffer = new byte[data.Length * sizeof(double)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteDoubleBigEndian(buffer.AsSpan()[(i * sizeof(double))..], data[i]);
            }
            await stream.WriteAsync(buffer);
        }
        public static async Task<double> ReadF64Async(this Stream stream)
        {
            byte[] buffer = await stream.ReadU8AAsync(sizeof(double));
            return BinaryPrimitives.ReadDoubleBigEndian(buffer);
        }
        public static async Task WriteF64Async(this Stream stream, double data)
        {
            byte[] buffer = new byte[sizeof(double)];
            BinaryPrimitives.WriteDoubleBigEndian(buffer, data);
            await stream.WriteAsync(buffer);
        }
        public static double[] ReadF64A(this Stream stream, int size)
        {
            byte[] buffer = stream.ReadU8A(size * sizeof(double));
            double[] data = new double[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadDoubleBigEndian(buffer.AsSpan()[(i * sizeof(double))..]);
            }
            return data;
        }
        public static void WriteF64A(this Stream stream, double[] data)
        {
            byte[] buffer = new byte[data.Length * sizeof(double)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteDoubleBigEndian(buffer.AsSpan()[(i * sizeof(double))..], data[i]);
            }
            stream.Write(buffer);
        }
        public static double[] ReadF64A(this Stream stream, SizePrefix prefix, int limit = int.MaxValue)
        {
            int size = prefix.ReadSync(stream, limit);
            byte[] buffer = stream.ReadU8A(size * sizeof(double));
            double[] data = new double[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadDoubleBigEndian(buffer.AsSpan()[(i * sizeof(double))..]);
            }
            return data;
        }
        public static void WriteF64A(this Stream stream, double[] data, SizePrefix prefix, int limit = int.MaxValue)
        {
            prefix.WriteSync(stream, limit, data.Length);
            byte[] buffer = new byte[data.Length * sizeof(double)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteDoubleBigEndian(buffer.AsSpan()[(i * sizeof(double))..], data[i]);
            }
            stream.Write(buffer);
        }
        public static double ReadF64(this Stream stream)
        {
            byte[] buffer = stream.ReadU8A(sizeof(double));
            return BinaryPrimitives.ReadDoubleBigEndian(buffer);
        }
        public static void WriteF64(this Stream stream, double data)
        {
            byte[] buffer = new byte[sizeof(double)];
            BinaryPrimitives.WriteDoubleBigEndian(buffer, data);
            stream.Write(buffer);
        }
        #endregion

        #region U8V
        public static async Task<byte> ReadU8VAsync(this Stream stream)
        {
            byte data = 0;
            int position = 0;
            while (true)
            {
                byte current = await stream.ReadU8Async();
                data |= (byte)((current & 127U) << position);
                if ((current & 128) == 0) return data;
                position += 7;
                if (position >= sizeof(ushort) * 8) throw new ProtocolViolationException();
            }
        }
        public static async Task<int> WriteU8VAsync(this Stream stream, byte data)
        {
            int size = 0;
            do
            {
                byte current = data;
                data >>= 7;
                if (data != 0) current |= 128;
                else current &= 127;
                await stream.WriteU8Async(current);
                size++;
            }
            while (data != 0);
            return size;
        }
        public static byte ReadU8V(this Stream stream)
        {
            byte data = 0;
            int position = 0;
            while (true)
            {
                byte current = stream.ReadU8();
                data |= (byte)((current & 127U) << position);
                if ((current & 128) == 0) return data;
                position += 7;
                if (position >= sizeof(ushort) * 8) throw new ProtocolViolationException();
            }
        }
        public static int WriteU8V(this Stream stream, byte data)
        {
            int size = 0;
            do
            {
                byte current = data;
                data >>= 7;
                if (data != 0) current |= 128;
                else current &= 127;
                stream.WriteU8(current);
                size++;
            }
            while (data != 0);
            return size;
        }
        #endregion
        #region S8V
        public static async Task<sbyte> ReadS8VAsync(this Stream stream)
        {
            return (sbyte)await stream.ReadU8VAsync();
        }
        public static async Task<int> WriteS8VAsync(this Stream stream, sbyte data)
        {
            int size = 0;
            do
            {
                byte current = (byte)data;
                data >>= 7;
                if (data != 0) current |= 128;
                else current &= 127;
                await stream.WriteU8Async(current);
                size++;
            }
            while (data != 0);
            return size;
        }
        public static sbyte ReadS8V(this Stream stream)
        {
            return (sbyte)stream.ReadU8V();
        }
        public static int WriteS8V(this Stream stream, sbyte data)
        {
            int size = 0;
            do
            {
                byte current = (byte)data;
                data >>= 7;
                if (data != 0) current |= 128;
                else current &= 127;
                stream.WriteU8(current);
                size++;
            }
            while (data != 0);
            return size;
        }
        #endregion
        #region U16V
        public static async Task<ushort> ReadU16VAsync(this Stream stream)
        {
            ushort data = 0;
            int position = 0;
            while (true)
            {
                byte current = await stream.ReadU8Async();
                data |= (ushort)((current & 127U) << position);
                if ((current & 128) == 0) return data;
                position += 7;
                if (position >= sizeof(ushort) * 8) throw new ProtocolViolationException();
            }
        }
        public static async Task<int> WriteU16VAsync(this Stream stream, ushort data)
        {
            int size = 0;
            do
            {
                byte current = (byte)data;
                data >>= 7;
                if (data != 0) current |= 128;
                else current &= 127;
                await stream.WriteU8Async(current);
                size++;
            }
            while (data != 0);
            return size;
        }
        public static ushort ReadU16V(this Stream stream)
        {
            ushort data = 0;
            int position = 0;
            while (true)
            {
                byte current = stream.ReadU8();
                data |= (ushort)((current & 127U) << position);
                if ((current & 128) == 0) return data;
                position += 7;
                if (position >= sizeof(ushort) * 8) throw new ProtocolViolationException();
            }
        }
        public static int WriteU16V(this Stream stream, ushort data)
        {
            int size = 0;
            do
            {
                byte current = (byte)data;
                data >>= 7;
                if (data != 0) current |= 128;
                else current &= 127;
                stream.WriteU8(current);
                size++;
            }
            while (data != 0);
            return size;
        }
        #endregion
        #region S16V
        public static async Task<short> ReadS16VAsync(this Stream stream)
        {
            return (short)await stream.ReadU16VAsync();
        }
        public static async Task<int> WriteS16VAsync(this Stream stream, short data)
        {
            int size = 0;
            do
            {
                byte current = (byte)data;
                data >>= 7;
                if (data != 0) current |= 128;
                else current &= 127;
                await stream.WriteU8Async(current);
                size++;
            }
            while (data != 0);
            return size;
        }
        public static short ReadS16V(this Stream stream)
        {
            return (short)stream.ReadU16V();
        }
        public static int WriteS16V(this Stream stream, short data)
        {
            int size = 0;
            do
            {
                byte current = (byte)data;
                data >>= 7;
                if (data != 0) current |= 128;
                else current &= 127;
                stream.WriteU8(current);
                size++;
            }
            while (data != 0);
            return size;
        }
        #endregion
        #region U32V
        public static async Task<uint> ReadU32VAsync(this Stream stream)
        {
            uint data = 0;
            int position = 0;
            while (true)
            {
                byte current = await stream.ReadU8Async();
                data |= (current & 127U) << position;
                if ((current & 128) == 0) return data;
                position += 7;
                if (position >= sizeof(uint) * 8) throw new ProtocolViolationException();
            }
        }
        public static async Task<int> WriteU32VAsync(this Stream stream, uint data)
        {
            int size = 0;
            do
            {
                byte current = (byte)data;
                data >>= 7;
                if (data != 0) current |= 128;
                else current &= 127;
                await stream.WriteU8Async(current);
                size++;
            }
            while (data != 0);
            return size;
        }
        public static uint ReadU32V(this Stream stream)
        {
            uint data = 0;
            int position = 0;
            while (true)
            {
                byte current = stream.ReadU8();
                data |= (current & 127U) << position;
                if ((current & 128) == 0) return data;
                position += 7;
                if (position >= sizeof(uint) * 8) throw new ProtocolViolationException();
            }
        }
        public static int WriteU32V(this Stream stream, uint data)
        {
            int size = 0;
            do
            {
                byte current = (byte)data;
                data >>= 7;
                if (data != 0) current |= 128;
                else current &= 127;
                stream.WriteU8(current);
                size++;
            }
            while (data != 0);
            return size;
        }
        #endregion
        #region S32
        public static async Task<int> ReadS32VAsync(this Stream stream)
        {
            return (int)await stream.ReadU32VAsync();
        }
        public static async Task<int> WriteS32VAsync(this Stream stream, int data)
        {
            int size = 0;
            do
            {
                byte current = (byte)data;
                data >>= 7;
                if (data != 0) current |= 128;
                else current &= 127;
                await stream.WriteU8Async(current);
                size++;
            }
            while (data != 0);
            return size;
        }
        public static int ReadS32V(this Stream stream)
        {
            return (int)stream.ReadU32V();
        }
        public static int WriteS32V(this Stream stream, int data)
        {
            int size = 0;
            do
            {
                byte current = (byte)data;
                data >>= 7;
                if (data != 0) current |= 128;
                else current &= 127;
                stream.WriteU8(current);
                size++;
            }
            while (data != 0);
            return size;
        }
        #endregion
        #region U64V
        public static async Task<ulong> ReadU64VAsync(this Stream stream)
        {
            ulong data = 0;
            int position = 0;
            while (true)
            {
                byte current = await stream.ReadU8Async();
                data |= (current & 127U) << position;
                if ((current & 128) == 0) return data;
                position += 7;
                if (position >= 64) throw new ProtocolViolationException();
            }
        }
        public static async Task<int> WriteU64VAsync(this Stream stream, ulong data)
        {
            int size = 0;
            do
            {
                byte current = (byte)data;
                data >>= 7;
                if (data != 0) current |= 128;
                else current &= 127;
                await stream.WriteU8Async(current);
                size++;
            }
            while (data != 0);
            return size;
        }
        public static ulong ReadU64V(this Stream stream)
        {
            ulong data = 0;
            int position = 0;
            while (true)
            {
                byte current = stream.ReadU8();
                data |= (current & 127U) << position;
                if ((current & 128) == 0) return data;
                position += 7;
                if (position >= 64) throw new ProtocolViolationException();
            }
        }
        public static int WriteU64V(this Stream stream, ulong data)
        {
            int size = 0;
            do
            {
                byte current = (byte)data;
                data >>= 7;
                if (data != 0) current |= 128;
                else current &= 127;
                stream.WriteU8(current);
                size++;
            }
            while (data != 0);
            return size;
        }
        #endregion
        #region S64
        public static async Task<long> ReadS64VAsync(this Stream stream)
        {
            return (long)await stream.ReadU64VAsync();
        }
        public static async Task<int> WriteS64VAsync(this Stream stream, long data)
        {
            int size = 0;
            do
            {
                byte current = (byte)data;
                data >>= 7;
                if (data != 0) current |= 128;
                else current &= 127;
                await stream.WriteU8Async(current);
                size++;
            }
            while (data != 0);
            return size;
        }
        public static long ReadS64V(this Stream stream)
        {
            return (long)stream.ReadU64V();
        }
        public static int WriteS64V(this Stream stream, long data)
        {
            int size = 0;
            do
            {
                byte current = (byte)data;
                data >>= 7;
                if (data != 0) current |= 128;
                else current &= 127;
                stream.WriteU8(current);
                size++;
            }
            while (data != 0);
            return size;
        }
        #endregion

        #region Bool
        public static async Task<bool> ReadBoolAsync(this Stream stream)
        {
            return (await stream.ReadU8Async()) != 0;
        }
        public static async Task WriteBoolAsync(this Stream stream, bool data)
        {
            await stream.WriteAsync(MemoryMarshal.AsBytes<bool>(new bool[] { data }).ToArray());
        }
        public static bool ReadBool(this Stream stream)
        {
            return (stream.ReadU8()) != 0;
        }
        public static void WriteBool(this Stream stream, bool data)
        {
            stream.Write(MemoryMarshal.AsBytes<bool>(new bool[] { data }).ToArray());
        }
        #endregion
        #region Guid
        public static async Task<Guid> ReadGuidAsync(this Stream stream)
        {
            byte[] buffer = await stream.ReadU8AAsync(16);
            if (BitConverter.IsLittleEndian) buffer = [buffer[3], buffer[2], buffer[1], buffer[0], buffer[5], buffer[4], buffer[7], buffer[6], buffer[8], buffer[9], buffer[10], buffer[11], buffer[12], buffer[13], buffer[14], buffer[15]];
            return MemoryMarshal.Cast<byte, Guid>(buffer)[0];
        }
        public static async Task WriteGuidAsync(this Stream stream, Guid data)
        {
            byte[] buffer = MemoryMarshal.AsBytes([data]).ToArray();
            if (!BitConverter.IsLittleEndian)
            {
                await stream.WriteAsync(buffer);
                return;
            }
            await stream.WriteAsync(new byte[] { buffer[3], buffer[2], buffer[1], buffer[0], buffer[5], buffer[4], buffer[7], buffer[6], buffer[8], buffer[9], buffer[10], buffer[11], buffer[12], buffer[13], buffer[14], buffer[15] });
        }
        public static Guid ReadGuid(this Stream stream)
        {
            byte[] buffer = stream.ReadU8A(16);
            if (BitConverter.IsLittleEndian) buffer = [buffer[3], buffer[2], buffer[1], buffer[0], buffer[5], buffer[4], buffer[7], buffer[6], buffer[8], buffer[9], buffer[10], buffer[11], buffer[12], buffer[13], buffer[14], buffer[15]];
            return MemoryMarshal.Cast<byte, Guid>(buffer)[0];
        }
        public static void WriteGuid(this Stream stream, Guid data)
        {
            byte[] buffer = MemoryMarshal.AsBytes([data]).ToArray();
            if (!BitConverter.IsLittleEndian)
            {
                stream.Write(buffer);
                return;
            }
            stream.Write([buffer[3], buffer[2], buffer[1], buffer[0], buffer[5], buffer[4], buffer[7], buffer[6], buffer[8], buffer[9], buffer[10], buffer[11], buffer[12], buffer[13], buffer[14], buffer[15]]);
        }
        #endregion

        #region String
        public static async Task<string> ReadStringAsync(this Stream stream, SizePrefix prefix, int limit = int.MaxValue, Encoding? encoding = null)
        {
            return (encoding ?? Encoding.UTF8).GetString(await stream.ReadU8AAsync(await prefix.ReadAsync(stream, limit)));
        }
        public static async Task WriteStringAsync(this Stream stream, string data, SizePrefix prefix, int limit = int.MaxValue, Encoding? encoding = null)
        {
            byte[] buffer = (encoding ?? Encoding.UTF8).GetBytes(data);
            await prefix.WriteAsync(stream, limit, buffer.Length);
            await stream.WriteU8AAsync(buffer);
        }
        public static string ReadString(this Stream stream, SizePrefix prefix, int limit = int.MaxValue, Encoding? encoding = null)
        {
            return (encoding ?? Encoding.UTF8).GetString(stream.ReadU8A(prefix.ReadSync(stream, limit)));
        }
        public static void WriteString(this Stream stream, string data, SizePrefix prefix, int limit = int.MaxValue, Encoding? encoding = null)
        {
            byte[] buffer = (encoding ?? Encoding.UTF8).GetBytes(data);
            prefix.WriteSync(stream, limit, buffer.Length);
            stream.WriteU8A(buffer);
        }
        #endregion
    }
}
