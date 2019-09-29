using System;
using System.Collections.Generic;
using CodeAnalyzer.Interface;

namespace SimpleHeuristics
{
    public class DatabaseHeuristicsReport : ICommonResults
    {
        public DatabaseHeuristicsReport(IEnumerable<IFileSource> fileSources)
        {
            FileSources = fileSources;
        }

        public IEnumerable<IFileSource> FileSources { get; protected set; }
        public IEnumerable<IProcessingResult> Results { get; set; }
    }

    public class DatabaseHeuristicsModule : IProcessingModule
    {
        public ICommonResults Execute(IEnumerable<IFileSource> fileSources)
        {
            var result = new DatabaseHeuristicsReport(fileSources);

            return result;
        }

        public event Action<ProcessingModuleEventData> OnProgress;
    }
}