using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataFlow.Model
{
	public class OutputPin : Pin
	{
		public IEnumerable<InputPin> ConnectedPins { get { return _connectedInputPins; } }
		private ISet<InputPin> _connectedInputPins = new HashSet<InputPin>();

		private OutputPin()
		{
		}

		public static OutputPin Create(FilterPinDesc desc, FilterInstance filter)
		{
			return new OutputPin()
			{
				PinDesc = desc,
				FilterInstance = filter
			};
		}

		public override bool IsConnected
		{
			get
			{
				return _connectedInputPins.Any();
			}
		}

		internal void Connect(InputPin inputPin)
		{
			inputPin.Connect(this);

			_connectedInputPins.Add(inputPin);
		}

		public override void Disconnect()
		{
			foreach (var input in _connectedInputPins)
			{
				input.InternalDisconnect();
			}

			_connectedInputPins.Clear();
		}

		internal void InternalDisconnect(InputPin inputPin)
		{
			_connectedInputPins.Remove(inputPin);
		}

		public override void Push(object value)
		{
			Value = value;

			foreach (var input in ConnectedPins)
			{
				input.Push(value);
			}
		}
	}
}
