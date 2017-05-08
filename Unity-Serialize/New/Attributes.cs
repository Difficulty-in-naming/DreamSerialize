using System;

namespace DreamSerialize.New
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Struct)]
    public class SerializerContract : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class Index : Attribute
    {
        public int index;
        public bool SupportPrivate = false;
        public Index(int i)
        {
            index = i;
        }

        public Index(int i, bool supportPrivate)
        {
            index = i;
            SupportPrivate = supportPrivate;
        }
    }
}
