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
using CodeAnalyzer.Interface;

namespace WpfApp
{
    /// <summary>
    /// Логика взаимодействия для ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : Window
    {
        private int maxAllProgress = 100;
        private int maxSecondProgress = 100;
        public ProgressWindow(StartingModulesParameters parameters/*string Path, string ReestrPath*/)
        {
            InitializeComponent();
            SetProgressBarMaximum(ProgressBarCode.File, maxSecondProgress, 0);
            SetProgressBarMaximum(ProgressBarCode.All, maxAllProgress, 0);
            var thread = new Thread(() => ExecuteThread(parameters));
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

            SetProgressBarValue(ProgressBarCode.All, eventData.CurrentMainProgress);
            
            if (eventData.MaxSecondProgress != maxSecondProgress)
            {
                SetProgressBarMaximum(ProgressBarCode.File, eventData.MaxSecondProgress, eventData.CurrentSecondProgress);
                maxSecondProgress = eventData.MaxSecondProgress;
            }

            SetProgressBarValue(ProgressBarCode.File, eventData.CurrentSecondProgress);

            if (!string.IsNullOrEmpty(eventData.Message))
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
            ProgressText.Dispatcher.Invoke(() => { ProgressText.Text += $"\r\n{message}"; });
        }

        public void ExecuteThread(StartingModulesParameters parameters)
        {
            // todo: строить список модулей исходя из включенных галочек
            var modules = new List<IProcessingModule>();
            modules.Add(ModuleFactory.CreateCodeBaseProcessingModule(parameters.ReestrPath));
            if (parameters.IsHeuristicEnabled) modules.Add(ModuleFactory.CreateDatabaseHeuristicsModule());

            var progressModule = ModuleFactory.CreateContainer(modules);
            progressModule.OnProgress += ProgressModuleDisplayHandler;
            var fileSourceList = new FileSystemSource(parameters.Path).GetFiles().ToList();
            var results = progressModule.Execute(fileSourceList);

            ShowReport(results, fileSourceList);
        }

        private void ShowReport(CodeAnalyzer.Interface.ICommonResults results, List<IFileSource> fileSourceList)
        {
            this.Dispatcher.Invoke(() => this.Hide(), DispatcherPriority.Background);
            ResultWindow resultWindow = new ResultWindow(results, fileSourceList);
            resultWindow.Dispatcher.Invoke(() => resultWindow.ShowDialog(), DispatcherPriority.Normal);
            this.Dispatcher.Invoke(() => this.Close(), DispatcherPriority.Background);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ProgressText.ScrollToEnd();
        }
    }
}
