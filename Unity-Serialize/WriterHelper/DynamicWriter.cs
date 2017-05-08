using System;
using System.Collections.Generic;
using System.Reflection;
using DreamSerialize.New;
using DreamSerialize.Utility;
using Theraot.Core.System.Linq.Expressions;
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
                else if (t == typeof(byte))
                    return new WriteForByte();
                else if (t == typeof(uint))
                    return new WriteForUInt32();
                else if (t == typeof(ushort))
                    return new WriteForUInt16();
                else if (t == typeof(short))
                    return new WriteForInt16();
                else if (t == typeof(long))
                    return new WriteForInt64();
                else if (t == typeof(ulong))
                    return new WriteForUInt64();
                else if (t == typeof(float))
                    return new WriteForSingle();
                else if (t == typeof(decimal))
                    return new WriteForDecimal();
                else if (t == typeof(double))
                    return new WriteForDouble();
#if Unity
                else if (t == typeof(UnityEngine.Rect))
                {
                    return new WriteForRect();
                }
                else if (t == typeof(UnityEngine.Vector2))
                {
                    return new WriteForVector2();
                }
                else if (t == typeof(UnityEngine.Vector3))
                {
                    return new WriteForVector3();
                }
                else if (t == typeof(UnityEngine.Vector4))
                {
                    return new WriteForVector4();
                }
#endif
            }
            else if (t == typeof(string))
                return new WriteForString();
            else if (t == typeof(Type))
                return new WriteForType();
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
                    var elemtType = t.GetElementType();
                    if (elemtType.IsValueType)
                    {
                        if (t == typeof(int))
                            return new Int32ArrayWrite();
                        else if (t == typeof(uint))
                            return new UInt32ArrayWrite();
                        else if (t == typeof(ushort))
                            return new UInt16ArrayWrite();
                        else if (t == typeof(short))
                            return new Int16ArrayWrite();
                        else if (t == typeof(long))
                            return new Int64ArrayWrite();
                        else if (t == typeof(ulong))
                            return new UInt64ArrayWrite();
                        else if (t == typeof(float))
                            return new SingleArrayWrite();
                        else if (t == typeof(decimal))
                            return new DecimalArrayWrite();
                        else if (t == typeof(double))
                            return new DoubleArrayWrite();
                    }
                    else if (elemtType == typeof(string))
                        return new StringArrayWrite();
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
        public int Length { get; set; }
        private List<TypeUtility.FieldData> datas;
        private Type t;
        public ClassWriter()
        {
            t = typeof(T);
            datas = TypeUtility.GetAllVariableWithIndex(t);
            Length = datas.Count;

            //序列化
            {
                List<Expression> expression = new List<Expression>();
                var arg0 = Expression.Parameter(typeof(BitStream), "data");
                var arg1 = Expression.Parameter(t, "T");
                Expression finalCall = null;
                var writeHead = typeof(WriteForInt32).GetMethod("Serialize", new[] { typeof(BitStream), typeof(int) });
                if (WriteForInt32.Default == null)
                    new WriteForInt32();
                var iGetter = Expression.Constant(WriteForInt32.Default, typeof(WriteForInt32));
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
                    var call = Expression.Call(pGetter, method, arg0, fp);
                    var writeIndex = Expression.Call(iGetter, writeHead, arg0, arg2);
                    var classWriterInterface = getSer as ClassWriteInterface;
                    if (classWriterInterface != null)
                    {
                        var isNull = Expression.NotEqual(fp, Expression.Constant(null, typeof(object)));
                        BlockExpression haveValue = Expression.Block(writeIndex, call);
                        finalCall = Expression.Condition(isNull, haveValue, arg0);
                    }
                    else
                    {
                        finalCall = Expression.Block(writeIndex, call);
                    }
                    expression.Add(finalCall);
                }
                if (expression.Count != 0)
                {
                    expression.Add(Expression.Call(iGetter, writeHead, arg0, Expression.Constant(0, typeof(int))));
                    var block = Expression.Block(expression);
                    SerializerList = Expression.Lambda<Func<BitStream, T, BitStream>>(block, arg0, arg1).Compile();
                }
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
                if (sw.Count != 0)
                {
                    var s = Expression.Switch(arg1, Expression.Constant(null), sw.ToArray());
                    DeserializeArray = Expression.Lambda<Action<BitStream, int, T>>(s, arg0, arg1, arg2).Compile();
                }
            }
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
            if(SerializerList != null)
                return SerializerList(stream, value);
            return stream;
        }

        public override T Deserialize(BitStream stream)
        {
            if (DeserializeArray == null)
                return default(T);
            if (stream.Bytes.Length == 0)
                return default(T);
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
