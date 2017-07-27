using System;
using System.Text;
using DreamSerialize.New;

namespace DreamSerialize.WriterHelper
{
    public class BinaryWriter
    {
        private static byte[] mBuffer = new byte[16];
        private static Encoding mEncoding = Encoding.UTF8;
        private static Encoder mEncoder = mEncoding.GetEncoder();

        #region Ref Write
        public static void Write(ref byte[] bytes, ref int offset, bool value)
        {
            Write(ref bytes, ref offset, (byte)(value ? 1 : 0));
        }

        public unsafe static void Write(ref byte[] bytes, ref int offset, char ch)
        {
            int numBytes = 0;
            fixed (byte* pBytes = mBuffer)
            {
                numBytes = mEncoder.GetBytes(&ch, 1, pBytes, 16, true);
            }
            Write(ref bytes, ref offset, (byte)numBytes);
        }



        public static void Write(ref byte[] bytes, ref int offset, double value)
        {
            var union = new DoubleUnion { Value = value };
            Write(ref bytes, ref offset, union.byte0);
            Write(ref bytes, ref offset, union.byte1);
            Write(ref bytes, ref offset, union.byte2);
            Write(ref bytes, ref offset, union.byte3);
            Write(ref bytes, ref offset, union.byte4);
            Write(ref bytes, ref offset, union.byte5);
            Write(ref bytes, ref offset, union.byte6);
            Write(ref bytes, ref offset, union.byte7);
        }

        public static void Write(ref byte[] bytes, ref int offset, decimal value)
        {
            var union = new DecimalUnion { Value = value };
            Write(ref bytes, ref offset, union.byte0);
            Write(ref bytes, ref offset, union.byte1);
            Write(ref bytes, ref offset, union.byte2);
            Write(ref bytes, ref offset, union.byte3);
            Write(ref bytes, ref offset, union.byte4);
            Write(ref bytes, ref offset, union.byte5);
            Write(ref bytes, ref offset, union.byte6);
            Write(ref bytes, ref offset, union.byte7);
            Write(ref bytes, ref offset, union.byte8);
            Write(ref bytes, ref offset, union.byte9);
            Write(ref bytes, ref offset, union.byte10);
            Write(ref bytes, ref offset, union.byte11);
            Write(ref bytes, ref offset, union.byte12);
            Write(ref bytes, ref offset, union.byte13);
            Write(ref bytes, ref offset, union.byte14);
            Write(ref bytes, ref offset, union.byte15);
        }

        public static void Write(ref byte[] bytes, ref int offset, short value)
        {
            var union = new ShortUnion { Value = value };
            Write(ref bytes, ref offset, union.byte0);
            Write(ref bytes, ref offset, union.byte1);
        }

        public static void Write(ref byte[] bytes, ref int offset, ushort value)
        {
            var union = new UShortUnion { Value = value };
            Write(ref bytes, ref offset, union.byte0);
            Write(ref bytes, ref offset, union.byte1);
        }


        public static void Write(ref byte[] bytes, ref int offset, byte value)
        {
            int length = bytes.Length;
            if (length <= offset)
            {
                Array.Resize(ref bytes, (int)(offset * 2f));
            }
            bytes[offset++] = value;
        }

        public static void Write(ref byte[] bytes, ref int offset, int value)
        {
            var cc = new IntUnion { Value = value };
            /*            Write(ref bytes,ref offset, cc.byte0);
                        Write(ref bytes,ref offset, cc.byte1);
                        Write(ref bytes,ref offset, cc.byte2);
                        Write(ref bytes,ref offset, cc.byte3);*/
        }

        public static void Write(ref byte[] bytes, ref int offset, uint value)
        {
            var cc = new UIntUnion { Value = value };
            Write(ref bytes, ref offset, cc.byte0);
            Write(ref bytes, ref offset, cc.byte1);
            Write(ref bytes, ref offset, cc.byte2);
            Write(ref bytes, ref offset, cc.byte3);
        }

        public static void Write(ref byte[] bytes, ref int offset, long value)
        {
            var union = new LongUnion { Value = value };
            Write(ref bytes, ref offset, union.byte0);
            Write(ref bytes, ref offset, union.byte1);
            Write(ref bytes, ref offset, union.byte2);
            Write(ref bytes, ref offset, union.byte3);
            Write(ref bytes, ref offset, union.byte4);
            Write(ref bytes, ref offset, union.byte5);
            Write(ref bytes, ref offset, union.byte6);
            Write(ref bytes, ref offset, union.byte7);
        }

        public static void Write(ref byte[] bytes, ref int offset, ulong value)
        {
            var union = new ULongUnion { Value = value };
            Write(ref bytes, ref offset, union.byte0);
            Write(ref bytes, ref offset, union.byte1);
            Write(ref bytes, ref offset, union.byte2);
            Write(ref bytes, ref offset, union.byte3);
            Write(ref bytes, ref offset, union.byte4);
            Write(ref bytes, ref offset, union.byte5);
            Write(ref bytes, ref offset, union.byte6);
            Write(ref bytes, ref offset, union.byte7);
        }

        public static void Write(ref byte[] bytes, ref int offset, float value)
        {
            var cc = new FloatUnion { Value = value };
            Write(ref bytes, ref offset, cc.byte0);
            Write(ref bytes, ref offset, cc.byte1);
            Write(ref bytes, ref offset, cc.byte2);
            Write(ref bytes, ref offset, cc.byte3);
        }

        public static void Write(ref byte[] bytes, ref int offset, string value)
        {
            var stringByte = Encoding.UTF8.GetBytes(value);
            Write(ref bytes, ref offset, stringByte.Length);
            Write(ref bytes, ref offset, stringByte);
        }
        #endregion

        #region BitStream
        public static void Write(BitStream stream, bool value)
        {
            Write(stream, (byte)(value ? 1 : 0));
        }

        public unsafe static void Write(BitStream stream, char ch)
        {
            int numBytes = 0;
            fixed (byte* pBytes = mBuffer)
            {
                numBytes = mEncoder.GetBytes(&ch, 1, pBytes, 16, true);
            }
            Write(stream, (byte)numBytes);
        }



        public static void Write(BitStream stream, double value)
        {
            var union = new DoubleUnion { Value = value };
            Write(stream, union.byte0);
            Write(stream, union.byte1);
            Write(stream, union.byte2);
            Write(stream, union.byte3);
            Write(stream, union.byte4);
            Write(stream, union.byte5);
            Write(stream, union.byte6);
            Write(stream, union.byte7);
        }

        public static void Write(BitStream stream, float value)
        {
            var cc = new FloatUnion { Value = value };
            Write(stream, cc.byte0);
            Write(stream, cc.byte1);
            Write(stream, cc.byte2);
            Write(stream, cc.byte3);
        }

        public static void Write(BitStream stream, decimal value)
        {
            var union = new DecimalUnion { Value = value };
            Write(stream, union.byte0);
            Write(stream, union.byte1);
            Write(stream, union.byte2);
            Write(stream, union.byte3);
            Write(stream, union.byte4);
            Write(stream, union.byte5);
            Write(stream, union.byte6);
            Write(stream, union.byte7);
            Write(stream, union.byte8);
            Write(stream, union.byte9);
            Write(stream, union.byte10);
            Write(stream, union.byte11);
            Write(stream, union.byte12);
            Write(stream, union.byte13);
            Write(stream, union.byte14);
            Write(stream, union.byte15);
        }

        public static void Write(BitStream stream, byte value)
        {
            int length = stream.Bytes.Length;
            if (length <= stream.Offset)
            {
                Array.Resize(ref stream.Bytes, (int)(stream.Offset * 2f));
            }
            stream.Bytes[stream.Offset++] = value;
        }

        public static void Write(BitStream stream, ulong value)
        {
            if (value < 0x80)
            {
                Write(stream, (byte)value);
                return;
            }
            while (value > 127)
            {
                Write(stream, (byte)((value & 0x7F) | 0x80));
                value >>= 7;
            }
            Write(stream, (byte)value);
        }

        public static void Write(BitStream stream, long value)
        {
            var union = new LongUnion { Value = value };
            Write(stream, union.byte0);
            Write(stream, union.byte1);
            Write(stream, union.byte2);
            Write(stream, union.byte3);
            Write(stream, union.byte4);
            Write(stream, union.byte5);
            Write(stream, union.byte6);
            Write(stream, union.byte7);
        }

        public static void Write(BitStream stream, int value)
        {
            Write(stream, (uint)value);
        }

        public static void Write(BitStream stream, uint value)
        {
            if (value < 0x80)
            {
                Write(stream, (byte)value);
                return;
            }
            while (value > 127)
            {
                Write(stream, (byte)((value & 0x7F) | 0x80));
                value >>= 7;
            }
            Write(stream, (byte)value);
        }

        public static void Write(BitStream stream, string value)
        {
            if (value == null)
            {
                Write(stream, 0);
                return;
            }
            int length = mEncoding.GetByteCount(value);
            Write(stream, length);
            if (length == value.Length)
            {
                for (int i = 0; i < length; i++)
                {
                    Write(stream, (byte)value[i]);
                }
            }
            else
            {
                byte[] bytes = mEncoding.GetBytes(value);
                Write(stream, bytes);
            }
        }

        #endregion

        private static void Write(BitStream stream, byte[] value)
        {
            var length = value.Length;
            //Write(stream, length);
            for (int i = 0; i < length; i++)
                Write(stream, value[i]);
        }
        private static void Write(ref byte[] bytes, ref int offset, byte[] value)
        {
            var length = value.Length;
            //Write(ref bytes, ref offset, length);
            for (int i = 0; i < length; i++)
                Write(ref bytes, ref offset, value[i]);
        }
    }
}
