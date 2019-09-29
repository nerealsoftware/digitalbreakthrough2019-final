using System.Collections.Generic;
using CodeAnalyzer.Interface;
using CodeAnalyzer.Modules;
using CodeAnalyzer.Sources;
using CodeParser;
using SimilarityModule;
using SimilarityModule.MinHash;
using SimpleHeuristics;

namespace WpfApp
{
    public static class ModuleFactory
    {
        public static IProcessingModule CreateContainer(IEnumerable<IProcessingModule> modules)
        {
            return new ModuleContainer(modules);
        }

        public static IProcessingModule CreateDatabaseHeuristicsModule()
        {
            return new DatabaseHeuristicsModule();
        }

        public static IProcessingModule CreateCodeBaseProcessingModule(string basePath)
        {
            var baseSource = new FileSystemSource(basePath);
            var parser = new CsParser();
            var algorithm = new MinHashAlgorithm(20, 8, 4, 15, 5);
            return new CodeBaseProcessingModule(baseSource, parser, algorithm);
        }
    }
}