using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DreamSerialize.New;
using DreamSerialize.Utility;
using Theraot.Core.System.Linq.Expressions;
using UnityEngine;
using BitStream = DreamSerialize.New.BitStream;
using Expression = Theraot.Core.System.Linq.Expressions.Expression;


namespace DreamSerialize.WriterHelper
{
    public class DynamicWriter
    {
        public static void Write(BitStream stream, object value, Type t)
        {
            typeof(DynamicWriter<>).MakeGenericType(t)
                .InvokeMember("Write", BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public, null, null,
                    new[] {stream, value});
        }

        public static object Read(BitStream stream, Type t)
        {
            return typeof(DynamicWriter<>).MakeGenericType(t)
                .InvokeMember("Read", BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public, null, null,
                    new object[] {stream});
        }

        internal static object GeneratorWriter(Type t)
        {
            Type customT;
            if (CustomSerializer.Custom.TryGetValue(t, out customT))
            {
                return Activator.CreateInstance(customT);
            }
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
                else if (t == typeof(bool))
                    return new WriteForBoolean();
                else if (t.IsEnum)
                    return new WriteForInt32();
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
                /*                else if (genericT == typeof(KeyValuePair<,>))
                                {

                                }*/
            }
            if (t.IsClass)
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
                        if (elemtType == typeof(int))
                            return new Int32ArrayWrite();
                        else if (elemtType == typeof(uint))
                            return new UInt32ArrayWrite();
                        else if (elemtType == typeof(ushort))
                            return new UInt16ArrayWrite();
                        else if (elemtType == typeof(short))
                            return new Int16ArrayWrite();
                        else if (elemtType == typeof(long))
                            return new Int64ArrayWrite();
                        else if (elemtType == typeof(ulong))
                            return new UInt64ArrayWrite();
                        else if (elemtType == typeof(float))
                            return new SingleArrayWrite();
                        else if (elemtType == typeof(decimal))
                            return new DecimalArrayWrite();
                        else if (elemtType == typeof(double))
                            return new DoubleArrayWrite();
                        else if (elemtType.IsEnum)
                            return new Int32ArrayWrite();
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
            if (serializer == null)
            {
                throw new Exception("当前不支持该类型" + "    " + typeof(T));
                return default(T);
            }
            return serializer.Deserialize(stream);
        }
        internal static SupportSerializable<T> GeneratorWriter()
        {
            return SupportSerializable<T>.Default ?? (SupportSerializable<T>) DynamicWriter.GeneratorWriter(typeof(T));
        }
    }

    public interface ClassWriteInterface
    {
        int Length { get; set; }
    }

    public class ClassWriter<T> : SupportSerializable<T>, ClassWriteInterface where T : new()
    {
        internal Func<BitStream, T, BitStream> SerializerList;
        internal Info[] DeserializeArray;
        private const int TagTypeBits = 3;
        private const uint TagTypeMask = (1 << TagTypeBits) - 1;
        public static readonly Func<T> Instance = Expression.Lambda<Func<T>>(Expression.New(typeof(T))).Compile();
        public class Info 
        {
            public Action<BitStream, T> Action;
            public TypeUtility.FieldData Data;
        }
        public int Length { get; set; }
        private List<TypeUtility.FieldData> datas;
        private Type t;
        public ClassWriter()
        {
            t = typeof(T);
            datas = TypeUtility.GetAllVariableWithIndex(t);
            Length = datas.Count;
            DeserializeArray = new Info[Length];
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
                    Type serType;
                    FieldInfo serDefault;
                    object getSer;
                    MethodInfo method;
                    if (type.IsEnum)
                    {
                        serType = typeof(SupportSerializable<>).MakeGenericType(typeof(int));
                        serDefault = serType.GetField("Default");
                        getSer = Check(serDefault, typeof(int));
                        method = serType.GetMethod("Serialize", new[] {typeof(BitStream), typeof(int)});
                    }
                    else
                    {
                        serType = typeof(SupportSerializable<>).MakeGenericType(type);
                        serDefault = serType.GetField("Default");
                        getSer = Check(serDefault, type);
                        method = serType.GetMethod("Serialize", new[] { typeof(BitStream), type });
                    }
                    if (serDefault == null)
                    {
                        //throw new Exception("当前不支持该类型" + "    " + typeof(T));
                        continue;
                    }
                    var arg2 = Expression.Constant(GetIndex(data.Index,type), typeof(int));
                    var fp = Expression.PropertyOrField(arg1, data.Name);
                    var pGetter = Expression.Constant(getSer, getSer.GetType());
                    MethodCallExpression call;
                    if (!type.IsEnum)
                        call = Expression.Call(pGetter, method, arg0, fp);
                    else
                        call = Expression.Call(pGetter, method, arg0, Expression.Convert(fp, typeof(int)));
                    var writeIndex = Expression.Call(iGetter, writeHead, arg0, arg2);
                    var classWriterInterface = getSer as ClassWriteInterface;
                    if (classWriterInterface != null)
                    {
                        var isNull = Expression.NotEqual(fp, Expression.Constant(null, typeof(object)));
                        BlockExpression haveValue = Expression.Block(writeIndex, call);
                        finalCall = Expression.Condition(isNull, haveValue, Expression.Call(iGetter, writeHead, arg0, Expression.Constant(-1, typeof(int))));
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
                var arg1 = Expression.Parameter(t, "T");
                //Expression.Loop()
                //List<SwitchCase> sw = new List<SwitchCase>();
                for (int node = 0; node < Length; node++)
                {
                    var data = datas[node];
                    var type = data.Type;
                    var fp = Expression.PropertyOrField(arg1, data.Name);
                    Type serType;
                    if (type.IsEnum)
                        serType = typeof(SupportSerializable<>).MakeGenericType(typeof(int));
                    else
                        serType = typeof(SupportSerializable<>).MakeGenericType(type);

                    var serDefault = serType.GetField("Default");
                    if (serDefault == null)
                    {
                        //throw new Exception("当前不支持该类型" + "    " + typeof(T));
                        continue;
                    }
                    var o = Check(serDefault, type.IsEnum ? typeof(int) : type);
                    //var pGetter = Expression.Field(null, serDefault);
                    var constant = Expression.Constant(o, o.GetType());
                    var method = o.GetType().GetMethod("Deserialize", new[] { typeof(BitStream) });

                    BinaryExpression call;
                    if (!type.IsEnum)
                        call = Expression.Assign(fp, Expression.Call(constant, method, arg0));
                    else
                        call = Expression.Assign(fp, Expression.Convert(Expression.Call(constant, method, arg0),type));
                    var lambda = Expression.Lambda<Action<BitStream, T>>(call, arg0, arg1);
                    var action = lambda.Compile();
                    var sss = new Info();
                    sss.Action = action;
                    sss.Data = data;
                    DeserializeArray[node] = sss;
                }
            }
        }
         
        private static int GetIndex(int tag, Type type)
        {
            tag <<= TagTypeBits;
            if (type == typeof(byte) || type == typeof(sbyte) || type == typeof(int) || type == typeof(uint) || type == typeof(short) || type == typeof(ushort))
                tag |= (int) IType.Variable32;
            else if (type == typeof(float))
                tag |= (int) IType.Fixed32;
            else if (type == typeof(double))
                tag |= (int) IType.Fixed64;
            else if (type == typeof(long) || type == typeof(ulong))
                tag |= (int) IType.Variable64;
            else if (type == typeof(string) ||type.IsArray || typeof(IEnumerable).IsAssignableFrom(type))
                tag |= (int) IType.Dynamic;
            else if (type.IsClass || type.IsValueType)
                tag |= (int) IType.EndGroup;
            return tag;
        }

        private static object Check(FieldInfo field, Type type)
        {
            var obj = field.GetValue(null);
            return obj ?? DynamicWriter.GeneratorWriter(type);
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
            var obj = Instance();
            int index = 0;
            while (index < Length)
            {
                var deserialize = DeserializeArray[index];
                var tag = BinaryReader.ReadInt32(stream);
                if (tag == 0)
                    return obj;
                else if (tag == -1)
                {
                    index++;
                    continue;
                }
                if (deserialize.Data.Index == (tag >> 3))
                {
                    deserialize.Action(stream, obj);
                    index++;
                }
                else
                {
                    ReadTag(stream, tag);
                }
            }
            //End Group
            stream.Offset++;
            return obj;
        }

        private static void ReadTag(BitStream stream, int tag)
        {
            var type = (IType)(tag & TagTypeMask);
            switch (type)
            {
                case IType.Variable32:
                    BinaryReader.ReadUInt32(stream);
                    break;
                case IType.Variable64:
                    BinaryReader.ReadUInt64(stream);
                    break;
                case IType.Fixed32:
                    stream.Offset += 4;
                    break;
                case IType.Fixed64:
                    stream.Offset += 8;
                    break;
                case IType.Dynamic:
                    stream.Offset += BinaryReader.ReadInt32(stream);
                    break;
                case IType.EndGroup:
                    while (true)
                    {
                        int loopTag = BinaryReader.ReadInt32(stream);
                        if (loopTag == 0)
                            break;
                        ReadTag(stream, loopTag);
                    }
                    break;
            }
        }
    }
}
