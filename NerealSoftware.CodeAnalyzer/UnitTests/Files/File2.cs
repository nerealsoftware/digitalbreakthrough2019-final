using System;
using System.Linq;
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
            var algorithm = new MinHashAlgorithm(100, 4, 4, 100, 50);
            var calculator = new SimilarityCalculator();

            var data1 = new int[1000];
            var data2 = new int[1100];
            for (var i = 0; i < data1.Length; i++)
            {
                data1[i] = 37+i;
            }
            for (var i = 0; i < data2.Length; i++)
            {
                data2[i] = 113+i;
            }

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    break;
                }
            }

            var blocks1 = algorithm.CalculateBlocks(data1).ToList();
            var blocks2 = algorithm.CalculateBlocks(data2).ToList();

            for (var i = 0; i < blocks1.Count; i++)
            {
                for (int j = 0; j < blocks2.Count; j++)
                {
                    var similarity = calculator.Calculate(blocks1[i], blocks2[j]);
                    Console.Write((25*similarity).ToString("00"));
                    Console.Write(" ");
                }

                Console.WriteLine();
            }
        }
    }
}