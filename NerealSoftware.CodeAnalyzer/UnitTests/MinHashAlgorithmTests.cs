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
            var algorithm = new MinHashAlgorithm(100, 4, 4);
            var calculator = new SimilarityCalculator(algorithm);

            var data1 = new int[1000];
            var data2 = new int[1100];
            for (var i = 0; i < data1.Length; i++)
            {
                data1[i] = i;
            }
            for (var i = 0; i < data2.Length; i++)
            {
                data2[i] = i;
            }

            var similarity = calculator.Calculate(data1, data2);
            Console.WriteLine($"Similarity of [0..{data1.Length-1}] and [0..{data2.Length-1}] is {similarity:P}");
        }
    }
}