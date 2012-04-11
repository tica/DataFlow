using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataFlow.Model
{
	public class FilterPinDesc
	{
		public string Name { get; private set; }		
		public Type DataType { get; private set; }
		public string DisplayName { get; private set; }

		internal FilterPinDesc(string name, Type dataType, string displayName)
		{
			Name = name;
			DataType = dataType;
			DisplayName = displayName;
		}
	}
}
