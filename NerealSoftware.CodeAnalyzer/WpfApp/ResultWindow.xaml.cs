using System;
using System.Collections.Generic;
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

namespace WpfApp
{
    /// <summary>
    /// Логика взаимодействия для ResultWindow.xaml
    /// </summary>
    public partial class ResultWindow : Window
    {
        public ResultWindow()
        {
            InitializeComponent();
            FillFileDataGrid();
        }

        private void FillFileDataGrid()
        {
            var list = new List<FileGridItem>();
            for (var i = 0; i < 10; i++)
            {
                list.Add(new FileGridItem(i + 1, $"{i + 1}",$"Описание {i + 1}"));
            }
            fileGrid.ItemsSource = list;
        }

        private static FileGridItem currentSelectedItem;

        private void fileGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FileGridItem file =  (FileGridItem)fileGrid.SelectedItem;
            if ((currentSelectedItem == null) || (currentSelectedItem.Id != file.Id))
            {
                currentSelectedItem = file;
                fileDescription.Text = file.Description;
                BrowserBehavior.SetHtml(fileReportWB, GenerateHtmlForFile(file));
            }
        }

        private static string GenerateHtmlForFile(FileGridItem file)
        {
            return $"<html><meta http-equiv='Content-Type' content='text/html;charset=UTF-8'><body><p>{file.Description}</p></body></html>";
        }
    }
}
