using NUnit.Framework;
using ParalizationTools.ThreadSafeDataStructures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThreadSafeDataStructures;

namespace BasicTests
{
    class Experiments
    {
        [Test]
        /// <summary>
        ///     What if you wait for a task before it starts????? 
        /// </summary>
        public void TestUnStartedtask()
        {
            Task t = new Task(
                    () => {
                        // totally useless. 
                        return;
                    }
                );

            Thread tt = new Thread(
                    () => {
                        Thread.Sleep(5000);
                        t.Start();
                    }
                );

            tt.Start();
            t.Wait(); ;
            Console.WriteLine("Finished");
        }
    }

    public class ComputeNode : IComparable<ComputeNode>
    {
        int level_;


        public Task branching_ = new Task(() => { });
        public Task resultsReady = new Task(() => { });
       
        public int result_;
        public Queue<ComputeNode> children_;
        public ComputeNode parent_;
        public int rank_ = 0;

        public ComputeNode(int level)
        {
            level_ = level;
        }

        public int Compute()
        {
            if (level_ == 0)
            {
                DoWork(100);
                // Under the base case, no wait. 

                return 1;
            }

            ComputeNode sub1 = new ComputeNode(level_ - 1);
            ComputeNode sub2 = new ComputeNode(level_ - 1);

            this.AddChild(sub1);
            this.AddChild(sub2);

            WaitForIt(); // the meothod has to halt client code!!!!!

            return sub1.GetResult() + sub2.GetResult();
        }


        protected void AddChild(ComputeNode sub1)
        {
            children_ = children_ is null ? new Queue<ComputeNode>(): children_;
            this.children_.Enqueue(sub1);
            sub1.RegisterParent(this);
            rank_++;
        }

        protected void RegisterParent(ComputeNode computeNode)
        {
            parent_ = computeNode;
        }

        /// <summary>
        ///     This method is pivotal. 
        /// </summary>
        protected void WaitForIt()
        {
            branching_.Start();
            resultsReady.Wait();
        }

        public int GetResult()
        {
            return result_;
        }

        public static void DoWork(int amount)
        {
            int sum = 0;
            for (int I = 0; I < amount; I++)
            {
                sum += I;
            }
        }

        public int CompareTo(ComputeNode other)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Let's see...
    /// </summary>
    public class ComputeTree
    {
        ComputeNode root_;
        ParallelCollection<ComputeNode> nodesForBranchingOut_;
        ParallelCollection<ComputeNode> leafs_; 

        public ComputeTree(ComputeNode root)
        {
            root_ = root;
            nodesForBranchingOut_ = new ParallelStack<ComputeNode>();
            leafs_ = new ParallelPriorityQueue<ComputeNode>();
        }

        public void RunParallel()
        {
            // Branching out. 
            {

                int processors = Environment.ProcessorCount;
                while (processors-- > 0)
                {
                    ComputeNode thatNode = null;
                    if (!nodesForBranchingOut_.TryGet(out thatNode))
                    {
                        break;
                    }
                }

                Thread[] threads = new Thread[processors];
                for (int I = 0; I < processors; I++)
                {
                    threads[I] = new Thread(
                            () => {
                                while (true)
                                {
                                    ComputeNode thatNode = null;
                                    if (!nodesForBranchingOut_.TryGet(out thatNode))
                                    {
                                        break;
                                    }
                                }
                            }
                        );
                }

            }

            // Merging in. 
            { 
            
            
            
            }

        
        }

        public void BranchOutNode(ComputeNode node)
        {
            node.Compute();
            node.branching_.Wait();
            if (node.children_ is null)
            {
                leafs_.Put(node);
                return;
            }

            foreach (ComputeNode n in node.children_)
            {
                nodesForBranchingOut_.Put(n);
            }
        }
    }


}
