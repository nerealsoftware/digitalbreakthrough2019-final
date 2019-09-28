using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CodeAnalyzer.Sources;
using CodeParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimilarityModule.MinHash;

namespace UnitTests
{
    [TestClass]
    public class ComplexTests
    {
        [TestMethod]
        public void CompareTwoFiles()
        {
            var path = @"C:\Projects\digitalbreakthrough2019-final\NerealSoftware.CodeAnalyzer\UnitTests\Files";
            var source = new FileSystemSource(path);
            var files = source.GetFiles().ToList();
            var file1 = files[0];
            var file2 = files[1];
            
            var parser = new CsParser();
            var tokens1 = parser.GetTokens(file1);
            var tokens2 = parser.GetTokens(file2);

            Console.WriteLine($"{file1.GetFileName()} - {tokens1.Count} tokens");
            Console.WriteLine($"{file2.GetFileName()} - {tokens2.Count} tokens");

            var data1 = tokens1.Select(t => t.Code).ToArray();
            var data2 = tokens2.Select(t => t.Code).ToArray();

            var algorithm = new MinHashAlgorithm(20, 4, 4, 10, 5);
            var calculator = new SimilarityCalculator();

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