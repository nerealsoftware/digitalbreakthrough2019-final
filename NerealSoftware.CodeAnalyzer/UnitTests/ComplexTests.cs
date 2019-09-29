using System;
using System.Linq;
using CodeAnalyzer.Interface;
using CodeAnalyzer.Modules;
using CodeAnalyzer.Sources;
using CodeParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimilarityModule;
using SimilarityModule.MinHash;
using SimpleHeuristics;

namespace UnitTests
{
    [TestClass]
    public class ComplexTests
    {
        [TestMethod]
        public void FindConnectionStrings()
        {
            var basePath = TestHelper.GetBasePath("ConnStrings");
            var baseSource = new FileSystemSource(basePath);
            var files = baseSource.GetFiles();

            var module = CreateDatabaseHeuristicsModule();
            ICommonResults results = module.Execute(files);

            Assert.IsNotNull(results);
            Assert.IsNotNull(results.Results);
            Assert.IsTrue(results.Results.Any());


        }

        [TestMethod]
        public void CompareFiles()
        {
            var module = CreateCodeBaseModule();
            var path = TestHelper.GetBasePath("Files");
            Execute(module, path);
        }

        [TestMethod]
        public void Container()
        {
            var module = new ModuleContainer(new[] {CreateDatabaseHeuristicsModule(), CreateCodeBaseModule()});
            var path = TestHelper.GetBasePath("Files");
            Execute(module, path);
        }

        private static void Execute(IProcessingModule module, string path)
        {
            var source = new FileSystemSource(path);
            var results = module.Execute(source.GetFiles());
            foreach (var result in results.Results)
                if (false == string.IsNullOrWhiteSpace(result.Report))
                    Console.WriteLine(result.Report);
        }

        private static IProcessingModule CreateDatabaseHeuristicsModule()
        {
            return new DatabaseHeuristicsModule();
        }

        private static IProcessingModule CreateCodeBaseModule()
        {
            var basePath = @"C:\codebase";
            var baseSource = new FileSystemSource(basePath);
            var parser = new CsParser();
            var algorithm = new MinHashAlgorithm(20, 4, 4, 10, 5);
            var module = new CodeBaseProcessingModule(baseSource, parser, algorithm);
            return module;
        }
    }
}