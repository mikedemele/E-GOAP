using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EGoap.Source.Utils.Collections
{
    public class SmartPriorityQueue<TItem, TPriority> : IPriorityQueue<TItem> where TItem : IPriorityItem<TPriority>
    {
        private readonly IComparer<TItem> comparer;
        internal readonly SortedDictionary<TPriority, List<TItem>> subQueues;
        
        public SmartPriorityQueue(IComparer<TPriority> priorityComparer)
        {
            subQueues = new SortedDictionary<TPriority, List<TItem>>(priorityComparer);
        }
 
        public SmartPriorityQueue() : this(Comparer<TPriority>.Default) { }

        private void AddQueueOfPriority(TPriority priority)
        {
            subQueues.Add(priority, new List<TItem>());
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            return (IEnumerator<TItem>) ((IEnumerable)this).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new SmartPriorityQueueEnumerator<TItem, TPriority>(this);
        }

        private bool Any => subQueues.Any();

        public void Add(TItem item)
        {
            System.Diagnostics.Debug.Assert(item != null, nameof(item) + " != null");
            var priority = item.GetPriority();
            if (!subQueues.ContainsKey(priority))
            {
                AddQueueOfPriority(priority);
            }

            subQueues[priority].Add(item);
        }

        public void Clear()
        {
            subQueues.Clear();
        }

        public bool Contains(TItem item)
        {
            return subQueues.Any(queue => queue.Value.Contains(item));
        }

        public void CopyTo(TItem[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(TItem item)
        {
            return subQueues.Any(queue => queue.Value.Remove(item));
        }

        public int Count => subQueues.Sum(queue => queue.Value.Count);

        public bool IsReadOnly => false;
        
        public TItem Front
        {
            get
            {
                if (!Any)
                {
                    throw new InvalidOperationException(
                        $"Attempted to retrieve Front from an empty {GetType()}"
                    );
                }

                return subQueues.First().Value.First();
            }
        } 
        
        public TItem PopFront()
        {
            if (subQueues.Any())
                return DequeueFromHighPriorityQueue();
            throw new InvalidOperationException("The queue is empty");
        }
        
        private TItem DequeueFromHighPriorityQueue()
        {
            var first = subQueues.First();
            var nextItem = first.Value.First();
            first.Value.RemoveAt(0);
            if (!first.Value.Any())
            { 
                subQueues.Remove(first.Key);
            }
            return nextItem;
        }
    }

    public class SmartPriorityQueueEnumerator<TItem, TPriority> : IEnumerator where TItem : IPriorityItem<TPriority>
    {
        private readonly SmartPriorityQueue<TItem, TPriority> priorityQueue;
        private int position = -1;

        public SmartPriorityQueueEnumerator(SmartPriorityQueue<TItem, TPriority> smartPriorityQueue)
        {
            priorityQueue = smartPriorityQueue;
        }
        
        public bool MoveNext()
        {
            position++;
            return (position < priorityQueue.Count);
        }

        public void Reset()
        {
            position = -1;
        }

        public object Current
        {
            get
            {
                try
                {
                    var count = 0;
                    foreach (var queue in priorityQueue.subQueues)
                    {
                        if (position < count + queue.Value.Count)
                        {
                            return queue.Value[position - count];
                        }
                        count += queue.Value.Count;
                    }
                    throw new IndexOutOfRangeException();
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}