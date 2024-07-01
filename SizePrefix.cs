using System;
using System.IO;
using System.Threading.Tasks;

namespace Net.Myzuc.UtilLib
{
    public sealed class SizePrefix
    {
        public static readonly SizePrefix S32V = new((DataStream<Stream> stream) => stream.ReadS32VAsync(), (DataStream<Stream> stream) => stream.ReadS32V(), (DataStream<Stream> stream, int length) => stream.WriteS32VAsync(length), (DataStream<Stream> stream, int length) => stream.WriteS32V(length));
        public static readonly SizePrefix S16V = new(async (DataStream<Stream> stream) =>
        {
            return await stream.ReadS16VAsync();
        }, (DataStream<Stream> stream) =>
        {
            return stream.ReadS16V();
        }, async (DataStream<Stream> stream, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            await stream.WriteS16VAsync((short)length);
        }, (DataStream<Stream> stream, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            stream.WriteS16V((short)length);
        });
        public static readonly SizePrefix S8V = new(async (DataStream<Stream> stream) =>
        {
            return await stream.ReadS8VAsync();
        }, (DataStream<Stream> stream) =>
        {
            return stream.ReadS8V();
        }, async (DataStream<Stream> stream, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            await stream.WriteS8VAsync((sbyte)length);
        }, (DataStream<Stream> stream, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            stream.WriteS8V((sbyte)length);
        });
        public static readonly SizePrefix U16V = new(async (DataStream<Stream> stream) =>
        {
            return await stream.ReadU16VAsync();
        }, (DataStream<Stream> stream) =>
        {
            return stream.ReadU16V();
        }, async (DataStream<Stream> stream, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            await stream.WriteU16VAsync((ushort)length);
        }, (DataStream<Stream> stream, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            stream.WriteU16V((ushort)length);
        });
        public static readonly SizePrefix U8V = new(async (DataStream<Stream> stream) =>
        {
            return await stream.ReadU8VAsync();
        }, (DataStream<Stream> stream) =>
        {
            return stream.ReadU8V();
        }, async (DataStream<Stream> stream, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            await stream.WriteU8VAsync((byte)length);
        }, (DataStream<Stream> stream, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            stream.WriteU8V((byte)length);
        });
        public static readonly SizePrefix S32 = new((DataStream<Stream> stream) => stream.ReadS32Async(), (DataStream<Stream> stream) => stream.ReadS32(), (DataStream<Stream> stream, int length) => stream.WriteS32Async(length), (DataStream<Stream> stream, int length) => stream.WriteS32(length));
        public static readonly SizePrefix S16 = new(async (DataStream<Stream> stream) =>
        {
            return await stream.ReadS16Async();
        }, (DataStream<Stream> stream) =>
        {
            return stream.ReadS16();
        }, async (DataStream<Stream> stream, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            await stream.WriteS16Async((short)length);
        }, (DataStream<Stream> stream, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            stream.WriteS16((short)length);
        });
        public static readonly SizePrefix S8 = new(async (DataStream<Stream> stream) =>
        {
            return await stream.ReadS8Async();
        }, (DataStream<Stream> stream) =>
        {
            return stream.ReadS8();
        }, async (DataStream<Stream> stream, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            await stream.WriteS8Async((sbyte)length);
        }, (DataStream<Stream> stream, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            stream.WriteS8((sbyte)length);
        });        public static readonly SizePrefix U16 = new(async (DataStream<Stream> stream) =>
        {
            return await stream.ReadU16Async();
        }, (DataStream<Stream> stream) =>
        {
            return stream.ReadU16();
        }, async (DataStream<Stream> stream, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            await stream.WriteU16Async((ushort)length);
        }, (DataStream<Stream> stream, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            stream.WriteU16((ushort)length);
        });
        public static readonly SizePrefix U8 = new(async (DataStream<Stream> stream) =>
        {
            return await stream.ReadU8Async();
        }, (DataStream<Stream> stream) =>
        {
            return stream.ReadU8();
        }, async (DataStream<Stream> stream, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            await stream.WriteU8Async((byte)length);
        }, (DataStream<Stream> stream, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            stream.WriteU8((byte)length);
        });
        public readonly Func<DataStream<Stream>, Task<int>> ReadAsync;
        public readonly Func<DataStream<Stream>, int> ReadSync;
        public readonly Func<DataStream<Stream>, int, Task> WriteAsync;
        public readonly Action<DataStream<Stream>, int> WriteSync;
        public SizePrefix(Func<DataStream<Stream>, Task<int>> readAsync, Func<DataStream<Stream>, int> readSync, Func<DataStream<Stream>, int, Task> writeAsync, Action<DataStream<Stream>, int> writeSync)
        {
            ReadAsync = readAsync;
            ReadSync = readSync;
            WriteAsync = writeAsync;
            WriteSync = writeSync;
        }
    }
}
