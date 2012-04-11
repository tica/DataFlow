using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DataFlow.Model
{
	partial class FilterGraph
	{
		private CancellationTokenSource _cancel;

		public RunState RunState { get; private set; }

		public event EventHandler RunStateChanged;

		private void ChangeState(RunState newState)
		{
			RunState = newState;			
			
			if (RunStateChanged != null)
			{
				RunStateChanged(this, EventArgs.Empty);
			}
		}

		public void Stop()
		{
			System.Diagnostics.Debug.Assert(_cancel != null);

			ChangeState(RunState.Stopping);
			_cancel.Cancel();
		}

		public async void Run()
		{
			var order = GenerateExecutionOrder();

			ChangeState(RunState.Running);

			try
			{
				_cancel = new CancellationTokenSource();
				await Task.Run(() => DoRun(order, _cancel.Token));
			}
			catch (OperationCanceledException)
			{
			}
			finally
			{
				_cancel = null;
			}

			ChangeState(RunState.Stopped);
		}		

		public async Task FullStep()
		{
			var order = GenerateExecutionOrder();

			ChangeState(RunState.FullStep);
			await Task.Run(() => DoFullStep(order));
			ChangeState(RunState.Stopped);
		}

		public async void SingleStep()
		{
			ChangeState(RunState.FullStep);
			await Task.Delay(500);
			ChangeState(RunState.Stopped);
		}

		private List<FilterInstance> GenerateExecutionOrder()
		{
			// Candidate list: List of all filters that might be eligible to be executed next
			// A filter can be executed if
			//	- it is a source filter
			//	- all filters connected to its inputs have been executed

			LinkedList<FilterInstance> candidates = new LinkedList<FilterInstance>(_filters);
			List<FilterInstance> executionOrder = new List<FilterInstance>();

			// As long as we have candidates, find one that can be executed
			while (candidates.Count > 0)
			{
				FilterInstance next = null;

				foreach (var candidate in candidates)
				{
					// Source filters are always OK to execute next
					if (candidate.Type.IsSourceFilter)
					{
						next = candidate;
						break;						
					}

					// If all connected upstream filters are already scheduled to be executed, this candidate is OK
					if (candidate.EnumConnectedUpstreamFilters().All(f => executionOrder.Contains(f)))
					{
						next = candidate;
						break;
					}
				}

				if (next != null)
				{
					executionOrder.Add(next);
					candidates.Remove(next);

					// Traverse through all filters connected to the output pins
					foreach (var newCandidate in next.EnumConnectedDownstreamFilters())
					{
						// Move all new following filters to the front of the candidates list,
						// trying to make the order seem less random
						if (candidates.Remove(newCandidate))
						{
							candidates.AddFirst(newCandidate);
						}
					}
				}
				else
				{
					// We did not find a matching candidate in the candidates list, something is very wrong!
					throw new InvalidOperationException("Unable to generate execution order. Pins not connected?");
				}
			}

			return executionOrder;
		}

		private void DoFullStep( IEnumerable<FilterInstance> order )
		{
			foreach (var filter in order)
			{
				filter.Execute();
			}
		}

		private void DoRun(List<FilterInstance> order, CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				DoFullStep(order);
			}
		}
	}
}
