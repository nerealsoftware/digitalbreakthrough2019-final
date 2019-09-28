using System.Collections.Generic;

namespace CodeAnalyzer.Interface
{
    public interface ICommonResults
    {
        IEnumerable<IFileSource> FileSources { get; }

        
    }
}