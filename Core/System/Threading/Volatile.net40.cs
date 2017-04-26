﻿using System;
using System.Threading;

#if NET20 || NET30 || NET35 || NET40

namespace Theraot.Core.System.Threading
{
    public static class Volatile
    {
        public static bool Read(ref bool location)
        {
            var flag = location;
            Thread.MemoryBarrier();
            return flag;
        }

        [CLSCompliant(false)]
        public static sbyte Read(ref sbyte location)
        {
            return Thread.VolatileRead(ref location);
        }

        public static byte Read(ref byte location)
        {
            return Thread.VolatileRead(ref location);
        }

        public static short Read(ref short location)
        {
            return Thread.VolatileRead(ref location);
        }

        [CLSCompliant(false)]
        public static ushort Read(ref ushort location)
        {
            return Thread.VolatileRead(ref location);
        }

        public static int Read(ref int location)
        {
            return Thread.VolatileRead(ref location);
        }

        [CLSCompliant(false)]
        public static uint Read(ref uint location)
        {
            return Thread.VolatileRead(ref location);
        }

        public static long Read(ref long location)
        {
            return Thread.VolatileRead(ref location);
        }

        [CLSCompliant(false)]
        public static ulong Read(ref ulong location)
        {
            return Thread.VolatileRead(ref location);
        }

        public static IntPtr Read(ref IntPtr location)
        {
            return Thread.VolatileRead(ref location);
        }

        [CLSCompliant(false)]
        public static UIntPtr Read(ref UIntPtr location)
        {
            return Thread.VolatileRead(ref location);
        }

        public static float Read(ref float location)
        {
            return Thread.VolatileRead(ref location);
        }

        public static double Read(ref double location)
        {
            return Thread.VolatileRead(ref location);
        }

        public static T Read<T>(ref T location)
            where T : class
        {
            var copy = location;
            Thread.MemoryBarrier();
            return copy;
        }

        public static void Write(ref bool location, bool value)
        {
            GC.KeepAlive(location);
            Thread.MemoryBarrier();
            location = value;
        }

        [CLSCompliant(false)]
        public static void Write(ref sbyte location, sbyte value)
        {
            Thread.VolatileWrite(ref location, value);
        }

        public static void Write(ref byte location, byte value)
        {
            Thread.VolatileWrite(ref location, value);
        }

        public static void Write(ref short location, short value)
        {
            Thread.VolatileWrite(ref location, value);
        }

        [CLSCompliant(false)]
        public static void Write(ref ushort location, ushort value)
        {
            Thread.VolatileWrite(ref location, value);
        }

        public static void Write(ref int location, int value)
        {
            Thread.VolatileWrite(ref location, value);
        }

        [CLSCompliant(false)]
        public static void Write(ref uint location, uint value)
        {
            Thread.VolatileWrite(ref location, value);
        }

        public static void Write(ref long location, long value)
        {
            Thread.VolatileWrite(ref location, value);
        }

        [CLSCompliant(false)]
        public static void Write(ref ulong location, ulong value)
        {
            Thread.VolatileWrite(ref location, value);
        }

        public static void Write(ref IntPtr location, IntPtr value)
        {
            Thread.VolatileWrite(ref location, value);
        }

        [CLSCompliant(false)]
        public static void Write(ref UIntPtr location, UIntPtr value)
        {
            Thread.VolatileWrite(ref location, value);
        }

        public static void Write(ref float location, float value)
        {
            Thread.VolatileWrite(ref location, value);
        }

        public static void Write(ref double location, double value)
        {
            Thread.VolatileWrite(ref location, value);
        }

        public static void Write<T>(ref T location, T value)
            where T : class
        {
            GC.KeepAlive(location);
            Thread.MemoryBarrier();
            location = value;
        }
    }
}

#endif