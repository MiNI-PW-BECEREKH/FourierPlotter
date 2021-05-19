using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
        //To Remove Close Button -- WPF doesn't have such built in functionality
        // Prep stuff needed to remove close button on window
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);


        private DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Render);
        private Stopwatch stopwatch = new Stopwatch();
        private Point NewCircleLocation = new Point();



        public MainWindow()
        {
            InitializeComponent();
            
        }


        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            //To Remove Close Button -- WPF doesn't have such built in functionality
            // Code to remove close box from window
            var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);

            ObservableCollection < Circle > Circles    = new ObservableCollection<Circle> { new Circle { Radius = 200, Frequency = 1 } };
            this.DataContext = Circles;
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += Tick;
            
            //this.DataContext = Circles;
            //((INotifyCollectionChanged)circlesDataGrid.Items).CollectionChanged += Circles_CollectionChanged;
            //DrawCircles();




        }


        public async void Tick(object sender, EventArgs e)
        {
            if (stopwatch.Elapsed.TotalSeconds < 10)
            {
                ProgressBar.Value = stopwatch.ElapsedMilliseconds;
                //RotateTransform rt = new RotateTransform(10000/360);
                //ellipses.Transform = rt;
            }
            else
            {
                timer.Stop();
                stopwatch.Stop();
                stopwatch.Reset();
                //DrawCircles();

            }

        }
        private void ExitMenuOption_Clicked(object sender, RoutedEventArgs e)
        {
            MessageBoxResult Result = MessageBox.Show(this, "Are you sure you want to exit?", "Leaving?", MessageBoxButton.OKCancel);
            if (Result == MessageBoxResult.OK)
            {
                this.Close();

            }
        }

        private void Start_Clicked(object sender, RoutedEventArgs e)
        {
            timer.Start();
            stopwatch.Start();


        }

        private void Pause_Clicked(object sender, RoutedEventArgs e)
        {
            stopwatch.Stop();
            timer.Stop();
        }

        private void Reset_Clicked(object sender, RoutedEventArgs e)
        {
            ProgressBar.Value = 0;
            stopwatch.Reset();
            timer.Stop();
            //Plotter.Children.Clear();
        }

        private void MenuItem_OnChecked(object sender, RoutedEventArgs e)
        {
            if (circlesDataGrid != null)
                foreach (var item in Canvas.Children)
                {
                    if (item is Ellipse)
                    {
                        Ellipse ellipse = item as Ellipse;
                        ellipse.Visibility = Visibility.Visible;

                    }
                }
        }

        private void MenuItem_OnUnchecked(object sender, RoutedEventArgs e)
        {
            if (circlesDataGrid != null)
            foreach (var item in Canvas.Children)
            {
                if (item is Ellipse)
                {
                    Ellipse ellipse = item as Ellipse;
                    ellipse.Visibility = Visibility.Hidden;

                }
            }
        }



        //private void DrawCircle()
        //{
        //    var previousCircle = (Circle)null;
        //    foreach (var item in circlesDataGrid.Items.SourceCollection)
        //    {
        //        var circleItem = item as Circle;
        //        if (previousCircle != null)
        //        {

        //            circleItem.HorizontalRight = (int)(previousCircle.HorizontalRight + circleItem.ellipse.Width / 2);
        //            Canvas.SetLeft(circleItem.ellipse, previousCircle.HorizontalRight - circleItem.ellipse.Width / 2);
        //        }
        //        else
        //        {
        //            circleItem.HorizontalRight = (int)(Plotter.ActualWidth / 2 + circleItem.ellipse.Width / 2);
        //            Canvas.SetLeft(circleItem.ellipse, Plotter.ActualWidth / 2 - circleItem.ellipse.Width / 2);
        //        }
        //        Canvas.SetTop(circleItem.ellipse, Plotter.ActualHeight / 2 - circleItem.ellipse.Height / 2);
        //        previousCircle = circleItem;
        //        Plotter.Children.Add(circleItem.ellipse);
        //    }
        //}
        private void AddCircleToCanvas(Circle c)
        {
            if (c != null)
            {
                Canvas.SetLeft(c.ellipse, NewCircleLocation.X - c.ellipse.Width/2);
                Canvas.SetTop(c.ellipse,NewCircleLocation.Y - c.ellipse.Height/2);
                Canvas.Children.Add(c.ellipse);
                NewCircleLocation.X += c.Radius/2;

            }

        }

        private void CirclesDataGrid_OnLoadingRow(object? sender, DataGridRowEventArgs e)
        {
            var addedCircle = e.Row.Item as Circle;
            if (addedCircle != null && addedCircle.Radius != 0)
            {
                AddCircleToCanvas(addedCircle);

            }
            //Canvas.UpdateLayout();
        }

        private void CirclesDataGrid_OnRowEditEnding(object? sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var editedCircle = e.Row.Item as Circle;

                if (!Canvas.Children.Contains(editedCircle.ellipse))
                {
                    AddCircleToCanvas(editedCircle);
                }
                else
                {
                    Canvas.Children.Clear();
                    NewCircleLocation = new Point(Canvas.ActualWidth / 2, Canvas.ActualHeight / 2);
                    //var previousCircle = (Circle)null;
                    foreach (var item in circlesDataGrid.Items.SourceCollection)
                    {
                        var circleItem = item as Circle;
                        AddCircleToCanvas(circleItem);

                    }
                }


                Canvas.UpdateLayout();


            }
        }

        private void Canvas_OnLoaded(object sender, RoutedEventArgs e)
        {
            //Image image = sender as Image;
            NewCircleLocation = new Point(Canvas.ActualWidth / 2, Canvas.ActualHeight / 2);
        }
    }
}
