using ComputeTree;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Parallelization_Examples
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase(2, 0b10000000000_0000000000_0000000000)]
        public void RunBruteForcingPrimes(int start, int end)
        {
            BruteForcingPrimes theInstance = BruteForcingPrimes.GetInstance(start, end);
            Console.WriteLine($"Prime Seed: {BruteForcingPrimes.PrimeSeed}");

            IBHComputeNode rootComputeNode = theInstance as IBHComputeNode;
            BHBFSComputeNodeSpawner spawner = new BHBFSComputeNodeSpawner(rootComputeNode);
            spawner.SpawnParallel();

            SortedSet<int> allPrimes = theInstance.CollectResults();
            Console.WriteLine($"The total number of prime numbers in [{start}, {end}] is {allPrimes.Count}");
            
        }
    }
}