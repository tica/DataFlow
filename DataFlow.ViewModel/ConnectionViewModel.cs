using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DataFlow.ViewModel
{
	public class ConnectionViewModel : INotifyPropertyChanged
	{
		public OutputPinViewModel Source { get; private set; }
		public InputPinViewModel Destination { get; private set; }

		public ConnectionViewModel(OutputPinViewModel source, InputPinViewModel destination)
		{
			Source = source;
			Destination = destination;

			source.PropertyChanged += source_PropertyChanged;
			destination.PropertyChanged += destination_PropertyChanged;
		}

		void destination_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			FirePropertyChanged("PathGeometry");
		}

		void source_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			FirePropertyChanged("PathGeometry");
		}

		public PathGeometry PathGeometry
		{
			get
			{
				double dx = Destination.ConnectionPoint.X - Source.ConnectionPoint.X;
				double dy = Destination.ConnectionPoint.Y - Source.ConnectionPoint.Y;
				PathSegment segment;

				if (dx < 10)
				{
					segment = new BezierSegment(Source.ConnectionPoint + new Vector(100, 0), Destination.ConnectionPoint - new Vector(100, 0), Destination.ConnectionPoint, true);
				}
				else if (Math.Abs(dx / dy) > 0.5)
				{
					var center = Source.ConnectionPoint + new Vector(dx / 2, 0);
					segment = new QuadraticBezierSegment(center, Destination.ConnectionPoint, true);
				}
				else
				{
					segment = new BezierSegment(Source.ConnectionPoint + new Vector(dx, 0), Destination.ConnectionPoint - new Vector(dx, 0), Destination.ConnectionPoint, true);
				}

				var figure = new PathFigure();
				figure.StartPoint = Source.ConnectionPoint;
				figure.Segments.Add(segment);

				var geometry = new PathGeometry();
				geometry.Figures.Add(figure);
				return geometry;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void FirePropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public bool IsDraggable { get { return false; } }
		public double Left { get; set; }	// Suppress warnings
		public double Top { get; set; }		// Suppress warnings
	}
}
