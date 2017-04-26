﻿// Needed for NET40

using System;
using System.Threading;
using Theraot.Core.System;
using Theraot.Core.System.Threading;

namespace Theraot.Core.Theraot.Threading
{
    public static partial class ThreadingHelper
    {
        public static void SpinWaitSet(ref int check, int value, int comparand)
        {
            var spinWait = new SpinWait();
            retry:
            if (Interlocked.CompareExchange(ref check, value, comparand) != comparand)
            {
                spinWait.SpinOnce();
                goto retry;
            }
        }

        public static void SpinWaitSet(ref int check, int value, int comparand, CancellationToken cancellationToken)
        {
            var spinWait = new SpinWait();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            if (Interlocked.CompareExchange(ref check, value, comparand) != comparand)
            {
                spinWait.SpinOnce();
                goto retry;
            }
        }

        public static bool SpinWaitSet(ref int check, int value, int comparand, int milliseconds)
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }
            if (milliseconds == -1)
            {
                SpinWaitSet(ref check, value, comparand);
                return true;
            }
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            if (Interlocked.CompareExchange(ref check, value, comparand) == comparand)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitSet(ref int check, int value, int comparand, int milliseconds, CancellationToken cancellationToken)
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }
            if (milliseconds == -1)
            {
                SpinWaitSet(ref check, value, comparand, cancellationToken);
                return true;
            }
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            if (Interlocked.CompareExchange(ref check, value, comparand) == comparand)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitSet(ref int check, int value, int comparand, TimeSpan timeout)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            if (Interlocked.CompareExchange(ref check, value, comparand) == comparand)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitSet(ref int check, int value, int comparand, TimeSpan timeout, CancellationToken cancellationToken)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            if (Interlocked.CompareExchange(ref check, value, comparand) == comparand)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitSet(ref int check, int value, int comparand, IComparable<TimeSpan> timeout)
        {
            var spinWait = new SpinWait();
            var start = DateTime.Now;
            retry:
            if (Interlocked.CompareExchange(ref check, value, comparand) == comparand)
            {
                return true;
            }
            if (timeout.CompareTo(DateTime.Now.Subtract(start)) > 0)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitSet(ref int check, int value, int comparand, IComparable<TimeSpan> timeout, CancellationToken cancellationToken)
        {
            var spinWait = new SpinWait();
            var start = DateTime.Now;
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            if (Interlocked.CompareExchange(ref check, value, comparand) == comparand)
            {
                return true;
            }
            if (timeout.CompareTo(DateTime.Now.Subtract(start)) > 0)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static void SpinWaitUntil(ref int check, int comparand)
        {
            var spinWait = new SpinWait();
            while (Volatile.Read(ref check) != comparand)
            {
                spinWait.SpinOnce();
            }
        }

        public static void SpinWaitUntil(ref int check, int comparand, CancellationToken cancellationToken)
        {
            var spinWait = new SpinWait();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            if (Volatile.Read(ref check) == comparand)
            {
                return;
            }
            spinWait.SpinOnce();
            goto retry;
        }

        public static bool SpinWaitUntil(ref int check, int comparand, int milliseconds)
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }
            if (milliseconds == -1)
            {
                SpinWaitUntil(ref check, comparand);
                return true;
            }
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            if (Volatile.Read(ref check) == comparand)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitUntil(ref int check, int comparand, int milliseconds, CancellationToken cancellationToken)
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }
            if (milliseconds == -1)
            {
                SpinWaitUntil(ref check, comparand, cancellationToken);
                return true;
            }
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            if (Volatile.Read(ref check) == comparand)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitUntil(ref int check, int comparand, TimeSpan timeout)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            if (Volatile.Read(ref check) == comparand)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitUntil(ref int check, int comparand, TimeSpan timeout, CancellationToken cancellationToken)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            if (Volatile.Read(ref check) == comparand)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitUntil(ref int check, int comparand, IComparable<TimeSpan> timeout)
        {
            var spinWait = new SpinWait();
            var start = DateTime.Now;
            retry:
            if (Volatile.Read(ref check) == comparand)
            {
                return true;
            }
            if (timeout.CompareTo(DateTime.Now.Subtract(start)) > 0)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitUntil(ref int check, int comparand, IComparable<TimeSpan> timeout, CancellationToken cancellationToken)
        {
            var spinWait = new SpinWait();
            var start = DateTime.Now;
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            if (Volatile.Read(ref check) == comparand)
            {
                return true;
            }
            if (timeout.CompareTo(DateTime.Now.Subtract(start)) > 0)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static void SpinWaitUntil(Func<bool> verification)
        {
            var spinWait = new SpinWait();
            while (!verification.Invoke())
            {
                spinWait.SpinOnce();
            }
        }

        public static void SpinWaitUntil(Func<bool> verification, CancellationToken cancellationToken)
        {
            var spinWait = new SpinWait();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            if (verification.Invoke())
            {
                return;
            }
            spinWait.SpinOnce();
            goto retry;
        }

        public static bool SpinWaitUntil(Func<bool> verification, int milliseconds)
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }
            if (milliseconds == -1)
            {
                SpinWaitUntil(verification);
                return true;
            }
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            if (verification.Invoke())
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitUntil(Func<bool> verification, int milliseconds, CancellationToken cancellationToken)
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }
            if (milliseconds == -1)
            {
                SpinWaitUntil(verification, cancellationToken);
                return true;
            }
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            if (verification.Invoke())
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitUntil(Func<bool> verification, TimeSpan timeout)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            if (verification.Invoke())
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitUntil(Func<bool> verification, TimeSpan timeout, CancellationToken cancellationToken)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            if (verification.Invoke())
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitUntil(Func<bool> verification, IComparable<TimeSpan> timeout)
        {
            var spinWait = new SpinWait();
            var start = DateTime.Now;
            retry:
            if (verification.Invoke())
            {
                return true;
            }
            if (timeout.CompareTo(DateTime.Now.Subtract(start)) > 0)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitUntil(Func<bool> verification, IComparable<TimeSpan> timeout, CancellationToken cancellationToken)
        {
            var spinWait = new SpinWait();
            var start = DateTime.Now;
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            if (verification.Invoke())
            {
                return true;
            }
            if (timeout.CompareTo(DateTime.Now.Subtract(start)) > 0)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static void SpinWaitWhile(ref int check, int comparand)
        {
            var spinWait = new SpinWait();
            while (Volatile.Read(ref check) == comparand)
            {
                spinWait.SpinOnce();
            }
        }

        public static void SpinWaitWhile(ref int check, int comparand, CancellationToken cancellationToken)
        {
            var spinWait = new SpinWait();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            if (Volatile.Read(ref check) != comparand)
            {
                return;
            }
            spinWait.SpinOnce();
            goto retry;
        }

        public static bool SpinWaitWhile(ref int check, int comparand, int milliseconds)
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }
            if (milliseconds == -1)
            {
                SpinWaitWhile(ref check, comparand);
                return true;
            }
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            if (Volatile.Read(ref check) != comparand)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitWhile(ref int check, int comparand, int milliseconds, CancellationToken cancellationToken)
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }
            if (milliseconds == -1)
            {
                SpinWaitWhile(ref check, comparand, cancellationToken);
                return true;
            }
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            if (Volatile.Read(ref check) != comparand)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitWhile(ref int check, int comparand, TimeSpan timeout)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            if (Volatile.Read(ref check) != comparand)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitWhile(ref int check, int comparand, TimeSpan timeout, CancellationToken cancellationToken)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            if (Volatile.Read(ref check) != comparand)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitWhile(ref int check, int comparand, IComparable<TimeSpan> timeout)
        {
            var spinWait = new SpinWait();
            var start = DateTime.Now;
            retry:
            if (Volatile.Read(ref check) != comparand)
            {
                return true;
            }
            if (timeout.CompareTo(DateTime.Now.Subtract(start)) > 0)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitWhile(ref int check, int comparand, IComparable<TimeSpan> timeout, CancellationToken cancellationToken)
        {
            var spinWait = new SpinWait();
            var start = DateTime.Now;
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            if (Volatile.Read(ref check) != comparand)
            {
                return true;
            }
            if (timeout.CompareTo(DateTime.Now.Subtract(start)) > 0)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static void SpinWaitWhileNull<T>(ref T check)
            where T : class
        {
            var spinWait = new SpinWait();
            while (Volatile.Read(ref check) == null)
            {
                spinWait.SpinOnce();
            }
        }

        public static void SpinWaitWhileNull<T>(ref T check, CancellationToken cancellationToken)
        where T : class
        {
            var spinWait = new SpinWait();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            if (Volatile.Read(ref check) != null)
            {
                return;
            }
            spinWait.SpinOnce();
            goto retry;
        }

        public static bool SpinWaitWhileNull<T>(ref T check, int milliseconds)
        where T : class
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }
            if (milliseconds == -1)
            {
                SpinWaitWhileNull(ref check);
                return true;
            }
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            if (Volatile.Read(ref check) != null)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitWhileNull<T>(ref T check, int milliseconds, CancellationToken cancellationToken)
        where T : class
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }
            if (milliseconds == -1)
            {
                SpinWaitWhileNull(ref check, cancellationToken);
                return true;
            }
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            if (Volatile.Read(ref check) != null)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitWhileNull<T>(ref T check, TimeSpan timeout)
        where T : class
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            if (Volatile.Read(ref check) != null)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitWhileNull<T>(ref T check, TimeSpan timeout, CancellationToken cancellationToken)
        where T : class
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            if (Volatile.Read(ref check) != null)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitWhileNull<T>(ref T check, IComparable<TimeSpan> timeout)
        where T : class
        {
            var spinWait = new SpinWait();
            var start = DateTime.Now;
            retry:
            if (Volatile.Read(ref check) != null)
            {
                return true;
            }
            if (timeout.CompareTo(DateTime.Now.Subtract(start)) > 0)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitWhileNull<T>(ref T check, IComparable<TimeSpan> timeout, CancellationToken cancellationToken)
        where T : class
        {
            var spinWait = new SpinWait();
            var start = DateTime.Now;
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            if (Volatile.Read(ref check) != null)
            {
                return true;
            }
            if (timeout.CompareTo(DateTime.Now.Subtract(start)) > 0)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeSet(ref int check, int value)
        {
            var spinWait = new SpinWait();
            retry:
            var tmpA = Volatile.Read(ref check);
            var tmpB = Interlocked.CompareExchange(ref check, tmpA + value, tmpA);
            if (tmpB == tmpA)
            {
                return true;
            }
            spinWait.SpinOnce();
            goto retry;
        }

        public static bool SpinWaitRelativeSet(ref int check, int value, CancellationToken cancellationToken)
        {
            var spinWait = new SpinWait();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var tmpA = Volatile.Read(ref check);
            var tmpB = Interlocked.CompareExchange(ref check, tmpA + value, tmpA);
            if (tmpB == tmpA)
            {
                return true;
            }
            spinWait.SpinOnce();
            goto retry;
        }

        public static bool SpinWaitRelativeSet(ref int check, int value, int milliseconds)
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }
            if (milliseconds == -1)
            {
                return SpinWaitRelativeSet(ref check, value);
            }
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            var tmpA = Volatile.Read(ref check);
            var tmpB = Interlocked.CompareExchange(ref check, tmpA + value, tmpA);
            if (tmpB == tmpA)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeSet(ref int check, int value, int milliseconds, CancellationToken cancellationToken)
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }
            if (milliseconds == -1)
            {
                return SpinWaitRelativeSet(ref check, value);
            }
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var tmpA = Volatile.Read(ref check);
            var tmpB = Interlocked.CompareExchange(ref check, tmpA + value, tmpA);
            if (tmpB == tmpA)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeSet(ref int check, int value, TimeSpan timeout)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            var tmpA = Volatile.Read(ref check);
            var tmpB = Interlocked.CompareExchange(ref check, tmpA + value, tmpA);
            if (tmpB == tmpA)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeSet(ref int check, int value, TimeSpan timeout, CancellationToken cancellationToken)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var tmpA = Volatile.Read(ref check);
            var tmpB = Interlocked.CompareExchange(ref check, tmpA + value, tmpA);
            if (tmpB == tmpA)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeSet(ref int check, int value, IComparable<TimeSpan> timeout)
        {
            var spinWait = new SpinWait();
            var start = DateTime.Now;
            retry:
            var tmpA = Volatile.Read(ref check);
            var tmpB = Interlocked.CompareExchange(ref check, tmpA + value, tmpA);
            if (tmpB == tmpA)
            {
                return true;
            }
            if (timeout.CompareTo(DateTime.Now.Subtract(start)) > 0)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeSet(ref int check, int value, IComparable<TimeSpan> timeout, CancellationToken cancellationToken)
        {
            var spinWait = new SpinWait();
            var start = DateTime.Now;
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var tmpA = Volatile.Read(ref check);
            var tmpB = Interlocked.CompareExchange(ref check, tmpA + value, tmpA);
            if (tmpB == tmpA)
            {
                return true;
            }
            if (timeout.CompareTo(DateTime.Now.Subtract(start)) > 0)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeExchange(ref int check, int value, out int result)
        {
            var spinWait = new SpinWait();
            retry:
            var tmpA = Volatile.Read(ref check);
            var tmpB = Interlocked.CompareExchange(ref check, tmpA + value, tmpA);
            result = tmpB + value;
            if (tmpB == tmpA)
            {
                return true;
            }
            spinWait.SpinOnce();
            goto retry;
        }

        public static bool SpinWaitRelativeExchange(ref int check, int value, out int result, CancellationToken cancellationToken)
        {
            var spinWait = new SpinWait();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var tmpA = Volatile.Read(ref check);
            var tmpB = Interlocked.CompareExchange(ref check, tmpA + value, tmpA);
            result = tmpB + value;
            if (tmpB == tmpA)
            {
                return true;
            }
            spinWait.SpinOnce();
            goto retry;
        }

        public static bool SpinWaitRelativeExchange(ref int check, int value, out int result, int milliseconds)
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }
            if (milliseconds == -1)
            {
                return SpinWaitRelativeExchange(ref check, value, out result);
            }
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            var tmpA = Volatile.Read(ref check);
            var tmpB = Interlocked.CompareExchange(ref check, tmpA + value, tmpA);
            result = tmpB + value;
            if (tmpB == tmpA)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeExchange(ref int check, int value, out int result, int milliseconds, CancellationToken cancellationToken)
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }
            if (milliseconds == -1)
            {
                return SpinWaitRelativeExchange(ref check, value, out result);
            }
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var tmpA = Volatile.Read(ref check);
            var tmpB = Interlocked.CompareExchange(ref check, tmpA + value, tmpA);
            result = tmpB + value;
            if (tmpB == tmpA)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeExchange(ref int check, int value, out int result, TimeSpan timeout)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            var tmpA = Volatile.Read(ref check);
            var tmpB = Interlocked.CompareExchange(ref check, tmpA + value, tmpA);
            result = tmpB + value;
            if (tmpB == tmpA)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeExchange(ref int check, int value, out int result, TimeSpan timeout, CancellationToken cancellationToken)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var tmpA = Volatile.Read(ref check);
            var tmpB = Interlocked.CompareExchange(ref check, tmpA + value, tmpA);
            result = tmpB + value;
            if (tmpB == tmpA)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeExchange(ref int check, int value, out int result, IComparable<TimeSpan> timeout)
        {
            var spinWait = new SpinWait();
            var start = DateTime.Now;
            retry:
            var tmpA = Volatile.Read(ref check);
            var tmpB = Interlocked.CompareExchange(ref check, tmpA + value, tmpA);
            result = tmpB + value;
            if (tmpB == tmpA)
            {
                return true;
            }
            if (timeout.CompareTo(DateTime.Now.Subtract(start)) > 0)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeExchange(ref int check, int value, out int result, IComparable<TimeSpan> timeout, CancellationToken cancellationToken)
        {
            var spinWait = new SpinWait();
            var start = DateTime.Now;
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var tmpA = Volatile.Read(ref check);
            var tmpB = Interlocked.CompareExchange(ref check, tmpA + value, tmpA);
            result = tmpB + value;
            if (tmpB == tmpA)
            {
                return true;
            }
            if (timeout.CompareTo(DateTime.Now.Subtract(start)) > 0)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitSetUnless(ref int check, int value, int comparand, int unless)
        {
            var spinWait = new SpinWait();
            retry:
            var lastValue = Volatile.Read(ref check);
            if (lastValue == unless)
            {
                return false;
            }
            var tmpB = Interlocked.CompareExchange(ref check, value, comparand);
            if (tmpB == comparand)
            {
                return true;
            }
            spinWait.SpinOnce();
            goto retry;
        }

        public static bool SpinWaitSetUnless(ref int check, int value, int comparand, int unless, CancellationToken cancellationToken)
        {
            var spinWait = new SpinWait();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var lastValue = Volatile.Read(ref check);
            if (lastValue == unless)
            {
                return false;
            }
            var tmpB = Interlocked.CompareExchange(ref check, value, comparand);
            if (tmpB == comparand)
            {
                return true;
            }
            spinWait.SpinOnce();
            goto retry;
        }

        public static bool SpinWaitSetUnless(ref int check, int value, int comparand, int unless, int milliseconds)
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }
            if (milliseconds == -1)
            {
                return SpinWaitSetUnless(ref check, value, comparand, unless);
            }
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            var lastValue = Volatile.Read(ref check);
            if (lastValue == unless)
            {
                return false;
            }
            var tmpB = Interlocked.CompareExchange(ref check, value, comparand);
            if (tmpB == comparand)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitSetUnless(ref int check, int value, int comparand, int unless, int milliseconds, CancellationToken cancellationToken)
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }
            if (milliseconds == -1)
            {
                return SpinWaitSetUnless(ref check, value, comparand, unless);
            }
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var lastValue = Volatile.Read(ref check);
            if (lastValue == unless)
            {
                return false;
            }
            var tmpB = Interlocked.CompareExchange(ref check, value, comparand);
            if (tmpB == comparand)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitSetUnless(ref int check, int value, int comparand, int unless, TimeSpan timeout)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            var lastValue = Volatile.Read(ref check);
            if (lastValue == unless)
            {
                return false;
            }
            var tmpB = Interlocked.CompareExchange(ref check, value, comparand);
            if (tmpB == comparand)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitSetUnless(ref int check, int value, int comparand, int unless, TimeSpan timeout, CancellationToken cancellationToken)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var lastValue = Volatile.Read(ref check);
            if (lastValue == unless)
            {
                return false;
            }
            var tmpB = Interlocked.CompareExchange(ref check, value, comparand);
            if (tmpB == comparand)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitSetUnless(ref int check, int value, int comparand, int unless, IComparable<TimeSpan> timeout)
        {
            var spinWait = new SpinWait();
            var start = DateTime.Now;
            retry:
            var lastValue = Volatile.Read(ref check);
            if (lastValue == unless)
            {
                return false;
            }
            var tmpB = Interlocked.CompareExchange(ref check, value, comparand);
            if (tmpB == comparand)
            {
                return true;
            }
            if (timeout.CompareTo(DateTime.Now.Subtract(start)) > 0)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitSetUnless(ref int check, int value, int comparand, int unless, IComparable<TimeSpan> timeout, CancellationToken cancellationToken)
        {
            var spinWait = new SpinWait();
            var start = DateTime.Now;
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var lastValue = Volatile.Read(ref check);
            if (lastValue == unless)
            {
                return false;
            }
            var tmpB = Interlocked.CompareExchange(ref check, value, comparand);
            if (tmpB == comparand)
            {
                return true;
            }
            if (timeout.CompareTo(DateTime.Now.Subtract(start)) > 0)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeSetUnless(ref int check, int value, int unless)
        {
            var spinWait = new SpinWait();
            retry:
            var lastValue = Volatile.Read(ref check);
            var result = lastValue + value;
            if (lastValue == unless)
            {
                return false;
            }
            var tmpB = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmpB == lastValue)
            {
                return true;
            }
            spinWait.SpinOnce();
            goto retry;
        }

        public static bool SpinWaitRelativeSetUnless(ref int check, int value, int unless, CancellationToken cancellationToken)
        {
            var spinWait = new SpinWait();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var lastValue = Volatile.Read(ref check);
            var result = lastValue + value;
            if (lastValue == unless)
            {
                return false;
            }
            var tmpB = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmpB == lastValue)
            {
                return true;
            }
            spinWait.SpinOnce();
            goto retry;
        }

        public static bool SpinWaitRelativeSetUnless(ref int check, int value, int unless, int milliseconds)
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }
            if (milliseconds == -1)
            {
                return SpinWaitRelativeSetUnless(ref check, value, unless);
            }
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            var lastValue = Volatile.Read(ref check);
            var result = lastValue + value;
            if (lastValue == unless)
            {
                return false;
            }
            var tmpB = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmpB == lastValue)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeSetUnless(ref int check, int value, int unless, int milliseconds, CancellationToken cancellationToken)
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }
            if (milliseconds == -1)
            {
                return SpinWaitRelativeSetUnless(ref check, value, unless);
            }
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var lastValue = Volatile.Read(ref check);
            var result = lastValue + value;
            if (lastValue == unless)
            {
                return false;
            }
            var tmpB = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmpB == lastValue)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeSetUnless(ref int check, int value, int unless, TimeSpan timeout)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            var lastValue = Volatile.Read(ref check);
            var result = lastValue + value;
            if (lastValue == unless)
            {
                return false;
            }
            var tmpB = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmpB == lastValue)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeSetUnless(ref int check, int value, int unless, TimeSpan timeout, CancellationToken cancellationToken)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var lastValue = Volatile.Read(ref check);
            var result = lastValue + value;
            if (lastValue == unless)
            {
                return false;
            }
            var tmpB = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmpB == lastValue)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeSetUnless(ref int check, int value, int unless, IComparable<TimeSpan> timeout)
        {
            var spinWait = new SpinWait();
            var start = DateTime.Now;
            retry:
            var lastValue = Volatile.Read(ref check);
            var result = lastValue + value;
            if (lastValue == unless)
            {
                return false;
            }
            var tmpB = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmpB == lastValue)
            {
                return true;
            }
            if (timeout.CompareTo(DateTime.Now.Subtract(start)) > 0)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeSetUnless(ref int check, int value, int unless, IComparable<TimeSpan> timeout, CancellationToken cancellationToken)
        {
            var spinWait = new SpinWait();
            var start = DateTime.Now;
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var lastValue = Volatile.Read(ref check);
            var result = lastValue + value;
            if (lastValue == unless)
            {
                return false;
            }
            var tmpB = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmpB == lastValue)
            {
                return true;
            }
            if (timeout.CompareTo(DateTime.Now.Subtract(start)) > 0)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeExchangeUnless(ref int check, int value, int unless, out int result)
        {
            var spinWait = new SpinWait();
            retry:
            var lastValue = Volatile.Read(ref check);
            result = lastValue + value;
            if (lastValue == unless)
            {
                return false;
            }
            var tmpB = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmpB == lastValue)
            {
                return true;
            }
            spinWait.SpinOnce();
            goto retry;
        }

        public static bool SpinWaitRelativeExchangeUnless(ref int check, int value, int unless, out int result, CancellationToken cancellationToken)
        {
            var spinWait = new SpinWait();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var lastValue = Volatile.Read(ref check);
            result = lastValue + value;
            if (lastValue == unless)
            {
                return false;
            }
            var tmpB = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmpB == lastValue)
            {
                return true;
            }
            spinWait.SpinOnce();
            goto retry;
        }

        public static bool SpinWaitRelativeExchangeUnless(ref int check, int value, int unless, out int result, int milliseconds)
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }
            if (milliseconds == -1)
            {
                return SpinWaitRelativeExchangeUnless(ref check, value, unless, out result);
            }
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            var lastValue = Volatile.Read(ref check);
            result = lastValue + value;
            if (lastValue == unless)
            {
                return false;
            }
            var tmpB = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmpB == lastValue)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeExchangeUnless(ref int check, int value, int unless, out int result, int milliseconds, CancellationToken cancellationToken)
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }
            if (milliseconds == -1)
            {
                return SpinWaitRelativeExchangeUnless(ref check, value, unless, out result);
            }
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var lastValue = Volatile.Read(ref check);
            result = lastValue + value;
            if (lastValue == unless)
            {
                return false;
            }
            var tmpB = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmpB == lastValue)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeExchangeUnless(ref int check, int value, int unless, out int result, TimeSpan timeout)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            var lastValue = Volatile.Read(ref check);
            result = lastValue + value;
            if (lastValue == unless)
            {
                return false;
            }
            var tmpB = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmpB == lastValue)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeExchangeUnless(ref int check, int value, int unless, out int result, TimeSpan timeout, CancellationToken cancellationToken)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var lastValue = Volatile.Read(ref check);
            result = lastValue + value;
            if (lastValue == unless)
            {
                return false;
            }
            var tmpB = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmpB == lastValue)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeExchangeUnless(ref int check, int value, int unless, out int result, IComparable<TimeSpan> timeout)
        {
            var spinWait = new SpinWait();
            var start = DateTime.Now;
            retry:
            var lastValue = Volatile.Read(ref check);
            result = lastValue + value;
            if (lastValue == unless)
            {
                return false;
            }
            var tmpB = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmpB == lastValue)
            {
                return true;
            }
            if (timeout.CompareTo(DateTime.Now.Subtract(start)) > 0)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeExchangeUnless(ref int check, int value, int unless, out int result, IComparable<TimeSpan> timeout, CancellationToken cancellationToken)
        {
            var spinWait = new SpinWait();
            var start = DateTime.Now;
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var lastValue = Volatile.Read(ref check);
            result = lastValue + value;
            if (lastValue == unless)
            {
                return false;
            }
            var tmpB = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmpB == lastValue)
            {
                return true;
            }
            if (timeout.CompareTo(DateTime.Now.Subtract(start)) > 0)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeSetUnlessNegative(ref int check, int value)
        {
            var spinWait = new SpinWait();
            retry:
            var lastValue = Volatile.Read(ref check);
            if ((lastValue < 0) || (lastValue < -value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            spinWait.SpinOnce();
            goto retry;
        }

        public static bool SpinWaitRelativeSetUnlessNegative(ref int check, int value, CancellationToken cancellationToken)
        {
            var spinWait = new SpinWait();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var lastValue = Volatile.Read(ref check);
            if ((lastValue < 0) || (lastValue < -value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            spinWait.SpinOnce();
            goto retry;
        }

        public static bool SpinWaitRelativeSetUnlessNegative(ref int check, int value, int milliseconds)
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }
            if (milliseconds == -1)
            {
                return SpinWaitRelativeSetUnlessNegative(ref check, value);
            }
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            var lastValue = Volatile.Read(ref check);
            if ((lastValue < 0) || (lastValue < -value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeSetUnlessNegative(ref int check, int value, int milliseconds, CancellationToken cancellationToken)
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }
            if (milliseconds == -1)
            {
                return SpinWaitRelativeSetUnlessNegative(ref check, value);
            }
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var lastValue = Volatile.Read(ref check);
            if ((lastValue < 0) || (lastValue < -value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeSetUnlessNegative(ref int check, int value, TimeSpan timeout)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            var lastValue = Volatile.Read(ref check);
            if ((lastValue < 0) || (lastValue < -value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeSetUnlessNegative(ref int check, int value, TimeSpan timeout, CancellationToken cancellationToken)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var lastValue = Volatile.Read(ref check);
            if ((lastValue < 0) || (lastValue < -value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeSetUnlessNegative(ref int check, int value, IComparable<TimeSpan> timeout)
        {
            var spinWait = new SpinWait();
            var start = DateTime.Now;
            retry:
            var lastValue = Volatile.Read(ref check);
            if ((lastValue < 0) || (lastValue < -value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (timeout.CompareTo(DateTime.Now.Subtract(start)) > 0)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeSetUnlessNegative(ref int check, int value, IComparable<TimeSpan> timeout, CancellationToken cancellationToken)
        {
            var spinWait = new SpinWait();
            var start = DateTime.Now;
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var lastValue = Volatile.Read(ref check);
            if ((lastValue < 0) || (lastValue < -value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (timeout.CompareTo(DateTime.Now.Subtract(start)) > 0)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeExchangeUnlessNegative(ref int check, int value, out int lastValue)
        {
            var spinWait = new SpinWait();
            retry:
            lastValue = Volatile.Read(ref check);
            if ((lastValue < 0) || (lastValue < -value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            spinWait.SpinOnce();
            goto retry;
        }

        public static bool SpinWaitRelativeExchangeUnlessNegative(ref int check, int value, out int lastValue, CancellationToken cancellationToken)
        {
            var spinWait = new SpinWait();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            lastValue = Volatile.Read(ref check);
            if ((lastValue < 0) || (lastValue < -value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            spinWait.SpinOnce();
            goto retry;
        }

        public static bool SpinWaitRelativeExchangeUnlessNegative(ref int check, int value, out int lastValue, int milliseconds)
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }
            if (milliseconds == -1)
            {
                return SpinWaitRelativeExchangeUnlessNegative(ref check, value, out lastValue);
            }
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            lastValue = Volatile.Read(ref check);
            if ((lastValue < 0) || (lastValue < -value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeExchangeUnlessNegative(ref int check, int value, out int lastValue, int milliseconds, CancellationToken cancellationToken)
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }
            if (milliseconds == -1)
            {
                return SpinWaitRelativeExchangeUnlessNegative(ref check, value, out lastValue);
            }
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            lastValue = Volatile.Read(ref check);
            if ((lastValue < 0) || (lastValue < -value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeExchangeUnlessNegative(ref int check, int value, out int lastValue, TimeSpan timeout)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            lastValue = Volatile.Read(ref check);
            if ((lastValue < 0) || (lastValue < -value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeExchangeUnlessNegative(ref int check, int value, out int lastValue, TimeSpan timeout, CancellationToken cancellationToken)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            lastValue = Volatile.Read(ref check);
            if ((lastValue < 0) || (lastValue < -value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeExchangeUnlessNegative(ref int check, int value, out int lastValue, IComparable<TimeSpan> timeout)
        {
            var spinWait = new SpinWait();
            var start = DateTime.Now;
            retry:
            lastValue = Volatile.Read(ref check);
            if ((lastValue < 0) || (lastValue < -value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (timeout.CompareTo(DateTime.Now.Subtract(start)) > 0)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeExchangeUnlessNegative(ref int check, int value, out int lastValue, IComparable<TimeSpan> timeout, CancellationToken cancellationToken)
        {
            var spinWait = new SpinWait();
            var start = DateTime.Now;
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            lastValue = Volatile.Read(ref check);
            if ((lastValue < 0) || (lastValue < -value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (timeout.CompareTo(DateTime.Now.Subtract(start)) > 0)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeSetUnlessExcess(ref int check, int value, int maxValue)
        {
            var spinWait = new SpinWait();
            retry:
            var lastValue = Volatile.Read(ref check);
            if ((lastValue > maxValue) || (lastValue > maxValue - value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            spinWait.SpinOnce();
            goto retry;
        }

        public static bool SpinWaitRelativeSetUnlessExcess(ref int check, int value, int maxValue, CancellationToken cancellationToken)
        {
            var spinWait = new SpinWait();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var lastValue = Volatile.Read(ref check);
            if ((lastValue > maxValue) || (lastValue > maxValue - value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            spinWait.SpinOnce();
            goto retry;
        }

        public static bool SpinWaitRelativeSetUnlessExcess(ref int check, int value, int maxValue, int milliseconds)
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }
            if (milliseconds == -1)
            {
                return SpinWaitRelativeSetUnlessExcess(ref check, value, maxValue);
            }
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            var lastValue = Volatile.Read(ref check);
            if ((lastValue > maxValue) || (lastValue > maxValue - value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeSetUnlessExcess(ref int check, int value, int maxValue, int milliseconds, CancellationToken cancellationToken)
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }
            if (milliseconds == -1)
            {
                return SpinWaitRelativeSetUnlessExcess(ref check, value, maxValue);
            }
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var lastValue = Volatile.Read(ref check);
            if ((lastValue > maxValue) || (lastValue > maxValue - value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeSetUnlessExcess(ref int check, int value, int maxValue, TimeSpan timeout)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            var lastValue = Volatile.Read(ref check);
            if ((lastValue > maxValue) || (lastValue > maxValue - value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeSetUnlessExcess(ref int check, int value, int maxValue, TimeSpan timeout, CancellationToken cancellationToken)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var lastValue = Volatile.Read(ref check);
            if ((lastValue > maxValue) || (lastValue > maxValue - value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeSetUnlessExcess(ref int check, int value, int maxValue, IComparable<TimeSpan> timeout)
        {
            var spinWait = new SpinWait();
            var start = DateTime.Now;
            retry:
            var lastValue = Volatile.Read(ref check);
            if ((lastValue > maxValue) || (lastValue > maxValue - value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (timeout.CompareTo(DateTime.Now.Subtract(start)) > 0)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeSetUnlessExcess(ref int check, int value, int maxValue, IComparable<TimeSpan> timeout, CancellationToken cancellationToken)
        {
            var spinWait = new SpinWait();
            var start = DateTime.Now;
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var lastValue = Volatile.Read(ref check);
            if ((lastValue > maxValue) || (lastValue > maxValue - value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (timeout.CompareTo(DateTime.Now.Subtract(start)) > 0)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeExchangeUnlessExcess(ref int check, int value, int maxValue, out int lastValue)
        {
            var spinWait = new SpinWait();
            retry:
            lastValue = Volatile.Read(ref check);
            if ((lastValue > maxValue) || (lastValue > maxValue - value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            spinWait.SpinOnce();
            goto retry;
        }

        public static bool SpinWaitRelativeExchangeUnlessExcess(ref int check, int value, int maxValue, out int lastValue, CancellationToken cancellationToken)
        {
            var spinWait = new SpinWait();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            lastValue = Volatile.Read(ref check);
            if ((lastValue > maxValue) || (lastValue > maxValue - value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            spinWait.SpinOnce();
            goto retry;
        }

        public static bool SpinWaitRelativeExchangeUnlessExcess(ref int check, int value, int maxValue, out int lastValue, int milliseconds)
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }
            if (milliseconds == -1)
            {
                return SpinWaitRelativeExchangeUnlessExcess(ref check, value, maxValue, out lastValue);
            }
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            lastValue = Volatile.Read(ref check);
            if ((lastValue > maxValue) || (lastValue > maxValue - value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeExchangeUnlessExcess(ref int check, int value, int maxValue, out int lastValue, int milliseconds, CancellationToken cancellationToken)
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }
            if (milliseconds == -1)
            {
                return SpinWaitRelativeExchangeUnlessExcess(ref check, value, maxValue, out lastValue);
            }
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            lastValue = Volatile.Read(ref check);
            if ((lastValue > maxValue) || (lastValue > maxValue - value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeExchangeUnlessExcess(ref int check, int value, int maxValue, out int lastValue, TimeSpan timeout)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            lastValue = Volatile.Read(ref check);
            if ((lastValue > maxValue) || (lastValue > maxValue - value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeExchangeUnlessExcess(ref int check, int value, int maxValue, out int lastValue, TimeSpan timeout, CancellationToken cancellationToken)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            lastValue = Volatile.Read(ref check);
            if ((lastValue > maxValue) || (lastValue > maxValue - value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeExchangeUnlessExcess(ref int check, int value, int maxValue, out int lastValue, IComparable<TimeSpan> timeout)
        {
            var spinWait = new SpinWait();
            var start = DateTime.Now;
            retry:
            lastValue = Volatile.Read(ref check);
            if ((lastValue > maxValue) || (lastValue > maxValue - value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (timeout.CompareTo(DateTime.Now.Subtract(start)) > 0)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeExchangeUnlessExcess(ref int check, int value, int maxValue, out int lastValue, IComparable<TimeSpan> timeout, CancellationToken cancellationToken)
        {
            var spinWait = new SpinWait();
            var start = DateTime.Now;
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            lastValue = Volatile.Read(ref check);
            if ((lastValue > maxValue) || (lastValue > maxValue - value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (timeout.CompareTo(DateTime.Now.Subtract(start)) > 0)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeSetBounded(ref int check, int value, int minValue, int maxValue)
        {
            var spinWait = new SpinWait();
            retry:
            var lastValue = Volatile.Read(ref check);
            if ((lastValue < minValue || lastValue > maxValue) || (lastValue + value < minValue || lastValue > maxValue - value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            spinWait.SpinOnce();
            goto retry;
        }

        public static bool SpinWaitRelativeSetBounded(ref int check, int value, int minValue, int maxValue, CancellationToken cancellationToken)
        {
            var spinWait = new SpinWait();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var lastValue = Volatile.Read(ref check);
            if ((lastValue < minValue || lastValue > maxValue) || (lastValue + value < minValue || lastValue > maxValue - value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            spinWait.SpinOnce();
            goto retry;
        }

        public static bool SpinWaitRelativeSetBounded(ref int check, int value, int minValue, int maxValue, int milliseconds)
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }
            if (milliseconds == -1)
            {
                return SpinWaitRelativeSetBounded(ref check, value, minValue, maxValue);
            }
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            var lastValue = Volatile.Read(ref check);
            if ((lastValue < minValue || lastValue > maxValue) || (lastValue + value < minValue || lastValue > maxValue - value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeSetBounded(ref int check, int value, int minValue, int maxValue, int milliseconds, CancellationToken cancellationToken)
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }
            if (milliseconds == -1)
            {
                return SpinWaitRelativeSetBounded(ref check, value, minValue, maxValue);
            }
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var lastValue = Volatile.Read(ref check);
            if ((lastValue < minValue || lastValue > maxValue) || (lastValue + value < minValue || lastValue > maxValue - value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeSetBounded(ref int check, int value, int minValue, int maxValue, TimeSpan timeout)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            var lastValue = Volatile.Read(ref check);
            if ((lastValue < minValue || lastValue > maxValue) || (lastValue + value < minValue || lastValue > maxValue - value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeSetBounded(ref int check, int value, int minValue, int maxValue, TimeSpan timeout, CancellationToken cancellationToken)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var lastValue = Volatile.Read(ref check);
            if ((lastValue < minValue || lastValue > maxValue) || (lastValue + value < minValue || lastValue > maxValue - value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeSetBounded(ref int check, int value, int minValue, int maxValue, IComparable<TimeSpan> timeout)
        {
            var spinWait = new SpinWait();
            var start = DateTime.Now;
            retry:
            var lastValue = Volatile.Read(ref check);
            if ((lastValue < minValue || lastValue > maxValue) || (lastValue + value < minValue || lastValue > maxValue - value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (timeout.CompareTo(DateTime.Now.Subtract(start)) > 0)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeSetBounded(ref int check, int value, int minValue, int maxValue, IComparable<TimeSpan> timeout, CancellationToken cancellationToken)
        {
            var spinWait = new SpinWait();
            var start = DateTime.Now;
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var lastValue = Volatile.Read(ref check);
            if ((lastValue < minValue || lastValue > maxValue) || (lastValue + value < minValue || lastValue > maxValue - value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (timeout.CompareTo(DateTime.Now.Subtract(start)) > 0)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeExchangeBounded(ref int check, int value, int minValue, int maxValue, out int lastValue)
        {
            var spinWait = new SpinWait();
            retry:
            lastValue = Volatile.Read(ref check);
            if ((lastValue < minValue || lastValue > maxValue) || (lastValue + value < minValue || lastValue > maxValue - value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            spinWait.SpinOnce();
            goto retry;
        }

        public static bool SpinWaitRelativeExchangeBounded(ref int check, int value, int minValue, int maxValue, out int lastValue, CancellationToken cancellationToken)
        {
            var spinWait = new SpinWait();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            lastValue = Volatile.Read(ref check);
            if ((lastValue < minValue || lastValue > maxValue) || (lastValue + value < minValue || lastValue > maxValue - value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            spinWait.SpinOnce();
            goto retry;
        }

        public static bool SpinWaitRelativeExchangeBounded(ref int check, int value, int minValue, int maxValue, out int lastValue, int milliseconds)
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }
            if (milliseconds == -1)
            {
                return SpinWaitRelativeExchangeBounded(ref check, value, minValue, maxValue, out lastValue);
            }
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            lastValue = Volatile.Read(ref check);
            if ((lastValue < minValue || lastValue > maxValue) || (lastValue + value < minValue || lastValue > maxValue - value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeExchangeBounded(ref int check, int value, int minValue, int maxValue, out int lastValue, int milliseconds, CancellationToken cancellationToken)
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }
            if (milliseconds == -1)
            {
                return SpinWaitRelativeExchangeBounded(ref check, value, minValue, maxValue, out lastValue);
            }
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            lastValue = Volatile.Read(ref check);
            if ((lastValue < minValue || lastValue > maxValue) || (lastValue + value < minValue || lastValue > maxValue - value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeExchangeBounded(ref int check, int value, int minValue, int maxValue, out int lastValue, TimeSpan timeout)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            lastValue = Volatile.Read(ref check);
            if ((lastValue < minValue || lastValue > maxValue) || (lastValue + value < minValue || lastValue > maxValue - value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeExchangeBounded(ref int check, int value, int minValue, int maxValue, out int lastValue, TimeSpan timeout, CancellationToken cancellationToken)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            var spinWait = new SpinWait();
            var start = TicksNow();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            lastValue = Volatile.Read(ref check);
            if ((lastValue < minValue || lastValue > maxValue) || (lastValue + value < minValue || lastValue > maxValue - value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (Milliseconds(TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeExchangeBounded(ref int check, int value, int minValue, int maxValue, out int lastValue, IComparable<TimeSpan> timeout)
        {
            var spinWait = new SpinWait();
            var start = DateTime.Now;
            retry:
            lastValue = Volatile.Read(ref check);
            if ((lastValue < minValue || lastValue > maxValue) || (lastValue + value < minValue || lastValue > maxValue - value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (timeout.CompareTo(DateTime.Now.Subtract(start)) > 0)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static bool SpinWaitRelativeExchangeBounded(ref int check, int value, int minValue, int maxValue, out int lastValue, IComparable<TimeSpan> timeout, CancellationToken cancellationToken)
        {
            var spinWait = new SpinWait();
            var start = DateTime.Now;
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            lastValue = Volatile.Read(ref check);
            if ((lastValue < minValue || lastValue > maxValue) || (lastValue + value < minValue || lastValue > maxValue - value))
            {
                return false;
            }
            var result = lastValue + value;
            var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            if (timeout.CompareTo(DateTime.Now.Subtract(start)) > 0)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }
    }
}