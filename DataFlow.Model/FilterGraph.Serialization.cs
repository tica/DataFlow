using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DataFlow.Model
{
	partial class FilterGraph
	{
		public XElement SaveXml()
		{
			return new XElement("FilterGraph",
				new XElement("Filters",
					_filters.Select(f => f.Save())
				),
				new XElement("Connections",
					_filters.Select(f => f.SaveConnections())
				)
			);
		}

		public void SaveFile(string fileName)
		{
			var doc = new XDocument(SaveXml());
			doc.Save(fileName);
		}

		public static FilterGraph LoadXml(XElement xml)
		{
			System.Diagnostics.Debug.Assert(xml.Name == "FilterGraph");

			var filters = xml.Element("Filters").Elements().Select(FilterInstance.Load).ToList();
			var filterLookup = filters.ToDictionary(f => f.Guid);

			foreach (var connectionsFromFilter in xml.Elements("Connections").Elements("From"))
			{
				var fromGuid = Guid.Parse(connectionsFromFilter.Attribute("FilterGuid").Value);

				foreach (var connectionsFromPin in connectionsFromFilter.Elements("From"))
				{
					var fromPin = connectionsFromPin.Attribute("Pin").Value;

					foreach (var connectionsToFilterPin in connectionsFromPin.Elements("To"))
					{
						var toGuid = Guid.Parse(connectionsToFilterPin.Attribute("FilterGuid").Value);
						var toPin = connectionsToFilterPin.Attribute("Pin").Value;

						var fromFilter = filterLookup[fromGuid];
						var toFilter = filterLookup[toGuid];

						fromFilter.Connect(fromPin, toFilter, toPin);
					}
				}
			}

			return new FilterGraph(filters);
		}

		public static FilterGraph LoadFile(string fileName)
		{
			var doc = XDocument.Load(fileName);

			return LoadXml(doc.Root);
		}
	}
}
