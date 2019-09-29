using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Dialogs;

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
            ReestrCheck(null, null);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (((string.IsNullOrWhiteSpace(Path.Text)))||(!Directory.Exists(Path.Text)))
            {
                MessageBox.Show("Не задана директория для анализируемых файлов", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (CheckBoxReestr.IsChecked.GetValueOrDefault())
            {
                if (((string.IsNullOrWhiteSpace(ReestrPath.Text)))||(!Directory.Exists(Path.Text)))
                {
                    MessageBox.Show("Не задана директория для файлов реестра ПО","Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
                    return;
                }
                ProgressWindow progressWindow = new ProgressWindow(Path.Text, ReestrPath.Text);
                this.Hide();
                progressWindow.ShowDialog();
                this.Show();
            }
        }

        private void ReestrCheck(object sender, RoutedEventArgs e)
        {
            var enable = CheckBoxReestr.IsChecked.GetValueOrDefault();
            reestrPathBtn.IsEnabled = enable;
            ReestrPath.IsEnabled = enable;
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            //((Button)sender).Foreground = Brushes.DimGray;
            ((Button)sender).Background = System.Windows.Media.Brushes.Red;
        }

        private void BtnStart_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Button)sender).Background = System.Windows.Media.Brushes.LimeGreen;
            //((Button)sender).Foreground = Brushes.White;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog((folderPath) =>
            {
                Path.Text = folderPath;
            });
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog((folderPath) => 
            {
                ReestrPath.Text = folderPath;
            });
        }

        private static void OpenFolderDialog(Action<string> action)
        {
            var dlg = new CommonOpenFileDialog();
            dlg.Title = "My Title";
            dlg.IsFolderPicker = true;
            dlg.InitialDirectory = "C:\\";

            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.DefaultDirectory = "C:\\";
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                action(dlg.FileName);
            }
        }
    }
}
