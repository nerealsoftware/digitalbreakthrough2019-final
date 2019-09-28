using CodeAnalyzer.Interface;
using CodeAnalyzer.Sources;
using CodeParser;
using SimilarityModule;
using SimilarityModule.MinHash;

namespace WpfApp
{
    public static class ModuleFactory
    {
        public static IProcessingModule CreateCodeBaseProcessingModule(string basePath)
        {
            var baseSource = new FileSystemSource(basePath);
            var parser = new CsParser();
            var algorithm = new MinHashAlgorithm(20, 4, 4, 10, 5 );
            return new CodeBaseProcessingModule(baseSource, parser, algorithm);
        }
    }
}