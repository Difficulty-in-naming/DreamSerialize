using System;
using System.Collections.Generic;
using System.Reflection;
using DreamSerialize.New;

namespace DreamSerialize.Utility
{
    public static class TypeUtility
    {
        public class FieldData
        {
            public string Name;
            public int Index;
            public Type Type;
            public MemberInfo Member;

            public bool IsField
            {
                get { return (Member as FieldInfo) != null; }
            }

            public bool IsProp
            {
                get { return (Member as PropertyInfo) != null; }
            }

            public object GetValue(object instance,params object[] index)
            {
                if (IsField)
                {
                    return ((FieldInfo)Member).GetValue(instance);
                }
                else if (IsProp)
                {
                    if (index.Length == 0)
                        index = null;
                    return ((PropertyInfo) Member).GetValue(instance, index);
                }
                throw new NullReferenceException();
            }

            public void SetValue(object instance, object value,params object[] index)
            {
                if (IsField)
                {
                    ((FieldInfo)Member).SetValue(instance,value);
                }
                else if (IsProp)
                {
                    if (index.Length == 0)
                        index = null;
                    ((PropertyInfo) Member).SetValue(instance, value, index);
                }
            }
        }

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

        private const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        public static List<FieldData> GetAllVariableWithIndex(Type t)
        {
            var fields = t.GetFields(flags);
            var prop = t.GetProperties(flags);
            return SortFields(fields, prop);
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
                var attr = (Index[])fieldInfo.GetCustomAttributes(typeof(Index), false);
                if (attr.Length == 0)
                    continue;
                if (fieldInfo.IsPrivate && !attr[0].SupportPrivate)
                    continue;
                indexAttr = attr[0];
                fieldData.Add(new FieldData { Name = fieldInfo.Name, Index = indexAttr.index, Type = fieldInfo.FieldType ,Member = fieldInfo});
            }
            for (int index = 0; index < props.Length; index++)
            {
                propInfo = props[index];
                var attr = (Index[])propInfo.GetCustomAttributes(typeof(Index), false);
                if (attr.Length == 0)
                    continue;
                if ((propInfo.GetGetMethod(false) == null && propInfo.GetSetMethod(false) == null) && !attr[0].SupportPrivate)
                    continue;
                indexAttr = attr[0];
                fieldData.Add(new FieldData { Name = propInfo.Name, Index = indexAttr.index, Type = propInfo.PropertyType ,Member = propInfo });
            }
            fieldData.Sort((t1, t2) => t1.Index - t2.Index);
            return fieldData;
        }
    }


}
