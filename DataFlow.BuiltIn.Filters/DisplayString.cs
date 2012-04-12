using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataFlow.Model.Filters;

namespace DataFlow.BuiltIn.Filters
{
	[Filter(DisplayName = "Display String")]
	public class DisplayString : IFilter
	{
		[FilterInput]
		public string Text { get; set; }

		public void Run()
		{
			Console.WriteLine("\"{0}\"", Text);
		}
	}
}
