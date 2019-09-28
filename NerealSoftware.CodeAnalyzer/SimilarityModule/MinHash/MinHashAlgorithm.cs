using System;
using System.Data.HashFunction;
using System.Data.HashFunction.CityHash;
using System.Data.HashFunction.MurmurHash;

namespace SimilarityModule.MinHash
{
    public class MinHashAlgorithm
    {
        private readonly int _hashCount;
        private readonly int _groupSize;
        private readonly int _windowSize;
        private readonly IHashFunction[] _hashFunctions;
        private readonly IHashFunction _groupHashFunction;

        public MinHashAlgorithm(int hashCount, int groupSize, int windowSize)
        {
            _hashCount = hashCount;
            _groupSize = groupSize;
            _windowSize = windowSize;
            var factory = MurmurHash3Factory.Instance;
            _hashFunctions = new IHashFunction[_hashCount];
            for (var i = 0; i < _hashCount; i++)
            {
                var config = new MurmurHash3Config
                {
                    HashSizeInBits = 32,
                    Seed = uint.MaxValue / (uint) hashCount * (uint) i
                };
                _hashFunctions[i] = factory.Create(config);
            }

            _groupHashFunction = CityHashFactory.Instance.Create();
        }

        public uint[] Calculate(int[] data)
        {
            var result = Calculate(data, sizeof(int));
            if (_groupSize < 2)
            {
                return result;
            }

            var groupedResult = new uint[_hashCount / _groupSize];
            var bytes = new byte[_groupSize * sizeof(uint)];
            for (var i = 0; i < groupedResult.Length; i++)
            {
                Buffer.BlockCopy(result, i*_groupSize*sizeof(uint), bytes,0, bytes.Length);
                var hash = _groupHashFunction.ComputeHash(bytes);
                groupedResult[i] = BitConverter.ToUInt32(hash.Hash, 0);
            }
            return groupedResult;
        }

        private uint[] Calculate<T>(T[] data, int itemSize) where T : struct
        {
            var result = new uint[_hashCount];
            var bytes = new byte[_windowSize * itemSize];
            for (var i = 0; i < _hashCount; i++)
            {
                var func = _hashFunctions[i];
                var minValue = uint.MaxValue;
                for (var j = 0; j < data.Length - _windowSize + 1; j++)
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