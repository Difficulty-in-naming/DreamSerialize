using System;
using System.Security.Cryptography;
using DreamSerialize.WriterHelper;

namespace DreamSerialize.New
{
    public static class DreamSerializer
    {
        public static byte[] Serializer<T>(T obj)
        {
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
