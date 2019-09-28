using System;
using System.Collections.Generic;
using System.Linq;
using CodeAnalyzer.Interface;
using SimilarityModule.MinHash;

namespace SimilarityModule
{
    public class TokenListComparer
    {
        private readonly MinHashAlgorithm _algorithm;
        private readonly SimilarityCalculator _calculator;

        public TokenListComparer(MinHashAlgorithm algorithm)
        {
            _algorithm = algorithm;
            _calculator = new SimilarityCalculator();
        }

        public IEnumerable<ISimilarBlock> Compare(List<IToken> tokens1, List<IToken> tokens2)
        {
            var data1 = tokens1.Select(t => t.Code).ToArray();
            var data2 = tokens2.Select(t => t.Code).ToArray();
            var file1 = new FileData(null)
            {
                Tokens = tokens1,
                Blocks = _algorithm.CalculateBlocks(data1).ToList()
            };
            var file2 = new FileData(null)
            {
                Tokens = tokens2,
                Blocks = _algorithm.CalculateBlocks(data2).ToList()
            };
            return GetSimilarBlocks(file1, file2);
        }

        internal IEnumerable<ISimilarBlock> GetSimilarBlocks(FileData file1, FileData file2)
        {
            for (var i = file1.Blocks.Count - 1; i >= 0; i--)
            {
                var lines = CheckDiagonal(file1.Blocks, file2.Blocks, i, 0, Math.Min(file1.Blocks.Count - i, file2.Blocks.Count));
                foreach (var line in lines)
                {
                    yield return new SimilarBlock(file1.Tokens[_algorithm.GetBlockStartIndex(line.Item1)],
                        file1.Tokens[
                            Math.Min(_algorithm.GetBlockEndIndex(line.Item1 + line.Item3 - 1), file1.Tokens.Count - 1)],
                        file2.Tokens[_algorithm.GetBlockStartIndex(line.Item2)],
                        file2.Tokens[
                            Math.Min(_algorithm.GetBlockEndIndex(line.Item2 + line.Item3 - 1), file2.Tokens.Count - 1)]);
                }
            }

            for (var i = 1; i < file2.Blocks.Count; i++)
            {
                var lines = CheckDiagonal(file1.Blocks, file2.Blocks, 0, i, Math.Min(file2.Blocks.Count - i, file1.Blocks.Count));
                foreach (var line in lines)
                {
                    yield return new SimilarBlock(file1.Tokens[_algorithm.GetBlockStartIndex(line.Item1)],
                        file1.Tokens[
                            Math.Min(_algorithm.GetBlockEndIndex(line.Item1 + line.Item3 - 1), file1.Tokens.Count - 1)],
                        file2.Tokens[_algorithm.GetBlockStartIndex(line.Item2)],
                        file2.Tokens[
                            Math.Min(_algorithm.GetBlockEndIndex(line.Item2 + line.Item3 - 1), file2.Tokens.Count - 1)]);
                }
            }
        }

        private IEnumerable<Tuple<int,int,int>> CheckDiagonal(List<uint[]> blocks1, List<uint[]> blocks2, int i, int j, int length)
        {
            var s = 0;
            for (var k = 0; k < length; k++)
            {
                var similarity = _calculator.Calculate(blocks1[i + k], blocks2[j + k]);
                if (similarity > 0)
                {
                    continue;
                }

                if (s < k - 1)
                {
                    yield return Tuple.Create(i + s, j + s, k - 1 - s);
                }

                s = k;
            }

            if (s < length - 1)
            {
                yield return Tuple.Create(i + s, j + s, length - 1 - s);
            }
        }

        private class SimilarBlock : ISimilarBlock
        {
            public SimilarBlock(IToken file1Start, IToken file1End, IToken file2Start, IToken file2End)
            {
                File1Start = file1Start;
                File1End = file1End;
                File2Start = file2Start;
                File2End = file2End;
            }

            public IToken File1Start { get; }
            public IToken File1End { get; }
            public IToken File2Start { get; }
            public IToken File2End { get; }
        }
    }
}