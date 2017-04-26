using System;
using System.Security.Cryptography;
using DreamSerialize.WriterHelper;

namespace DreamSerialize.New
{
    public static class DreamSerializer
    {
        public static byte[] Serializer<T>(ISerializable<T> obj)
        {
            byte[] bytes = new byte[50];
            int length = 0;
            obj.Serialize(ref bytes, ref length);
            return bytes;
        }
/*

        public static T Deserializer<T>(byte[] bytes) where T : ISerializable<T>,new()
        {
            int length = 0;
            var obj = new T();
            return obj.Deserialize(bytes, ref length);
        }
*/
        public static byte[] Serializer<T>(T obj)
        {
/*            if (obj is ISerializable<T>)
                return Serializer((ISerializable<T>)obj);*/
            var bit = new BitStream(new byte[64], 0);
            DynamicWriter<T>.Write(bit,obj);
            Array.Resize(ref bit.Bytes,bit.Offset);
            return bit.Bytes;
        }

        public static T Deserializer<T>(byte[] bytes) where T : new()
        {
            var bit = new BitStream(bytes,0);
            return DynamicWriter<T>.Read(bit);
        }
    }
}
