using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DataFlow.ViewModel
{
	public abstract class PinViewModel : INotifyPropertyChanged
	{
		public FilterViewModel Parent { get; private set; }
		public Model.FilterPinDesc Desc { get { return _pin.PinDesc; } }
		public string TypeName { get { return _pin.PinDesc.DataType.Name; } }

		protected Rect _rect;

		private Model.Pin _pin;

		protected PinViewModel(FilterViewModel parent, Model.Pin pin)
		{
			Parent = parent;
			_pin = pin;

			parent.PropertyChanged += parent_PropertyChanged;
		}

		void parent_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Left" || e.PropertyName == "Top")
			{
				FirePropertyChanged("ConnectionPoint");
			}
		}

		public abstract Point ConnectionPoint { get; }		

		public event PropertyChangedEventHandler PropertyChanged;

		protected void FirePropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public void SetRelativePinRect(Rect rect)
		{
			_rect = rect;
			FirePropertyChanged("ConnectionPoint");
		}

		public bool IsConnected
		{
			get
			{
				return _pin.IsConnected;
			}
		}

		private static PinViewModel _newConnectionStart;

		public bool IsNewConnectionStart
		{
			get
			{
				return _newConnectionStart == this;
			}
			private set
			{
				if (value)
				{
					if (_newConnectionStart != null)
						_newConnectionStart.FirePropertyChanged("IsNewConnectionStart");

					_newConnectionStart = this;
					FirePropertyChanged("IsNewConnectionStart");
				}
				else
				{
					if (_newConnectionStart == this)
					{
						_newConnectionStart = null;
						FirePropertyChanged("IsNewConnectionStart");
					}
					else if( _newConnectionStart != null )
					{
						throw new InvalidOperationException("oO");
					}
				}
			}
		}		

		public void BeginConnect()
		{
			IsNewConnectionStart = true;

			foreach (var pin in Parent.Parent.OutputPins)
				pin.IsValidConnectionTarget = this is InputPinViewModel;
			foreach (var pin in Parent.Parent.InputPins)
				pin.IsValidConnectionTarget = this is OutputPinViewModel && !pin.IsConnected;

			if (this is InputPinViewModel)
			{
				Parent.MarkDownstreamOutputsInvalid();
			}
			else
			{
				Parent.MarkUpstreamInputsInvalid();
			}
		}

		public static void CancelConnect()
		{
			foreach( var pin in _newConnectionStart.Parent.Parent.Pins )
			{
				pin.IsValidConnectionTarget = true;
			}

			_newConnectionStart.IsNewConnectionStart = false;
		}

		public void EndConnect()
		{
			try
			{
				if (_newConnectionStart == this)
				{
					// Trying to connect to self => do nothing, probably just misclicked
				}
				else
				{
					if (!IsValidConnectionTarget)
						throw new InvalidOperationException("Cannot connect these pins!");

					_newConnectionStart.Connect(this);
				}
			}
			finally
			{
				CancelConnect();
			}
		}

		private bool _isValidConnectionTarget = true;
		public bool IsValidConnectionTarget
		{
			get
			{
				return _isValidConnectionTarget;
			}
			internal set
			{
				_isValidConnectionTarget = value;
				FirePropertyChanged("IsValidConnectionTarget");
			}
		}

		private void Connect(PinViewModel other)
		{
			if (this is OutputPinViewModel)
				(this as OutputPinViewModel).Connect(other as InputPinViewModel);
			else
				(other as OutputPinViewModel).Connect(this as InputPinViewModel);
		}

		public void Disconnect()
		{
			_pin.Disconnect();
		}

		public object Value
		{
			get
			{
				return _pin.Value;
			}
		}
	}
}
