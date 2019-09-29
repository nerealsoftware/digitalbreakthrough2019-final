using System.Collections.Generic;

namespace CodeAnalyzer.Interface
{
    public interface IProcessingResult
    {
        IFileSource File { get; }
        IEnumerable<IFileSource> LinkedFiles { get; }
        string Report { get; }

        IProcessingModule Module { get; }
    }
}