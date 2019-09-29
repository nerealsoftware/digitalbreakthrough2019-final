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
            _totalSteps = 2;
            _localSteps = list.Count + 1;

            var results = new List<IProcessingResult>();

            OnProgress?.Invoke(BuildProgress(_localSteps, "Поиск обращений к пропиетарным СУБД"));

            // поиск строк подключения
            FindConnectionStrings(list, results);

            _totalCurrent++;

            // поиск пространств имен
            FindNamespaces(list, results);

            _totalCurrent++;

            OnProgress?.Invoke(BuildProgress(_localSteps, "Поиск обращений к пропиетарным СУБД завершен"));

            return new DatabaseHeuristicsReport(list) {Results = results};
        }

        private void FindNamespaces(List<IFileSource> list, List<IProcessingResult> results)
        {
            OnProgress?.Invoke(BuildProgress(0, "Поиск пространств имен провайдеров БД"));

            var spaces = new Dictionary<string, List<string>>();

            for (var index = 0; index < list.Count; index++)
            {
                spaces.Clear();

                var fileSource = list[index];
                var fileName = fileSource.GetFileName();
                var localStep = index + 1;

                OnProgress?.Invoke(BuildProgress(localStep, $"Обработка файла {fileName}..."));

                var sb = new StringBuilder();
                var data = fileSource.GetData();

                using (var reader = new StringReader(data))
                {
                    var oline = reader.ReadLine();
                    int lineNumber = 1;
                    while (oline != null)
                    {
                        var line = oline.ToLower();
                        if (line.Contains("using "))
                        {
                            if (line.Contains("oracle.dataaccess.") && !spaces.ContainsKey("Oracle"))
                            {
                                OnProgress?.Invoke(BuildProgress(localStep, "Найдено обращение к провайдеру БД Oracle: {oline}"));
                                //if (!spaces.ContainsKey("Oracle")) spaces["Oracle"] = new List<string>();
                                spaces["Oracle"] = new List<string>();
                                spaces["Oracle"].Add($"строка {lineNumber}: {oline}");
                            }

                            if (line.Contains("system.data.sqlclient") && !spaces.ContainsKey("MSSQL"))
                            {
                                OnProgress?.Invoke(BuildProgress(localStep, "Найдено обращение к провайдеру БД MSSQL: {oline}"));
                                //if (!spaces.ContainsKey("Oracle")) spaces["Oracle"] = new List<string>();
                                spaces["MSSQL"] = new List<string>();
                                spaces["MSSQL"].Add($"строка {lineNumber}: {oline}");
                            }
                        }
                        oline = reader.ReadLine();
                        lineNumber++;
                    }
                }

                if (!spaces.Any()) continue;

                sb.Clear();
                sb.AppendLine($"В файле {fileName} найдено(ы) обращения к провайдерам БД");
                foreach (var pair in spaces)
                {
                    var engine = pair.Key;
                    sb.AppendLine($"Сервер {engine}:");
                    foreach (var msg in pair.Value) sb.AppendLine($" - {msg}");
                    sb.AppendLine();
                }
                results.Add(new DatabaseProcessingResult(fileSource, sb.ToString()));
            }

            OnProgress?.Invoke(BuildProgress(_localSteps, "Поиск пространств имен провайдеров БД завершен"));
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
                var data = fileSource.GetData();

                using (var reader = new StringReader(data))
                {
                    var oline = reader.ReadLine();
                    int lineNumber = 1;
                    while (oline != null)
                    {
                        var line = oline?.ToLower();
                        if (CheckForConnectionString(line))
                        {
                            OnProgress?.Invoke(BuildProgress(localStep, $"Найдена строка <{line}>"));
                            if (IsMsSqlConnectionString(line))
                            {
                                OnProgress?.Invoke(BuildProgress(localStep, "Найдена строка MS SQL: {oline}"));
                                if (!connStrings.ContainsKey("MSSQL")) connStrings["MSSQL"] = new List<string>();
                                connStrings["MSSQL"].Add($"строка {lineNumber}: {oline}");
                            }
                            else if (IsOracleConnectionString(line))
                            {
                                OnProgress?.Invoke(BuildProgress(localStep, "Найдена строка Oracle: {oline}"));
                                if (!connStrings.ContainsKey("Oracle")) connStrings["Oracle"] = new List<string>();
                                connStrings["Oracle"].Add($"строка {lineNumber}: {oline}");
                            }
                        }
                        oline = reader.ReadLine();
                        lineNumber++;
                    }
                }

                if (!connStrings.Any()) continue;

                sb.Clear();
                sb.AppendLine($"В файле {fileName} найдена(ы) строки подключения к БД");
                foreach (var pair in connStrings)
                {
                    var engine = pair.Key;
                    sb.AppendLine($"Сервер {engine}:");
                    foreach (var msg in pair.Value) sb.AppendLine($" - {msg}");
                    sb.AppendLine();
                }
                results.Add(new DatabaseProcessingResult(fileSource, sb.ToString()));
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
            "timeout=", "lifetime=", "pooling=", "uid=", "driver=", "pwd=", "database=",
            "trusted connection=", "data_source=", "user_id=", "integrated_security=",
            "pool_size=", "trusted_connection="
        };

        private bool CheckForConnectionString(string line, int limit = 2)
        {
            if (string.IsNullOrEmpty(line)) return false;
            int count = 0;
            foreach (var part in parts)
            {
                if (line.IndexOf(part) >= 0) count++;
            }

            //if (line.Contains(".mdf")) count++;
            //if (line.Contains(".ldf")) count++;

            return count > limit;
        }

        public int? GetMaxMainProgressValue()
        {
            return _totalSteps <= 0 ? (int?)null : _totalSteps;
        }

        public event Action<ProcessingModuleEventData> OnProgress;
    }
}