using System;
using System.Collections.Generic;
using CodeAnalyzer.Interface;

namespace SimpleHeuristics
{
    public interface IDatabaseHeuristicsModule
    {
        event Action<ProcessingModuleEventData> OnProgress;

        ICommonResults Execute(IEnumerable<IFileSource> fileSources);
        int? GetMaxMainProgressValue();
    }
}