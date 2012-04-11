using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataFlow.Utils
{
	public static class ReflectionHelpers
	{
		public static IEnumerable<PropertyInfo> FindAttributedProperties<TAttribute>(Type type)
		{
			foreach (var property in type.GetProperties())
			{
				foreach (var attribute in property.CustomAttributes)
				{
					if (attribute.AttributeType.Equals(typeof(TAttribute)))
					{
						yield return property;
					}
				}
			}
		}

		public static TValue GetAttributeValue<TAttributeType, TValue>(MemberInfo mi, Func<TAttributeType, TValue> selector) where TAttributeType : Attribute
		{
			var attr = mi.GetCustomAttribute<TAttributeType>();
			if (attr != null)
			{
				return selector(attr);
			}
			else
			{
				return default(TValue);
			}
		}
	}
}
