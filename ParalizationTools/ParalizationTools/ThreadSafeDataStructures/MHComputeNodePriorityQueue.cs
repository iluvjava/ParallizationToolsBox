using BenchMarkingWithPrimeNumbers;
using ParalizationTools.ComputeTrees;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThreadSafeDataStructures;

namespace ParalizationTools.ThreadSafeDataStructures
{
    /// <summary>
    ///     Accept the Node of a MHComputeTree. 
    /// </summary>
    /// <typeparam name="T">
    ///     The return type of the compute node. 
    /// </typeparam>
    class MHComputeNodePriorityQueue<T>
    {

        ParallelStack<IMHComputeNode<T>> topologicalOrder_;
        Dictionary<IMHComputeNode<T>, Task> flags_;

        public MHComputeNodePriorityQueue(IMHComputeNode<T> root)
        {
            Queue<IMHComputeNode<T>> prevLevel = new Queue<IMHComputeNode<T>>();
            prevLevel.Enqueue(root);
            Queue<IMHComputeNode<T>> nextLevel = new Queue<IMHComputeNode<T>>();
            topologicalOrder_ = new ParallelStack<IMHComputeNode<T>>();
            flags_ = new Dictionary<IMHComputeNode<T>, Task>();

            // Topological ordering
            while (true)
            {
                foreach (IMHComputeNode<T> parent in prevLevel)
                {
                    foreach (IMHComputeNode<T> n in parent.GetChildren())
                    {
                        nextLevel.Enqueue(n);
                    }
                    topologicalOrder_.Put(parent); // this is for topological order.
                    flags_[parent] = new Task(() => { }); // Empty task for waiting. 
                }
                if (nextLevel.Count == 0) break;
                prevLevel = nextLevel;
                nextLevel = new Queue<IMHComputeNode<T>>();
            }
        }

        /// <summary>
        ///     Get an IMHCompuetNode from the priority queue. 
        /// </summary>
        /// <param name="placeHolder">
        ///     null, it should always be null. 
        /// </param>
        /// <returns></returns>
        public bool TryGet(out IMHComputeNode<T> placeHolder)
        {
            return topologicalOrder_.TryGet(out placeHolder);
        }

        /// <summary>
        ///     Get the flag and modify it in the compute tree. 
        /// </summary>
        /// <param name="node">
        ///     The node. 
        /// </param>
        /// <returns>
        ///     A task, just for the purpose of letting a thread to wait for
        ///     it to finish. 
        /// </returns>
        public Task GetFlagOf(IMHComputeNode<T> node)
        {
            return flags_[node]; 
        }


    }


   
}
