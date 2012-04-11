using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataFlow.Model.Filters;

namespace DataFlow.BuiltIn.Filters
{
	[Filter(DisplayName="Number Sequence")]
	class NumberSequence : IFilter
	{
		[FilterOutput]
		public int Result { get; private set; }

		private int _prev = 0;

		public void Run()
		{
			Result = ++_prev;
		}
	}
}
