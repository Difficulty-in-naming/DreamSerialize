﻿using System;

#if NET20 || NET30 || NET35 || NET40

namespace Theraot.Core.System.Reflection
{
    public static class IntrospectionExtensions
    {
        public static Type GetTypeInfo(this Type type)
        {
            return type;
        }
    }
}

#endif