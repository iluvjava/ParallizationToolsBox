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



    }


    public class BranchingNode : BranchHeavyComputeNode
    {

        BranchingNode left_;
        BranchingNode right_;

        int height_;

        public BranchingNode(int height = 25)
        {
            height_ = height;
        }

        public override Queue<BranchHeavyComputeNode> Branch()
        {
            if (height_ == 0)
            {
                UselessWorks();
                return null; // a leaf node, not branching allowed. 
            }
            UselessWorks();
            left_ = new BranchingNode(height_ - 1);
            right_ = new BranchingNode(height_ - 1);
            Queue<BranchHeavyComputeNode> q = new Queue<BranchHeavyComputeNode>();
            q.Enqueue(left_);
            q.Enqueue(right_);
            return q;
        }

        protected void UselessWorks()
        {
            // int useLess = 0;
            // for (int I = 0; I < 0b1_00000000_00000000; I++)
            // {
            //     useLess += I;
            // }

        }


    }


}