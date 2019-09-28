using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                var FilePath = openFileDialog.FileName;
                Path.Text = FilePath;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ProgressWindow progressWindow = new ProgressWindow();
            this.Hide();
            progressWindow.ShowDialog();
            this.Show();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                var FilePath = openFileDialog.FileName;
                ReestrPath.Text = FilePath;
            }
        }

        private void ReestrCheck(object sender, RoutedEventArgs e)
        {
            var enable = CheckBoxReestr.IsChecked.GetValueOrDefault();
            reestrPathBtn.IsEnabled = enable;
            ReestrPath.IsEnabled = enable;
        }
    }
}
