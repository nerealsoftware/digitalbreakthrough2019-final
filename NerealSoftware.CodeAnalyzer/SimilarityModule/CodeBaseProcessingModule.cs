﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CodeAnalyzer.Interface;
using CodeAnalyzer.Utils;
using SimilarityModule.MinHash;

namespace SimilarityModule
{
    public class CodeBaseProcessingModule : IProcessingModule, IReportRenderer
    {
        private readonly MinHashAlgorithm _algorithm;
        private readonly IParser _parser;
        private readonly ISource _source;
        private int _maxMainProgress = -1;

        public CodeBaseProcessingModule(ISource source, IParser parser, MinHashAlgorithm algorithm)
        {
            _source = source;
            _parser = parser;
            _algorithm = algorithm;
        }

        public string GetName() => "Сопоставление с кодовой базой";

        public ICommonResults Execute(IEnumerable<IFileSource> fileSources)
        {
            var baseFiles = _source.GetFiles().Select(f => new FileData(f)).ToList();
            var processingFiles = fileSources.Select(f => new FileData(f)).ToList();
            _maxMainProgress = processingFiles.Count + 1;
            OnProgress?.Invoke(new ProcessingModuleEventData(null, "Чтение кодовой базы", 0,
                _maxMainProgress, 0, baseFiles.Count, this));
            for (var i = 0; i < baseFiles.Count; i++)
            {
                var fileData = baseFiles[i];
                OnProgress?.Invoke(new ProcessingModuleEventData(fileData.File, null, 0,
                    _maxMainProgress, i, baseFiles.Count, this));
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
                    _maxMainProgress, 0, baseFiles.Count, this));
                fileData.Tokens = _parser.GetTokens(fileData.File);
                var data = fileData.Tokens.Select(t => t.Code).ToArray();
                fileData.Blocks = _algorithm.CalculateBlocks(data).ToList();
                var report = new StringBuilder();
                var linkedFiles = new List<IFileSource>();
                for (var j = 0; j < baseFiles.Count; j++)
                {
                    var baseFile = baseFiles[j];
                    OnProgress?.Invoke(new ProcessingModuleEventData(fileData.File, null, i + 1,
                        _maxMainProgress, j, baseFiles.Count, this));
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

                results.Add(new Result(fileData.File, linkedFiles, report.ToString(), this));
            }

            return new ModuleResults(results);
        }

        public int? GetMaxMainProgressValue()
        {
            return _maxMainProgress <= 0 ? (int?)null : _maxMainProgress;
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

        public string ToHtml(IProcessingResult result)
        {
            var sb = new StringBuilder();
            using (var rdr = new StringReader(result.Report))
            {
                int mode = 0;
                var line = rdr.ReadLine();
                while (line != null)
                {
                    if (mode == 0)
                    {
                        sb.AppendLine($"<hr>");
                        sb.AppendLine($"<br/><b style='font-family:monospace;width:100%;'>{line}</b><br/>");
                        sb.AppendLine($"<table border=0 style='font-family:monospace;width:100%;'>");
                        mode = 1;
                    }
                    else if (mode == 1)
                    {
                        if (string.IsNullOrEmpty(line))
                        {
                            sb.AppendLine($"</table>");
                            mode = 0;
                        }
                        else
                        {
                            var isRed = line.StartsWith(" ");
                            var style = isRed ? "background-color: #FFC0C0;" : "";
                            sb.AppendLine($"<tr><td style='{style}'>");
                            sb.AppendLine($"{line}");
                            sb.AppendLine($"</td></tr>");
                        }
                    }
                    line = rdr.ReadLine();
                }
            }
            return $"<pre>{sb.ToString()}</pre>";
        }

        public IReportRenderer GetReportRenderer()
        {
            return this;
        }

        private class ModuleResults : ICommonResults
        {
            public ModuleResults(IEnumerable<IProcessingResult> results)
            {
                Results = results;
            }

            public IEnumerable<IProcessingResult> Results { get; }
        }

        public class Result : IProcessingResult
        {
            public Result(IFileSource file, IEnumerable<IFileSource> linkedFiles, string report, IProcessingModule module)
            {
                File = file;
                LinkedFiles = linkedFiles;
                Report = report;
                Module = module;
            }

            public IFileSource File { get; }
            public IEnumerable<IFileSource> LinkedFiles { get; }
            public string Report { get; }

            public IProcessingModule Module { get; }
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