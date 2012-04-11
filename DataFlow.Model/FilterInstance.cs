using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DataFlow.Model.Filters;

namespace DataFlow.Model
{
	public class FilterInstance
	{
		public FilterType Type { get; private set; }
		public Guid Guid { get; private set; }
		public System.Windows.Point EditorPosition { get; set; }
		public FilterGraph Parent { get; internal set; }

		public IReadOnlyList<InputPin> InputPins { get; private set; }
		public IReadOnlyDictionary<string, InputPin> InputPinLookup { get; private set; }

		public IReadOnlyList<OutputPin> OutputPins { get; private set; }
		public IReadOnlyDictionary<string, OutputPin> OutputPinLookup { get; private set; }

		private IFilter _impl;

		private IReadOnlyDictionary<Pin, System.Reflection.PropertyInfo> ImplPropertyLookup;

		private FilterInstance(FilterType type)
		{
			Type = type;
			Parent = null;

			_impl = Type.CreateInstance();			

			BuildPinLists();

			var allPins = InputPins.Cast<Pin>().Concat(OutputPins);
			var implType = _impl.GetType();

			ImplPropertyLookup = allPins.ToDictionary(pin => pin, pin => implType.GetProperty(pin.PinDesc.Name));
		}

		public FilterInstance(FilterType type, System.Windows.Point editorPosition, FilterGraph parent)
			:	this( type )
		{
			Guid = Guid.NewGuid();
			EditorPosition = editorPosition;
			Parent = parent;
		}

		private void BuildPinLists()
		{
			InputPins = Type.Inputs.Select(desc => InputPin.Create(desc, this)).ToList();
			InputPinLookup = InputPins.ToDictionary(pin => pin.PinDesc.Name);
			OutputPins = Type.Outputs.Select(desc => OutputPin.Create(desc, this)).ToList();
			OutputPinLookup = OutputPins.ToDictionary(pin => pin.PinDesc.Name);
		}

		public void Connect(string outputPinName, FilterInstance other, string otherInputPinName)
		{
			OutputPin outputPin;
			if (!OutputPinLookup.TryGetValue(outputPinName, out outputPin))
			{
				throw new ArgumentException("Invalid output pin name: " + outputPinName);
			}

			InputPin otherInputPin;
			if (!other.InputPinLookup.TryGetValue(otherInputPinName, out otherInputPin))
			{
				throw new ArgumentException("Invalid other input pin name: " + otherInputPinName);
			}

			outputPin.Connect(otherInputPin);
		}

		public XElement Save()
		{
			return new XElement("Filter",
				new XAttribute("Type", Type.AssemblyQualifiedName),
				new XAttribute("Guid", Guid.ToString()),
				new XAttribute("Position", EditorPosition.ToString(new System.Globalization.CultureInfo("en-US")))
			);
		}

		public static FilterInstance Load(XElement xml)
		{
			System.Diagnostics.Debug.Assert(xml.Name == "Filter");

			try
			{
				var type = System.Type.GetType(xml.Attribute("Type").Value);
				var guid = System.Guid.Parse(xml.Attribute("Guid").Value);
				var pos = System.Windows.Point.Parse(xml.Attribute("Position").Value);

				return new FilterInstance(new FilterType(type))
				{
					EditorPosition = pos,
					Guid = guid
				};
			}
			catch
			{
				return null;
			}
		}

		private XElement SaveConnection(OutputPin outputPin, InputPin inputPin )
		{
			return new XElement("To",
				new XAttribute("FilterGuid", inputPin.FilterInstance.Guid.ToString()),
				new XAttribute("Pin", inputPin.PinDesc.Name)
			);
		}

		private XElement SaveConnections(OutputPin pin)
		{
			if (!pin.ConnectedPins.Any())
				return null;

			return new XElement("From",
				new XAttribute("Pin", pin.PinDesc.Name),
				pin.ConnectedPins.Select( inputPin => SaveConnection(pin, inputPin) )
			);
		}

		internal XElement SaveConnections()
		{
			if( !OutputPins.Any( pin => pin.ConnectedPins.Any() ) )
				return null;

			return new XElement("From",
				new XAttribute("FilterGuid", Guid),
				OutputPins.Select( SaveConnections )
			);			
		}

		public void Delete()
		{
			foreach (var input in InputPins)
			{
				input.Disconnect();
			}
			foreach (var output in OutputPins)
			{
				output.Disconnect();
			}

			Parent.RemoveFilter(this);
		}

		internal IEnumerable<FilterInstance> EnumConnectedUpstreamFilters()
		{
			foreach (var input in InputPins)
			{
				yield return input.ConnectedPin.FilterInstance;
			}
		}

		internal IEnumerable<FilterInstance> EnumConnectedDownstreamFilters()
		{
			foreach (var output in OutputPins)
			{
				foreach (var input in output.ConnectedPins)
				{
					yield return input.FilterInstance;
				}
			}
		}

		internal void Execute()
		{
			foreach( var input in InputPins )
			{
				object val = input.Value;

				// Do conversion here?

				ImplPropertyLookup[input].SetValue(_impl, val);
			}

			_impl.Run();

			foreach (var output in OutputPins)
			{
				object val = ImplPropertyLookup[output].GetValue(_impl);

				output.Push(val);
			}
		}
	}
}
