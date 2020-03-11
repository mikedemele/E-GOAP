using System;
using System.Collections;
using System.Collections.Generic;

namespace EGoap.Source.Utils.Collections
{
	// Priority queue implementation, based on the underlying List
    public class ListBasedPriorityQueue<T> : IPriorityQueue<T>
    {
        private readonly IComparer<T> comparer;
        private readonly List<T> list;

        public ListBasedPriorityQueue()
        {
            list = new List<T>();
            comparer = Comparer<T>.Default;
        }

        public ListBasedPriorityQueue(IComparer<T> comparer)
        {
            list = new List<T>();
        }

        #region IEnumerable implementation

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion

        #region IEnumerable implementation

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) list).GetEnumerator();
        }

        #endregion

        #region ICollection implementation

        public void Add(T item)
        {
            var insertAt = 0;
            for (; insertAt < list.Count && comparer.Compare(item, list[insertAt]) > 0; insertAt++)
            {
            }

            list.Insert(insertAt, item);
        }

        public void Clear()
        {
            list.Clear();
        }

        public bool Contains(T item)
        {
            return list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return list.Remove(item);
            // Removal does not upset the order
        }

        public int Count => list.Count;

        public bool IsReadOnly => false;

        #endregion

        #region IPriorityQueue implementation

        public T PopFront()
        {
            var poppedValue = Front;
            list.RemoveAt(0);
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
                return list[0];
            }
        }

        #endregion
    }
}