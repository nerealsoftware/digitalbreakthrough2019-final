using System;
using System.Collections.Generic;
using System.Linq;
using CodeAnalyzer.Interface;
using SimilarityModule.MinHash;

namespace SimilarityModule
{
    public class CodeBaseProcessingModule : IProcessingModule
    {
        private readonly ISource _source;
        private readonly IParser _parser;
        private readonly MinHashAlgorithm _algorithm;

        public CodeBaseProcessingModule(ISource source, IParser parser, MinHashAlgorithm algorithm)
        {
            _source = source;
            _parser = parser;
            _algorithm = algorithm;
        }

        public ICommonResults Execute(IEnumerable<IFileSource> fileSources)
        {
            var baseFiles = _source.GetFiles().Select(f=>new FileData(f)).ToList();
            var processingFiles = fileSources.Select(f=>new FileData(f)).ToList();
            for (var i = 0; i < baseFiles.Count; i++)
            {
                var fileData = baseFiles[i];
                OnProgress?.Invoke(new ProcessingModuleEventData(fileData.File, "Чтение кодовой базы", 0,
                    processingFiles.Count + 1, i, baseFiles.Count));
                fileData.Tokens = _parser.GetTokens(fileData.File);
                var data = fileData.Tokens.Select(t => t.Code).ToArray();
                fileData.Blocks = _algorithm.CalculateBlocks(data).ToList();
            }

            var comparer = new TokenListComparer(_algorithm);
            for (var i = 0; i < processingFiles.Count; i++)
            {
                var fileData = processingFiles[i];
                OnProgress?.Invoke(new ProcessingModuleEventData(fileData.File, "Анализ входного файла", i+1,
                    processingFiles.Count + 1, 0, baseFiles.Count));
                fileData.Tokens = _parser.GetTokens(fileData.File);
                var data = fileData.Tokens.Select(t => t.Code).ToArray();
                fileData.Blocks = _algorithm.CalculateBlocks(data).ToList();
                for (var j = 0; j < baseFiles.Count; j++)
                {
                    var baseFile = baseFiles[j];
                    OnProgress?.Invoke(new ProcessingModuleEventData(fileData.File, $"Анализ входного файла на дубликаты с {baseFile.File.GetFileName()}", i+1,
                        processingFiles.Count + 1, j, baseFiles.Count));
                    var similarBlocks = comparer.GetSimilarBlocks(fileData, baseFile);
                }
            }

            return null;
        }

        public event Action<ProcessingModuleEventData> OnProgress;
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