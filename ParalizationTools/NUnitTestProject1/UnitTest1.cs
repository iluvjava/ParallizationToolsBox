using NUnit.Framework;
using TaskRunners;
using System;
using System.Threading.Tasks;
using ParalizationTools;
using System.Collections.Generic;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestBranchHeavyNode()
        {
            BranchingNode root = new BranchingNode();
            ComputeNodeSpawner sp = new ComputeNodeSpawner(root);

            sp.SpawnParallel();
        }


        [Test]
        public void TestSimpleBranch()
        {
            BranchingNode root = new BranchingNode();
            root.SimpleBranch();
        }


    }


    public class BranchingNode : BranchHeavyComputeNode
    {

        BranchingNode left_;
        BranchingNode right_;

        int height_;

        public BranchingNode(int height = 16)
        {
            height_ = height;
        }

        public override Queue<BranchHeavyComputeNode> Branch()
        {
            if (height_ == 0)
            {
                UselessWorks(1024);
                return null; // a leaf node, not branching allowed. 
            }
            UselessWorks(65536);
            left_ = new BranchingNode(height_ - 1);
            right_ = new BranchingNode(height_ - 1);
            Queue<BranchHeavyComputeNode> q = new Queue<BranchHeavyComputeNode>();
            q.Enqueue(left_);
            q.Enqueue(right_);
            return q;
        }

        public void SimpleBranch()
        {
            if (height_ == 0)
            {
                UselessWorks(1024);
                return;  // a leaf node, not branching allowed. 
            }
            UselessWorks(65536);
            left_ = new BranchingNode(height_ - 1);
            right_ = new BranchingNode(height_ - 1);
            left_.SimpleBranch();
            right_.SimpleBranch();

        }

        protected void UselessWorks(int workSize)
        {
            int useLess = 0;
            for (int I = 0; I < workSize; I++)
            {
                useLess += I;
            }

        }


    }


}