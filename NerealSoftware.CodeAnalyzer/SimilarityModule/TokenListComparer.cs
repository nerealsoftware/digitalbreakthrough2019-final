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
            var blocks1 = _algorithm.CalculateBlocks(data1).ToList();
            var blocks2 = _algorithm.CalculateBlocks(data2).ToList();
            for (var i = blocks1.Count - 1; i >= 0; i--)
            {
                var lines = CheckDiagonal(blocks1, blocks2, i, 0, Math.Min(blocks1.Count - i, blocks2.Count));
                foreach (var line in lines)
                {
                    yield return new SimilarBlock(tokens1[_algorithm.GetBlockStartIndex(line.Item1)],
                        tokens1[
                            Math.Min(_algorithm.GetBlockEndIndex(line.Item1 + line.Item3 - 1), tokens1.Count - 1)],
                        tokens2[_algorithm.GetBlockStartIndex(line.Item2)],
                        tokens2[
                            Math.Min(_algorithm.GetBlockEndIndex(line.Item2 + line.Item3 - 1), tokens2.Count - 1)]);

                }
            }
            for (var i = 1; i < blocks2.Count; i++)
            {
                var lines = CheckDiagonal(blocks1, blocks2, 0, i, Math.Min(blocks2.Count - i, blocks1.Count));
                foreach (var line in lines)
                {
                    yield return new SimilarBlock(tokens1[_algorithm.GetBlockStartIndex(line.Item1)],
                        tokens1[
                            Math.Min(_algorithm.GetBlockEndIndex(line.Item1 + line.Item3 - 1), tokens1.Count - 1)],
                        tokens2[_algorithm.GetBlockStartIndex(line.Item2)],
                        tokens2[
                            Math.Min(_algorithm.GetBlockEndIndex(line.Item2 + line.Item3 - 1), tokens2.Count - 1)]);
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