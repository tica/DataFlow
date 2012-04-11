using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataFlow.Model
{
	public abstract class Pin
	{
		public FilterPinDesc PinDesc { get; protected set; }
		public FilterInstance FilterInstance { get; protected set; }

		public abstract bool IsConnected { get; }
		public abstract void Disconnect();

		public object Value { get; protected set; }

		public abstract void Push(object value);
	}
}
