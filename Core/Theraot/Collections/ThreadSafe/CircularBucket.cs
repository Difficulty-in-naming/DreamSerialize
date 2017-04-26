using System;
using System.Collections.Generic;
using System.Threading;
using Theraot.Core.Theraot.Core;

namespace Theraot.Core.Theraot.Collections.ThreadSafe
{
    /// <summary>
    /// Represents a thread-safe wait-free fixed size circular bucket.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <remarks>
    /// The items added an instance of this type will be overwritten after the entry point reaches the end of the bucket. This class was created for the purpose of storing in-memory logs for debugging threaded software.
    /// </remarks>
    [Serializable]
    public sealed class CircularBucket<T> : IEnumerable<T>
    {
        private readonly int _capacity;
        private readonly FixedSizeBucket<T> _entries;
        private int _index;

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularBucket{T}" /> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public CircularBucket(int capacity)
        {
            _capacity = NumericHelper.PopulationCount(capacity) == 1 ? capacity : NumericHelper.NextPowerOf2(capacity);
            _index = -1;
            _entries = new FixedSizeBucket<T>(_capacity);
        }

        /// <summary>
        /// Gets the capacity.
        /// </summary>
        public int Capacity
        {
            get { return _capacity; }
        }

        /// <summary>
        /// Gets the number of items that has been added.
        /// </summary>
        public int Count
        {
            get { return _entries.Count; }
        }

        /// <summary>
        /// Adds the specified item. May overwrite an existing item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Returns the position where the item was added.</returns>
        public int Add(T item)
        {
            var index = Interlocked.Increment(ref _index) & (_capacity - 1);
            bool isNew;
            _entries.SetInternal(index, item, out isNew);
            return index;
        }

        /// <summary>
        /// Returns an <see cref="System.Collections.Generic.IEnumerator{T}" /> that allows to iterate through the collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator{T}" /> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>
        ///   <c>true</c> if the item was removed; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        public bool RemoveAt(int index)
        {
            return _entries.RemoveAt(index);
        }

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="previous">The previous item in the specified index.</param>
        /// <returns>
        ///   <c>true</c> if the item was removed; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        public bool RemoveAt(int index, out T previous)
        {
            return _entries.RemoveAt(index, out previous);
        }

        global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds the specified item. Will not overwrite existing items.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Returns the position where the item was added, -1 otherwise.</returns>
        public int TryAdd(T item)
        {
            var index = Interlocked.Increment(ref _index) & (_capacity - 1);
            if (_entries.InsertInternal(index, item))
            {
                return index;
            }
            return -1;
        }

        /// <summary>
        /// Tries to retrieve the item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the item was retrieved; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        public bool TryGet(int index, out T value)
        {
            return _entries.TryGet(index, out value);
        }
    }
}