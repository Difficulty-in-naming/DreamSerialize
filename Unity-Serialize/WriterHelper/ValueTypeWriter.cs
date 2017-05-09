using DreamSerialize.New;
using BitStream = DreamSerialize.New.BitStream;

namespace DreamSerialize.WriterHelper
{

    public class WriteForByte : SupportSerializable<byte>
    {
        public override BitStream Serialize(BitStream stream, byte value)
        {
            BinaryWriter.Write(stream, (uint)value);
            return stream;
        }

        public override byte Deserialize(BitStream stream)
        {
            return (byte)BinaryReader.ReadUInt32(stream);
        }
    }
    public class WriteForSByte : SupportSerializable<sbyte>
    {
        public override BitStream Serialize(BitStream stream, sbyte value)
        {
            BinaryWriter.Write(stream, value);
            return stream;
        }

        public override sbyte Deserialize(BitStream stream)
        {
            return (sbyte)BinaryReader.ReadUInt32(stream);
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
            BinaryWriter.Write(stream, (int)value);
            return stream;
        }

        public override short Deserialize(BitStream stream)
        {
            return (short)BinaryReader.ReadInt32(stream);
        }
    }

    public class WriteForUInt16 : SupportSerializable<ushort>
    {
        public override BitStream Serialize(BitStream stream, ushort value)
        {
            BinaryWriter.Write(stream, (uint)value);
            return stream;
        }

        public override ushort Deserialize(BitStream stream)
        {
            return (ushort)BinaryReader.ReadUInt32(stream);
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

#if Unity
    internal class WriteForRect : SupportSerializable<UnityEngine.Rect>
    {
        public override BitStream Serialize(BitStream stream, UnityEngine.Rect value)
        {
            BinaryWriter.Write(stream, value.x);
            BinaryWriter.Write(stream, value.y);
            BinaryWriter.Write(stream, value.width);
            BinaryWriter.Write(stream, value.height);
            return stream;
        }

        public override UnityEngine.Rect Deserialize(BitStream stream)
        {
            var x = BinaryReader.ReadSingle(stream);
            var y = BinaryReader.ReadSingle(stream);
            var width = BinaryReader.ReadSingle(stream);
            var height = BinaryReader.ReadSingle(stream);
            return new UnityEngine.Rect(x,y,width,height);
        }
    }
    internal class WriteForVector2 : SupportSerializable<UnityEngine.Vector2>
    {
        public override BitStream Serialize(BitStream stream, UnityEngine.Vector2 value)
        {
            BinaryWriter.Write(stream, value.x);
            BinaryWriter.Write(stream, value.y);
            return stream;
        }

        public override UnityEngine.Vector2 Deserialize(BitStream stream)
        {
            var x = BinaryReader.ReadSingle(stream);
            var y = BinaryReader.ReadSingle(stream);
            return new UnityEngine.Vector2(x, y);
        }
    }
    internal class WriteForVector3 : SupportSerializable<UnityEngine.Vector3>
    {
        public override BitStream Serialize(BitStream stream, UnityEngine.Vector3 value)
        {
            BinaryWriter.Write(stream, value.x);
            BinaryWriter.Write(stream, value.y);
            BinaryWriter.Write(stream, value.z);
            return stream;
        }

        public override UnityEngine.Vector3 Deserialize(BitStream stream)
        {
            var x = BinaryReader.ReadSingle(stream);
            var y = BinaryReader.ReadSingle(stream);
            var z = BinaryReader.ReadSingle(stream);
            return new UnityEngine.Vector3(x, y, z);
        }
    }
    internal class WriteForVector4 : SupportSerializable<UnityEngine.Vector4>
    {
        public override BitStream Serialize(BitStream stream, UnityEngine.Vector4 value)
        {
            BinaryWriter.Write(stream, value.x);
            BinaryWriter.Write(stream, value.y);
            BinaryWriter.Write(stream, value.w);
            BinaryWriter.Write(stream, value.z);
            return stream;
        }

        public override UnityEngine.Vector4 Deserialize(BitStream stream)
        {
            var x = BinaryReader.ReadSingle(stream);
            var y = BinaryReader.ReadSingle(stream);
            var w = BinaryReader.ReadSingle(stream);
            var z = BinaryReader.ReadSingle(stream);
            return new UnityEngine.Vector4(x, y, w, z);
        }
    }
#endif
}
