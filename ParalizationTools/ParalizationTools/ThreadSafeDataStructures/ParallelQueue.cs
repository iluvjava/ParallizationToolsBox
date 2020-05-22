using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ThreadSafeDataStructures
{
    public class ParallelQueue<E> : ParallelCollection<E>
    {
        ConcurrentQueue<E> items_;


        public ParallelQueue(IEnumerable<E> items) : this()
        {
            items = new ConcurrentQueue<E>(items);
        }
        public ParallelQueue()
        {
            items_ = new ConcurrentQueue<E>();
        }

        public void Put(E item)
        {
            items_.Enqueue(item);
        }

        public bool TryGet(out E placeHolder)
        {
            return items_.TryDequeue(out placeHolder);
        }
        public bool IsEmpty()
        {
            return items_.IsEmpty;
        }
    }
}
