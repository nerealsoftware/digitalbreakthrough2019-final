using System.Collections.Generic;

namespace CodeAnalyzer.Interface
{
    public interface IProcessingModule
    {
        ICommonResults Execute(IEnumerable<IFileSource> fileSources);

    }

    public interface ICommonResults
    {
        IEnumerable<IFileSource> FileSources { get; }

        
    }

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