using System.Collections.Generic;

namespace CodeAnalyzer.Interface
{
    public interface ICommonResults
    {
        IEnumerable<IProcessingResult> Results { get; }
    }
}