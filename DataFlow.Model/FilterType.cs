using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DataFlow.Model.Filters;

namespace DataFlow.Model
{
	public class FilterType
	{
		private Type _type;

		public FilterType(Type type)
		{
			_type = type;
		}

		public static bool IsFilterType(Type type)
		{
			return typeof(IFilter).IsAssignableFrom(type);
		}	

		public string DisplayName
		{
			get
			{
				var displayName = Utils.ReflectionHelpers.GetAttributeValue<FilterAttribute, string>(_type, a => a.DisplayName);

				return displayName ?? _type.Name;
			}
		}

		public string AssemblyQualifiedName
		{
			get
			{
				return _type.AssemblyQualifiedName;
			}
		}

		private IEnumerable<PropertyInfo> InputProperties
		{
			get
			{
				return DataFlow.Utils.ReflectionHelpers.FindAttributedProperties<FilterInputAttribute>(_type);
			}
		}

		private IEnumerable<PropertyInfo> OutputProperties
		{
			get
			{
				return DataFlow.Utils.ReflectionHelpers.FindAttributedProperties<FilterOutputAttribute>(_type);
			}
		}

		public bool IsSourceFilter
		{
			get
			{
				return !InputProperties.Any() && OutputProperties.Any();
			}
		}

		public bool IsSinkFilter
		{
			get
			{
				return InputProperties.Any() && !OutputProperties.Any();
			}
		}

		public bool IsTransformFilter
		{
			get
			{
				return InputProperties.Any() && OutputProperties.Any();
			}
		}

		public IFilter CreateInstance()
		{
			return (IFilter)Activator.CreateInstance(_type);
		}

		public IEnumerable<FilterPinDesc> Inputs
		{
			get
			{
				foreach (var property in InputProperties)
				{
					var displayName = DataFlow.Utils.ReflectionHelpers.GetAttributeValue<FilterInputAttribute,string>(property, a => a.DisplayName);

					yield return new FilterPinDesc(property.Name, property.PropertyType, displayName ?? property.Name);
				}
			}
		}

		public IEnumerable<FilterPinDesc> Outputs
		{
			get
			{
				foreach (var property in OutputProperties)
				{
					var displayName = DataFlow.Utils.ReflectionHelpers.GetAttributeValue<FilterOutputAttribute, string>(property, a => a.DisplayName);

					yield return new FilterPinDesc(property.Name, property.PropertyType, displayName ?? property.Name);
				}
			}
		}
	}
}
