using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataFlow.Model.Filters;

namespace DataFlow.BuiltIn.Filters
{
	[Filter(DisplayName="Add Integers")]
    public class AddIntegers : IFilter
    {
		[FilterInput]
		public int A { get; set; }

		[FilterInput]
		public int B { get; set; }

		[FilterOutput]
		public int Result { get; private set; }

		public void Run()
		{
			Result = A + B;
		}
	}
}
