using ParalizationTools.ThreadSafeDataStructures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThreadSafeDataStructures;

namespace ParalizationTools.ComputeTrees
{
    /// <summary>
    ///     Give a well defined compute tree, this method will compute the 
    ///     compute tree, branching and merging it
    /// </summary>
    /// <typeparam name="T">
    ///     The return type of the compute node. 
    /// </typeparam>
    public class MHComputeNodeEvaluator<T>
    {
        ParallelCollection<IMHComputeNode<T>> forBranching_;
        MergeHCNodePQ<T> topologicalQueue;
        IMHComputeNode<T> root_;

        public MHComputeNodeEvaluator(MHComputeNode<T> root)
        {
            root_ = root;
            forBranching_ = new ParallelStack<IMHComputeNode<T>>();
            forBranching_.Put(root);
           
        }


        public void Compute()
        {
            ParallelBranch();
            topologicalQueue = new MergeHCNodePQ<T>(root_);
            ParallelMerge();
        }

        protected void ParallelBranch()
        {
            int processor = Environment.ProcessorCount;
            int loopCount = processor;
            while (loopCount-- > 0)
            {
                IMHComputeNode<T> n = null;
                if (!forBranching_.TryGet(out n))
                {
                    return;
                };
                BranchNode(n);
            }

            Thread[] threads = new Thread[processor];
            for (int I = 0; I < processor; I++)
            {
                threads[I] = new Thread(
                        () => {
                            while (true)
                            {
                                IMHComputeNode<T> n = null;
                                if (!forBranching_.TryGet(out n))
                                {
                                    break;
                                };
                                BranchNode(n);
                            }
                        }
                    );
                threads[I].Name = $"MHComputeTree Branching: {I}";
                threads[I].Start();
            }
            foreach (Thread t in threads) t.Join();
        }

        protected void ParallelMerge()
        {
            int processor = Environment.ProcessorCount;

            Thread[] threads = new Thread[processor];
            for (int I = 0; I < processor; I++)
            {
                threads[I] = new Thread(
                        () => {
                            while (true)
                            {
                                IMHComputeNode<T> n = null;
                                if (!topologicalQueue.TryGet(out n))
                                {
                                    break;
                                };
                                MergeThatNode(n);
                            }
                        }
                    );
                threads[I].Name = $"MHComputeTree Branching: {I}";
                threads[I].Start();
            }
            foreach (Thread t in threads) t.Join();
        }

        protected void BranchNode(IMHComputeNode<T> n)
        {
            n.Branch();
            foreach (IMHComputeNode<T> child in n.GetChildren())
            {
                forBranching_.Put(child);
            }
           
        }

        protected void MergeThatNode(IMHComputeNode<T> node)
        {
            // prereq
            foreach (IMHComputeNode<T> n in node.GetChildren())
            {
                Task f = topologicalQueue.GetFlagOf(n);
                if (f.IsCompleted)
                {
                    continue;
                }
                f.Wait();
            }

            node.Merge();
            topologicalQueue.GetFlagOf(node).Start();
            
        }
    }

}
