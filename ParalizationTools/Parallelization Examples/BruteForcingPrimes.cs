using ComputeTree;
using System;
using System.Collections.Generic;
using System.Text;

namespace Parallelization_Examples
{
    class BruteForcingPrimes : IBHComputeNode
    {
        public static int BaseTaskSize = 65536;

        protected int _start;
        protected int _end;

        protected BruteForcingPrimes _left;
        protected BruteForcingPrimes _right;

        public BruteForcingPrimes(int start, int end)
        {
            _start = start;
            _end = end;
        }

        public Queue<IBHComputeNode> Branch()
        {
            
        }

        protected void BranchToRightBeforeLeafNode()
        { 
            
        }

        static bool BruteForcePrimeTest(int n)
        {
            if (n == 2) return true;
            if (n % 2 == 0) return false;
            for (int I = 3; I < Math.Sqrt(n) + 1; I++)
            {
                if (n % I == 0) return false;
            }
            return true;
        }

    }
}
