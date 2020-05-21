using System;
using System.Collections.Generic;
using System.Text;

namespace ParallelAccessPool
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    namespace ParalizationTools
    {

        /// <summary>
        ///     It's a pool of task, good for multi-threads accessing the tasks without locking each other
        ///     The retrival of element is random. 
        ///     * It has a list of queues to retrieve elements from, this minimizes thread locking in 
        ///     a multi-thread environment.
        /// </summary>
        /// <typeparam name="E">
        ///     It can contain element of type E
        /// </typeparam>
        class TaskPool<E>
        {
            Queue<E>[] queueList_;
            private int previousAccess_ = 0;

            public TaskPool(int poolSize = 4)
            {
                queueList_ = new Queue<E>[poolSize];
                for (int I = 0; I < poolSize; I++) queueList_[I] = new Queue<E>();
            }

            public void Add(IEnumerable<E> taskIterator)
            {
                foreach (E e in taskIterator) queueList_[NextQueue()].Enqueue(e);
            }

            /// <summary>
            ///     Retrieve an element, pass in a reference, and the method will modify it 
            ///     to the next element. 
            ///     * If the reference is not modified, then there are no more tasks left in 
            ///     the Pool. 
            /// </summary>
            /// <param name="placeHolder">
            /// 
            /// </param>
            public void TryGet(ref E placeHolder)
            {
                int failedProbing = 0;
                while (failedProbing++ < queueList_.Length)
                {
                    lock (this)
                    {
                        int nextQueueId = NextQueue();
                        if (queueList_[nextQueueId].Count == 0)
                            continue;
                        placeHolder = queueList_[nextQueueId].Dequeue();
                    }
                }
            }

            protected int NextQueue()
            {
                previousAccess_ = previousAccess_ == queueList_.Length - 1 ? 0 : previousAccess_ + 1;
                return previousAccess_;
            }



        }
    }

}
