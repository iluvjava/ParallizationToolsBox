using ComputeTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Parallelization_Examples
{
    class BruteForcingPrimes : IBHComputeNode
    {
        public static int BaseTaskSize = 65536;
        public static int[] PrimeSeed;
        protected int start_;
        protected int end_;

        protected BruteForcingPrimes left_;
        protected BruteForcingPrimes right_;

        SortedSet<int> res_;

        protected BruteForcingPrimes(int start, int end)
        {
            start_ = start;
            end_ = end;
        }

        public static BruteForcingPrimes GetInstance(int start, int end)
        {
            int lowerBound = (int)(Math.Sqrt(end) + 1);
            BruteForcingPrimes.PrimeSeed = (from n in Enumerable.Range(0, lowerBound)
                                       where IsPrimeBruteForce(n)
                                       orderby n ascending
                                       select n).ToArray();

            return new BruteForcingPrimes(start, end);
        }

        public Queue<IBHComputeNode> Branch()
        {
            if (end_ - start_ <= BaseTaskSize)
            {
                SortedSet<int> res = new SortedSet<int>();
                for (int I = start_; I < end_; I++)
                {
                    if (IsPrimeCheckSeed(I))
                        res.Add(I);
                }
                res_ = res;
                return null;
            }

            Queue<IBHComputeNode> q = new Queue<IBHComputeNode>();
            BranchToRightBeforeLeafNode(q);
            return q;
        }

        protected void BranchToRightBeforeLeafNode(Queue<IBHComputeNode> q)
        {
            if (end_ - start_ <= BaseTaskSize)
            {
                q.Enqueue(this);
                return;
            }
            int mid = (end_ + start_) / 2;
            left_ = new BruteForcingPrimes(start_, mid);
            right_ = new BruteForcingPrimes(mid, end_);
            left_.BranchToRightBeforeLeafNode(q);
            right_.BranchToRightBeforeLeafNode(q);
        }

        static bool IsPrimeBruteForce(int n)
        {
            if (n <= 1) return false; 
            if (n == 2) return true;
            if (n % 2 == 0) return false;
            for (int I = 3; I < Math.Sqrt(n) + 1; I+=2)
            {
                if (n % I == 0) return false;
            }
            return true;
        }

        bool IsPrimeCheckSeed(int n)
        {
            int upperBound = (int)(Math.Sqrt(n) + 1);
            for(int I = 0; I < PrimeSeed.Length && PrimeSeed[I] < upperBound; I++)
            {
                if (n % PrimeSeed[I] == 0) return false;
            }
            return true;
        }

        public SortedSet<int> CollectResults()
        {
            if (end_ - start_ <= BaseTaskSize)
            {
                return res_; 
            }
            var leftResult = left_.CollectResults();
            var rightResult = right_.CollectResults();
            leftResult.UnionWith(rightResult);
            left_.res_ = null;
            right_.res_ = null; 
            return leftResult; 
        }

    }
}
