using System;
using System.Collections.Generic;
using System.Reflection;
using DreamSerialize.New;
using DreamSerialize.Utility;
using Theraot.Core.System.Linq.Expressions;
using Theraot.Core.Theraot.Core;
using Expression = Theraot.Core.System.Linq.Expressions.Expression;

//using BlockExpression = DreamSerialize.Utility.BlockExpression;
//using Expression = DreamSerialize.Utility.Expression;

namespace DreamSerialize.WriterHelper
{
    public class DynamicWriter<T>
    {
        public static void Write(BitStream stream, T value)
        {
            if (value == null)
                return;
            SupportSerializable<T> serializer = GeneratorWriter();
            serializer.Serialize(stream,value);
        }
        public static T Read(BitStream stream)
        {
            SupportSerializable<T> serializer = GeneratorWriter();
            return serializer.Deserialize(stream);
        }
        public static SupportSerializable<T> GeneratorWriter()
        {
            return SupportSerializable<T>.Default ?? (SupportSerializable<T>)GeneratorWriter(typeof(T));
        }

        private static object GeneratorWriter(Type t)
        {
            if (t.IsValueType)
            {
                if (t == typeof(int))
                    return new WriteForInt32();
                else if(t == typeof(byte))
                    return new WriteForByte();
            }
            else if (t == typeof(string))
                return new WriteForString();
            else if (t.IsGenericType)
            {
                var genericT = t.GetGenericTypeDefinition();
                if (genericT == typeof(Dictionary<,>))
                {
                    var obj = typeof(DictWriter<,>).MakeGenericType(t.GenericTypeArguments());
                    return Activator.CreateInstance(obj);
                }
                else if (genericT == typeof(List<>))
                {
                    var obj = typeof(ListWriter<>).MakeGenericType(t.GenericTypeArguments());
                    return Activator.CreateInstance(obj);
                }
                else if (genericT == typeof(KeyValuePair<,>))
                {
                    
                }
            }
            else if (t.IsClass)
            {
                if (!t.IsArray)
                {
                    var obj = typeof(ClassWriter<>).MakeGenericType(t);
                    return Activator.CreateInstance(obj);
                }
                else
                {
                    var obj = typeof(ClassArrayWriter<>).MakeGenericType(t.GetElementType());
                    return Activator.CreateInstance(obj);
                }
            }
            return null;
        }
    }

    public interface ClassWriteInterface
    {
        int Length { get; set; }
    }

    public class ClassWriter<T> : SupportSerializable<T>, ClassWriteInterface where T : new()
    {
        internal FieldData[] SerializerList;
        internal FieldData[] DeserializeArray;

        internal class FieldData
        {
            public string Name;
            public int Index;
            public Type Type;
            public Func<BitStream, T,BitStream> Serialize;
            public Action<BitStream, T> Deserialize;
        }
        public int Length { get; set; }
        private List<FieldData> datas;
        private Type t;
        public ClassWriter()
        {
            t = typeof(T);
            var fields = t.GetFields();
            var prop = t.GetProperties();
            datas = SortFields(fields, prop);
            Length = datas.Count;
            SerializerList = new FieldData[Length];
            var writeHead = typeof(WriteForInt32).GetMethod("Serialize",new[] {typeof(BitStream), typeof(int)});
            if (WriteForInt32.Default == null)
                new WriteForInt32();
            //序列化
            for (int i = 0; i < Length; i++)
            {
                var data = datas[i];
                var type = data.Type;
                var serType = typeof(SupportSerializable<>).MakeGenericType(type);
                var serDefault = serType.GetField("Default");
                var getSer = Check(serDefault, type);
                var method = serType.GetMethod("Serialize", new[] { typeof(BitStream), type });
                var arg0 = Expression.Parameter(typeof(BitStream), "data");
                var arg1 = Expression.Parameter(t, "T");
                var arg2 = Expression.Constant(data.Index,typeof(int));
                var fp = Expression.PropertyOrField(arg1, data.Name);
                var pGetter = Expression.Constant(getSer, getSer.GetType());
                //var pGetter = Expression.Field(null, serDefault);
                var iGetter = Expression.Constant(WriteForInt32.Default, typeof(WriteForInt32));
                var call = Expression.Call(pGetter, method, arg0, fp);
                var writeIndex = Expression.Call(iGetter,writeHead, arg0, arg2);
                Func<BitStream, T,BitStream> ser = null;
                var classWriterInterface = getSer as ClassWriteInterface;
                if (classWriterInterface != null)
                {
                    if (classWriterInterface.Length == 0)
                    {
                        ser = Expression.Lambda<Func<BitStream, T, BitStream>>(arg0, arg0, arg1).Compile();
                    }
                    else
                    {
                        var check = Expression.NotEqual(fp, Expression.Constant(null, typeof(object)));
                        BlockExpression block = Expression.Block(writeIndex, call);
                        var refCall = Expression.Condition(check, block, arg0);
                        ser = Expression.Lambda<Func<BitStream, T, BitStream>>(refCall, arg0, arg1).Compile();
                    }
                }
                else
                {
                    var block = Expression.Block(writeIndex,call);
                    ser = Expression.Lambda<Func<BitStream, T,BitStream>>(block, arg0, arg1).Compile();
                }
                data.Serialize = ser;
                SerializerList[i] = data;
            }
        }

        private static List<FieldData> SortFields(FieldInfo[] fields, PropertyInfo[] props)
        {
            var fieldData = new List<FieldData>();
            FieldInfo fieldInfo;
            PropertyInfo propInfo;
            Index indexAttr;
            for (int index = 0; index < fields.Length; index++)
            {
                fieldInfo = fields[index];
                var attr = fieldInfo.GetAttributes<Index>(false);
                if (attr == null || attr.Length == 0)
                    continue;
                indexAttr = attr[0];
                fieldData.Add(new FieldData { Name = fieldInfo.Name, Index = indexAttr.index, Type = fieldInfo.FieldType });
            }
            for (int index = 0; index < props.Length; index++)
            {
                propInfo = props[index];
                var attr = propInfo.GetAttributes<Index>(false);
                if (attr == null || attr.Length == 0)
                    continue;
                indexAttr = attr[0];
                fieldData.Add(new FieldData { Name = propInfo.Name, Index = indexAttr.index, Type = propInfo.PropertyType });
            }
            fieldData.Sort((t1, t2) => t1.Index - t2.Index);
            return fieldData;
        }

        private static object Check(FieldInfo field, Type type)
        {
            var obj = field.GetValue(null);
            if (obj == null)
            {
                var generate = typeof(DynamicWriter<>).MakeGenericType(type).GetMethod("GeneratorWriter");
                return generate.Invoke(null, null);
            }
            return obj;
        }

        public override BitStream Serialize(BitStream stream, T value)
        {
            for (int i = 0; i < Length; i++)
            {
                SerializerList[i].Serialize(stream, value);
            }
            WriteForInt32.Default.Serialize(stream,-1);
            return stream;
        }

        public override T Deserialize(BitStream stream)
        {
            var obj = new T();
            int maxLength = stream.Bytes.Length;
            int i = 0;
            if (DeserializeArray == null)
            {
                DeserializeArray = new FieldData[Length];
                //反序列化
                while (stream.Offset < maxLength)
                {
                    int index = BinaryReader.ReadInt32(stream);
                    if (index == -1)
                        return obj;
                    for (int node = 0; node < Length; node++)
                    {
                        var data = datas[node];
                        if (data.Index == index)
                        {
                            var type = data.Type;
                            BinaryExpression assign;
                            Action<BitStream, T> ser;
                            var arg0 = Expression.Parameter(typeof(BitStream), "data");
                            var arg1 = Expression.Parameter(t, "T");
                            var fp = Expression.PropertyOrField(arg1, data.Name);
                            var serType = typeof(SupportSerializable<>).MakeGenericType(type);
                            var serDefault = serType.GetField("Default");
                            Check(serDefault, type);
                            var method = serType.GetMethod("Deserialize", new[] { typeof(BitStream) });
                            var pGetter = Expression.Field(null, serDefault);
                            assign = Expression.Assign(fp, Expression.Call(pGetter, method, arg0));
                            ser = Expression.Lambda<Action<BitStream, T>>(assign, arg0, arg1).Compile();
                            ser(stream, obj);
                            data.Deserialize = ser;
                            DeserializeArray[i++] = data;
                            break;
                        }
                    }
                }
                return obj;
            }
            while (i < Length)
            {
                BinaryReader.ReadInt32(stream);
                DeserializeArray[i++].Deserialize(stream, obj);
            }
            return obj;
        }
    }
}
