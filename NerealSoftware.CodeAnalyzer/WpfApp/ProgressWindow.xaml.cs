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
            FileProgressBar.Value = 0;
            AllProgressBar.Value = 0;
            FileProgressBar.Maximum = 100;
            AllProgressBar.Maximum = 100;
            var thread = new Thread(ProgressBarUpdate);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public void ProgressBarUpdate()
        {
            var i = 0;
            while (i < 10)
            {
                Thread.Sleep(1000);
                i++;
                FileProgressBar.Dispatcher.Invoke(() => FileProgressBar.Value = i, DispatcherPriority.Background);
                AllProgressBar.Dispatcher.Invoke(() => AllProgressBar.Value = i, DispatcherPriority.Background);
            }
            this.Dispatcher.Invoke(() => this.Hide(), DispatcherPriority.Background);
            ResultWindow resultWindow = new ResultWindow();
            resultWindow.Dispatcher.Invoke(()=> resultWindow.ShowDialog(), DispatcherPriority.Normal);
            this.Dispatcher.Invoke(() => this.Close(), DispatcherPriority.Background);
        }
    }
}
