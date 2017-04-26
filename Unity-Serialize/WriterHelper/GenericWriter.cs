using System.Collections.Generic;
using DreamSerialize.New;

namespace DreamSerialize.WriterHelper
{
    internal class DictWriter<TKey,TValue> : SupportSerializable<Dictionary<TKey, TValue>>
    {
        public SupportSerializable<TKey> SerializeKey;
        public SupportSerializable<TValue> SerializeValue;

        public DictWriter()
        {
            SerializeKey = DynamicWriter<TKey>.GeneratorWriter();
            SerializeValue = DynamicWriter<TValue>.GeneratorWriter();
        }

        public override BitStream Serialize(BitStream stream, Dictionary<TKey, TValue> value)
        {
            BinaryWriter.Write(stream, value.Count);
            foreach (var node in value)
            {
                SerializeKey.Serialize(stream, node.Key);
                SerializeValue.Serialize(stream, node.Value);
            }
            return stream;
        }

        public override Dictionary<TKey,TValue> Deserialize(BitStream stream)
        {
            var dict = new Dictionary<TKey, TValue>();
            int length = BinaryReader.ReadInt32(stream);
            TKey key;
            TValue value;
            for (int i = 0; i < length; i++)
            {
                key = SerializeKey.Deserialize(stream);
                value = SerializeValue.Deserialize(stream);
                dict.Add(key, value);
            }
            return dict;
        }
    }

    internal class ListWriter<TValue> : SupportSerializable<List<TValue>>
    {

        public SupportSerializable<TValue> SerializeValue;

        public ListWriter()
        {
            SerializeValue = DynamicWriter<TValue>.GeneratorWriter();
        }

        public override BitStream Serialize(BitStream stream, List<TValue> value)
        {
            int count = value.Count;
            BinaryWriter.Write(stream, count);
            for (int index = 0; index < count; index++)
            {
                SerializeValue.Serialize(stream, value[index]);
            }
            return stream;
        }

        public override List<TValue> Deserialize(BitStream stream)
        {
            var list = new List<TValue>();
            int length = BinaryReader.ReadInt32(stream);
            TValue value;
            for (int i = 0; i < length; i++)
            {
                value = SerializeValue.Deserialize(stream);
                list.Add(value);
            }
            return list;
        }
    }
}
