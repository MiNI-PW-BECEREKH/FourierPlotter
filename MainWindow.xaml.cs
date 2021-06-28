using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
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
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Win32;

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
        ObservableCollection<Circle> Circles = new ObservableCollection<Circle> { new Circle { Radius = 200, Frequency = 1 } };


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

            
            //circlesDataGrid.ItemsSource = Circles;
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += Tick;
            
            circlesDataGrid.ItemsSource = Circles;
            ((INotifyCollectionChanged)Circles).CollectionChanged += Circles_CollectionChanged;
            //DrawCircles();




        }

        private void Circles_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            Canvas.UpdateLayout();
            circlesDataGrid.UpdateLayout();
        }


        public async void Tick(object sender, EventArgs e)
        {
            if (stopwatch.Elapsed.TotalSeconds <= 11)
            {
                ProgressBar.Value = stopwatch.ElapsedMilliseconds;

                var prv = (Circle) null;
                for(int i = 0 ; i < Circles.Count; i++)
                {
                    Circles[i].PreviousCircle = i - 1 < 0 ? (Circle)null : Circles[i - 1];
                    Circles[i].NextCircle = i + 1 >= Circles.Count ? (Circle)null : Circles[i + 1];
                }

                //change is inevidable I am making the mistake of not handling the change

                //let's try to rewrite above code
                foreach (Circle item in Circles)
                {
                    //VISIT AGAIN LATER TO FINISH --> COULD IMPLEMENT AI TO DO SOMETHING SIMILAR AS IN https://gofigure.impara.ai/inspiration
                    //CIRCLES ARE NOT ROTATING PROPERLY AROUND EACH OTHER, FREQUENCY CALCULATIONS ARE NOT WORKING
                    //COULD HAVE USED GEOMETRY CLASSES IN WPF
                    TransformGroup myTransforms = new TransformGroup();

                    myTransforms.Children.Add(new RotateTransform(ProgressBar.Value / 10000 * 100 * 3.6 * item.Frequency, item.Center.X, item.Center.Y));
                    //item.ellipse.RenderTransform = new RotateTransform(ProgressBar.Value/27.5 *item.Frequency , item.Radius / 2, item.Radius / 2);
                    //item.line.RenderTransform = 
                    Debug.WriteLine(ProgressBar.Value/10000*100 * 3.6);

                    //rotate circle around previous circles
                    for(int i = Circles.IndexOf(item); 0<i; i--)
                    {
                        myTransforms.Children.Add(new RotateTransform(ProgressBar.Value / 10000 * 100 * 3.6 * Circles[i].Frequency, Circles[i-1].Center.X, Circles[i - 1].Center.Y));
                        //item.ellipse.RenderTransform = new RotateTransform(ProgressBar.Value / 10000 * 100 * 3.6 * currentCircle.PreviousCircle.Frequency, Canvas.TranslatePoint(currentCircle.PreviousCircle.Center, item.ellipse).X, Canvas.TranslatePoint(currentCircle.PreviousCircle.Center, item.ellipse).Y);
                        //item.line.RenderTransform = new RotateTransform(ProgressBar.Value / 10000 * 100 * 3.6 * currentCircle.Frequency, Canvas.TranslatePoint(currentCircle.Center, item.line).X, Canvas.TranslatePoint(currentCircle.Center, item.line).Y);
                    }

                    //item.ellipse.RenderTransform = myTransforms;
                    item.line.RenderTransform = myTransforms;

                }

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

        private void ShowCircles(object sender, RoutedEventArgs e)
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

        private void HideCircles(object sender, RoutedEventArgs e)
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



        private void ShowLines(object sender, RoutedEventArgs e)
        {
            if (circlesDataGrid != null)
                foreach (var item in Canvas.Children)
                {
                    if (item is Line)
                    {
                        Line line = item as Line;
                        line.Visibility = Visibility.Visible;

                    }
                }
        }

        private void HideLines(object sender, RoutedEventArgs e)
        {
            if (circlesDataGrid != null)
                foreach (var item in Canvas.Children)
                {
                    if (item is Line)
                    {
                        Line line = item as Line;
                        line.Visibility = Visibility.Hidden;

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
                
                c.ellipse = new Ellipse
                {
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 1,
                    Width = c.Radius,
                    Height = c.Radius
                };
                c.line = new Line
                {
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 1,
                    X1= NewCircleLocation.X,
                    Y1 = NewCircleLocation.Y,
                    X2 = NewCircleLocation.X + c.Radius/2,
                    Y2 = NewCircleLocation.Y
                };
                c.angle =  Math.PI * c.Frequency/100;
                c.Center = new Point(NewCircleLocation.X , NewCircleLocation.Y );
                Canvas.SetLeft(c.ellipse, NewCircleLocation.X - c.ellipse.Width/2);
                Canvas.SetTop(c.ellipse,NewCircleLocation.Y - c.ellipse.Height/2);
                Canvas.Children.Add(c.ellipse);
                Canvas.Children.Add(c.line);
                //Ellipse temp = new Ellipse
                //{
                //    Stroke = new SolidColorBrush(Colors.Red),
                //    StrokeThickness = 5,
                //    Width = 5,
                //    Height = 5
                //};
                //Canvas.SetLeft(temp, NewCircleLocation.X);
                //Canvas.SetTop(temp, NewCircleLocation.Y );
                //Canvas.Children.Add(temp);
                NewCircleLocation.X += c.Radius/2;
                c.HorizontalRight = NewCircleLocation;

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
                    Canvas.UpdateLayout();
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

        private void NewMenuOption_Clicked(object sender, RoutedEventArgs e)
        {
            ProgressBar.Value = 0;
            stopwatch.Reset();
            timer.Stop();
            Canvas.Children.Clear();
            Circles.Clear();
            NewCircleLocation = new Point(Canvas.ActualWidth / 2, Canvas.ActualHeight / 2);
        }

        private void SaveMenuOption_Clicked(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML Files (*.xml)| *.xml";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog(this) == true)
            {
                if (saveFileDialog.FileName != "")
                {
                    var stream = new FileStream(saveFileDialog.FileName, FileMode.OpenOrCreate, FileAccess.Write);
                    var xmlSerializer = new XmlSerializer(circlesDataGrid.Items.SourceCollection.GetType());
                    xmlSerializer.Serialize(stream, circlesDataGrid.Items.SourceCollection);
                    stream.Close();
                }
            }
        }


        private void OpenMenuOption_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {

                OpenFileDialog openFileDialog = new OpenFileDialog();
                {
                    openFileDialog.Filter = "xml files (*.xml) | *.xml";
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.RestoreDirectory = true;
                    if (openFileDialog.ShowDialog(this) == true)
                    {
                        if (openFileDialog.FileName != "")
                        {
                            var stream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read);
                            var xmlSerializer = new XmlSerializer(circlesDataGrid.Items.SourceCollection.GetType());

                            //XmlDictionaryReader reader =
                            //    XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas());



                            //RESET THE APP
                            ProgressBar.Value = 0;
                            stopwatch.Reset();
                            timer.Stop();
                            Canvas.Children.Clear();
                            Circles.Clear();
                            NewCircleLocation = new Point(Canvas.ActualWidth / 2, Canvas.ActualHeight / 2);

                            Circles = (ObservableCollection<Circle>)xmlSerializer.Deserialize(stream);
                            circlesDataGrid.ItemsSource = Circles;
                            stream.Close();



                        }
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK);
            }
        }
    }
}
