﻿// Needed for Workadound

using System.Text;

namespace Theraot.Core.Theraot.Core
{
    public static class StringBuilderExtensions
    {
        public static void Clear(this StringBuilder stringBuilder)
        {
            stringBuilder.Length = 0;
        }
    }
}