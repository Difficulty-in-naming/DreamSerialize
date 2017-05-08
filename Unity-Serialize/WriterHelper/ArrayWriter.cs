using DreamSerialize.New;

namespace DreamSerialize.WriterHelper
{
    public class ClassArrayWriter<T> : SupportSerializable<T[]> where T : new()
    {
        private ClassWriter<T> mSerializer;
        public ClassArrayWriter()
        {
            mSerializer = (ClassWriter<T>)ClassWriter<T>.Default ?? new ClassWriter<T>();
        }

        public override BitStream Serialize(BitStream stream, T[] value)
        {
            int length = value.Length;
            BinaryWriter.Write(stream, length);
            for (int i = 0; i < length; i++)
            {
                mSerializer.Serialize(stream, value[i]);
            }
            return stream;
        }

        public override T[] Deserialize(BitStream stream)
        {
            int length = BinaryReader.ReadInt32(stream);
            T[] array = new T[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = mSerializer.Deserialize(stream);
            }
            return array;
        }
    }

    public class Int32ArrayWrite : SupportSerializable<int[]>
    {
        public override BitStream Serialize(BitStream stream, int[] value)
        {
            int length = value.Length;
            BinaryWriter.Write(stream, length);
            for (int i = 0; i < length; i++)
            {
                BinaryWriter.Write(stream, value[i]);
            }
            return stream;
        }

        public override int[] Deserialize(BitStream stream)
        {
            int length = BinaryReader.ReadInt32(stream);
            int[] array = new int[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = BinaryReader.ReadInt32(stream);
            }
            return array;
        }
    }

    public class UInt32ArrayWrite : SupportSerializable<uint[]>
    {
        public override BitStream Serialize(BitStream stream, uint[] value)
        {
            int length = value.Length;
            BinaryWriter.Write(stream, length);
            for (int i = 0; i < length; i++)
            {
                BinaryWriter.Write(stream, value[i]);
            }
            return stream;
        }

        public override uint[] Deserialize(BitStream stream)
        {
            int length = BinaryReader.ReadInt32(stream);
            uint[] array = new uint[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = BinaryReader.ReadUInt32(stream);
            }
            return array;
        }
    }

    public class UInt16ArrayWrite : SupportSerializable<ushort[]>
    {
        public override BitStream Serialize(BitStream stream, ushort[] value)
        {
            int length = value.Length;
            BinaryWriter.Write(stream, length);
            for (int i = 0; i < length; i++)
            {
                BinaryWriter.Write(stream, value[i]);
            }
            return stream;
        }

        public override ushort[] Deserialize(BitStream stream)
        {
            int length = BinaryReader.ReadInt32(stream);
            ushort[] array = new ushort[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = BinaryReader.ReadUInt16(stream);
            }
            return array;
        }
    }

    public class Int16ArrayWrite : SupportSerializable<short[]>
    {
        public override BitStream Serialize(BitStream stream, short[] value)
        {
            int length = value.Length;
            BinaryWriter.Write(stream, length);
            for (int i = 0; i < length; i++)
            {
                BinaryWriter.Write(stream, value[i]);
            }
            return stream;
        }

        public override short[] Deserialize(BitStream stream)
        {
            int length = BinaryReader.ReadInt32(stream);
            short[] array = new short[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = BinaryReader.ReadInt16(stream);
            }
            return array;
        }
    }

    public class Int64ArrayWrite : SupportSerializable<long[]>
    {
        public override BitStream Serialize(BitStream stream, long[] value)
        {
            int length = value.Length;
            BinaryWriter.Write(stream, length);
            for (int i = 0; i < length; i++)
            {
                BinaryWriter.Write(stream, value[i]);
            }
            return stream;
        }

        public override long[] Deserialize(BitStream stream)
        {
            int length = BinaryReader.ReadInt32(stream);
            long[] array = new long[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = BinaryReader.ReadInt64(stream);
            }
            return array;
        }
    }

    public class UInt64ArrayWrite : SupportSerializable<ulong[]>
    {
        public override BitStream Serialize(BitStream stream, ulong[] value)
        {
            int length = value.Length;
            BinaryWriter.Write(stream, length);
            for (int i = 0; i < length; i++)
            {
                BinaryWriter.Write(stream, value[i]);
            }
            return stream;
        }

        public override ulong[] Deserialize(BitStream stream)
        {
            int length = BinaryReader.ReadInt32(stream);
            ulong[] array = new ulong[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = BinaryReader.ReadUInt64(stream);
            }
            return array;
        }
    }

    public class SingleArrayWrite : SupportSerializable<float[]>
    {
        public override BitStream Serialize(BitStream stream, float[] value)
        {
            int length = value.Length;
            BinaryWriter.Write(stream, length);
            for (int i = 0; i < length; i++)
            {
                BinaryWriter.Write(stream, value[i]);
            }
            return stream;
        }

        public override float[] Deserialize(BitStream stream)
        {
            int length = BinaryReader.ReadInt32(stream);
            float[] array = new float[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = BinaryReader.ReadSingle(stream);
            }
            return array;
        }
    }

    public class DoubleArrayWrite : SupportSerializable<double[]>
    {
        public override BitStream Serialize(BitStream stream, double[] value)
        {
            int length = value.Length;
            BinaryWriter.Write(stream, length);
            for (int i = 0; i < length; i++)
            {
                BinaryWriter.Write(stream, value[i]);
            }
            return stream;
        }

        public override double[] Deserialize(BitStream stream)
        {
            int length = BinaryReader.ReadInt32(stream);
            double[] array = new double[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = BinaryReader.ReadDouble(stream);
            }
            return array;
        }
    }

    public class DecimalArrayWrite : SupportSerializable<decimal[]>
    {
        public override BitStream Serialize(BitStream stream, decimal[] value)
        {
            int length = value.Length;
            BinaryWriter.Write(stream, length);
            for (int i = 0; i < length; i++)
            {
                BinaryWriter.Write(stream, value[i]);
            }
            return stream;
        }

        public override decimal[] Deserialize(BitStream stream)
        {
            int length = BinaryReader.ReadInt32(stream);
            decimal[] array = new decimal[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = BinaryReader.ReadDecimal(stream);
            }
            return array;
        }
    }

    public class StringArrayWrite : SupportSerializable<string[]>
    {
        public override BitStream Serialize(BitStream stream, string[] value)
        {
            int length = value.Length;
            BinaryWriter.Write(stream, length);
            for (int i = 0; i < length; i++)
            {
                BinaryWriter.Write(stream, value[i]);
            }
            return stream;
        }

        public override string[] Deserialize(BitStream stream)
        {
            int length = BinaryReader.ReadInt32(stream);
            string[] array = new string[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = BinaryReader.ReadString(stream);
            }
            return array;
        }
    }
}
