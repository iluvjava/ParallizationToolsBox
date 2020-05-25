using ComputeTree;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Parallelization_Examples
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase(2, 0b1_0000000000_0000000000_0000000)]
        [TestCase(2, 0b1_0000000000_0000000000_00000000)]
        [TestCase(2, 0b1_0000000000_0000000000_0000000000)]
        public void RunBruteForcingPrimes(int start, int end)
        {
            SortedSet<int> allPrimes = new SortedSet<int>();
            Stopwatch sw = new Stopwatch();
            {
                BruteForcingPrimes theInstance = BruteForcingPrimes.GetInstance(start, end);
                Console.WriteLine($"Prime Seed: {BruteForcingPrimes.PrimeSeed}");

                IBHComputeNode rootComputeNode = theInstance as IBHComputeNode;
                BHBFSComputeNodeSpawner spawner = new BHBFSComputeNodeSpawner(rootComputeNode);
                sw.Start();
                spawner.SpawnParallel();
                theInstance.CollectResults(allPrimes);
                sw.Stop();
                Console.WriteLine($"The total number of prime numbers in [{start}, {end}] is {allPrimes.Count}");
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"All {allPrimes.Count} of primes in range [{start}, {end}].");
            sb.AppendLine($"Duration of computing: {sw.ElapsedMilliseconds} ms");
            while (allPrimes.Count > 0)
            {
                sb.Append($"{allPrimes.Min}, ");
                allPrimes.Remove(allPrimes.Min);
            };

            using (StreamWriter streamWriter = new StreamWriter($"Primes in range [{start}, {end}].txt"))
            {
                streamWriter.WriteLine(sb.ToString());
            }

        }
    }
}