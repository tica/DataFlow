using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataFlow.Model.Filters;

namespace DataFlow.BuiltIn.Filters
{
	[Filter(DisplayName="Random Number Generator")]
	public class RandomNumberGenerator : IFilter
	{
		[FilterOutput(DisplayName="Random Number")]
		public int Result { get; private set; }

		private Random _random = new Random();		

		public void Run()
		{
			Result = _random.Next();
		}
	}
}
