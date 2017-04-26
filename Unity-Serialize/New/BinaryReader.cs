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

        private static byte[] ReadByte(byte[] bytes,ref int offset,int count)
        {
            byte[] newBytes = new byte[count];
            Array.Copy(bytes,offset,newBytes,0,count);
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
            var value = (decimal) BitConverter.ToDouble(bytes, offset);
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

        #region Ref Array

        public static bool[] ReadBooleanArray(byte[] bytes, ref int offset)
        {
            int count = ReadInt32(bytes, ref offset);
            bool[] valueArray = new bool[count];
            for (int i = 0; i < count; i++)
                valueArray[i] = ReadBoolean(bytes, ref offset);
            return valueArray;
        }

        public static byte[] ReadByteArray(byte[] bytes, ref int offset)
        {
            int count = ReadInt32(bytes, ref offset);
            byte[] valueArray = new byte[count];
            for (int i = 0; i < count; i++)
                valueArray[i] = ReadByte(bytes, ref offset);
            return valueArray;
        }

        public static sbyte[] ReadSByteArray(byte[] bytes, ref int offset)
        {
            int count = ReadInt32(bytes, ref offset);
            sbyte[] valueArray = new sbyte[count];
            for (int i = 0; i < count; i++)
                valueArray[i] = ReadSByte(bytes, ref offset);
            return valueArray;
        }

        public static ushort[] ReadUInt16Array(byte[] bytes, ref int offset)
        {
            int count = ReadInt32(bytes, ref offset);
            ushort[] valueArray = new ushort[count];
            for (int i = 0; i < count; i++)
                valueArray[i] = ReadUInt16(bytes, ref offset);
            return valueArray;
        }

        public static short[] ReadInt16Array(byte[] bytes, ref int offset)
        {
            int count = ReadInt32(bytes, ref offset);
            short[] valueArray = new short[count];
            for (int i = 0; i < count; i++)
                valueArray[i] = ReadInt16(bytes, ref offset);
            return valueArray;
        }

        public static uint[] ReadUInt32Array(byte[] bytes, ref int offset)
        {
            int count = ReadInt32(bytes, ref offset);
            uint[] valueArray = new uint[count];
            for (int i = 0; i < count; i++)
                valueArray[i] = ReadUInt32(bytes, ref offset);
            return valueArray;
        }

        public static int[] ReadInt32Array(byte[] bytes, ref int offset)
        {
            int count = ReadInt32(bytes, ref offset);
            int[] valueArray = new int[count];
            for (int i = 0; i < count; i++)
                valueArray[i] = ReadInt32(bytes, ref offset);
            return valueArray;
        }

        public static ulong[] ReadUInt64Array(byte[] bytes, ref int offset)
        {
            int count = ReadInt32(bytes, ref offset);
            ulong[] valueArray = new ulong[count];
            for (int i = 0; i < count; i++)
                valueArray[i] = ReadUInt64(bytes, ref offset);
            return valueArray;
        }

        public static long[] ReadInt64Array(byte[] bytes, ref int offset)
        {
            int count = ReadInt32(bytes, ref offset);
            long[] valueArray = new long[count];
            for (int i = 0; i < count; i++)
                valueArray[i] = ReadInt64(bytes, ref offset);
            return valueArray;
        }

        public static float[] ReadSingleArray(byte[] bytes, ref int offset)
        {
            int count = ReadInt32(bytes, ref offset);
            float[] valueArray = new float[count];
            for (int i = 0; i < count; i++)
                valueArray[i] = ReadSingle(bytes, ref offset);
            return valueArray;
        }

        public static double[] ReadDoubleArray(byte[] bytes, ref int offset)
        {
            int count = ReadInt32(bytes, ref offset);
            double[] valueArray = new double[count];
            for (int i = 0; i < count; i++)
                valueArray[i] = ReadDouble(bytes, ref offset);
            return valueArray;
        }

        public static decimal[] ReadDecimalArray(byte[] bytes, ref int offset)
        {
            int count = ReadInt32(bytes, ref offset);
            decimal[] valueArray = new decimal[count];
            for (int i = 0; i < count; i++)
                valueArray[i] = ReadDecimal(bytes, ref offset);
            return valueArray;
        }
        public static char[] ReadCharArray(byte[] bytes, ref int offset)
        {
            int count = ReadInt32(bytes, ref offset);
            char[] valueArray = new char[count];
            for (int i = 0; i < count; i++)
                valueArray[i] = ReadChar(bytes, ref offset);
            return valueArray;
        }

        public static string[] ReadStringArray(byte[] bytes, ref int offset)
        {
            int count = ReadInt32(bytes, ref offset);
            string[] valueArray = new string[count];
            for (int i = 0; i < count; i++)
                valueArray[i] = ReadString(bytes, ref offset);
            return valueArray;
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
            fixed (byte* ptr = stream.Bytes)
            {
                int a =  *(int*)(ptr + stream.Offset);
                stream.Offset += 4;
                return a;
            }
        }

        public static uint ReadUInt32(BitStream stream)
        {
            return (uint)(stream.Bytes[stream.Offset++] | stream.Bytes[stream.Offset++] << 8 | stream.Bytes[stream.Offset++] << 16 | stream.Bytes[stream.Offset++] << 24);
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
            uint lo = (uint)(stream.Bytes[stream.Offset++] | stream.Bytes[stream.Offset++] << 8 |
                             stream.Bytes[stream.Offset++] << 16 | stream.Bytes[stream.Offset++] << 24);
            uint hi = (uint)(stream.Bytes[stream.Offset++] | stream.Bytes[stream.Offset++] << 8 |
                             stream.Bytes[stream.Offset++] << 16 | stream.Bytes[stream.Offset++] << 24);
            return (ulong)hi << 32 | lo;
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

        #region BitStream Array

        public static bool[] ReadBooleanArray(BitStream stream)
        {
            int count = ReadInt32(stream);
            bool[] valueArray = new bool[count];
            for (int i = 0; i < count; i++)
                valueArray[i] = ReadBoolean(stream);
            return valueArray;
        }

        public static byte[] ReadByteArray(BitStream stream)
        {
            int count = ReadInt32(stream);
            byte[] valueArray = new byte[count];
            for (int i = 0; i < count; i++)
                valueArray[i] = ReadByte(stream);
            return valueArray;
        }

        public static sbyte[] ReadSByteArray(BitStream stream)
        {
            int count = ReadInt32(stream);
            sbyte[] valueArray = new sbyte[count];
            for (int i = 0; i < count; i++)
                valueArray[i] = ReadSByte(stream);
            return valueArray;
        }

        public static ushort[] ReadUInt16Array(BitStream stream)
        {
            int count = ReadInt32(stream);
            ushort[] valueArray = new ushort[count];
            for (int i = 0; i < count; i++)
                valueArray[i] = ReadUInt16(stream);
            return valueArray;
        }

        public static short[] ReadInt16Array(BitStream stream)
        {
            int count = ReadInt32(stream);
            short[] valueArray = new short[count];
            for (int i = 0; i < count; i++)
                valueArray[i] = ReadInt16(stream);
            return valueArray;
        }

        public static uint[] ReadUInt32Array(BitStream stream)
        {
            int count = ReadInt32(stream);
            uint[] valueArray = new uint[count];
            for (int i = 0; i < count; i++)
                valueArray[i] = ReadUInt32(stream);
            return valueArray;
        }

        public static int[] ReadInt32Array(BitStream stream)
        {
            int count = ReadInt32(stream);
            int[] valueArray = new int[count];
            for (int i = 0; i < count; i++)
                valueArray[i] = ReadInt32(stream);
            return valueArray;
        }

        public static ulong[] ReadUInt64Array(BitStream stream)
        {
            int count = ReadInt32(stream);
            ulong[] valueArray = new ulong[count];
            for (int i = 0; i < count; i++)
                valueArray[i] = ReadUInt64(stream);
            return valueArray;
        }

        public static long[] ReadInt64Array(BitStream stream)
        {
            int count = ReadInt32(stream);
            long[] valueArray = new long[count];
            for (int i = 0; i < count; i++)
                valueArray[i] = ReadInt64(stream);
            return valueArray;
        }

        public static float[] ReadSingleArray(BitStream stream)
        {
            int count = ReadInt32(stream);
            float[] valueArray = new float[count];
            for (int i = 0; i < count; i++)
                valueArray[i] = ReadSingle(stream);
            return valueArray;
        }

        public static double[] ReadDoubleArray(BitStream stream)
        {
            int count = ReadInt32(stream);
            double[] valueArray = new double[count];
            for (int i = 0; i < count; i++)
                valueArray[i] = ReadDouble(stream);
            return valueArray;
        }

        public static decimal[] ReadDecimalArray(BitStream stream)
        {
            int count = ReadInt32(stream);
            decimal[] valueArray = new decimal[count];
            for (int i = 0; i < count; i++)
                valueArray[i] = ReadDecimal(stream);
            return valueArray;
        }
        public static char[] ReadCharArray(BitStream stream)
        {
            int count = ReadInt32(stream);
            char[] valueArray = new char[count];
            for (int i = 0; i < count; i++)
                valueArray[i] = ReadChar(stream);
            return valueArray;
        }

        public static string[] ReadStringArray(BitStream stream)
        {
            int count = ReadInt32(stream);
            string[] valueArray = new string[count];
            for (int i = 0; i < count; i++)
                valueArray[i] = ReadString(stream);
            return valueArray;
        }
        #endregion
    }
}
