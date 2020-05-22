using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ThreadSafeDataStructures
{
    public class ParallelStack<E>:ParallelCollection<E>
    {
        ConcurrentStack<E> items_;


        public ParallelStack(IEnumerable<E> items) : this()
        {
            items = new ConcurrentStack<E>(items);
        }
        public ParallelStack()
        {
            items_ = new ConcurrentStack<E>();
        }

        public void Put(E item)
        {
            items_.Push(item);
        }

        public bool TryGet(out E placeHolder)
        {
            return items_.TryPop(out placeHolder);
        }
        public bool IsEmpty()
        {
            return items_.IsEmpty;
        }
    }
}
