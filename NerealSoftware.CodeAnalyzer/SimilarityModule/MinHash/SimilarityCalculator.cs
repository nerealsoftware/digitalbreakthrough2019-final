namespace SimilarityModule.MinHash
{
    public class SimilarityCalculator
    {
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