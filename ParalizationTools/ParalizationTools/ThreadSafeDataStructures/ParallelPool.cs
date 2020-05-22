using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ThreadSafeDataStructures
{

    /// <summary>
    ///     A dequeue that supports Queue and Stack operations.
    ///     * Thread safe! 
    ///     * Non Nullable! 
    /// </summary>
    /// <typeparam name="E"></typeparam>
    public class ParallelPool<E> : ParallelCollection<E>
    {
        ConcurrentBag<E> items_;
        public ParallelPool(IEnumerable<E> items): this()
        {
            items = new ConcurrentBag<E>(items);
        }
        public ParallelPool()
        {
            items_ = new ConcurrentBag<E>();
        }

        public void Put(E item)
        {
            items_.Add(item);
        }

        public bool TryGet(out E placeHolder)
        {
            return items_.TryTake(out placeHolder);
        }

        public bool IsEmpty()
        {
            return items_.IsEmpty;
        }
    }
}
