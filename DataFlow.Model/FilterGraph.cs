using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataFlow.Model
{
	public partial class FilterGraph
	{
		private List<FilterInstance> _filters;

		public IEnumerable<FilterInstance> Filters
		{
			get
			{
				return _filters;
			}
		}

		public FilterGraph()
		{
			_filters = new List<FilterInstance>();
		}

		private FilterGraph(IEnumerable<FilterInstance> filters)
		{
			_filters = filters.ToList();

			foreach (var f in _filters)
			{
				f.Parent = this;
			}
		}		

		public FilterInstance AddFilter(FilterType type, System.Windows.Point position)
		{
			var filter = new FilterInstance(type, position, this);

			_filters.Add(filter);

			return filter;
		}

		internal void RemoveFilter(FilterInstance filterInstance)
		{
			_filters.Remove(filterInstance);
		}
	}
}
