using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DataFlow.Model;

namespace DataFlow
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private FilterEnumerator _filterEnum = new FilterEnumerator();

		private FilterGraph _filterGraph;

		public MainWindow()
		{
			InitializeComponent();
		}

		private IEnumerable<UIElement> CreateWorkspaceBackground()
		{
			var back = new Rectangle();
			back.Fill = Brushes.Wheat;
			back.AllowDrop = true;
			back.Drop += workSpace_Drop_1;

			var widthBinding = new Binding()
			{
				RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Canvas), 1),
				Path = new PropertyPath("ActualWidth")
			};
			var heightBinding = new Binding()
			{
				RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Canvas), 1),
				Path = new PropertyPath("ActualHeight")
			};
			back.SetBinding(Rectangle.WidthProperty, widthBinding);
			back.SetBinding(Rectangle.HeightProperty, heightBinding);

			yield return back;
		}

		private void UpdateWorkspaceViewModel()
		{
			var filterGraphViewModel = new ViewModel.FilterGraphViewModel(_filterGraph);
			var workspaceViewModel = CreateWorkspaceBackground().Concat(filterGraphViewModel.AllObjects);

			workSpace.ItemsSource = workspaceViewModel;
			menuBar.DataContext = filterGraphViewModel;
		}

		private void Window_Loaded_1(object sender, RoutedEventArgs e)
		{
			SourceFilterList.ItemsSource = _filterEnum.SourceFilters;
			TransformFilterList.ItemsSource = _filterEnum.TransformFilters;
			SinkFilterList.ItemsSource = _filterEnum.SinkFilters;

			try
			{
				_filterGraph = FilterGraph.LoadFile("FilterGraph.xml");
			}
			catch
			{
				_filterGraph = new FilterGraph();

				var source0 = _filterGraph.AddFilter(_filterEnum.SourceFilters.First(), new Point(10, 100));
				var source1 = _filterGraph.AddFilter(_filterEnum.SourceFilters.First(), new Point(10, 200));

				var transform = _filterGraph.AddFilter(_filterEnum.TransformFilters.First(), new Point(150, 150));

				var sink = _filterGraph.AddFilter(_filterEnum.SinkFilters.First(), new Point(300, 150));

				source0.Connect("Result", transform, "A");
				source1.Connect("Result", transform, "B");
				transform.Connect("Result", sink, "A");
			}

			UpdateWorkspaceViewModel();
		}

		private void FilterList_PreviewMouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
		{
			ListBox listBox = (ListBox)sender;

			object data = Utils.WPFHelpers.GetObjectDataFromPoint(listBox, e.GetPosition(listBox));

			if (data != null)
			{
				DragDrop.DoDragDrop(listBox, data, DragDropEffects.Copy);
			}
		}

		private void workSpace_Drop_1(object sender, DragEventArgs e)
		{
			var filterType = e.Data.GetData(typeof(FilterType)) as FilterType;
			var pos = e.GetPosition((IInputElement)sender);

			_filterGraph.AddFilter(filterType, pos);

			UpdateWorkspaceViewModel();
		}

		private void Pin_LayoutUpdated_1(object sender, EventArgs e)
		{
			var ellipse = sender as Ellipse;
		}

		private void Ellipse_SizeChanged_1(object sender, SizeChangedEventArgs e)
		{
			var ellipse = sender as Ellipse;
		}

		private void Window_Closed_1(object sender, EventArgs e)
		{
			_filterGraph.SaveFile("FilterGraph.xml");
		}

		private class LineAdorner : Adorner
		{
			public Point From { get; set; }
			public Point To { get; set; }
			
			public LineAdorner(UIElement adornedElement)
				:	base(adornedElement)
			{
				IsHitTestVisible = false;
			}

			protected override void OnRender(DrawingContext drawingContext)
			{
				var pen = new Pen(Brushes.Red, 2);

				drawingContext.DrawLine(pen, From, To);				
			}
		}

		private LineAdorner _draggingConnectionLineAdorner = null;

		private void Pin_PreviewMouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
		{
			var pin = ((FrameworkElement)sender).DataContext as ViewModel.PinViewModel;
			pin.BeginConnect();

			var adornerLayer = AdornerLayer.GetAdornerLayer(workSpace);

			if( _draggingConnectionLineAdorner != null )
			{
				adornerLayer.Remove(_draggingConnectionLineAdorner);
			}

			var pos = e.GetPosition(workSpace);

			_draggingConnectionLineAdorner = new LineAdorner(workSpace) { From = pos, To = pos };

			adornerLayer.Add(_draggingConnectionLineAdorner);

			//workSpace.CaptureMouse();

			e.Handled = true;
		}

		private void workSpace_PreviewMouseMove_1(object sender, MouseEventArgs e)
		{
			if (_draggingConnectionLineAdorner != null)
			{
				_draggingConnectionLineAdorner.To = e.GetPosition(workSpace);
				_draggingConnectionLineAdorner.InvalidateVisual();
			}
		}

		private void workSpace_PreviewMouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
		{
			var adornerLayer = AdornerLayer.GetAdornerLayer(workSpace);

			if (_draggingConnectionLineAdorner != null)
			{
				adornerLayer.Remove(_draggingConnectionLineAdorner);
				_draggingConnectionLineAdorner = null;

				//workSpace.ReleaseMouseCapture();

				ViewModel.PinViewModel.CancelConnect();

				e.Handled = true;				
			}
		}

		private void PinEllipse_PreviewMouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
		{
			var adornerLayer = AdornerLayer.GetAdornerLayer(workSpace);

			if (_draggingConnectionLineAdorner != null)
			{
				adornerLayer.Remove(_draggingConnectionLineAdorner);
				_draggingConnectionLineAdorner = null;

				//workSpace.ReleaseMouseCapture();

				var pin = ((FrameworkElement)sender).DataContext as ViewModel.PinViewModel;

				try
				{
					pin.EndConnect();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}

				UpdateWorkspaceViewModel();

				e.Handled = true;
			}
		}

		private void Filter_MouseDown_1(object sender, MouseButtonEventArgs e)
		{
			var filter = ((FrameworkElement)sender).DataContext as ViewModel.FilterViewModel;

			if (e.ChangedButton == MouseButton.Middle)
			{
				filter.Delete();

				UpdateWorkspaceViewModel();
			}
		}

		private void PinEllipse_MouseDown_1(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Middle)
			{
				var pin = ((FrameworkElement)sender).DataContext as ViewModel.PinViewModel;
				pin.Disconnect();

				UpdateWorkspaceViewModel();

				e.Handled = true;
			}
		}

		private void SingleStepCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
		}

		private void SingleStepCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}	

		private void FullStepCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
		}

		private void RunCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{	
		}

		private void StopCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
		}			
	}
}
