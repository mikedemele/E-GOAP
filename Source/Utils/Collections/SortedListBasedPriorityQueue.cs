using System;
using System.Collections;
using System.Collections.Generic;

namespace EGoap.Source.Utils.Collections
{
	// Priority queue implementation, based on the underlying SortedList
    // SortedListBasedPriorityQueue does NOT allow duplicate items!
	public class SortedListBasedPriorityQueue<T> : IPriorityQueue<T>
    {
        private readonly SortedList<T, object> sortedList;

        public SortedListBasedPriorityQueue()
        {
            sortedList = new SortedList<T, object>();
        }

        public SortedListBasedPriorityQueue(IComparer<T> comparer)
        {
            sortedList = new SortedList<T, object>(comparer);
        }

        #region IEnumerable implementation

        public IEnumerator<T> GetEnumerator()
        {
            return sortedList.Keys.GetEnumerator();
        }

        #endregion

        #region IEnumerable implementation

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) sortedList.Keys).GetEnumerator();
        }

        #endregion

        #region ICollection implementation

        public void Add(T item)
        {
            System.Diagnostics.Debug.Assert(item != null, nameof(item) + " != null");
            sortedList.Add(item, null);
        }

        public void Clear()
        {
            sortedList.Clear();
        }

        public bool Contains(T item)
        {
            return item != null && sortedList.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            sortedList.Keys.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return item != null && sortedList.Remove(item);
        }

        public int Count => sortedList.Count;

        public bool IsReadOnly => false;

        #endregion

        #region IPriorityQueue implementation

        public T PopFront()
        {
            var poppedValue = Front;
            sortedList.RemoveAt(0);
            return poppedValue;
        }

        public T Front
        {
            get
            {
                if (Count == 0)
                    throw new InvalidOperationException(
                        $"Attempted to retrieve Front from an empty {GetType()}"
                    );
                return sortedList.Keys[0];
            }
        }

        #endregion
    }
}