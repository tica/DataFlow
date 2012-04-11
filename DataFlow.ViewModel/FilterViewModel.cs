using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataFlow.ViewModel
{
	public class FilterViewModel : INotifyPropertyChanged
	{
		public IReadOnlyList<InputPinViewModel> InputPins { get; private set; }
		public IReadOnlyList<OutputPinViewModel> OutputPins { get; private set; }

		private Model.FilterInstance _filterInstance;
		public FilterGraphViewModel Parent { get; private set; }

		public FilterViewModel(Model.FilterInstance instance, FilterGraphViewModel parent)
		{
			_filterInstance = instance;
			Parent = parent;

			InputPins = _filterInstance.InputPins.Select(pin => new InputPinViewModel(this, pin)).ToList();
			OutputPins = _filterInstance.OutputPins.Select(pin => new OutputPinViewModel(this, pin)).ToList();
		}

		public InputPinViewModel FindInput(string name)
		{
			return InputPins.Where(pin => pin.Desc.Name == name).FirstOrDefault();
		}

		public OutputPinViewModel FindOutput(string name)
		{
			return OutputPins.Where(pin => pin.Desc.Name == name).FirstOrDefault();
		}

		public double Left
		{
			get
			{
				return _filterInstance.EditorPosition.X;
			}
			set
			{
				_filterInstance.EditorPosition = new System.Windows.Point(value, _filterInstance.EditorPosition.Y);
				FirePropertyChanged("Left");
			}
		}

		public double Top
		{
			get
			{
				return _filterInstance.EditorPosition.Y;
			}
			set
			{
				_filterInstance.EditorPosition = new System.Windows.Point(_filterInstance.EditorPosition.X, value);
				FirePropertyChanged("Top");
			}
		}

		public string DisplayName
		{
			get
			{
				return _filterInstance.Type.DisplayName;
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

		public bool IsDraggable { get { return true; } }

		internal void Connect(OutputPinViewModel outputPin, InputPinViewModel other)
		{
			_filterInstance.Connect(outputPin.Desc.Name, other.Parent._filterInstance, other.Desc.Name);
		}

		internal void MarkDownstreamOutputsInvalid()
		{
			foreach (var pin in OutputPins)
			{
				pin.IsValidConnectionTarget = false;
			}

			foreach (var output in _filterInstance.OutputPins)
			{
				foreach (var input in output.ConnectedPins)
				{
					Parent.FilterLookup[input.FilterInstance.Guid].MarkDownstreamOutputsInvalid();
				}
			}
		}

		internal void MarkUpstreamInputsInvalid()
		{
			foreach (var pin in InputPins)
			{
				pin.IsValidConnectionTarget = false;
			}

			foreach (var input in _filterInstance.InputPins)
			{
				if (input.IsConnected)
				{
					Parent.FilterLookup[input.ConnectedPin.FilterInstance.Guid].MarkUpstreamInputsInvalid();
				}
			}
		}

		public void Delete()
		{
			_filterInstance.Delete();
		}
	}
}
