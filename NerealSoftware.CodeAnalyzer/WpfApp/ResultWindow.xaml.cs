using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CodeAnalyzer.Interface;

namespace WpfApp
{
    /// <summary>
    /// Логика взаимодействия для ResultWindow.xaml
    /// </summary>
    public partial class ResultWindow : Window
    {
        public ResultWindow(CodeAnalyzer.Interface.ICommonResults results, List<CodeAnalyzer.Interface.IFileSource> fileSourceList)
        {
            InitializeComponent();
            _fileSourceList = fileSourceList;
            FillFileDataGrid(results);
        }

        private void FillFileDataGrid(CodeAnalyzer.Interface.ICommonResults results)
        {
            int i = 1;
            var list = new List<FileGridItem>();
            foreach (var result in results.Results)
            {
                var fileSource = result.File;
                var fileNames = result.LinkedFiles.Select(x => x.GetFileName());
                list.Add(new FileGridItem(i, $"{fileSource.GetFileName()}", $"{result.Report}", fileNames.ToList()));
                i++;
            }
            fileGrid.ItemsSource = list;
            BrowserBehavior.SetHtml(totalReportWB, GenerateTotal(results, _fileSourceList));
        }

        private FileGridItem currentSelectedItem;
        private List<IFileSource> _fileSourceList;

        private void fileGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FileGridItem file =  (FileGridItem)fileGrid.SelectedItem;
            if ((currentSelectedItem == null) || (currentSelectedItem.Id != file.Id))
            {
                currentSelectedItem = file;
                BrowserBehavior.SetHtml(fileReportWB, GenerateHtmlForFile(file));
            }
        }

        private string GenerateHtmlForFile(FileGridItem file)
        {
            return $"<html><meta http-equiv='Content-Type' content='text/html;charset=UTF-8'><body>{file.Report}</body></html>";
        }

        private static string[] fileResumeText = new[] { "Отсутствует", "Небольшая часть файлов содержит совпадения",
            "Значительная часть файлов содержит совпадения.", "Около половины файлов содержит совпадения.",
            "Больше половины файлов содержит совпадения.", "Большинство файлов содержат совпадения."};

        private static string[] dbResumeText = new[] { "Отсутствует", "Есть файлы, работающие с БД.",
            "Много файлов работает с БД.", "Большинство файлов работают с БД."};

        private string GenerateTotal(CodeAnalyzer.Interface.ICommonResults results, List<IFileSource> _fileSourceList)
        {
            var recommendations = new List<string>();
            var sb = new StringBuilder();
            sb.AppendLine($"<html><meta http-equiv='Content-Type' content='text/html;charset=UTF-8'><body>");
            var totalFileCount = _fileSourceList.Count;
            var fileDubleCount = results.Results.Count(x => x.Report?.Contains("~=") ?? false);
            var filePercent = fileDubleCount * 100 / totalFileCount;
            var fileResumeIndex = 0;
            if (filePercent > 80) fileResumeIndex = 5;
            else if (filePercent > 50) fileResumeIndex = 4;
            else if (filePercent > 30) fileResumeIndex = 3;
            else if (filePercent > 10) fileResumeIndex = 2;
            else if (filePercent > 0) fileResumeIndex = 1;
            if (filePercent > 75)
            {
                recommendations.Add("Слишком большая доля заимствованного кода, вероятно уже есть существующий аналог.");
            }

            var fileResume = fileResumeText[fileResumeIndex];

            sb.AppendLine("<h2>Отчёт по анализу исходного кода</h2>");
            sb.AppendLine("<table border=0 padding=4>");
            sb.AppendLine($"<tr><td>Общее количество проверенных файлов</td><td>{totalFileCount}</td></tr>");
            sb.AppendLine("<tr><th colspan=2>Проверка дублирования</th></tr>");
            sb.AppendLine($"<tr><td>Количество файлов с совпадениями</td><td>{fileDubleCount}</td></tr>");
            sb.AppendLine($"<tr><td>Процент файлов с совпадениями с базой кода</td><td>{filePercent}%</td></tr>");
            sb.AppendLine($"<tr><td>Вывод</td><td>{fileResume}</td></tr>");
            
            var fileMsSqlCount = results.Results.Count(x => x.Report?.Contains("MSSQL") ?? false);
            var fileOracleCount = results.Results.Count(x => x.Report?.Contains("Oracle") ?? false);
            var fileDbCount = results.Results.Count(x => (x.Report?.Contains("Oracle") ?? false)||(x.Report?.Contains("MSSQL") ?? false));
            
            var dbPercent = fileDbCount * 100 / totalFileCount;
            var dbResumeIndex = 0;
            if (dbPercent > 30) dbResumeIndex = 3;
            else if (dbPercent > 15) dbResumeIndex = 2;
            else if (dbPercent > 0) dbResumeIndex = 1;

            if (fileDbCount > 0)
            {
                recommendations.Add("В ПО используются иностранные БД, лицензирование и обслуживание которых может стоить дорого.");
            }

            var dbResume = dbResumeText[dbResumeIndex];

            sb.AppendLine("<tr><th colspan=2>Проверка использования БД</th></tr>");
            sb.AppendLine($"<tr><td>Количество файлов работающих с БД</td><td>{fileDbCount}</td></tr>");
            sb.AppendLine($"<tr><td>Из них работает с MSSQL</td><td>{fileMsSqlCount}</td></tr>");
            sb.AppendLine($"<tr><td>Из них работает с Oracle</td><td>{fileOracleCount}</td></tr>");
            sb.AppendLine($"<tr><td>Процент файлов работающих с БД</td><td>{dbPercent}%</td></tr>");
            sb.AppendLine($"<tr><td>Вывод</td><td>{dbResume}</td></tr>");
            sb.AppendLine("</table>");

            var totalIndex = fileResumeIndex + dbResumeIndex;
            var totalResume = "Отказать, продавца расстрелять";
            if (totalIndex <= 2) totalResume = "Да, да, дайте две!";
            else if (totalIndex <=5) totalResume = "Ну, если денег не жалко, то можно";
            else if (totalIndex <=7) totalResume = "Можно, за откат в 50%";

            sb.AppendLine($"<p><strong>Общий вывод:</strong> {totalResume}</p>");

            if (recommendations.Count > 0)
            {
                sb.AppendLine("<h3>Рекомендации</h3>");
                foreach (var s in recommendations)
                {
                    sb.AppendLine($"<p>{s}</p>");
                }
            }

            sb.AppendLine("</body></html>");

            return sb.ToString();
        }
    }
}
