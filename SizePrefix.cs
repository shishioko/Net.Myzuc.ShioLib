using System;
using System.IO;
using System.Threading.Tasks;

namespace Net.Myzuc.UtilLib
{
    public sealed class SizePrefix
    {
        public static readonly SizePrefix S32V = new((Stream stream) => stream.ReadS32VAsync(), (Stream stream) => stream.ReadS32V(), (Stream stream, int length) => stream.WriteS32VAsync(length), (Stream stream, int length) => stream.WriteS32V(length));
        public static readonly SizePrefix S16V = new(async (Stream stream) =>
        {
            return await stream.ReadS16VAsync();
        }, (Stream stream) =>
        {
            return stream.ReadS16V();
        }, async (Stream stream, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            await stream.WriteS16VAsync((short)length);
        }, (Stream stream, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            stream.WriteS16V((short)length);
        });
        public static readonly SizePrefix S8V = new(async (Stream stream) =>
        {
            return await stream.ReadS8VAsync();
        }, (Stream stream) =>
        {
            return stream.ReadS8V();
        }, async (Stream stream, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            await stream.WriteS8VAsync((sbyte)length);
        }, (Stream stream, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            stream.WriteS8V((sbyte)length);
        });
        public static readonly SizePrefix U16V = new(async (Stream stream) =>
        {
            return await stream.ReadU16VAsync();
        }, (Stream stream) =>
        {
            return stream.ReadU16V();
        }, async (Stream stream, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            await stream.WriteU16VAsync((ushort)length);
        }, (Stream stream, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            stream.WriteU16V((ushort)length);
        });
        public static readonly SizePrefix U8V = new(async (Stream stream) =>
        {
            return await stream.ReadU8VAsync();
        }, (Stream stream) =>
        {
            return stream.ReadU8V();
        }, async (Stream stream, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            await stream.WriteU8VAsync((byte)length);
        }, (Stream stream, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            stream.WriteU8V((byte)length);
        });
        public static readonly SizePrefix S32 = new((Stream stream) => stream.ReadS32Async(), (Stream stream) => stream.ReadS32(), (Stream stream, int length) => stream.WriteS32Async(length), (Stream stream, int length) => stream.WriteS32(length));
        public static readonly SizePrefix S16 = new(async (Stream stream) =>
        {
            return await stream.ReadS16Async();
        }, (Stream stream) =>
        {
            return stream.ReadS16();
        }, async (Stream stream, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            await stream.WriteS16Async((short)length);
        }, (Stream stream, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            stream.WriteS16((short)length);
        });
        public static readonly SizePrefix S8 = new(async (Stream stream) =>
        {
            return await stream.ReadS8Async();
        }, (Stream stream) =>
        {
            return stream.ReadS8();
        }, async (Stream stream, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            await stream.WriteS8Async((sbyte)length);
        }, (Stream stream, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            stream.WriteS8((sbyte)length);
        });        public static readonly SizePrefix U16 = new(async (Stream stream) =>
        {
            return await stream.ReadU16Async();
        }, (Stream stream) =>
        {
            return stream.ReadU16();
        }, async (Stream stream, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            await stream.WriteU16Async((ushort)length);
        }, (Stream stream, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            stream.WriteU16((ushort)length);
        });
        public static readonly SizePrefix U8 = new(async (Stream stream) =>
        {
            return await stream.ReadU8Async();
        }, (Stream stream) =>
        {
            return stream.ReadU8();
        }, async (Stream stream, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            await stream.WriteU8Async((byte)length);
        }, (Stream stream, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            stream.WriteU8((byte)length);
        });
        public readonly Func<Stream, Task<int>> ReadAsync;
        public readonly Func<Stream, int> ReadSync;
        public readonly Func<Stream, int, Task> WriteAsync;
        public readonly Action<Stream, int> WriteSync;
        public SizePrefix(Func<Stream, Task<int>> readAsync, Func<Stream, int> readSync, Func<Stream, int, Task> writeAsync, Action<Stream, int> writeSync)
        {
            ReadAsync = readAsync;
            ReadSync = readSync;
            WriteAsync = writeAsync;
            WriteSync = writeSync;
        }
    }
}
