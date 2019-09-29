using System;
using System.Collections.Generic;
using System.Linq;
using CodeAnalyzer.Interface;

namespace CodeAnalyzer.Modules
{
    public class ModuleContainer : IProcessingModule
    {
        private readonly int _maxMainProgressValue;
        private readonly List<IProcessingModule> _modules;
        private int _currentMainProgressValue;

        public ModuleContainer(IEnumerable<IProcessingModule> modules)
        {
            _modules = modules.ToList();
            _maxMainProgressValue = _modules.Select(m => m.GetMaxMainProgressValue() ?? 0).Sum();
        }

        public ICommonResults Execute(IEnumerable<IFileSource> fileSources)
        {
            _currentMainProgressValue = 0;
            var files = fileSources.ToList();
            var results = new List<IProcessingResult>();
            foreach (var module in _modules)
            {
                module.OnProgress += ModuleOnProgress;
                var result = module.Execute(files);
                module.OnProgress -= ModuleOnProgress;

                if (result.Results != null)
                    results.AddRange(result.Results);
                _currentMainProgressValue += module.GetMaxMainProgressValue() ?? 0;
            }

            return new GroupedResults(results);
        }

        public int? GetMaxMainProgressValue()
        {
            return _maxMainProgressValue;
        }

        public event Action<ProcessingModuleEventData> OnProgress;

        private void ModuleOnProgress(ProcessingModuleEventData e)
        {
            OnProgress?.Invoke(new ProcessingModuleEventData(e.CurrentFile, e.Message,
                _currentMainProgressValue + e.CurrentMainProgress, _maxMainProgressValue, e.CurrentSecondProgress,
                e.MaxSecondProgress, e.Module));
        }

        private class GroupedResults : ICommonResults
        {
            public GroupedResults(IEnumerable<IProcessingResult> results)
            {
                Results = results.GroupBy(r => r.File).Select(g => new GroupedResult(g)).ToList();
            }

            public IEnumerable<IProcessingResult> Results { get; }
        }

        private class GroupedResult : IProcessingResult
        {
            public GroupedResult(IGrouping<IFileSource, IProcessingResult> g)
            {
                File = g.Key;
                LinkedFiles = g.Where(r => r.LinkedFiles != null).SelectMany(r => r.LinkedFiles).Distinct().ToList();
                Report = string.Join("\n\n", g.Select(r => r.Report).Where(r => false == string.IsNullOrWhiteSpace(r)));
            }

            public IFileSource File { get; }
            public IEnumerable<IFileSource> LinkedFiles { get; }
            public string Report { get; }
        }
    }
}