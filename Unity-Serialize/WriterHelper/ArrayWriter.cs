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
            BinaryWriter.Write(stream,length);
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
}
