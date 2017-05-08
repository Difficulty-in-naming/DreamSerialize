using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DreamSerialize.New;

namespace DreamSerialize.WriterHelper
{
    public class WriteForType : SupportSerializable<Type>
    {
        public override BitStream Serialize(BitStream stream, Type value)
        {
            var assembly = value.Assembly.FullName;
            BinaryWriter.Write(stream,value.FullName + "," + assembly.Substring(0,assembly.IndexOf(",")));
            return stream;
        }

        public override Type Deserialize(BitStream stream)
        {
            return Type.GetType(BinaryReader.ReadString(stream));
        }
    }
}
