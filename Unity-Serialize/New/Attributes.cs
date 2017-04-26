using System;

namespace DreamSerialize.New
{
    internal static class ReaderType
    {
        public const byte Dict = 0;
        public const byte List = 1;
    }
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Struct)]
    public class SerializerContract : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class Index : Attribute
    {
        public int index;

        public Index(int i)
        {
            index = i;
        }
    }
}
