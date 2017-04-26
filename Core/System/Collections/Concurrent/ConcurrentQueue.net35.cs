﻿#if NET20 || NET30 || NET35

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Theraot.Core.System.Collections.Generic;
using Theraot.Core.System.Linq;
using Theraot.Core.Theraot.Collections;
using Theraot.Core.Theraot.Collections.ThreadSafe;

namespace Theraot.Core.System.Collections.Concurrent
{
    [Serializable]
    [ComVisible(false)]
    [DebuggerDisplay("Count = {Count}")]
    [HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
    public class ConcurrentQueue<T> : IProducerConsumerCollection<T>, IReadOnlyCollection<T>
    {
        private readonly SafeQueue<T> _wrapped;

        public ConcurrentQueue()
        {
            _wrapped = new SafeQueue<T>();
        }

        public ConcurrentQueue(IEnumerable<T> collection)
        {
            _wrapped = new SafeQueue<T>(collection);
        }

        public int Count
        {
            get { return _wrapped.Count; }
        }

        public bool IsEmpty
        {
            get { return _wrapped.Count == 0; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { throw new NotSupportedException(); }
        }

        public void CopyTo(T[] array, int index)
        {
            Extensions.CanCopyTo(Count, array, index);
            Extensions.CopyTo(this, array, index);
        }

        public void Enqueue(T item)
        {
            _wrapped.Add(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _wrapped.GetEnumerator();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            Extensions.CanCopyTo(Count, array, index);
            this.DeprecatedCopyTo(array, index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool IProducerConsumerCollection<T>.TryAdd(T item)
        {
            _wrapped.Add(item);
            return true;
        }

        bool IProducerConsumerCollection<T>.TryTake(out T item)
        {
            return _wrapped.TryTake(out item);
        }

        public T[] ToArray()
        {
            return _wrapped.ToArray();
        }

        public bool TryDequeue(out T result)
        {
            return _wrapped.TryTake(out result);
        }

        public bool TryPeek(out T result)
        {
            return _wrapped.TryPeek(out result);
        }
    }
}

#endif