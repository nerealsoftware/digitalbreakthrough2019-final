using System;
using System.Collections.Generic;
using System.Linq;
using CodeAnalyzer.Interface;

namespace CodeAnalyzer.Utils
{
    public class LcsAlgorithm
    {
        public IEnumerable<IDiffSegment<T>> GetDiff<T>(IList<T> s1, IList<T> s2)
            where T : IEquatable<T>
        {
            var c = GetLengths(s1, s2);
            var segments = new List<IDiffSegment<T>>();
            GetDiff(c, s1, s2, s1.Count, s2.Count, segments);
            return segments;
        }

        private static int[,] GetLengths<T>(IList<T> s1, IList<T> s2) where T : IEquatable<T>
        {
            var c = new int[s1.Count + 1, s2.Count + 1];
            for (var i = 1; i <= s1.Count; i++)
            for (var j = 1; j <= s2.Count; j++)
                if (s1[i - 1].Equals(s2[j - 1]))
                    c[i, j] = c[i - 1, j - 1] + 1;
                else
                    c[i, j] = c[i - 1, j] > c[i, j - 1] ? c[i - 1, j] : c[i, j - 1];
            return c;
        }

        private static void GetDiff<T>(int[,] c, IList<T> s1, IList<T> s2, int i, int j, List<IDiffSegment<T>> segments)
            where T : IEquatable<T>
        {
            void AddDiff(DiffOperation operation, T item)
            {
                if (segments.Any() && segments.Last().Operation == operation)
                    segments.Last().Items.Add(item);
                else
                    segments.Add(new DiffSegment<T>(operation, item));
            }

            if (i > 0 && j > 0 && s1[i - 1].Equals(s2[j - 1]))
            {
                GetDiff(c, s1, s2, i - 1, j - 1, segments);
                AddDiff(DiffOperation.Copied, s1[i - 1]);
            }
            else if (j > 0 && (i == 0 || c[i, j - 1] > c[i - 1, j]))
            {
                GetDiff(c, s1, s2, i, j - 1, segments);
                AddDiff(DiffOperation.Added, s2[j - 1]);
            }
            else if (i > 0 && (j == 0 || c[i, j - 1] <= c[i - 1, j]))
            {
                GetDiff(c, s1, s2, i - 1, j, segments);
                AddDiff(DiffOperation.Deleted, s1[i - 1]);
            }
        }

        private class DiffSegment<T> : IDiffSegment<T>
        {
            public DiffSegment(DiffOperation operation, T item)
            {
                Operation = operation;
                Items = new List<T> {item};
            }

            public DiffOperation Operation { get; }
            public List<T> Items { get; }
        }
    }
}