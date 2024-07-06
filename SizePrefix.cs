using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Net.Myzuc.ShioLib
{
    public sealed class SizePrefix
    {
        public static readonly SizePrefix S32V = new(async (Stream stream, int limit) =>
        {
            int length = await stream.ReadS32VAsync();
            if (length > limit) throw new ProtocolViolationException();
            return length;
        }, (Stream stream, int limit) =>
        {
            int length = stream.ReadS32V();
            if (length > limit) throw new ProtocolViolationException();
            return length;
        }, async (Stream stream, int limit, int length) =>
        {
            if (length > limit) throw new ProtocolViolationException();
            await stream.WriteS32VAsync((short)length);
        }, (Stream stream, int limit, int length) =>
        {
            if (length > limit) throw new ProtocolViolationException();
            stream.WriteS32V((short)length);
        });
        public static readonly SizePrefix S16V = new(async (Stream stream, int limit) =>
        {
            int length = await stream.ReadS16VAsync();
            if (length > limit) throw new ProtocolViolationException();
            return length;
        }, (Stream stream, int limit) =>
        {
            int length = stream.ReadS16V();
            if (length > limit) throw new ProtocolViolationException();
            return length;
        }, async (Stream stream, int limit, int length) =>
        {
            if (length > short.MaxValue) throw new ArgumentOutOfRangeException();
            if (length > limit) throw new ProtocolViolationException();
            await stream.WriteS16VAsync((short)length);
        }, (Stream stream, int limit, int length) =>
        {
            if (length > short.MaxValue) throw new ArgumentOutOfRangeException();
            if (length > limit) throw new ProtocolViolationException();
            stream.WriteS16V((short)length);
        });
        public static readonly SizePrefix S8V = new(async (Stream stream, int limit) =>
        {
            int length = await stream.ReadS8VAsync();
            if (length > limit) throw new ProtocolViolationException();
            return length;
        }, (Stream stream, int limit) =>
        {
            int length = stream.ReadS8V();
            if (length > limit) throw new ProtocolViolationException();
            return length;
        }, async (Stream stream, int limit, int length) =>
        {
            if (length > sbyte.MaxValue) throw new ArgumentOutOfRangeException();
            if (length > limit) throw new ProtocolViolationException();
            await stream.WriteS8VAsync((sbyte)length);
        }, (Stream stream, int limit, int length) =>
        {
            if (length > sbyte.MaxValue) throw new ArgumentOutOfRangeException();
            if (length > limit) throw new ProtocolViolationException();
            stream.WriteS8V((sbyte)length);
        });
        public static readonly SizePrefix U16V = new(async (Stream stream, int limit) =>
        {
            int length = await stream.ReadU16VAsync();
            if (length > limit) throw new ProtocolViolationException();
            return length;
        }, (Stream stream, int limit) =>
        {
            int length = stream.ReadU16V();
            if (length > limit) throw new ProtocolViolationException();
            return length;
        }, async (Stream stream, int limit, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            if (length > limit) throw new ProtocolViolationException();
            await stream.WriteU16VAsync((ushort)length);
        }, (Stream stream, int limit, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            if (length > limit) throw new ProtocolViolationException();
            stream.WriteU16V((ushort)length);
        });
        public static readonly SizePrefix U8V = new(async (Stream stream, int limit) =>
        {
            int length = await stream.ReadU8VAsync();
            if (length > limit) throw new ProtocolViolationException();
            return length;
        }, (Stream stream, int limit) =>
        {
            int length = stream.ReadU8V();
            if (length > limit) throw new ProtocolViolationException();
            return length;
        }, async (Stream stream, int limit, int length) =>
        {
            if (length > byte.MaxValue) throw new ArgumentOutOfRangeException();
            if (length > limit) throw new ProtocolViolationException();
            await stream.WriteU8VAsync((byte)length);
        }, (Stream stream, int limit, int length) =>
        {
            if (length > byte.MaxValue) throw new ArgumentOutOfRangeException();
            if (length > limit) throw new ProtocolViolationException();
            stream.WriteU8V((byte)length);
        });
        public static readonly SizePrefix S32 = new(async (Stream stream, int limit) =>
        {
            int length = await stream.ReadS32Async();
            if (length > limit) throw new ProtocolViolationException();
            return length;
        }, (Stream stream, int limit) =>
        {
            int length = stream.ReadS32();
            if (length > limit) throw new ProtocolViolationException();
            return length;
        }, async (Stream stream, int limit, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            if (length > limit) throw new ProtocolViolationException();
            await stream.WriteS32Async((short)length);
        }, (Stream stream, int limit, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            if (length > limit) throw new ProtocolViolationException();
            stream.WriteS32((short)length);
        });
        public static readonly SizePrefix S16 = new(async (Stream stream, int limit) =>
        {
            int length = await stream.ReadS16Async();
            if (length > limit) throw new ProtocolViolationException();
            return length;
        }, (Stream stream, int limit) =>
        {
            int length = stream.ReadS16();
            if (length > limit) throw new ProtocolViolationException();
            return length;
        }, async (Stream stream, int limit, int length) =>
        {
            if (length > short.MaxValue) throw new ArgumentOutOfRangeException();
            if (length > limit) throw new ProtocolViolationException();
            await stream.WriteS16Async((short)length);
        }, (Stream stream, int limit, int length) =>
        {
            if (length > short.MaxValue) throw new ArgumentOutOfRangeException();
            if (length > limit) throw new ProtocolViolationException();
            stream.WriteS16((short)length);
        });
        public static readonly SizePrefix S8 = new(async (Stream stream, int limit) =>
        {
            int length = await stream.ReadS8Async();
            if (length > limit) throw new ProtocolViolationException();
            return length;
        }, (Stream stream, int limit) =>
        {
            int length = stream.ReadS8();
            if (length > limit) throw new ProtocolViolationException();
            return length;
        }, async (Stream stream, int limit, int length) =>
        {
            if (length > sbyte.MaxValue) throw new ArgumentOutOfRangeException();
            if (length > limit) throw new ProtocolViolationException();
            await stream.WriteS8Async((sbyte)length);
        }, (Stream stream, int limit, int length) =>
        {
            if (length > sbyte.MaxValue) throw new ArgumentOutOfRangeException();
            if (length > limit) throw new ProtocolViolationException();
            stream.WriteS8((sbyte)length);
        });        
        public static readonly SizePrefix U16 = new(async (Stream stream, int limit) =>
        {
            int length = await stream.ReadU16Async();
            if (length > limit) throw new ProtocolViolationException();
            return length;
        }, (Stream stream, int limit) =>
        {
            int length = stream.ReadU16();
            if (length > limit) throw new ProtocolViolationException();
            return length;
        }, async (Stream stream, int limit, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            if (length > limit) throw new ProtocolViolationException();
            await stream.WriteU16Async((ushort)length);
        }, (Stream stream, int limit, int length) =>
        {
            if (length > ushort.MaxValue) throw new ArgumentOutOfRangeException();
            if (length > limit) throw new ProtocolViolationException();
            stream.WriteU16((ushort)length);
        });
        public static readonly SizePrefix U8 = new(async (Stream stream, int limit) =>
        {
            int length = await stream.ReadU8Async();
            if (length > limit) throw new ProtocolViolationException();
            return length;
        }, (Stream stream, int limit) =>
        {
            int length = stream.ReadU8();
            if (length > limit) throw new ProtocolViolationException();
            return length;
        }, async (Stream stream, int limit, int length) =>
        {
            if (length > byte.MaxValue) throw new ArgumentOutOfRangeException();
            if (length > limit) throw new ProtocolViolationException();
            await stream.WriteU8Async((byte)length);
        }, (Stream stream, int limit, int length) =>
        {
            if (length > byte.MaxValue) throw new ArgumentOutOfRangeException();
            if (length > limit) throw new ProtocolViolationException();
            stream.WriteU8((byte)length);
        });
        public readonly Func<Stream, int, Task<int>> ReadAsync;
        public readonly Func<Stream, int, int> ReadSync;
        public readonly Func<Stream, int, int, Task> WriteAsync;
        public readonly Action<Stream, int, int> WriteSync;
        public SizePrefix(Func<Stream, int, Task<int>> readAsync, Func<Stream, int, int> readSync, Func<Stream, int, int, Task> writeAsync, Action<Stream, int, int> writeSync)
        {
            ReadAsync = readAsync;
            ReadSync = readSync;
            WriteAsync = writeAsync;
            WriteSync = writeSync;
        }
    }
}
