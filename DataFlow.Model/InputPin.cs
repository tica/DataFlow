using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataFlow.Model
{
	public class InputPin : Pin
	{
		public OutputPin ConnectedPin { get; private set; }

		private InputPin()
		{
		}

		public static InputPin Create(FilterPinDesc desc, FilterInstance filter)
		{
			return new InputPin()
			{
				PinDesc = desc,
				FilterInstance = filter
			};
		}

		public override bool IsConnected
		{
			get
			{
				return ConnectedPin != null;
			}
		}

		internal void Connect(OutputPin pin)
		{
			if (IsConnected)
				throw new InvalidOperationException("InputPin is already connected");

			ConnectedPin = pin;
		}

		public override void Disconnect()
		{
			if (ConnectedPin != null)
			{
				ConnectedPin.InternalDisconnect(this);
			}

			InternalDisconnect();
		}

		internal void InternalDisconnect()
		{
			ConnectedPin = null;
		}

		public override void Push(object value)
		{
			Value = value;
		}
	}
}
