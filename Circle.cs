using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Serialization;
using WPFLAB.Annotations;

namespace WPFLAB
{
    [DataContract]
    public class Circle :  INotifyPropertyChanged
    {
        [XmlIgnore]
        public Ellipse ellipse = new Ellipse
        {
            Stroke = new SolidColorBrush(Colors.Black),
            StrokeThickness = 1

        };

        [XmlIgnore]
        public Line line = new Line
        {
            Stroke = new SolidColorBrush(Colors.Black),
            StrokeThickness = 1
        };
        

        
        private int radius;

        [DataMember]
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

        [XmlIgnore]
        public double angle { get; set; }

        [XmlIgnore] public Circle PreviousCircle = (Circle)null;
        [XmlIgnore] public Circle NextCircle = (Circle)null;

        [XmlIgnore]
        public Point HorizontalRight { get; set; }
        [DataMember]
        public int Frequency { get; set; }
        [XmlIgnore]
        public Point Center { get; set; }

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
