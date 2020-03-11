using System.Collections.Generic;

namespace EGoap.Source.Utils.Collections
{
    public interface IPriorityQueue<T> : ICollection<T>
    {
        T Front { get; }

        T PopFront();
    }
}