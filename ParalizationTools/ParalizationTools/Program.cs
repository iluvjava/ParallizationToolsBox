using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaskRunners;

namespace BenchMarkingWithPrimeNumbers
{
    class Program
    {
        static void Main(string[] args)
        {
            
        }
    }

    class ComputeNode {

        public static int BaseTaskSize = 65536*4;

        protected int _start;
        protected int _end;

        protected ComputeNode _left;
        protected ComputeNode _right;

        public ComputeNode(int start, int end)
        {
            _start = start;
            _end = end;
            Branch();
        }

        /// <summary>
        ///     Branch all the way until base tasks 
        ///     * Branching must be symmetrical. 
        /// </summary>
        protected void Branch()
        {
            if (_end - _start >= ComputeNode.BaseTaskSize)
            {
                int mid = (_start + _end) / 2;
                _left = new ComputeNode(_start, mid);
                _right = new ComputeNode(mid, _end);
            }
            // Base task, leaf node, do nothing. 
        }

        public void AddAllLeafTasks(Queue<Task<SortedSet<int>>> taskBucket)
        {
            if (_left is null) // leaf node, return tasks
            {
                Task<SortedSet<int>> baseTask = new Task<SortedSet<int>>(
                        () => {
                            SortedSet<int> res = new SortedSet<int>();
                            for (int I = _start; I < _end; I++)
                            {
                                if (BruteForcePrimeTest(I))
                                    res.Add(I);
                            }
                            return res;
                        }
                    );
                taskBucket.Enqueue(baseTask);
            }
            else {
                _left.AddAllLeafTasks(taskBucket);
                _right.AddAllLeafTasks(taskBucket);
            }

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


        public static int[] FindAllPrimesUnder(int n)
        {
            if (n <= 1024) throw new Exception("Input too small to compuate in parallel.");
            ComputeNode rootNode = new ComputeNode(2, n);
            var listOftasks = new Queue<Task<SortedSet<int>>>();
            rootNode.AddAllLeafTasks(listOftasks);

            Console.WriteLine($"List of Tasks count: {listOftasks.Count}");

            var taskRunner = new QueueBasedTaskRunner<SortedSet<int>>(listOftasks);
            taskRunner.RunParallel();

            List<int> primes = new List<int>(); 
            foreach (SortedSet<int> batch in taskRunner._results)
            {
                foreach (int prime in batch)
                {
                    primes.Add(prime);
                }
            }
            primes.Sort();

            return primes.ToArray();
        }

    }

    
}
