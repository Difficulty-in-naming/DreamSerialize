using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using DreamSerialize.New;
using DreamSerialize.Utility;
using Theraot.Core.System.Linq.Expressions;
using Theraot.Core.Theraot.Core;
using Expression = Theraot.Core.System.Linq.Expressions.Expression;


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
        internal Func<BitStream, T, BitStream> SerializerList;
        internal Action<BitStream, int,T> DeserializeArray;

        internal class FieldData
        {
            public string Name;
            public int Index;
            public Type Type;
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
            var writeHead = typeof(WriteForInt32).GetMethod("Serialize",new[] {typeof(BitStream), typeof(int)});
            if (WriteForInt32.Default == null)
                new WriteForInt32();

            //序列化
            {
                List<Expression> expression = new List<Expression>();
                var arg0 = Expression.Parameter(typeof(BitStream), "data");
                var arg1 = Expression.Parameter(t, "T");
                Expression finalCall = null;
                for (int i = 0; i < Length; i++)
                {
                    var data = datas[i];
                    var type = data.Type;
                    var serType = typeof(SupportSerializable<>).MakeGenericType(type);
                    var serDefault = serType.GetField("Default");
                    var getSer = Check(serDefault, type);
                    var method = serType.GetMethod("Serialize", new[] { typeof(BitStream), type });
                    var arg2 = Expression.Constant(data.Index, typeof(int));
                    var fp = Expression.PropertyOrField(arg1, data.Name);
                    var pGetter = Expression.Constant(getSer, getSer.GetType());
                    var iGetter = Expression.Constant(WriteForInt32.Default, typeof(WriteForInt32));
                    var call = Expression.Call(pGetter, method, arg0, fp);
                    var writeIndex = Expression.Call(iGetter, writeHead, arg0, arg2);
                    var classWriterInterface = getSer as ClassWriteInterface;
                    if (classWriterInterface != null)
                    {
                        if (classWriterInterface.Length != 0)
                        {
                            var isNull = Expression.NotEqual(fp, Expression.Constant(null, typeof(object)));
                            BlockExpression haveValue = Expression.Block(writeIndex, call);
                            var notValue = Expression.Call(iGetter, writeHead, arg0, Expression.Constant(0, typeof(int)));
                            finalCall = Expression.Condition(isNull, haveValue, notValue);
                        }
                    }
                    else
                    {
                        finalCall = Expression.Block(writeIndex, call);
                    }
                    expression.Add(finalCall);
                }
                var block = Expression.Block(expression);
                SerializerList = Expression.Lambda<Func<BitStream, T, BitStream>>(block, arg0, arg1).Compile();
            }

            //反序列化
            {
                var arg0 = Expression.Parameter(typeof(BitStream), "data");
                var arg1 = Expression.Parameter(typeof(int), "index");
                var arg2 = Expression.Parameter(t, "T");
                List<SwitchCase> sw = new List<SwitchCase>();
                for (int node = 0; node < Length; node++)
                {
                    var data = datas[node];
                    var type = data.Type;

                    var fp = Expression.PropertyOrField(arg2, data.Name);
                    var serType = typeof(SupportSerializable<>).MakeGenericType(type);
                    var serDefault = serType.GetField("Default");
                    Check(serDefault, type);
                    var method = serType.GetMethod("Deserialize", new[] { typeof(BitStream) });
                    var pGetter = Expression.Field(null, serDefault);
                    var assign = Expression.Assign(fp, Expression.Call(pGetter, method, arg0));
                    sw.Add(Expression.SwitchCase(Expression.Block(assign,Expression.Constant(null)), Expression.Constant(data.Index,typeof(int))));
                }
                var s = Expression.Switch(arg1, Expression.Constant(null), sw.ToArray());
                DeserializeArray = Expression.Lambda<Action<BitStream,int, T>>(s, arg0, arg1,arg2).Compile();
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
                var attr = (Index[])fieldInfo.GetCustomAttributes(typeof(Index),false);
                if (attr.Length == 0)
                    continue;
                indexAttr = attr[0];
                fieldData.Add(new FieldData { Name = fieldInfo.Name, Index = indexAttr.index, Type = fieldInfo.FieldType });
            }
            for (int index = 0; index < props.Length; index++)
            {
                propInfo = props[index];
                var attr = (Index[])propInfo.GetCustomAttributes(typeof(Index), false);
                if (attr.Length == 0)
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
            return SerializerList(stream, value);
        }

        public override T Deserialize(BitStream stream)
        {
            var obj = new T();
            int index = 0;
            while ((index = BinaryReader.ReadInt32(stream)) != 0)
            {
                DeserializeArray(stream, index, obj);
            }
            return obj;
        }
    }
}
