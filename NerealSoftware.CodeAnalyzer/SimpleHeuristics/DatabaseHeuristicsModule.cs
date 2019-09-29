using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CodeAnalyzer.Interface;

namespace SimpleHeuristics
{
    public class DatabaseHeuristicsReport : ICommonResults
    {
        public DatabaseHeuristicsReport(List<IFileSource> fileSources)
        {
            FileSources = fileSources;
        }

        public List<IFileSource> FileSources { get; protected set; }
        public IEnumerable<IProcessingResult> Results { get; set; }
    }

    public class DatabaseProcessingResult : IProcessingResult
    {
        public IFileSource File { get; set; }
        public IEnumerable<IFileSource> LinkedFiles { get; set; }
        public string Report { get; set; }

        public DatabaseProcessingResult(IFileSource fileSource, string report)
        {
            File = fileSource;
            Report = report;
        }
    }

    public class DatabaseHeuristicsModule : IProcessingModule
    {
        private int _totalSteps = 0;
        private int _localSteps = 0;
        private int _totalCurrent = 0;

        public ICommonResults Execute(IEnumerable<IFileSource> fileSources)
        {
            var list = fileSources.ToList();

            _totalCurrent = 0;
            _totalSteps = 1;
            _localSteps = list.Count + 1;

            var report = new DatabaseHeuristicsReport(list);
            var results = new List<IProcessingResult>();

            // поиск строк подключения
            FindConnectionStrings(list, results);

            return report;
        }

        private ProcessingModuleEventData BuildProgress(int localStep, string message)
        {
            return new ProcessingModuleEventData(null, message,
                _totalCurrent, _totalSteps,
                localStep, _localSteps, this);
        }

        private void FindConnectionStrings(List<IFileSource> fileSources, List<IProcessingResult> results)
        {
            OnProgress?.Invoke(BuildProgress(0, "Поиск строк подключения к БД"));

            var connStrings = new Dictionary<string, List<string>>();

            for (var index = 0; index < fileSources.Count; index++)
            {
                connStrings.Clear();

                var fileSource = fileSources[index];
                var fileName = fileSource.GetFileName();

                var localStep = index + 1;

                OnProgress?.Invoke(BuildProgress(localStep, $"Обработка файла {fileName}..."));

                var sb = new StringBuilder();
                var data = fileSource.GetData().ToLower();

                using (var reader = new StringReader(data))
                {
                    sb.Clear();
                    var line = reader.ReadLine();
                    int lineNumber = 1;
                    while (line != null)
                    {
                        if (CheckForConnectionString(line))
                        {
                            OnProgress?.Invoke(BuildProgress(localStep, $"Найдена строка <{line}>"));
                            if (IsMsSqlConnectionString(line))
                            {
                                OnProgress?.Invoke(BuildProgress(localStep, "Найдена строка MS SQL: {line}"));
                                if (!connStrings.ContainsKey("MSSQL")) connStrings["MSSQL"] = new List<string>();
                                connStrings["MSSQL"].Add($"строка {lineNumber}: {line}");
                            }
                            else if (IsOracleConnectionString(line))
                            {
                                OnProgress?.Invoke(BuildProgress(localStep, "Найдена строка Oracle: {line}"));
                                if (!connStrings.ContainsKey("ORA")) connStrings["ORA"] = new List<string>();
                                connStrings["ORA"].Add($"строка {lineNumber}: {line}");
                            }
                        }
                        line = reader.ReadLine();
                        lineNumber++;
                    }
                }

            }
        }

        private bool IsOracleConnectionString(string line)
        {
            if (line.Contains(@"oracle")) return true;
            if (line.Contains(@"oraoledb")) return true;
            if (line.Contains(@"user id=sys")) return true;
            if (line.Contains(@"dba privilege=sysoper")) return true;
            if (line.Contains(@"dba privilege=sysoper")) return true;
            return false;
        }

        private bool IsMsSqlConnectionString(string line, int limit = 2)
        {
            int sum = 0;
            if (line.Contains(@".\sql")) sum++;
            if (line.Contains(@".mdf")) sum+=2;
            if (line.Contains(@"sqlexpress")) sum+=2;
            if (line.Contains(@"sqlconnection")) sum++;
            if (line.Contains(@"sqlncli")) sum++;
            if (line.Contains(@"sqloledb")) sum++;
            if (line.Contains(@"sqlxmloledb")) sum++;
            if (line.Contains(@"sql server native client")) sum++;
            if (line.Contains(@"sql server")) sum++;
            return sum > limit;
        }

        private static string[] parts = new string[] { "data source=", "user id=", "password=",
            "integrated security=", "server=", "protocol=", "host=", "address=", "pool size=",
            "timeout=", "lifetime=", "pooling=", "uid=", "driver=", "pwd=" };

        private bool CheckForConnectionString(string line, int limit = 2)
        {
            if (string.IsNullOrEmpty(line)) return false;
            return parts.Count(part => line.IndexOf(part) >= 0) > limit;
        }

        public int? GetMaxMainProgressValue()
        {
            return _totalSteps <= 0 ? (int?)null : _totalSteps;
        }

        public event Action<ProcessingModuleEventData> OnProgress;
    }
}