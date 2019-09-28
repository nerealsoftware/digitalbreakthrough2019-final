using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimilarityModule.MinHash;

namespace SimilarityModule.Tests
{
    [TestClass]
    public class MinHashAlgorithmTests
    {
        [TestMethod]
        public void SimpleTest()
        {
            var algorithm = new MinHashAlgorithm(100);
            var data = new int[1000];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = i;
            }

            var result1 = algorithm.Calculate(data, 4);

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = i + 100;
            }
            var result2 = algorithm.Calculate(data, 4);

            for (int i = 0; i < result1.Length; i++)
            {
                if (result1[i] != result2[i])
                {
                    Console.WriteLine($"Result diffs at index {i}");
                }
            }
        }
    }
}