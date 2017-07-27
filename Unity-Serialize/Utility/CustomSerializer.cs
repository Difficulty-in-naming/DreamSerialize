using System;
using System.Collections.Generic;
using DreamSerialize.New;

namespace DreamSerialize.Utility
{
    /// <summary>
    /// 自定义序列化类型
    /// </summary>
    public class CustomSerializer
    {
        internal static Dictionary<Type,Type> Custom = new Dictionary<Type, Type>();

        public static void Add(Type type)
        {
            if (type != null)
            {
                var @base = type.BaseType;
                if (@base.GetGenericTypeDefinition() != typeof(SupportSerializable<>))
                    throw new Exception("自定义序列化基类必须继承自SupportSerializable,如 public class GameObjectSerializer:SupportSerializable<GameObject>");
                var genericType = @base.GetGenericArguments()[0];
                if (!Custom.ContainsKey(genericType))
                    Custom.Add(genericType, type);
            }
        }
    }
}
