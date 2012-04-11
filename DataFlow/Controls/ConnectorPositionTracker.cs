using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DataFlow.Controls
{
	class ConnectorPositionTracker : Decorator
	{
		private Point _position = new Point();

		public ConnectorPositionTracker()
		{
			LayoutUpdated += ConnectorPositionTracker_LayoutUpdated;
		}

		public static DependencyObject GetAncestorByType(DependencyObject element, Type type)
		{
			if (element == null) return null;

			if (element.GetType() == type) return element;

			return GetAncestorByType(System.Windows.Media.VisualTreeHelper.GetParent(element), type);
		}

		void ConnectorPositionTracker_LayoutUpdated(object sender, EventArgs e)
		{
			var filter = GetAncestorByType(this, typeof(FilterPositionTracker));
			var point = TranslatePoint(new Point(0, 0), filter as UIElement);

			if (filter != null && _position != point)
			{
				var pin = this.DataContext as ViewModel.PinViewModel;
				pin.SetRelativePinRect(new Rect(point, new Size(ActualWidth, ActualHeight)));

				_position = point;
			}
		}
	}
}
