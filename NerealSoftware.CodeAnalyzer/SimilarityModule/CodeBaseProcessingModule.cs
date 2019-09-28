using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeAnalyzer.Interface;
using CodeAnalyzer.Utils;
using SimilarityModule.MinHash;

namespace SimilarityModule
{
    public class CodeBaseProcessingModule : IProcessingModule
    {
        private readonly MinHashAlgorithm _algorithm;
        private readonly IParser _parser;
        private readonly ISource _source;

        public CodeBaseProcessingModule(ISource source, IParser parser, MinHashAlgorithm algorithm)
        {
            _source = source;
            _parser = parser;
            _algorithm = algorithm;
        }

        public ICommonResults Execute(IEnumerable<IFileSource> fileSources)
        {
            var baseFiles = _source.GetFiles().Select(f => new FileData(f)).ToList();
            var processingFiles = fileSources.Select(f => new FileData(f)).ToList();
            OnProgress?.Invoke(new ProcessingModuleEventData(null, "Чтение кодовой базы", 0,
                processingFiles.Count + 1, 0, baseFiles.Count));
            for (var i = 0; i < baseFiles.Count; i++)
            {
                var fileData = baseFiles[i];
                OnProgress?.Invoke(new ProcessingModuleEventData(fileData.File, null, 0,
                    processingFiles.Count + 1, i, baseFiles.Count));
                fileData.Tokens = _parser.GetTokens(fileData.File);
                var data = fileData.Tokens.Select(t => t.Code).ToArray();
                fileData.Blocks = _algorithm.CalculateBlocks(data).ToList();
            }

            var results = new List<IProcessingResult>();
            var comparer = new TokenListComparer(_algorithm);
            for (var i = 0; i < processingFiles.Count; i++)
            {
                var fileData = processingFiles[i];
                OnProgress?.Invoke(new ProcessingModuleEventData(fileData.File,
                    $"Анализ входного файла {fileData.File.GetFileName()} на дубликаты", i + 1,
                    processingFiles.Count + 1, 0, baseFiles.Count));
                fileData.Tokens = _parser.GetTokens(fileData.File);
                var data = fileData.Tokens.Select(t => t.Code).ToArray();
                fileData.Blocks = _algorithm.CalculateBlocks(data).ToList();
                var report = new StringBuilder();
                var linkedFiles = new List<IFileSource>();
                for (var j = 0; j < baseFiles.Count; j++)
                {
                    var baseFile = baseFiles[j];
                    OnProgress?.Invoke(new ProcessingModuleEventData(fileData.File, null, i + 1,
                        processingFiles.Count + 1, j, baseFiles.Count));
                    var similarBlocks = comparer.GetSimilarBlocks(fileData, baseFile).ToList();
                    if (similarBlocks.Count > 0)
                    {
                        linkedFiles.Add(baseFile.File);
                        var extractor = new FileLineExtractor();
                        var lcs = new LcsAlgorithm();
                        foreach (var similarBlock in similarBlocks)
                        {
                            report.AppendLine(
                                $"{similarBlock.File1Start.FileSource.GetFileName()} [{similarBlock.File1Start.Position}..{similarBlock.File1End.Position}] ~= {similarBlock.File2Start.FileSource.GetFileName()} [{similarBlock.File2Start.Position}..{similarBlock.File2End.Position}]");
                            var lines1 = extractor.ExtractLines(similarBlock.File1Start.FileSource,
                                similarBlock.File1Start.Position,
                                similarBlock.File1End.Position);
                            var lines2 = extractor.ExtractLines(similarBlock.File2Start.FileSource,
                                similarBlock.File2Start.Position,
                                similarBlock.File2End.Position);
                            var diffs = lcs.GetDiff(lines2, lines1);
                            foreach (var diff in diffs)
                            {
                                var symbol = GetDiffOperationSymbol(diff.Operation);
                                foreach (var item in diff.Items) report.AppendLine($"{symbol} {item}");
                            }

                            report.AppendLine();
                        }
                    }
                }

                results.Add(new Result(fileData.File, linkedFiles, report.ToString()));
            }

            return new ModuleResults(results);
        }

        public event Action<ProcessingModuleEventData> OnProgress;

        private char GetDiffOperationSymbol(DiffOperation operation)
        {
            switch (operation)
            {
                case DiffOperation.Copied:
                    return ' ';
                case DiffOperation.Added:
                    return '+';
                case DiffOperation.Deleted:
                    return '-';
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
            }
        }

        private class ModuleResults : ICommonResults
        {
            public ModuleResults(IEnumerable<IProcessingResult> results)
            {
                Results = results;
            }

            public IEnumerable<IProcessingResult> Results { get; }
        }

        private class Result : IProcessingResult
        {
            public Result(IFileSource file, IEnumerable<IFileSource> linkedFiles, string report)
            {
                File = file;
                LinkedFiles = linkedFiles;
                Report = report;
            }

            public IFileSource File { get; }
            public IEnumerable<IFileSource> LinkedFiles { get; }
            public string Report { get; }
        }
    }

    internal class FileData
    {
        public FileData(IFileSource file)
        {
            File = file;
        }

        public IFileSource File { get; }
        public List<IToken> Tokens { get; set; }
        public List<uint[]> Blocks { get; set; }
    }
}