using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeParser
{
    public class OffsetsStore : Dictionary<int,int>
    {
        protected int ShiftSize = 0;
        protected void AddToOffsets(int newPos, int oldPos)
        {
            if (newPos < 0 || this.ContainsKey(newPos)) return;
            this[newPos] = oldPos;
            _keys = null;
        }

        public void StoreOffset(int p, int length, int replaceLength)
        {
            int np = p - ShiftSize;
            AddToOffsets(np, p);
            ShiftSize += length - replaceLength;
            AddToOffsets(np + replaceLength - 1, p + length - 1);
        }

        protected int[] _keys = null;

        public int RecoverOffset(int newOffset)
        {
            _keys = _keys ?? this.Keys.ToArray();

            if (_keys.Length < 1) return newOffset;

            int l = 0, r = _keys.Length - 1;

            if (_keys[l] > newOffset) return 0;
            if (_keys[r] <= newOffset)
            {
                var newValue = _keys[r];
                if (newValue == newOffset) return this[newValue];
                var offset = Math.Max(0, this[newValue] + (newOffset - newValue) - 2);
                return offset;
            }

            while ((r - l) > 1)
            {
                int m = (r + l) >> 1;
                if (_keys[m] > newOffset)
                    r = m;
                else
                    l = m;
            }

            int key = _keys[l];

            if (key == newOffset) return this[key];
            return Math.Max(0, this[key] + (newOffset - key) - 2);
        }
    }
}