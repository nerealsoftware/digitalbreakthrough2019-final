namespace SimilarityModule.MinHash
{
    public class SimilarityCalculator
    {
        private readonly MinHashAlgorithm _algorithm;

        public SimilarityCalculator(MinHashAlgorithm algorithm)
        {
            _algorithm = algorithm;
        }

        public double Calculate(int[] data1, int[] data2)
        {
            return Calculate(_algorithm.Calculate(data1), _algorithm.Calculate(data2));
        }

        public double Calculate(int[] data, uint[] signature)
        {
            return Calculate(_algorithm.Calculate(data), signature);
        }

        public double Calculate(uint[] signature1, uint[] signature2)
        {
            var equalCount = 0;
            for (int i = 0; i < signature1.Length; i++)
            {
                if (signature1[i] == signature2[i])
                {
                    equalCount++;
                }
            }

            return equalCount * 1.0 / signature1.Length;
        }
    }
}