using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WPFLAB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new List<RowEntry>{new RowEntry{A = 1,B=1,C=1}};
            timer.Interval = new TimeSpan(0, 0, 0,0,1);
            timer.Tick += new EventHandler(Tick);
            
        }



        public void Tick(object sender, EventArgs e)
        {
            if (ProgressBar.Value < 100)
            {
                ProgressBar.Value += ProgressBar.ActualWidth/10000;
            }
            else
            {
                timer.Stop();
                Debug.WriteLine("TIMER STOPPED");
                
            }
        }
        private void ExitMenuOption_Clicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Start_Clicked(object sender, RoutedEventArgs e)
        {
            timer.Start();
        }

        private void Pause_Clicked(object sender, RoutedEventArgs e)
        {
            timer.Stop();
        }

        private void Reset_Clicked(object sender, RoutedEventArgs e)
        {
            ProgressBar.Value = 0;
            timer.Stop();
        }

    }
}
