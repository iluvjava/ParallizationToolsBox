using System;
using System.Collections.Generic;
using System.Threading;

namespace ComputeTree
{
    /// <summary>
    ///     A branch heavy compute node representing the recursive algorithms that has
    ///     a non-trivial task before recurring.
    /// </summary>
    public abstract class BranchHeavyComputeNode
    {
        /// <summary>
        ///     Brranch out your children compute node, you have the option to
        ///     branch all the way to the leaf, or somewhere between, it depends on the
        ///     problem size of the branching problem.
        /// </summary>
        /// <param name="pipe">
        ///
        /// </param>
        public abstract Queue<BranchHeavyComputeNode> Branch();
    }

    /// <summary>
    ///     A merge heavy compute node is for recursive algorithms that has non
    ///     trivial tasks when merging the solution after recurring.
    /// </summary>
    public abstract class MergeHeavyComputeNode
    {
        protected MergeHeavyComputeNode itsParent_;
        protected int numberOfPrereq; // number of uncompleted children compute nodes. 

        /// <summary>
        ///     This method registers the parent of the compute node, it's essential
        ///     for the Compute tree, otherwise, it's parent compute node won't
        ///     get executed at all when this compute node completed its tasks.
        /// </summary>
        /// <param name="parent"></param>
        public void Branch(MergeHeavyComputeNode parent)
        {
            itsParent_ = parent;
        }

        /// <summary>
        ///     This method register total number of children this compute node has. 
        /// </summary>
        /// <param name="total"></param>
        public void RegisterTotalNumberOfChildren(int total)
        { 
            
        }
    }

    /// <summary>
    ///     This is a class that expand a BranchHeavyConputeNode, it does the following:
    ///         * expand and execute your branching graph in parallel.
    /// </summary>
    public class BHComputeNodeSpawner
    {
        private BranchHeavyComputeNode root_;

        // cannot contain null!!!
        private Queue<BranchHeavyComputeNode> toBranch_;

        public BHComputeNodeSpawner(BranchHeavyComputeNode root)
        {
            root_ = root;
            toBranch_ = new Queue<BranchHeavyComputeNode>();
            toBranch_.Enqueue(root_);
        }

        public void SpawnParallel()
        {
            int processors = Environment.ProcessorCount;
            while (toBranch_.Count <= processors && toBranch_.Count > 0)
            {
                Queue<BranchHeavyComputeNode> moreNodes = toBranch_.Dequeue().Branch();
                if (moreNodes is null) continue; // leaf
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
                                BranchHeavyComputeNode n = GetNextBranching();
                                if (n is null) break; // shared works all doned.
                                Queue<BranchHeavyComputeNode> moreNodes = n.Branch(); // actual works
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

        public BranchHeavyComputeNode GetNextBranching()
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

        protected void AddMoreNode(Queue<BranchHeavyComputeNode> n)
        {
            lock (toBranch_)
            {
                foreach (BranchHeavyComputeNode node in n)
                {
                    toBranch_.Enqueue(node);
                }
            }
        }
    }


}