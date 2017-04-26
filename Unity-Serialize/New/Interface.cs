
namespace DreamSerialize.New
{
    public interface ISerializable<T>
    {
        int Serialize(ref byte[] bytes,ref int offset);
        T Deserialize(byte[] bytes,ref int offset);
    }

    public abstract class SupportSerializable<T>
    {

        public static SupportSerializable<T> Default;
        public SupportSerializable()
        {
            Default = this;
        }

        //public virtual int Serialize(ref byte[] bytes, ref int offset, T value);
        //public abstract T Deserialize(byte[] bytes, ref int offset);

        public abstract BitStream Serialize(BitStream stream, T value);
        public abstract T Deserialize(BitStream stream);
    }
}
