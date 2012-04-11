using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataFlow.Model.Filters
{
	[AttributeUsage(AttributeTargets.Class)]
	public class FilterAttribute : Attribute
	{
		public string DisplayName { get; set; }		
	}
}
