using System;
using System.Text;

namespace DreamSerialize.New
{
    public class BinaryReader
    {
        #region Ref Reader
        public static bool ReadBoolean(byte[] bytes, ref int offset)
        {
            return bytes[offset++] != 0;
        }

        public static byte ReadByte(byte[] bytes, ref int offset)
        {
            return bytes[offset++];
        }

        public static sbyte ReadSByte(byte[] bytes, ref int offset)
        {
            return (sbyte)bytes[offset++];
        }

        public static char ReadChar(byte[] bytes, ref int offset)
        {
            return (char)bytes[offset++];
        }

        public static short ReadInt16(byte[] bytes, ref int offset)
        {
            return (short)(bytes[offset++] | bytes[offset++] << 8);
        }

        public static ushort ReadUInt16(byte[] bytes, ref int offset)
        {
            return (ushort)(bytes[offset++] | bytes[offset++] << 8);
        }

        private static byte[] ReadByte(byte[] bytes, ref int offset, int count)
        {
            byte[] newBytes = new byte[count];
            Array.Copy(bytes, offset, newBytes, 0, count);
            offset += count;
            return newBytes;
        }


        public static int ReadInt32(byte[] bytes, ref int offset)
        {
            return bytes[offset++] | bytes[offset++] << 8 | bytes[offset++] << 16 | bytes[offset++] << 24;
        }

        public static uint ReadUInt32(byte[] bytes, ref int offset)
        {
            return (uint)(bytes[offset++] | bytes[offset++] << 8 | bytes[offset++] << 16 | bytes[offset++] << 24);
        }

        public static long ReadInt64(byte[] bytes, ref int offset)
        {
            uint lo = (uint)(bytes[offset++] | bytes[offset++] << 8 |
                             bytes[offset++] << 16 | bytes[offset++] << 24);
            uint hi = (uint)(bytes[offset++] | bytes[offset++] << 8 |
                             bytes[offset++] << 16 | bytes[offset++] << 24);
            return (long)hi << 32 | lo;
        }

        public static ulong ReadUInt64(byte[] bytes, ref int offset)
        {
            uint lo = (uint)(bytes[offset++] | bytes[offset++] << 8 |
                             bytes[offset++] << 16 | bytes[offset++] << 24);
            uint hi = (uint)(bytes[offset++] | bytes[offset++] << 8 |
                             bytes[offset++] << 16 | bytes[offset++] << 24);
            return (ulong)hi << 32 | lo;
        }

        public static unsafe float ReadSingle(byte[] bytes, ref int offset)
        {
            uint tmpBuffer = (uint)(bytes[offset++] | bytes[offset++] << 8 | bytes[offset++] << 16 | bytes[offset++] << 24);
            return *(float*)&tmpBuffer;
        }

        public static unsafe double ReadDouble(byte[] bytes, ref int offset)
        {
            uint lo = (uint)(bytes[offset++] | bytes[offset++] << 8 |
                             bytes[offset++] << 16 | bytes[offset++] << 24);
            uint hi = (uint)(bytes[offset++] | bytes[offset++] << 8 |
                             bytes[offset++] << 16 | bytes[offset++] << 24);
            ulong tmpBuffer = (ulong)hi << 32 | lo;
            return *((double*)&tmpBuffer);
        }

        public static decimal ReadDecimal(byte[] bytes, ref int offset)
        {
            var value = (decimal)BitConverter.ToDouble(bytes, offset);
            offset += 16;
            return value;
        }

        public static string ReadString(byte[] bytes, ref int offset)
        {
            var count = ReadInt32(bytes, ref offset);
            var str = Encoding.UTF8.GetString(bytes, offset, count);
            offset += count;
            return str;
        }

        public static T ReadT<T>(byte[] bytes, ref int offset)
        {
            return default(T);
        }
        #endregion

        #region BitStream
        public static bool ReadBoolean(BitStream stream)
        {
            return stream.Bytes[stream.Offset++] != 0;
        }

        public static byte ReadByte(BitStream stream)
        {
            return stream.Bytes[stream.Offset++];
        }

        public static sbyte ReadSByte(BitStream stream)
        {
            return (sbyte)stream.Bytes[stream.Offset++];
        }

        public static char ReadChar(BitStream stream)
        {
            return (char)stream.Bytes[stream.Offset++];
        }

        public static short ReadInt16(BitStream stream)
        {
            return (short)(stream.Bytes[stream.Offset++] | stream.Bytes[stream.Offset++] << 8);
        }

        public static ushort ReadUInt16(BitStream stream)
        {
            return (ushort)(stream.Bytes[stream.Offset++] | stream.Bytes[stream.Offset++] << 8);
        }

        private static byte[] ReadByte(BitStream stream, int count)
        {
            byte[] newBytes = new byte[count];
            Array.Copy(stream.Bytes, stream.Offset, newBytes, 0, count);
            stream.Offset += count;
            return newBytes;
        }


        public static unsafe int ReadInt32(BitStream stream)
        {
            return (int)ReadUInt32(stream);
        }

        public static uint ReadUInt32(BitStream stream)
        {
            uint value = 0;
            for (int i = 0; ; i += 7)
            {
                int v = stream.Bytes[stream.Offset++];
                value |= (uint)(v & 0x7F) << i;
                if ((v & 0x80) == 0)
                    return value;
            }
        }

        public static long ReadInt64(BitStream stream)
        {
            uint lo = (uint)(stream.Bytes[stream.Offset++] | stream.Bytes[stream.Offset++] << 8 |
                             stream.Bytes[stream.Offset++] << 16 | stream.Bytes[stream.Offset++] << 24);
            uint hi = (uint)(stream.Bytes[stream.Offset++] | stream.Bytes[stream.Offset++] << 8 |
                             stream.Bytes[stream.Offset++] << 16 | stream.Bytes[stream.Offset++] << 24);
            return (long)hi << 32 | lo;
        }

        public static ulong ReadUInt64(BitStream stream)
        {
            ulong value = 0;
            for (int i = 0; ; i += 7)
            {
                int v = stream.Bytes[stream.Offset++];
                value |= (ulong)((v & 0x7F) << i);
                if ((v & 0x80) == 0)
                    return value;
            }
        }

        public static unsafe float ReadSingle(BitStream stream)
        {
            uint tmpBuffer = (uint)(stream.Bytes[stream.Offset++] | stream.Bytes[stream.Offset++] << 8 | stream.Bytes[stream.Offset++] << 16 | stream.Bytes[stream.Offset++] << 24);
            return *(float*)&tmpBuffer;
        }

        public static unsafe double ReadDouble(BitStream stream)
        {
            uint lo = (uint)(stream.Bytes[stream.Offset++] | stream.Bytes[stream.Offset++] << 8 |
                             stream.Bytes[stream.Offset++] << 16 | stream.Bytes[stream.Offset++] << 24);
            uint hi = (uint)(stream.Bytes[stream.Offset++] | stream.Bytes[stream.Offset++] << 8 |
                             stream.Bytes[stream.Offset++] << 16 | stream.Bytes[stream.Offset++] << 24);
            ulong tmpBuffer = (ulong)hi << 32 | lo;
            return *((double*)&tmpBuffer);
        }

        public static decimal ReadDecimal(BitStream stream)
        {
            var value = (decimal)BitConverter.ToDouble(stream.Bytes, stream.Offset);
            stream.Offset += 16;
            return value;
        }

        public static string ReadString(BitStream stream)
        {
            var count = ReadInt32(stream);
            if (count == 0)
                return "";
            else if (count == -1)
                return null;
            var str = Encoding.UTF8.GetString(stream.Bytes, stream.Offset, count);
            stream.Offset += count;
            return str;
        }

        public static T ReadT<T>(BitStream stream)
        {
            return default(T);
        }
        #endregion
    }
}
