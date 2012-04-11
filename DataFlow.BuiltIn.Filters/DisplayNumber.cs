using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataFlow.Model.Filters;

namespace DataFlow.BuiltIn.Filters
{
	[Filter(DisplayName="Display Number")]
	public class DisplayNumber : IFilter
	{
		[FilterInput]
		public int A { get; set; }

		public void Run()
		{
			Console.WriteLine(A);
		}
	}
}
