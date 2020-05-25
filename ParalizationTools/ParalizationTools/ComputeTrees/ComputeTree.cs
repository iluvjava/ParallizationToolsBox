using ThreadSafeDataStructures;
using System;
using System.Collections.Generic;
using System.Threading;

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
        /// <summary>
        ///     Return all the leafs in the compute tree, under this node, 
        ///     this method be used by the MHComputeNode shrinker to execute tasks
        ///     in parallel in topological order. 
        /// </summary>
        /// <returns>
        ///     A queue containing all the leaf compute node under this particular node. 
        /// </returns>
        Queue<IMHComputeNode> GetAllLeaves();

        /// <summary>
        ///     This method will merge the results obtained from all it's children,
        ///     this is invoked when compute node bubbled all the way up to the front of the
        ///     priority queue.
        /// </summary>
        void Merge();

        /// <summary>
        ///     Get the parent of this compute node.
        /// </summary>
        /// <returns>
        /// </returns>
        IMHComputeNode GetParent();

        /// <summary>
        ///     Decrease the rank, meaning that one of its children has completed its tasks.
        /// </summary>
        /// <returns></returns>
        void DecreaseRank();

        /// <summary>
        ///     The rank is the number of unfinished prerequistive tasks. 
        /// </summary>
        /// <returns>
        ///     An int representing the number of unfinished prerequisite tasks. 
        /// </returns>
        int GetRank();
    }

    public class MHComputeNode : IMHComputeNode
    {

        public int rank_;
        public SortedSet<IMHComputeNode> prereqs_; // The children. 


        public int CompareTo(IMHComputeNode other)
        {
            if (other.GetHashCode() == this.GetHashCode() && object.ReferenceEquals(this, other)) return 0;
            return Math.Sign(this.GetRank() - other.GetRank());
        }

        public void DecreaseRank()
        {
            throw new NotImplementedException();
        }

       public void add

        public Queue<IMHComputeNode> GetAllLeaves()
        {
            throw new NotImplementedException();
        }

        public IMHComputeNode GetParent()
        {
            throw new NotImplementedException();
        }

        public int GetRank()
        {
            throw new NotImplementedException();
        }

        public void Merge()
        {
            throw new NotImplementedException();
        }

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