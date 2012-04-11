using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace DataFlow.Utils
{
	public static class WPFHelpers
	{
		public static object GetObjectDataFromPoint(ItemsControl source, Point point)
		{
			UIElement element = source.InputHitTest(point) as UIElement;
			if (element != null)
			{
				object data = DependencyProperty.UnsetValue;
				while (data == DependencyProperty.UnsetValue)
				{
					data = source.ItemContainerGenerator.ItemFromContainer(element);
					if (data == DependencyProperty.UnsetValue)
						element = VisualTreeHelper.GetParent(element) as UIElement;
					if (element == source)
						return null;
				}
				if (data != DependencyProperty.UnsetValue)
					return data;
			}
			return null;
		}
	}
}
