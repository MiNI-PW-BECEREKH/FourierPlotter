using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Shapes;
using WPFLAB.Annotations;

namespace WPFLAB
{

    public class Circle : UIElement, INotifyPropertyChanged
    {

        public Ellipse ellipse = new Ellipse
        {
            Stroke = new SolidColorBrush(Colors.Black),
            StrokeThickness = 1

        };


        private int radius;

        public int Radius
        {
            get
            {
                return radius;
            }
            set
            {
                radius = value;
                OnPropertyChanged();
            }
        }

        public int HorizontalRight { get; set; }
        public int Frequency { get; set; }
        public Point Position { get; set; }

        public Circle()
        {
            PropertyChanged += Circle_PropertyChanged;
        }

        private void Circle_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Radius":
                {
                    
                    ellipse.Width = Radius;
                    ellipse.Height = Radius;

                }
                    break;

            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }

}
