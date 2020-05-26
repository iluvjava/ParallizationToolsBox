using NUnit.Framework;
using ParalizationTools.ComputeTrees;
using ParalizationTools.ThreadSafeDataStructures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using ThreadSafeDataStructures;

namespace Experiments2
{

    public class TestThisIdea
    {
        [Test]
        public void TestBHComputeTree()
        {
            ExampleBHComputeNode root = new ExampleBHComputeNode(22);
            MHComputeNodeEvaluator<int> tree = new MHComputeNodeEvaluator<int>(root);
            tree.Compute();
            Console.WriteLine(root.GetResult());
        }

        public class ExampleBHComputeNode : MHComputeNode<int>
        {
            int level_;
            int result_;
            string path_;

            public ExampleBHComputeNode(int level, string path = "T")
            {
                level_ = level;
                path_ = path; 
            }

            public override void Branch()
            {
                if (level_ == 0)
                {
                    SomeUselessWorks(1024);
                    result_ = 1;
                    return;
                }
                AddChild(new ExampleBHComputeNode(level_ - 1, path_ + "1"));
                AddChild(new ExampleBHComputeNode(level_ - 1, path_ + "2")); 
            }

            public override int GetResult()
            {
                return result_; 
            }

            public override void Merge()
            {
                foreach (MHComputeNode<int> child in GetChildren())
                {
                    result_ += child.GetResult();
                }
                SomeUselessWorks(1024);
            }

            public void SomeUselessWorks(int amount)
            {
                int sum = 0;
                for (int I = 0; I < amount; I++)
                {
                    sum += I;
                }
            }

            override
            public string ToString()
            {
                return path_;
            }
        }

    }
    
   
}
