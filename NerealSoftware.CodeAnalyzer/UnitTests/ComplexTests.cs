using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CodeAnalyzer.Interface;
using CodeAnalyzer.Sources;
using CodeParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimilarityModule;
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
            
            CompareTwoFiles(file1, file2);
        }

        private void CompareTwoFiles(IFileSource file1, IFileSource file2)
        {
            var parser = new CsParser();
            //var offsetStore1 = new OffsetsStore();
            var tokens1 = parser.GetTokens(file1 /*, offsetStore1*/);
            //var offsetStore2 = new OffsetsStore();
            var tokens2 = parser.GetTokens(file2 /*, offsetStore2*/);

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
                    Console.Write((99 * similarity).ToString("00"));
                    Console.Write(" ");
                }

                Console.WriteLine();
            }

            var comparer = new TokenListComparer(algorithm);
            var result = comparer.Compare(tokens1, tokens2);
            var extractor = new FileLineExtractor();
            var lcs = new LcsAlgorithm();
            foreach (var similarBlock in result)
            {
                Console.WriteLine(
                    $"{similarBlock.File1Start.FileSource.GetFileName()} [{similarBlock.File1Start.Position}..{similarBlock.File1End.Position}] ~= {similarBlock.File2Start.FileSource.GetFileName()} [{similarBlock.File2Start.Position}..{similarBlock.File2End.Position}]");
                var lines1 = extractor.ExtractLines(similarBlock.File1Start.FileSource, similarBlock.File1Start.Position,
                    similarBlock.File1End.Position);
                var lines2 = extractor.ExtractLines(similarBlock.File2Start.FileSource, similarBlock.File2Start.Position,
                    similarBlock.File2End.Position);
                var diffs = lcs.GetDiff(lines1, lines2);
                foreach (var diff in diffs)
                {
                    var symbol = GetDiffOperationSymbol(diff.Operation);
                    foreach (var item in diff.Items)
                    {
                        Console.WriteLine($"{symbol} {item}");
                    }
                }
            }
        }

        private char GetDiffOperationSymbol(DiffOperation operation)
        {
            switch (operation)
            {
                case DiffOperation.Copied:
                    return ' ';
                case DiffOperation.Added:
                    return '+';
                case DiffOperation.Deleted:
                    return '-';
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
            }
        }
    }
}