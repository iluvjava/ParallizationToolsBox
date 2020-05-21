using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ComputeTree
{
    
    /// <summary>
    ///     A branch heavy computede models recursive problem that has more works 
    ///     before recursing
    /// </summary>
    public interface IBHComputeNode
    {
        /// <summary>
        ///     Brranch out your children compute node, you have the option to 
        ///     branch all the way to the leaf (not recommended cause this is not going to be 
        ///     used by the paralization tools )
        ///     , or somewhere between, it depends on the 
        ///     problem size of the branching problem. 
        /// </summary>
        /// <param name="pipe"></param>
        Queue<IBHComputeNode> Branch();

    }

    /// <summary>
    ///     A merge heavy compute node is for recursive algorithms that has non
    ///     trivial tasks when merging the solution after recurring.
    /// </summary>
    public interface IMergeHeavyComputeNode : IComparable<IMergeHeavyComputeNode>
    {
        /// <summary>
        ///     Get the parent of this compute node. 
        /// </summary>
        /// <returns>
        /// </returns>
        IMergeHeavyComputeNode GetParent();

        /// <summary>
        ///     A merge heavy compute node will need to branch, but it needs to remmbers its parent. 
        /// </summary>
        /// <param name="n"></param>
        void RegisterParentComputeNode(IMergeHeavyComputeNode n);

        /// <summary>
        ///     Decrease the rank, meaning that one of its children has completed its tasks. 
        /// </summary>
        /// <returns></returns>
        void DecreaseRank();
    }

    /// <summary>
    ///     This is a class that expand a BranchHeavyConputeNode, it does the following: 
    ///         * expand and execute your branching graph in parallel. 
    /// </summary>
    public class BHComputeNodeSpawner
    {

        IBHComputeNode root_;

        // cannot contain null!!!
        Queue<IBHComputeNode> toBranch_;

        public BHComputeNodeSpawner(IBHComputeNode root)
        {
            root_ = root;
            toBranch_ = new Queue<IBHComputeNode>();
            toBranch_.Enqueue(root_);
        }

        public void SpawnParallel()
        {
            int processors = Environment.ProcessorCount;
            while (toBranch_.Count <= processors && toBranch_.Count > 0)
            {
                Queue<IBHComputeNode> moreNodes = toBranch_.Dequeue().Branch();
                if (moreNodes is null) continue;
                AddMoreNode(moreNodes);
            }
            
            Thread[] threads = new Thread[processors];

            for (int I = 0; I < threads.Length; I++)
            {
                threads[I] = new Thread(
                        () =>
                        {
                            while (true)
                            {
                                IBHComputeNode n = GetNextBranching();
                                if (n is null) break; // shared works all doned. 
                                Queue<IBHComputeNode> moreNodes = n.Branch();
                                if (moreNodes is null) continue; // is a leaf. 
                                AddMoreNode(moreNodes);
                            }

                        }
                    );
            }

            for (int I = 0; I < threads.Length; I++)
            {
                threads[I].Start();
            }

            for (int I = 0; I < threads.Length; I++)
            {
                threads[I].Join();
            }
        }

        public IBHComputeNode GetNextBranching()
        {
            lock (toBranch_)
            {
                if (toBranch_.Count == 0)
                {
                    return null; 
                }
                return toBranch_.Dequeue();

            }
        }

        protected void AddMoreNode(Queue<IBHComputeNode> n)
        {
            lock (toBranch_)
            {
                foreach (IBHComputeNode node in n)
                {
                    toBranch_.Enqueue(node);
                }
            }
        }
    }



}
