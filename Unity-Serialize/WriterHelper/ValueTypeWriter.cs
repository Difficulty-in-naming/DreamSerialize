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
