using System;
using CodeAnalyzer.Sources;
using CodeParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimilarityModule;
using SimilarityModule.MinHash;

namespace UnitTests
{
    [TestClass]
    public class ComplexTests
    {
        [TestMethod]
        public void CompareFiles()
        {
            var basePath = @"C:\codebase";
            var baseSource = new FileSystemSource(basePath);
            var parser = new CsParser();
            var algorithm = new MinHashAlgorithm(20, 4, 4, 10, 5);
            var module = new CodeBaseProcessingModule(baseSource, parser, algorithm);

            var path = @"C:\Projects\digitalbreakthrough2019-final\NerealSoftware.CodeAnalyzer\UnitTests\Files";
            var source = new FileSystemSource(path);
            var results = module.Execute(source.GetFiles());
            foreach (var result in results.Results)
                if (false == string.IsNullOrWhiteSpace(result.Report))
                    Console.WriteLine(result.Report);
        }
    }
}