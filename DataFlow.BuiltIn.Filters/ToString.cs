using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataFlow.Model.Filters;

namespace DataFlow.BuiltIn.Filters
{
	[Filter(DisplayName = "Convert to String")]
	public class ToString : IFilter
	{
		[FilterInput]
		public object Value { get; set; }

		[FilterOutput]
		public string Result { get; private set; }

		public void Run()
		{
			Result = Value.ToString();
		}
	}
}
