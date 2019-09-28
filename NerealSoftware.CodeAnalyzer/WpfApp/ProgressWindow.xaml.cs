using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApp
{
    /// <summary>
    /// Логика взаимодействия для ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : Window
    {
        public ProgressWindow()
        {
            InitializeComponent();
            SetProgressBarMaximum(ProgressBarCode.File, 100, 0);
            SetProgressBarMaximum(ProgressBarCode.All, 100, 0);
            var thread = new Thread(ProgressBarUpdate);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public enum ProgressBarCode
        {
            File,
            All
        }

        public void SetProgressBarMaximum(ProgressBarCode code, int maximum, int currentValue)
        {
            if (code == ProgressBarCode.All)
            {
                AllProgressBar.Dispatcher.Invoke(() => { AllProgressBar.Maximum = maximum; AllProgressBar.Value = currentValue; }, DispatcherPriority.Background);
            }
            else if (code == ProgressBarCode.File)
            {
                FileProgressBar.Dispatcher.Invoke(() => { FileProgressBar.Maximum = maximum; AllProgressBar.Value = currentValue; }, DispatcherPriority.Background);
            }
        }
        public void SetProgressBarValue(ProgressBarCode code, int value)
        {
            if (code == ProgressBarCode.All)
            {
                AllProgressBar.Dispatcher.Invoke(() => AllProgressBar.Value = value, DispatcherPriority.Background);
            }
            else if (code == ProgressBarCode.File)
            {
                FileProgressBar.Dispatcher.Invoke(() => FileProgressBar.Value = value, DispatcherPriority.Background);
            }
        }

        public void AddTextBoxMessage(string message)
        {
            ProgressText.Dispatcher.Invoke(() => { ProgressText.Text += $"{message}\r\n"; });
        }

        public void ProgressBarUpdate()
        {
            var i = 0;
            while (i < 100)
            {
                Thread.Sleep(100);
                i++;
                SetProgressBarValue(ProgressBarCode.File, i);
                SetProgressBarValue(ProgressBarCode.All, i);
                AddTextBoxMessage($"Тест {i}.");
            }
            this.Dispatcher.Invoke(() => this.Hide(), DispatcherPriority.Background);
            ResultWindow resultWindow = new ResultWindow();
            resultWindow.Dispatcher.Invoke(()=> resultWindow.ShowDialog(), DispatcherPriority.Normal);
            this.Dispatcher.Invoke(() => this.Close(), DispatcherPriority.Background);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ProgressText.ScrollToEnd();
        }
    }
}
