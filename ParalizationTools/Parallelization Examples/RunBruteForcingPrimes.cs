using ComputeTree;
using NUnit.Framework;
using System;

namespace Parallelization_Examples
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase(50, 1000000)]
        public void RunBruteForcingPrimes(int start, int end)
        {
            BruteForcingPrimes theInstance = new BruteForcingPrimes(start, end);
            IBHComputeNode rootComputeNode = theInstance as IBHComputeNode;
            BHComputeNodeSpawner spawner = new BHComputeNodeSpawner(rootComputeNode);
            spawner.SpawnParallel();
            foreach (int I in theInstance.CollectResults())
            {
                Console.Write($"{I}, ");
            }
        }
    }
}