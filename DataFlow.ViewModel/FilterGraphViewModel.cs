using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DataFlow.ViewModel
{
	public class FilterGraphViewModel : INotifyPropertyChanged
	{
		private Model.FilterGraph _filterGraph;

		public IReadOnlyDictionary<Guid, FilterViewModel> FilterLookup { get; private set; }
		public IEnumerable<ConnectionViewModel> Connections { get; private set; }

		public IEnumerable<object> AllObjects
		{
			get
			{
				return FilterLookup.Values.Cast<object>().Concat(Connections);
			}
		}

		public IEnumerable<FilterViewModel> Filters
		{
			get
			{
				return FilterLookup.Values;
			}
		}

		public IEnumerable<InputPinViewModel> InputPins
		{
			get
			{
				return Filters.SelectMany(f => f.InputPins);
			}
		}

		public IEnumerable<OutputPinViewModel> OutputPins
		{
			get
			{
				return Filters.SelectMany(f => f.OutputPins);
			}
		}

		public IEnumerable<PinViewModel> Pins
		{
			get
			{
				return InputPins.Cast<PinViewModel>().Concat(OutputPins);
			}
		}

		public FilterGraphViewModel(Model.FilterGraph graph)
		{
			_filterGraph = graph;

			var filterViewModels = graph.Filters.ToDictionary(i => i, i => new ViewModel.FilterViewModel(i, this));
			var connections = new List<ConnectionViewModel>();

			foreach (var f in filterViewModels)
			{
				foreach (var outputPin in f.Key.OutputPins)
				{
					var outputPinViewModel = f.Value.FindOutput(outputPin.PinDesc.Name);

					foreach (var inputPin in outputPin.ConnectedPins)
					{
						var otherFilter = inputPin.FilterInstance;

						var inputPinViewModel = filterViewModels[otherFilter].FindInput(inputPin.PinDesc.Name);

						connections.Add(new ConnectionViewModel(outputPinViewModel, inputPinViewModel));
					}
				}
			}

			FilterLookup = filterViewModels.ToDictionary(kvp => kvp.Key.Guid, kvp => kvp.Value);
			Connections = connections;

			graph.RunStateChanged += graph_RunStateChanged;
		}

		void graph_RunStateChanged(object sender, EventArgs e)
		{
			CommandManager.InvalidateRequerySuggested();
		}

		public ICommand SingleStepCommand { get { return new MVVM.RelayCommand(param => this.SingleStep(), param => this.CanRun); } }
		public ICommand FullStepCommand { get { return new MVVM.RelayCommand(param => this.FullStep(), param => this.CanRun); } }
		public ICommand RunCommand { get { return new MVVM.RelayCommand(param => this.Run(), param => this.CanRun); } }
		public ICommand StopCommand { get { return new MVVM.RelayCommand(param => this.Stop(), param => this.CanStop); } }

		public bool CanRun
		{
			get
			{
				return _filterGraph.RunState == Model.RunState.Stopped;
			}
		}

		public bool CanStop
		{
			get
			{
				return _filterGraph.RunState == Model.RunState.Running;
			}
		}

		public void SingleStep()
		{
			_filterGraph.SingleStep();
		}

		public void FullStep()
		{
			var t = _filterGraph.FullStep();
			if (t.Status == TaskStatus.Faulted)
			{
				var msg = string.Format("Execution failed ({0})", t.Exception.InnerExceptions.First().Message);
				System.Windows.MessageBox.Show(msg, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
			}
		}

		public void Run()
		{
			_filterGraph.Run();
		}

		public void Stop()
		{
			_filterGraph.Stop();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void FirePropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
