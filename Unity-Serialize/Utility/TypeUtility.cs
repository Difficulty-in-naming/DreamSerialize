using System;
namespace DreamSerialize.Utility
{
    public static class TypeUtility
    {
        public static Type[] GenericTypeArguments(this Type type)
        {
            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                return type.GetGenericArguments();
            }
            else
            {
                return Type.EmptyTypes;
            }
        }
    }
}
