using System;
using System.Buffers.Binary;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Net.Myzuc.UtilLib
{
    public sealed class DataStream : IDisposable
    {
        private readonly bool CanWrite;
        private readonly bool CanRead;
        public readonly Stream Stream;
        public DataStream()
        {
            Stream = new MemoryStream();
            CanRead = false;
            CanWrite = true;
        }
        public DataStream(byte[] data)
        {
            Stream = new MemoryStream(data);
            CanRead = true;
            CanWrite = false;
        }
        public DataStream(Stream stream)
        {
            CanRead = true;
            CanWrite = true;
            Stream = stream;
        }
        public void Dispose()
        {
            Stream.Dispose();
        }
        public byte[] Get()
        {
            Contract.Requires(CanRead);
            Contract.Requires(Stream is not MemoryStream);
            return ((MemoryStream)Stream).ToArray();
        }
        public async Task WriteU8AAsync(byte[] data)
        {
            Contract.Requires(CanWrite);
            await Stream.WriteAsync(data);
        }
        public async Task WriteS8AAsync(sbyte[] data)
        {
            Contract.Requires(CanWrite);
            await Stream.WriteAsync(MemoryMarshal.AsBytes(data.AsSpan()).ToArray());
        }
        public async Task WriteU16AAsync(ushort[] data)
        {
            Contract.Requires(CanWrite);
            Contract.Requires(data.Length * sizeof(ushort) <= int.MaxValue);
            byte[] buffer = new byte[data.Length * sizeof(ushort)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteUInt16BigEndian(buffer.AsSpan()[(i * sizeof(ushort))..], data[i]);
            }
            await Stream.WriteAsync(buffer);
        }
        public async Task WriteS16AAsync(short[] data)
        {
            Contract.Requires(CanWrite);
            Contract.Requires(data.Length * sizeof(short) <= int.MaxValue);
            byte[] buffer = new byte[data.Length * sizeof(short)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteInt16BigEndian(buffer.AsSpan()[(i * sizeof(short))..], data[i]);
            }
            await Stream.WriteAsync(buffer);
        }
        public async Task WriteU32AAsync(uint[] data)
        {
            Contract.Requires(CanWrite);
            Contract.Requires(data.Length * sizeof(uint) <= int.MaxValue);
            byte[] buffer = new byte[data.Length * sizeof(uint)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteUInt32BigEndian(buffer.AsSpan()[(i * sizeof(uint))..], data[i]);
            }
            await Stream.WriteAsync(buffer);
        }
        public async Task WriteS32AAsync(int[] data)
        {
            Contract.Requires(CanWrite);
            Contract.Requires(data.Length * sizeof(int) <= int.MaxValue);
            byte[] buffer = new byte[data.Length * sizeof(int)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteInt32BigEndian(buffer.AsSpan()[(i * sizeof(int))..], data[i]);
            }
            await Stream.WriteAsync(buffer);
        }
        public async Task WriteU64AAsync(ulong[] data)
        {
            Contract.Requires(CanWrite);
            Contract.Requires(data.Length * sizeof(ulong) <= int.MaxValue);
            byte[] buffer = new byte[data.Length * sizeof(ulong)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteUInt64BigEndian(buffer.AsSpan()[(i * sizeof(ulong))..], data[i]);
            }
            await Stream.WriteAsync(buffer);
        }
        public async Task WriteS64AAsync(long[] data)
        {
            Contract.Requires(CanWrite);
            Contract.Requires(data.Length * sizeof(long) <= int.MaxValue);
            byte[] buffer = new byte[data.Length * sizeof(long)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteInt64BigEndian(buffer.AsSpan()[(i * sizeof(long))..], data[i]);
            }
            await Stream.WriteAsync(buffer);
        }
        public async Task WriteF32AAsync(float[] data)
        {
            Contract.Requires(CanWrite);
            Contract.Requires(data.Length * sizeof(float) <= int.MaxValue);
            byte[] buffer = new byte[data.Length * sizeof(float)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteSingleBigEndian(buffer.AsSpan()[(i * sizeof(float))..], data[i]);
            }
            await Stream.WriteAsync(buffer);
        }
        public async Task WriteF64AAsync(double[] data)
        {
            Contract.Requires(CanWrite);
            Contract.Requires(data.Length * sizeof(double) <= int.MaxValue);
            byte[] buffer = new byte[data.Length * sizeof(double)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteDoubleBigEndian(buffer.AsSpan()[(i * sizeof(double))..], data[i]);
            }
            await Stream.WriteAsync(buffer);
        }
        public async Task WriteU8Async(byte data)
        {
            Contract.Requires(CanWrite);
            await Stream.WriteAsync(new byte[] { data });
        }
        public async Task WriteS8Async(sbyte data)
        {
            Contract.Requires(CanWrite);
            await Stream.WriteAsync(MemoryMarshal.AsBytes<sbyte>(new sbyte[] { data }).ToArray());
        }
        public async Task WriteU16Async(ushort data)
        {
            Contract.Requires(CanWrite);
            byte[] buffer = new byte[sizeof(ushort)];
            BinaryPrimitives.WriteUInt16BigEndian(buffer, data);
            await Stream.WriteAsync(buffer);
        }
        public async Task WriteS16Async(short data)
        {
            Contract.Requires(CanWrite);
            byte[] buffer = new byte[sizeof(short)];
            BinaryPrimitives.WriteInt16BigEndian(buffer, data);
            await Stream.WriteAsync(buffer);
        }
        public async Task WriteU32Async(uint data)
        {
            Contract.Requires(CanWrite);
            byte[] buffer = new byte[sizeof(uint)];
            BinaryPrimitives.WriteUInt32BigEndian(buffer, data);
            await Stream.WriteAsync(buffer);
        }
        public async Task WriteS32Async(int data)
        {
            Contract.Requires(CanWrite);
            byte[] buffer = new byte[sizeof(int)];
            BinaryPrimitives.WriteInt32BigEndian(buffer, data);
            await Stream.WriteAsync(buffer);
        }
        public async Task WriteU64Async(ulong data)
        {
            Contract.Requires(CanWrite);
            byte[] buffer = new byte[sizeof(ulong)];
            BinaryPrimitives.WriteUInt64BigEndian(buffer, data);
            await Stream.WriteAsync(buffer);
        }
        public async Task WriteS64Async(long data)
        {
            Contract.Requires(CanWrite);
            byte[] buffer = new byte[sizeof(long)];
            BinaryPrimitives.WriteInt64BigEndian(buffer, data);
            await Stream.WriteAsync(buffer);
        }
        public async Task WriteF32Async(float data)
        {
            Contract.Requires(CanWrite);
            byte[] buffer = new byte[sizeof(float)];
            BinaryPrimitives.WriteSingleBigEndian(buffer, data);
            await Stream.WriteAsync(buffer);
        }
        public async Task WriteF64Async(double data)
        {
            Contract.Requires(CanWrite);
            byte[] buffer = new byte[sizeof(double)];
            BinaryPrimitives.WriteDoubleBigEndian(buffer, data);
            await Stream.WriteAsync(buffer);
        }
        public async Task<int> WriteU32VAsync(uint data)
        {
            Contract.Requires(CanWrite);
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
        public async Task<int> WriteS32VAsync(int data)
        {
            Contract.Requires(CanWrite);
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
        public async Task<int> WriteU64VAsync(ulong data)
        {
            Contract.Requires(CanWrite);
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
        public async Task<int> WriteS64VAsync(long data)
        {
            Contract.Requires(CanWrite);
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
        public async Task WriteU8AVAsync(byte[] buffer)
        {
            Contract.Requires(CanWrite);
            await WriteS32VAsync(buffer.Length);
            await Stream.WriteAsync(buffer);
        }
        public async Task WriteS8AVAsync(sbyte[] buffer)
        {
            Contract.Requires(CanWrite);
            await WriteS32VAsync(buffer.Length);
            await Stream.WriteAsync(MemoryMarshal.Cast<sbyte, byte>(buffer).ToArray());
        }
        public async Task WriteU16AVAsync(ushort[] buffer)
        {
            Contract.Requires(CanWrite);
            await WriteS32VAsync(buffer.Length);
            await WriteU16AAsync(buffer);
        }
        public async Task WriteS16AVAsync(short[] buffer)
        {
            Contract.Requires(CanWrite);
            await WriteS32VAsync(buffer.Length);
            await WriteS16AAsync(buffer);
        }
        public async Task WriteU32AVAsync(uint[] buffer)
        {
            Contract.Requires(CanWrite);
            await WriteS32VAsync(buffer.Length);
            await WriteU32AAsync(buffer);
        }
        public async Task WriteS32AVAsync(int[] buffer)
        {
            Contract.Requires(CanWrite);
            await WriteS32VAsync(buffer.Length);
            await WriteS32AAsync(buffer);
        }
        public async Task WriteU64AVAsync(ulong[] buffer)
        {
            Contract.Requires(CanWrite);
            await WriteS32VAsync(buffer.Length);
            await WriteU64AAsync(buffer);
        }
        public async Task WriteS64AVAsync(long[] buffer)
        {
            Contract.Requires(CanWrite);
            await WriteS32VAsync(buffer.Length);
            await WriteS64AAsync(buffer);
        }
        public async Task WriteF32AVAsync(float[] buffer)
        {
            Contract.Requires(CanWrite);
            await WriteS32VAsync(buffer.Length);
            await WriteF32AAsync(buffer);
        }
        public async Task WriteF64AVAsync(double[] buffer)
        {
            Contract.Requires(CanWrite);
            await WriteS32VAsync(buffer.Length);
            await WriteF64AAsync(buffer);
        }
        public async Task WriteBoolAsync(bool data)
        {
            Contract.Requires(CanWrite);
            await Stream.WriteAsync(MemoryMarshal.AsBytes<bool>(new bool[] { data }).ToArray());
        }
        public async Task WriteGuidAsync(Guid data)
        {
            Contract.Requires(CanWrite);
            byte[] buffer = MemoryMarshal.AsBytes([data]).ToArray(); //TODO: replace all array initialization syntax with this
            if (!BitConverter.IsLittleEndian)
            {
                await Stream.WriteAsync(buffer);
                return;
            }
            await Stream.WriteAsync(new byte[] { buffer[3], buffer[2], buffer[1], buffer[0], buffer[5], buffer[4], buffer[7], buffer[6], buffer[8], buffer[9], buffer[10], buffer[11], buffer[12], buffer[13], buffer[14], buffer[15] });
        }
        public async Task WriteStringS32VAsync(string data)
        {
            Contract.Requires(CanWrite);
            await WriteU8AVAsync(Encoding.UTF8.GetBytes(data));
        }
        public async Task WriteStringS16Async(string data)
        {
            Contract.Requires(CanWrite);
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            await WriteU16Async((ushort)buffer.Length);
            await Stream.WriteAsync(buffer);
        }
        public async Task<byte[]> ReadU8AAsync(int size)
        {
            Contract.Requires(CanRead);
            byte[] data = new byte[size];
            int position = 0;
            while (position < size)
            {
                int read = await Stream.ReadAsync(data, position, size - position);
                position += read;
                if (read == 0) throw new EndOfStreamException();
            }
            return data;
        }
        public async Task<sbyte[]> ReadS8AAsync(int size)
        {
            Contract.Requires(CanRead);
            return MemoryMarshal.Cast<byte, sbyte>(await ReadU8AAsync(size)).ToArray();
        }
        public async Task<ushort[]> ReadU16AAsync(int size)
        {
            Contract.Requires(CanRead);
            Contract.Requires(size * sizeof(ushort) <= int.MaxValue);
            byte[] buffer = await ReadU8AAsync(size * sizeof(ushort));
            ushort[] data = new ushort[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadUInt16BigEndian(buffer[(i * sizeof(ushort))..]);
            }
            return data;
        }
        public async Task<short[]> ReadS16AAsync(int size)
        {
            Contract.Requires(CanRead);
            Contract.Requires(size * sizeof(short) <= int.MaxValue);
            byte[] buffer = await ReadU8AAsync(size * sizeof(short));
            short[] data = new short[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadInt16BigEndian(buffer[(i * sizeof(short))..]);
            }
            return data;
        }
        public async Task<uint[]> ReadU32AAsync(int size)
        {
            Contract.Requires(CanRead);
            Contract.Requires(size * sizeof(uint) <= int.MaxValue);
            byte[] buffer = await ReadU8AAsync(size * sizeof(uint));
            uint[] data = new uint[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadUInt32BigEndian(buffer[(i * sizeof(uint))..]);
            }
            return data;
        }
        public async Task<int[]> ReadS32AAsync(int size)
        {
            Contract.Requires(CanRead);
            Contract.Requires(size * sizeof(int) <= int.MaxValue);
            byte[] buffer = await ReadU8AAsync(size * sizeof(int));
            int[] data = new int[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadInt32BigEndian(buffer[(i * sizeof(int))..]);
            }
            return data;
        }
        public async Task<ulong[]> ReadU64AAsync(int size)
        {
            Contract.Requires(CanRead);
            Contract.Requires(size * sizeof(ulong) <= int.MaxValue);
            byte[] buffer = await ReadU8AAsync(size * sizeof(ulong));
            ulong[] data = new ulong[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadUInt64BigEndian(buffer[(i * sizeof(ulong))..]);
            }
            return data;
        }
        public async Task<long[]> ReadS64AAsync(int size)
        {
            Contract.Requires(CanRead);
            Contract.Requires(size * sizeof(long) <= int.MaxValue);
            byte[] buffer = await ReadU8AAsync(size * sizeof(long));
            long[] data = new long[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadInt64BigEndian(buffer[(i * sizeof(long))..]);
            }
            return data;
        }
        public async Task<float[]> ReadF32AAsync(int size)
        {
            Contract.Requires(CanRead);
            Contract.Requires(size * sizeof(float) <= int.MaxValue);
            byte[] buffer = await ReadU8AAsync(size * sizeof(float));
            float[] data = new float[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadSingleBigEndian(buffer[(i * sizeof(float))..]);
            }
            return data;
        }
        public async Task<double[]> ReadF64AAsync(int size)
        {
            Contract.Requires(CanRead);
            Contract.Requires(size * sizeof(double) <= int.MaxValue);
            byte[] buffer = await ReadU8AAsync(size * sizeof(double));
            double[] data = new double[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadDoubleBigEndian(buffer[(i * sizeof(double))..]);
            }
            return data;
        }
        public async Task<byte> ReadU8Async()
        {
            Contract.Requires(CanRead);
            return (await ReadU8AAsync(1))[0];
        }
        public async Task<sbyte> ReadS8Async()
        {
            Contract.Requires(CanRead);
            return (await ReadS8AAsync(1))[0];
        }
        public async Task<ushort> ReadU16Async()
        {
            Contract.Requires(CanRead);
            byte[] buffer = await ReadU8AAsync(sizeof(ushort));
            return BinaryPrimitives.ReadUInt16BigEndian(buffer);
        }
        public async Task<short> ReadS16Async()
        {
            Contract.Requires(CanRead);
            byte[] buffer = await ReadU8AAsync(sizeof(short));
            return BinaryPrimitives.ReadInt16BigEndian(buffer);
        }
        public async Task<uint> ReadU32Async()
        {
            Contract.Requires(CanRead);
            byte[] buffer = await ReadU8AAsync(sizeof(uint));
            return BinaryPrimitives.ReadUInt32BigEndian(buffer);
        }
        public async Task<int> ReadS32Async()
        {
            Contract.Requires(CanRead);
            byte[] buffer = await ReadU8AAsync(sizeof(int));
            return BinaryPrimitives.ReadInt32BigEndian(buffer);
        }
        public async Task<ulong> ReadU64Async()
        {
            Contract.Requires(CanRead);
            byte[] buffer = await ReadU8AAsync(sizeof(ulong));
            return BinaryPrimitives.ReadUInt64BigEndian(buffer);
        }
        public async Task<long> ReadS64Async()
        {
            Contract.Requires(CanRead);
            byte[] buffer = await ReadU8AAsync(sizeof(long));
            return BinaryPrimitives.ReadInt64BigEndian(buffer);
        }
        public async Task<float> ReadF32Async()
        {
            Contract.Requires(CanRead);
            byte[] buffer = await ReadU8AAsync(sizeof(float));
            return BinaryPrimitives.ReadSingleBigEndian(buffer);
        }
        public async Task<double> ReadF64Async()
        {
            Contract.Requires(CanRead);
            byte[] buffer = await ReadU8AAsync(sizeof(double));
            return BinaryPrimitives.ReadDoubleBigEndian(buffer);
        } //TODO: add readtoend method
        public async Task<uint> ReadU32VAsync()
        {
            Contract.Requires(CanRead);
            uint data = 0;
            int position = 0;
            while (true)
            {
                byte current = await ReadU8Async();
                data |= (current & 127U) << position;
                if ((current & 128) == 0) return data;
                position += 7;
                if (position >= 32) throw new ProtocolViolationException();
            }
        }
        public async Task<int> ReadS32VAsync()
        {
            Contract.Requires(CanRead);
            return (int)await ReadU32VAsync();
        }
        public async Task<ulong> ReadU64VAsync()
        {
            Contract.Requires(CanRead);
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
        public async Task<long> ReadS64VAsync()
        {
            Contract.Requires(CanRead);
            return (long)await ReadU64VAsync();
        }
        public async Task<byte[]> ReadU8AVAsync()
        {
            Contract.Requires(CanRead);
            return await ReadU8AAsync(await ReadS32VAsync());
        }
        public async Task<sbyte[]> ReadS8AVAsync()
        {
            Contract.Requires(CanRead);
            return await ReadS8AAsync(await ReadS32VAsync());
        }
        public async Task<ushort[]> ReadU16AVAsync()
        {
            Contract.Requires(CanRead);
            return await ReadU16AAsync(await ReadS32VAsync());
        }
        public async Task<short[]> ReadS16AVAsync()
        {
            Contract.Requires(CanRead);
            return await ReadS16AAsync(await ReadS32VAsync());
        }
        public async Task<uint[]> ReadU32AVAsync()
        {
            Contract.Requires(CanRead);
            return await ReadU32AAsync(await ReadS32VAsync());
        }
        public async Task<int[]> ReadS32AVAsync()
        {
            Contract.Requires(CanRead);
            return await ReadS32AAsync(await ReadS32VAsync());
        }
        public async Task<ulong[]> ReadU64AVAsync()
        {
            Contract.Requires(CanRead);
            return await ReadU64AAsync(await ReadS32VAsync());
        }
        public async Task<long[]> ReadS64AVAsync()
        {
            Contract.Requires(CanRead);
            return await ReadS64AAsync(await ReadS32VAsync());
        }
        public async Task<bool> ReadBoolAsync()
        {
            return (await ReadU8Async()) != 0;
        }
        public async Task<Guid> ReadGuidAsync()
        {
            Contract.Requires(CanRead);
            byte[] buffer = await ReadU8AAsync(16);
            if (BitConverter.IsLittleEndian) buffer = [buffer[3], buffer[2], buffer[1], buffer[0], buffer[5], buffer[4], buffer[7], buffer[6], buffer[8], buffer[9], buffer[10], buffer[11], buffer[12], buffer[13], buffer[14], buffer[15]];
            return MemoryMarshal.Cast<byte, Guid>(buffer)[0];
        }
        public async Task<string> ReadStringS32VAsync()
        {
            Contract.Requires(CanRead);
            string data = Encoding.UTF8.GetString(await ReadU8AVAsync());
            return data;
        }
        public async Task<string> ReadStringS16Async()
        {
            Contract.Requires(CanRead);
            string data = Encoding.UTF8.GetString(await ReadU8AAsync(await ReadS16Async()));
            return data;
        }
        public void WriteU8A(byte[] data)
        {
            Contract.Requires(CanWrite);
            Stream.Write(data);
        }
        public void WriteS8A(sbyte[] data)
        {
            Contract.Requires(CanWrite);
            Stream.Write(MemoryMarshal.AsBytes(data.AsSpan()).ToArray());
        }
        public void WriteU16A(ushort[] data)
        {
            Contract.Requires(CanWrite);
            Contract.Requires(data.Length * sizeof(ushort) <= int.MaxValue);
            byte[] buffer = new byte[data.Length * sizeof(ushort)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteUInt16BigEndian(buffer.AsSpan()[(i * sizeof(ushort))..], data[i]);
            }
            Stream.Write(buffer);
        }
        public void WriteS16A(short[] data)
        {
            Contract.Requires(CanWrite);
            Contract.Requires(data.Length * sizeof(short) <= int.MaxValue);
            byte[] buffer = new byte[data.Length * sizeof(short)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteInt16BigEndian(buffer.AsSpan()[(i * sizeof(short))..], data[i]);
            }
            Stream.Write(buffer);
        }
        public void WriteU32A(uint[] data)
        {
            Contract.Requires(CanWrite);
            Contract.Requires(data.Length * sizeof(uint) <= int.MaxValue);
            byte[] buffer = new byte[data.Length * sizeof(uint)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteUInt32BigEndian(buffer.AsSpan()[(i * sizeof(uint))..], data[i]);
            }
            Stream.Write(buffer);
        }
        public void WriteS32A(int[] data)
        {
            Contract.Requires(CanWrite);
            Contract.Requires(data.Length * sizeof(int) <= int.MaxValue);
            byte[] buffer = new byte[data.Length * sizeof(int)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteInt32BigEndian(buffer.AsSpan()[(i * sizeof(int))..], data[i]);
            }
            Stream.Write(buffer);
        }
        public void WriteU64A(ulong[] data)
        {
            Contract.Requires(CanWrite);
            Contract.Requires(data.Length * sizeof(ulong) <= int.MaxValue);
            byte[] buffer = new byte[data.Length * sizeof(ulong)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteUInt64BigEndian(buffer.AsSpan()[(i * sizeof(ulong))..], data[i]);
            }
            Stream.Write(buffer);
        }
        public void WriteS64A(long[] data)
        {
            Contract.Requires(CanWrite);
            Contract.Requires(data.Length * sizeof(long) <= int.MaxValue);
            byte[] buffer = new byte[data.Length * sizeof(long)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteInt64BigEndian(buffer.AsSpan()[(i * sizeof(long))..], data[i]);
            }
            Stream.Write(buffer);
        }
        public void WriteF32A(float[] data)
        {
            Contract.Requires(CanWrite);
            Contract.Requires(data.Length * sizeof(float) <= int.MaxValue);
            byte[] buffer = new byte[data.Length * sizeof(float)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteSingleBigEndian(buffer.AsSpan()[(i * sizeof(float))..], data[i]);
            }
            Stream.Write(buffer);
        }
        public void WriteF64A(double[] data)
        {
            Contract.Requires(CanWrite);
            Contract.Requires(data.Length * sizeof(double) <= int.MaxValue);
            byte[] buffer = new byte[data.Length * sizeof(double)];
            for (int i = 0; i < data.Length; i++)
            {
                BinaryPrimitives.WriteDoubleBigEndian(buffer.AsSpan()[(i * sizeof(double))..], data[i]);
            }
            Stream.Write(buffer);
        }
        public void WriteU8(byte data)
        {
            Contract.Requires(CanWrite);
            Stream.Write([data]);
        }
        public void WriteS8(sbyte data)
        {
            Contract.Requires(CanWrite);
            Stream.Write(MemoryMarshal.AsBytes<sbyte>(new sbyte[] { data }).ToArray());
        }
        public void WriteU16(ushort data)
        {
            Contract.Requires(CanWrite);
            byte[] buffer = new byte[sizeof(ushort)];
            BinaryPrimitives.WriteUInt16BigEndian(buffer, data);
            Stream.Write(buffer);
        }
        public void WriteS16(short data)
        {
            Contract.Requires(CanWrite);
            byte[] buffer = new byte[sizeof(short)];
            BinaryPrimitives.WriteInt16BigEndian(buffer, data);
            Stream.Write(buffer);
        }
        public void WriteU32(uint data)
        {
            Contract.Requires(CanWrite);
            byte[] buffer = new byte[sizeof(uint)];
            BinaryPrimitives.WriteUInt32BigEndian(buffer, data);
            Stream.Write(buffer);
        }
        public void WriteS32(int data)
        {
            Contract.Requires(CanWrite);
            byte[] buffer = new byte[sizeof(int)];
            BinaryPrimitives.WriteInt32BigEndian(buffer, data);
            Stream.Write(buffer);
        }
        public void WriteU64(ulong data)
        {
            Contract.Requires(CanWrite);
            byte[] buffer = new byte[sizeof(ulong)];
            BinaryPrimitives.WriteUInt64BigEndian(buffer, data);
            Stream.Write(buffer);
        }
        public void WriteS64(long data)
        {
            Contract.Requires(CanWrite);
            byte[] buffer = new byte[sizeof(long)];
            BinaryPrimitives.WriteInt64BigEndian(buffer, data);
            Stream.Write(buffer);
        }
        public void WriteF32(float data)
        {
            Contract.Requires(CanWrite);
            byte[] buffer = new byte[sizeof(float)];
            BinaryPrimitives.WriteSingleBigEndian(buffer, data);
            Stream.Write(buffer);
        }
        public void WriteF64(double data)
        {
            Contract.Requires(CanWrite);
            byte[] buffer = new byte[sizeof(double)];
            BinaryPrimitives.WriteDoubleBigEndian(buffer, data);
            Stream.Write(buffer);
        }
        public int WriteU32V(uint data)
        {
            Contract.Requires(CanWrite);
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
        public int WriteS32V(int data)
        {
            Contract.Requires(CanWrite);
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
        public int WriteU64V(ulong data)
        {
            Contract.Requires(CanWrite);
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
        public int WriteS64V(long data)
        {
            Contract.Requires(CanWrite);
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
        public void WriteU8AV(byte[] buffer)
        {
            Contract.Requires(CanWrite);
            WriteS32V(buffer.Length);
            Stream.Write(buffer);
        }
        public void WriteS8AV(sbyte[] buffer)
        {
            Contract.Requires(CanWrite);
            WriteS32V(buffer.Length);
            Stream.Write(MemoryMarshal.Cast<sbyte, byte>(buffer).ToArray());
        }
        public void WriteU16AV(ushort[] buffer)
        {
            Contract.Requires(CanWrite);
            WriteS32V(buffer.Length);
            WriteU16A(buffer);
        }
        public void WriteS16AV(short[] buffer)
        {
            Contract.Requires(CanWrite);
            WriteS32V(buffer.Length);
            WriteS16A(buffer);
        }
        public void WriteU32AV(uint[] buffer)
        {
            Contract.Requires(CanWrite);
            WriteS32V(buffer.Length);
            WriteU32A(buffer);
        }
        public void WriteS32AV(int[] buffer)
        {
            Contract.Requires(CanWrite);
            WriteS32V(buffer.Length);
            WriteS32A(buffer);
        }
        public void WriteU64AV(ulong[] buffer)
        {
            Contract.Requires(CanWrite);
            WriteS32V(buffer.Length);
            WriteU64A(buffer);
        }
        public void WriteS64AV(long[] buffer)
        {
            Contract.Requires(CanWrite);
            WriteS32V(buffer.Length);
            WriteS64A(buffer);
        }
        public void WriteF32AV(float[] buffer)
        {
            Contract.Requires(CanWrite);
            WriteS32V(buffer.Length);
            WriteF32A(buffer);
        }
        public void WriteF64AV(double[] buffer)
        {
            Contract.Requires(CanWrite);
            WriteS32V(buffer.Length);
            WriteF64A(buffer);
        }
        public void WriteBool(bool data)
        {
            Contract.Requires(CanWrite);
            Stream.Write(MemoryMarshal.AsBytes<bool>(new bool[] { data }).ToArray());
        }
        public void WriteGuid(Guid data)
        {
            Contract.Requires(CanWrite);
            byte[] buffer = MemoryMarshal.AsBytes([data]).ToArray(); //TODO: replace all array initialization syntax with this
            if (!BitConverter.IsLittleEndian)
            {
                Stream.Write(buffer);
                return;
            }
            Stream.Write([buffer[3], buffer[2], buffer[1], buffer[0], buffer[5], buffer[4], buffer[7], buffer[6], buffer[8], buffer[9], buffer[10], buffer[11], buffer[12], buffer[13], buffer[14], buffer[15]]);
        }
        public void WriteStringS32V(string data)
        {
            Contract.Requires(CanWrite);
            WriteU8AV(Encoding.UTF8.GetBytes(data));
        }
        public void WriteStringS16(string data)
        {
            Contract.Requires(CanWrite);
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            WriteU16((ushort)buffer.Length);
            Stream.Write(buffer);
        }
        public byte[] ReadU8A(int size)
        {
            Contract.Requires(CanRead);
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
        public sbyte[] ReadS8A(int size)
        {
            Contract.Requires(CanRead);
            return MemoryMarshal.Cast<byte, sbyte>(ReadU8A(size)).ToArray();
        }
        public ushort[] ReadU16A(int size)
        {
            Contract.Requires(CanRead);
            Contract.Requires(size * sizeof(ushort) <= int.MaxValue);
            byte[] buffer = ReadU8A(size * sizeof(ushort));
            ushort[] data = new ushort[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadUInt16BigEndian(buffer[(i * sizeof(ushort))..]);
            }
            return data;
        }
        public short[] ReadS16A(int size)
        {
            Contract.Requires(CanRead);
            Contract.Requires(size * sizeof(short) <= int.MaxValue);
            byte[] buffer = ReadU8A(size * sizeof(short));
            short[] data = new short[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadInt16BigEndian(buffer[(i * sizeof(short))..]);
            }
            return data;
        }
        public uint[] ReadU32A(int size)
        {
            Contract.Requires(CanRead);
            Contract.Requires(size * sizeof(uint) <= int.MaxValue);
            byte[] buffer = ReadU8A(size * sizeof(uint));
            uint[] data = new uint[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadUInt32BigEndian(buffer[(i * sizeof(uint))..]);
            }
            return data;
        }
        public int[] ReadS32A(int size)
        {
            Contract.Requires(CanRead);
            Contract.Requires(size * sizeof(int) <= int.MaxValue);
            byte[] buffer = ReadU8A(size * sizeof(int));
            int[] data = new int[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadInt32BigEndian(buffer[(i * sizeof(int))..]);
            }
            return data;
        }
        public ulong[] ReadU64A(int size)
        {
            Contract.Requires(CanRead);
            Contract.Requires(size * sizeof(ulong) <= int.MaxValue);
            byte[] buffer = ReadU8A(size * sizeof(ulong));
            ulong[] data = new ulong[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadUInt64BigEndian(buffer[(i * sizeof(ulong))..]);
            }
            return data;
        }
        public long[] ReadS64A(int size)
        {
            Contract.Requires(CanRead);
            Contract.Requires(size * sizeof(long) <= int.MaxValue);
            byte[] buffer = ReadU8A(size * sizeof(long));
            long[] data = new long[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadInt64BigEndian(buffer[(i * sizeof(long))..]);
            }
            return data;
        }
        public float[] ReadF32A(int size)
        {
            Contract.Requires(CanRead);
            Contract.Requires(size * sizeof(float) <= int.MaxValue);
            byte[] buffer = ReadU8A(size * sizeof(float));
            float[] data = new float[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadSingleBigEndian(buffer[(i * sizeof(float))..]);
            }
            return data;
        }
        public double[] ReadF64A(int size)
        {
            Contract.Requires(CanRead);
            Contract.Requires(size * sizeof(double) <= int.MaxValue);
            byte[] buffer = ReadU8A(size * sizeof(double));
            double[] data = new double[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = BinaryPrimitives.ReadDoubleBigEndian(buffer[(i * sizeof(double))..]);
            }
            return data;
        }
        public byte ReadU8()
        {
            Contract.Requires(CanRead);
            return (ReadU8A(1))[0];
        }
        public sbyte ReadS8()
        {
            Contract.Requires(CanRead);
            return (ReadS8A(1))[0];
        }
        public ushort ReadU16()
        {
            Contract.Requires(CanRead);
            byte[] buffer = ReadU8A(sizeof(ushort));
            return BinaryPrimitives.ReadUInt16BigEndian(buffer);
        }
        public short ReadS16()
        {
            Contract.Requires(CanRead);
            byte[] buffer = ReadU8A(sizeof(short));
            return BinaryPrimitives.ReadInt16BigEndian(buffer);
        }
        public uint ReadU32()
        {
            Contract.Requires(CanRead);
            byte[] buffer = ReadU8A(sizeof(uint));
            return BinaryPrimitives.ReadUInt32BigEndian(buffer);
        }
        public int ReadS32()
        {
            Contract.Requires(CanRead);
            byte[] buffer = ReadU8A(sizeof(int));
            return BinaryPrimitives.ReadInt32BigEndian(buffer);
        }
        public ulong ReadU64()
        {
            Contract.Requires(CanRead);
            byte[] buffer = ReadU8A(sizeof(ulong));
            return BinaryPrimitives.ReadUInt64BigEndian(buffer);
        }
        public long ReadS64()
        {
            Contract.Requires(CanRead);
            byte[] buffer = ReadU8A(sizeof(long));
            return BinaryPrimitives.ReadInt64BigEndian(buffer);
        }
        public float ReadF32()
        {
            Contract.Requires(CanRead);
            byte[] buffer = ReadU8A(sizeof(float));
            return BinaryPrimitives.ReadSingleBigEndian(buffer);
        }
        public double ReadF64()
        {
            Contract.Requires(CanRead);
            byte[] buffer = ReadU8A(sizeof(double));
            return BinaryPrimitives.ReadDoubleBigEndian(buffer);
        } //TODO: add readtoend method
        public uint ReadU32V()
        {
            Contract.Requires(CanRead);
            uint data = 0;
            int position = 0;
            while (true)
            {
                byte current = ReadU8();
                data |= (current & 127U) << position;
                if ((current & 128) == 0) return data;
                position += 7;
                if (position >= 32) throw new ProtocolViolationException();
            }
        }
        public int ReadS32V()
        {
            Contract.Requires(CanRead);
            return (int)ReadU32V();
        }
        public ulong ReadU64V()
        {
            Contract.Requires(CanRead);
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
        public long ReadS64V()
        {
            Contract.Requires(CanRead);
            return (long)ReadU64V();
        }
        public byte[] ReadU8AV()
        {
            Contract.Requires(CanRead);
            return ReadU8A(ReadS32V());
        }
        public sbyte[] ReadS8AV()
        {
            Contract.Requires(CanRead);
            return ReadS8A(ReadS32V());
        }
        public ushort[] ReadU16AV()
        {
            Contract.Requires(CanRead);
            return ReadU16A(ReadS32V());
        }
        public short[] ReadS16AV()
        {
            Contract.Requires(CanRead);
            return ReadS16A(ReadS32V());
        }
        public uint[] ReadU32AV()
        {
            Contract.Requires(CanRead);
            return ReadU32A(ReadS32V());
        }
        public int[] ReadS32AV()
        {
            Contract.Requires(CanRead);
            return ReadS32A(ReadS32V());
        }
        public ulong[] ReadU64AV()
        {
            Contract.Requires(CanRead);
            return ReadU64A(ReadS32V());
        }
        public long[] ReadS64AV()
        {
            Contract.Requires(CanRead);
            return ReadS64A(ReadS32V());
        }
        public bool ReadBool()
        {
            return (ReadU8()) != 0;
        }
        public Guid ReadGuid()
        {
            Contract.Requires(CanRead);
            byte[] buffer = ReadU8A(16);
            if (BitConverter.IsLittleEndian) buffer = [buffer[3], buffer[2], buffer[1], buffer[0], buffer[5], buffer[4], buffer[7], buffer[6], buffer[8], buffer[9], buffer[10], buffer[11], buffer[12], buffer[13], buffer[14], buffer[15]];
            return MemoryMarshal.Cast<byte, Guid>(buffer)[0];
        }
        public string ReadStringS32V()
        {
            Contract.Requires(CanRead);
            string data = Encoding.UTF8.GetString(ReadU8AV());
            return data;
        }
        public string ReadStringS16()
        {
            Contract.Requires(CanRead);
            string data = Encoding.UTF8.GetString(ReadU8A(ReadS16()));
            return data;
        }
    }
}
