using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using DataFlow.Model.Filters;
using DataFlow.Model;

namespace DataFlow
{
	class FilterEnumerator
	{
		public IReadOnlyList<FilterType> SourceFilters { get; private set; }
		public IReadOnlyList<FilterType> TransformFilters { get; private set; }
		public IReadOnlyList<FilterType> SinkFilters { get; private set; }

		public FilterEnumerator()
		{
			EnumFilters();
		}

		private static Assembly TryLoadAssembly(string fileName)
		{
			try
			{
				return Assembly.LoadFile(fileName);
			}
			catch
			{
				Console.WriteLine("Failed to load {0}", fileName);
				return null;
			}
		}

		private IEnumerable<Assembly> FindLocalAssemblies()
		{
			var entry = Assembly.GetEntryAssembly();
			var entryDirectory = System.IO.Path.GetDirectoryName(entry.Location);
			var dllFileNames = System.IO.Directory.GetFiles(entryDirectory, "*.dll");

			foreach (var fileName in dllFileNames)
			{
				Assembly assembly = TryLoadAssembly(fileName);				

				if (assembly != null)
				{
					yield return assembly;
				}
			}
		}

		private void EnumFilters()
		{
			var allFilters = new List<FilterType>();
			
			foreach (var asm in FindLocalAssemblies())
			{
				foreach (var type in asm.GetTypes())
				{
					if( FilterType.IsFilterType(type) )
					{
						allFilters.Add(new FilterType(type));
					}
				}
			}

			SourceFilters = allFilters.Where(filter => filter.IsSourceFilter).ToList();
			TransformFilters = allFilters.Where(filter => filter.IsTransformFilter).ToList();
			SinkFilters = allFilters.Where(filter => filter.IsSinkFilter).ToList();
		}
	}
}
