using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DreamSerialize.New;
using DreamSerialize.Utility;
using Theraot.Core.System.Linq.Expressions;
#if Unity
using UnityEngine;
#endif
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
    }

    public class ClassWriter<T> : SupportSerializable<T>, ClassWriteInterface where T : new()
    {
        internal Func<BitStream, T, BitStream> SerializerList;
        internal Info[] DeserializeArray;
        private const int TagTypeBits = 3;
        private const uint TagTypeMask = (1 << TagTypeBits) - 1;
        public static readonly Func<T> Instance = Expression.Lambda<Func<T>>(Expression.New(typeof(T))).Compile();
        internal static ConstantExpression Null = Expression.Constant(null, typeof(object));
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
            if (Length == 0)
                return;
            DeserializeArray = new Info[Length];
#if DEBUG && Unity
            var debugMethod = typeof(Debug).GetMethod("Log", new[] { typeof(object) });
            var int2StringMethod = typeof(int).GetMethod("ToString", Type.EmptyTypes);
            var concat = typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string), });
#endif
            //序列化
            {
                List<Expression> expression = new List<Expression>();
                var bitStream = Expression.Parameter(typeof(BitStream), "data");
                var instance = Expression.Parameter(t, "T");
                Expression finalCall = null;
                var writeIndexMethod = typeof(WriteForInt32).GetMethod("Serialize", new[] { typeof(BitStream), typeof(int) });
                if (WriteForInt32.Default == null)
                    new WriteForInt32();
                var writeIndexInstance = Expression.Constant(WriteForInt32.Default, typeof(WriteForInt32));

                for (int i = 0; i < Length; i++)
                {
                    var data = datas[i];
                    var type = data.Type;
                    Type serType;
                    FieldInfo serDefault;
                    object serializeClass;
                    MethodInfo serMethod;
                    if (type.IsEnum)
                    {
                        serType = typeof(SupportSerializable<>).MakeGenericType(typeof(int));
                        serDefault = serType.GetField("Default");
                        serializeClass = Check(serDefault, typeof(int));
                        serMethod = serType.GetMethod("Serialize", new[] {typeof(BitStream), typeof(int)});
                    }
                    else
                    {
                        serType = typeof(SupportSerializable<>).MakeGenericType(type);
                        serDefault = serType.GetField("Default");
                        serializeClass = Check(serDefault, type);
                        serMethod = serType.GetMethod("Serialize", new[] { typeof(BitStream), type });
                    }
                    if (serDefault == null)
                    {
                        throw new Exception("当前不支持该类型" + "    " + typeof(T));
                        //continue;
                    }
                    var compressIndex = Expression.Constant(GetIndex(data.Index,type), typeof(int));
                    var field = Expression.PropertyOrField(instance, data.Name);
                    var serialize = Expression.Constant(serializeClass, serializeClass.GetType());
                    MethodCallExpression call;
                    if (!type.IsEnum)
                        call = Expression.Call(serialize, serMethod, bitStream, field);
                    else
                        call = Expression.Call(serialize, serMethod, bitStream, Expression.Convert(field, typeof(int)));
                    var writeIndex = Expression.Call(writeIndexInstance, writeIndexMethod, bitStream, compressIndex);
                    var classWriterInterface = serializeClass as ClassWriteInterface;
                    if (classWriterInterface != null)
                    {
                        //如果是类为空则写入Null标记符,如果不为空则写入数据
                        var notNull = Expression.NotEqual(field, Null);
                        finalCall = Expression.IfThenElse(
                            notNull,
                            Expression.Block(
                                writeIndex,
#if Unity && DEBUG
                                Expression.Call(debugMethod,
                                    Expression.Convert(
                                        Expression.Call(concat, Expression.Constant("类型:" + t.FullName + ",正在写入Index:" + data.Index + ",压缩后:" + GetIndex(data.Index, type) + "当前占用字节:", typeof(string)),
                                            Expression.Call(Expression.PropertyOrField(bitStream, "Offset"), int2StringMethod)), typeof(object))),
#endif
                                call,
#if Unity && DEBUG
                                Expression.Call(debugMethod,
                                    Expression.Convert(
                                        Expression.Call(concat, Expression.Constant("类型:" + t.FullName + ",名称:" + data.Name + ",当前占用字节:", typeof(string)),
                                            Expression.Call(Expression.PropertyOrField(bitStream, "Offset"), int2StringMethod)), typeof(object)))
#endif
                            ),
                            Expression.Call(writeIndexInstance, writeIndexMethod, bitStream, Expression.Constant(-1, typeof(int))));
                    }
                    else
                    {
                        finalCall = Expression.Block(writeIndex,
#if Unity && DEBUG
                            Expression.Call(debugMethod,
                                Expression.Convert(
                                    Expression.Call(concat, Expression.Constant("类型:" + t.FullName + ",正在写入Index:" + data.Index + ",压缩后:" + GetIndex(data.Index, type) + "当前占用字节:", typeof(string)),
                                        Expression.Call(Expression.PropertyOrField(bitStream, "Offset"), int2StringMethod)), typeof(object))),
#endif
                            call,
#if Unity && DEBUG
                            Expression.Call(debugMethod,
                                Expression.Convert(
                                    Expression.Call(concat, Expression.Constant("类型:" + t.FullName + ",名称:" + data.Name + ",当前占用字节:", typeof(string)),
                                        Expression.Call(Expression.PropertyOrField(bitStream, "Offset"), int2StringMethod)), typeof(object)))
#endif
                        );
                    }
                    expression.Add(finalCall);
                }
                if (expression.Count != 0)
                {
                    expression.Add(Expression.Call(writeIndexInstance, writeIndexMethod, bitStream, Expression.Constant(0, typeof(int))));
                    var block = Expression.Block(expression);
                    SerializerList = Expression.Lambda<Func<BitStream, T, BitStream>>(block, bitStream, instance).Compile();
                }
            }


            //反序列化
            {
                var bitStream = Expression.Parameter(typeof(BitStream), "data");
                var instance = Expression.Parameter(t, "T");
                //Expression.Loop()
                //List<SwitchCase> sw = new List<SwitchCase>();
                for (int node = 0; node < Length; node++)
                {
                    var data = datas[node];
                    var type = data.Type;
                    var fp = Expression.PropertyOrField(instance, data.Name);
                    Type deserializeType;
                    if (type.IsEnum)
                        deserializeType = typeof(SupportSerializable<>).MakeGenericType(typeof(int));
                    else
                        deserializeType = typeof(SupportSerializable<>).MakeGenericType(type);

                    var deserializeDefault = deserializeType.GetField("Default");
                    if (deserializeDefault == null)
                    {
                        throw new Exception("当前不支持该类型" + "    " + typeof(T));
                        //continue;
                    }
                    var deserializeAdapter = Check(deserializeDefault, type.IsEnum ? typeof(int) : type);
                    //var pGetter = Expression.Field(null, serDefault);
                    var constant = Expression.Constant(deserializeAdapter, deserializeAdapter.GetType());
                    var deserializeMethod = deserializeAdapter.GetType().GetMethod("Deserialize", new[] { typeof(BitStream) });

                    BinaryExpression call;
                    if (!type.IsEnum)
                        call = Expression.Assign(fp, Expression.Call(constant, deserializeMethod, bitStream));
                    else
                        call = Expression.Assign(fp, Expression.Convert(Expression.Call(constant, deserializeMethod, bitStream),type));
                    Expression<Action<BitStream, T>> lambda = null;
#if Unity && DEBUG
                    var block = Expression.Block(call,
                        Expression.Call(debugMethod,
                            Expression.Convert(
                                Expression.Call(concat, Expression.Constant("类型:" + t.FullName + ",名称:" + data.Name + ",当前占用字节:", typeof(string)),
                                    Expression.Call(Expression.PropertyOrField(bitStream, "Offset"), int2StringMethod)), typeof(object)))
                    );
                    lambda = Expression.Lambda<Action<BitStream, T>>(block, bitStream, instance);
#else
                    lambda = Expression.Lambda<Action<BitStream, T>>(call, bitStream, instance);
#endif

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
            int compressTag;
            while (true)
            {
                compressTag = BinaryReader.ReadInt32(stream);
                if (compressTag == 0)
                {
                    //stream.Offset++;
                    return obj;
                }
                if (DeserializeArray.Length == index)
                {
                    ReadTag(stream, compressTag);
                    continue;
                }
                var deserialize = DeserializeArray[index];
#if Unity && DEBUG
                Debug.Log("读取Tag(压缩):" + compressTag + ",读取Tag(解压):" + (compressTag >> 3) + "当前占用字节:" + stream.Offset);
#endif
                if (stream.Offset == 334)
                {
                    Debug.Log("ddasdsadsa");
                }
                else if (compressTag == -1)
                {
                    index++;
                    continue;
                }
                if (deserialize.Data.Index == compressTag >> 3)
                {
                    try
                    {
                        deserialize.Action(stream, obj);
                        index++;
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Index:" + deserialize.Data.Index + ",Name:" + deserialize.Data.Name + ",Type:" + deserialize.Data.Type + ",CurType:" + obj.GetType() + "\n" + e);
                    }
                }
                else
                {
                    ReadTag(stream, compressTag);
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
                    int length = BinaryReader.ReadInt32(stream);
                    stream.Offset += length;
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
