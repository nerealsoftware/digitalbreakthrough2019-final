using System;
using System.Data.HashFunction;
using System.Data.HashFunction.MurmurHash;

namespace SimilarityModule.MinHash
{
    public class MinHashAlgorithm
    {
        private readonly int _hashCount;
        private readonly int _blockSize;
        private readonly IHashFunction[] _hashFunctions;

        public MinHashAlgorithm(int hashCount, int blockSize)
        {
            _hashCount=hashCount;
            _blockSize = blockSize;
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

        public uint[] Calculate(int[] data)
        {
            return Calculate(data, sizeof(int));
        }

        private uint[] Calculate<T>(T[] data, int itemSize) where T : struct
        {
            var result = new uint[_hashCount];
            var bytes = new byte[_blockSize * itemSize];
            for (int i = 0; i < _hashCount; i++)
            {
                var func = _hashFunctions[i];
                var minValue = uint.MaxValue;
                for (int j = 0; j < data.Length - _blockSize + 1; j++)
                {
                    Buffer.BlockCopy(data, j * itemSize, bytes, 0, bytes.Length);
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