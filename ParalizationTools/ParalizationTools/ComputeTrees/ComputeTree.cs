using ThreadSafeDataStructures;
using System;
using System.Collections.Generic;
using System.Threading;
using System.ComponentModel;

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
    public interface IMHComputeNode : IComparable<IMHComputeNode>
    {
       
    }

    public abstract class MHComputeNode: IComparer<MHComputeNode>
    {
        protected MHComputeNode parent_;
        protected Queue<MHComputeNode> children_;
        protected int Rank_; 
        
        /// <summary>
        ///     Branch your compute Node. 
        /// </summary>
        /// <returns></returns>
        public abstract void Branch();

        public int Compare(MHComputeNode x, MHComputeNode y)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Register a children to this compute node. 
        /// </summary>
        /// <param name="child"></param>
        protected void AddChildren(MHComputeNode child)
        { 
            
        }

        /// <summary>
        ///     Merge the compute results. 
        /// </summary>
        public abstract void Merge(); 
    }


    public abstract class ComputeNodeNodeSpawner
    {
        protected IBHComputeNode root_;

        // cannot contain null!!!
        protected ParallelCollection<IBHComputeNode> toBranch_;

        public ComputeNodeNodeSpawner(IBHComputeNode root)
        {
            root_ = root;
            // Specify data structure for putting in compute node in the super class. 
        }

        public void SpawnParallel()
        {
            int processors = Environment.ProcessorCount;
            int initialNodeSize = 1;
            while (!toBranch_.IsEmpty() && initialNodeSize++ <= processors)
            {
                IBHComputeNode node = null;
                if (!toBranch_.TryGet(out node))
                {
                    return;
                };
                Queue<IBHComputeNode> moreNodes = node.Branch();
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
                                IBHComputeNode n = null;
                                if (!toBranch_.TryGet(out n)) return;
                                Queue<IBHComputeNode> moreNodes = n.Branch();
                                if (moreNodes is null) continue; // computenode is a leaf 
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

        protected void AddMoreNode(Queue<IBHComputeNode> n)
        {
            lock (toBranch_)
            {
                foreach (IBHComputeNode node in n)
                {
                    toBranch_.Put(node);
                }
            }
        }

    }

    /// <summary>
    ///     This is a class that expand a BranchHeavyConputeNode in a BFS mannner, it does the following:
    ///         * expand and execute your branching graph in parallel.
    ///         
    ///         
    /// </summary>
    public class BHBFSComputeNodeSpawner: ComputeNodeNodeSpawner
    {
        public BHBFSComputeNodeSpawner(IBHComputeNode root) : base(root)
        {
            toBranch_ = new ParallelQueue<IBHComputeNode>();
            toBranch_.Put(root_);
        }
    }

    /// <summary>
    ///     This is a class that expand a BranchHeavyConputeNode in a DFS mannner, it does the following:
    ///         * expand and execute your branching graph in parallel.
    ///         
    ///         
    /// </summary>
    public class BHDFSComputeNodeSpawner : ComputeNodeNodeSpawner
    {
        public BHDFSComputeNodeSpawner(IBHComputeNode root) : base(root)
        {
            toBranch_ = new ParallelStack<IBHComputeNode>();
            toBranch_.Put(root_);
        }
    }



    public class MHComputeNodeShrinker
    {

    }
}