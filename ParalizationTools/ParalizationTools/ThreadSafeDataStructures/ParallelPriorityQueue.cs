using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using ThreadSafeDataStructures;

namespace ParalizationTools.ThreadSafeDataStructures
{
    /// <summary>
    ///     All the element in this prioirty queue will be in order, ascending order.
    ///     I will lock a SortedSet to achieve this. 
    ///     
    ///     * Changing priority is just removing and adding the same element. 
    ///       !!! The client must make sure to refresh the dynamic element themselve, it's 
    ///       not up to this class to handle the dynamic re ordering of elements. 
    ///     
    ///     * When it's viewed as a parallel collection, this is going to return minimum 
    ///     element in the queue. 
    /// </summary>
    /// 
    /// <typeparam name="E">
    ///     The type of the elements that goes into the queue. 
    /// </typeparam>
    class ParallelPriorityQueue<E> : ParallelCollection<E> where E : IComparable<E>
    {
        protected SortedSet<E> elements_;
        public ParallelPriorityQueue()
        {
            elements_ = new SortedSet<E>(); 
        }

        /// <summary>
        ///     this method is not good for multi-thread, take it with 
        ///     a bag salt. 
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return elements_.Count == 0; 
        }

        public void Put(E item)
        {
            lock (elements_) { elements_.Add(item); }
        }

        public bool TryGet(out E placeHolder)
        {
            return TryGetMinOrMax(out placeHolder, false);
        }

        public bool TryGetMinOrMax(out E placeHolder, bool getMax)
        {
            lock (elements_)
            {
                placeHolder = default;
                if (elements_.Count == 0) return false;
                if (getMax) 
                {
                    placeHolder = elements_.Max;
                    return true;
                }
                placeHolder = elements_.Min;
                return true;
            }
        }

        public bool TryGetMax(out E placeHolder)
        {
            return TryGetMinOrMax(out placeHolder, true);
        }

        /// <summary>
        ///     Remove the dynamic element and then add it back to the queue. 
        /// </summary>
        /// <param name="item"></param>
        public void ChangePriority(E item)
        {
            lock (elements_)
            {
                if (elements_.Contains(item)) throw new Exception("Invalid operation. ");
                elements_.Remove(item);
                elements_.Add(item);
            }
        }
    }
}
