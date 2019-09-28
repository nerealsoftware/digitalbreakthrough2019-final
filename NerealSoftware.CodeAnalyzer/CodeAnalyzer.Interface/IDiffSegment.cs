using System.Collections.Generic;

namespace CodeAnalyzer.Interface
{
    public interface IDiffSegment<T>
    {
        DiffOperation Operation { get; }
        List<T> Items { get; }
    }

    public enum DiffOperation
    {
        Copied,
        Added,
        Deleted
    }
}