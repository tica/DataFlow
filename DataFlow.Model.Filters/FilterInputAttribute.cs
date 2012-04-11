using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataFlow.Model.Filters
{
	[AttributeUsage(AttributeTargets.Property)]
	public class FilterInputAttribute : Attribute
	{
		public string DisplayName { get; set; }
	}
}
