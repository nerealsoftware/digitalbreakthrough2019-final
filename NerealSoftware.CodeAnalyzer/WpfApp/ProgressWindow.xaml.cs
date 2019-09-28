using CodeAnalyzer.Sources;
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
        private int maxAllProgress = 100;
        private int maxSecondProgress = 100;
        public ProgressWindow(string path, string reestrPath)
        {
            InitializeComponent();
            SetProgressBarMaximum(ProgressBarCode.File, maxSecondProgress, 0);
            SetProgressBarMaximum(ProgressBarCode.All, maxAllProgress, 0);
            var thread = new Thread(() => ExecuteThread(path, reestrPath));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void ProgressModuleDisplayHandler(CodeAnalyzer.Interface.ProcessingModuleEventData eventData)
        {
            if (eventData.MaxMainProgress != maxAllProgress)
            {
                SetProgressBarMaximum(ProgressBarCode.All, eventData.MaxMainProgress, eventData.CurrentMainProgress);
                maxAllProgress = eventData.MaxMainProgress;
            }
            else
            {
                SetProgressBarValue(ProgressBarCode.All, eventData.CurrentMainProgress);
            }
            if (eventData.MaxSecondProgress != maxSecondProgress)
            {
                SetProgressBarMaximum(ProgressBarCode.File, eventData.MaxSecondProgress, eventData.CurrentSecondProgress);
                maxSecondProgress = eventData.MaxSecondProgress;
            }
            else
            {
                SetProgressBarValue(ProgressBarCode.File, eventData.CurrentSecondProgress);
            }
            if (!string.IsNullOrEmpty((eventData.Message)))
            {
                AddTextBoxMessage(eventData.Message);
            }
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

        public void ExecuteThread(string path, string reestrPath)
        {
            var progressModule = ModuleFactory.CreateCodeBaseProcessingModule(reestrPath);
            progressModule.OnProgress += ProgressModuleDisplayHandler;
            var fileSourceList = new FileSystemSource(path).GetFiles();
            var results = progressModule.Execute(fileSourceList);

            ShowReport(results);

            /*var i = 0;
            while (i < 10)
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
            this.Dispatcher.Invoke(() => this.Close(), DispatcherPriority.Background);*/
        }

        private void ShowReport(CodeAnalyzer.Interface.ICommonResults results)
        {
            this.Dispatcher.Invoke(() => this.Hide(), DispatcherPriority.Background);
            ResultWindow resultWindow = new ResultWindow(results);
            resultWindow.Dispatcher.Invoke(() => resultWindow.ShowDialog(), DispatcherPriority.Normal);
            this.Dispatcher.Invoke(() => this.Close(), DispatcherPriority.Background);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ProgressText.ScrollToEnd();
        }
    }
}
