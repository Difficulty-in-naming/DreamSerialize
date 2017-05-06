using DreamSerialize.New;

namespace DreamSerialize.WriterHelper
{

    public class WriteForByte : SupportSerializable<byte>
    {
        public override BitStream Serialize(BitStream stream, byte value)
        {
            BinaryWriter.Write(stream, value);
            return stream;
        }

        public override byte Deserialize(BitStream stream)
        {
            return BinaryReader.ReadByte(stream);
        }
    }

    public class WriteForSingle : SupportSerializable<float>
    {
        public override BitStream Serialize(BitStream stream, float value)
        {
            BinaryWriter.Write(stream, value);
            return stream;
        }

        public override float Deserialize(BitStream stream)
        {
            return BinaryReader.ReadSingle(stream);
        }
    }

    public class WriteForDouble : SupportSerializable<double>
    {
        public override BitStream Serialize(BitStream stream, double value)
        {
            BinaryWriter.Write(stream, value);
            return stream;
        }

        public override double Deserialize(BitStream stream)
        {
            return BinaryReader.ReadDouble(stream);
        }
    }

    public class WriteForDecimal : SupportSerializable<decimal>
    {
        public override BitStream Serialize(BitStream stream, decimal value)
        {
            BinaryWriter.Write(stream, value);
            return stream;
        }

        public override decimal Deserialize(BitStream stream)
        {
            return BinaryReader.ReadDecimal(stream);
        }
    }

    public class WriteForInt64 : SupportSerializable<long>
    {
        public override BitStream Serialize(BitStream stream, long value)
        {
            BinaryWriter.Write(stream, value);
            return stream;
        }

        public override long Deserialize(BitStream stream)
        {
            return BinaryReader.ReadInt64(stream);
        }
    }

    public class WriteForUInt64 : SupportSerializable<ulong>
    {
        public override BitStream Serialize(BitStream stream, ulong value)
        {
            BinaryWriter.Write(stream, value);
            return stream;
        }

        public override ulong Deserialize(BitStream stream)
        {
            return BinaryReader.ReadUInt64(stream);
        }
    }

    public class WriteForInt32 : SupportSerializable<int>
    {
        public override BitStream Serialize(BitStream stream, int value)
        {
            BinaryWriter.Write(stream, value);
            return stream;
        }

        public override int Deserialize(BitStream stream)
        {
            return BinaryReader.ReadInt32(stream);
        }
    }

    public class WriteForUInt32 : SupportSerializable<uint>
    {
        public override BitStream Serialize(BitStream stream, uint value)
        {
            BinaryWriter.Write(stream, value);
            return stream;
        }

        public override uint Deserialize(BitStream stream)
        {
            return BinaryReader.ReadUInt32(stream);
        }
    }

    public class WriteForInt16 : SupportSerializable<short>
    {
        public override BitStream Serialize(BitStream stream, short value)
        {
            BinaryWriter.Write(stream, value);
            return stream;
        }

        public override short Deserialize(BitStream stream)
        {
            return BinaryReader.ReadInt16(stream);
        }
    }

    public class WriteForUInt16 : SupportSerializable<ushort>
    {
        public override BitStream Serialize(BitStream stream, ushort value)
        {
            BinaryWriter.Write(stream, value);
            return stream;
        }

        public override ushort Deserialize(BitStream stream)
        {
            return BinaryReader.ReadUInt16(stream);
        }
    }

    internal class WriteForString : SupportSerializable<string>
    {
        public override BitStream Serialize(BitStream stream, string value)
        {
            BinaryWriter.Write(stream, value);
            return stream;
        }

        public override string Deserialize(BitStream stream)
        {
            return BinaryReader.ReadString(stream);

        }
    }
}
