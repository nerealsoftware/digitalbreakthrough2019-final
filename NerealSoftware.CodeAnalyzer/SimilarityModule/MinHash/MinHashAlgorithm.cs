using System;
using System.Data.HashFunction;
using System.Data.HashFunction.MurmurHash;

namespace SimilarityModule.MinHash
{
    public class MinHashAlgorithm
    {
        private readonly int _hashCount;
        private readonly IHashFunction[] _hashFunctions;

        public MinHashAlgorithm(int hashCount)
        {
            _hashCount=hashCount;
            var factory = MurmurHash3Factory.Instance;
            _hashFunctions = new IHashFunction[_hashCount];
            for (int i = 0; i < _hashCount; i++)
            {
                var config = new MurmurHash3Config
                {
                    HashSizeInBits = 32,
                    Seed = uint.MaxValue / (uint) hashCount * (uint) i
                };
                _hashFunctions[i] = factory.Create(config);
            }
        }

        public uint[] Calculate(int[] data, int blockSize)
        {
            var result = new uint[_hashCount];
            var bytes = new byte[blockSize * sizeof(int)];
            for (int i = 0; i < _hashCount; i++)
            {
                var func = _hashFunctions[i];
                var minValue = uint.MaxValue;
                for (int j = 0; j < data.Length-blockSize+1; j++)
                {
                    Buffer.BlockCopy(data, j * sizeof(int), bytes, 0, bytes.Length);
                    var hash = func.ComputeHash(bytes);
                    var value = BitConverter.ToUInt32(hash.Hash, 0);
                    if (value < minValue)
                    {
                        minValue = value;
                    }
                }

                result[i] = minValue;
            }

            return result;
        }
    }
}