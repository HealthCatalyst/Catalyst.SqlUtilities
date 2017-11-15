using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ColumnExtractor
{
		public static class Helpers
		{
				public static object GetPropertyValueWithName(object obj, string name)
				{
						var property = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
							.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

						return property == null ? null : property.GetValue(obj, null);
				}

				public static IEnumerable GetEnumerablePropertyValueWithName(object obj, string name)
				{
						var property = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
							.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

						if (property == null) return null;

						IEnumerable value = property.GetIndexParameters().Length > 0
									? AsEnumerable(property, obj)
									: (IEnumerable)property.GetValue(obj, null);

						return value;
				}

				public static IEnumerable<PropertyInfo> GetPropertiesWithNames(object obj, params string[] names)
				{
						var allNames = names.Select(n => n.ToLowerInvariant()).ToArray();
						return obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
							.Where(p => allNames.Contains(p.Name.ToLowerInvariant()));
				}

				public static PropertyInfo GetPropertyWithName(object obj, string name)
				{
						return obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
							.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
				}

				public static IEnumerable AsEnumerable(PropertyInfo indexerProperty, object o)
				{
						var list = new List<object>();

						if (indexerProperty != null)
						{
								var len = GetPropertyValue<int>(o, "Length");
								if (len == 0) return null;

								for (var i = 0; i < len; i++)
								{
										var item = indexerProperty.GetValue(o, new object[] { i });
										if (item != null)
										{
												list.Add(item);
										}
								}
						}

						return list;
				}

				public static T GetPropertyValue<T>(object source, string property)
				{
						if (source == null)
								throw new ArgumentNullException("source");

						var sourceType = source.GetType();
						var sourceProperties = sourceType.GetProperties();
						var properties = sourceProperties
							.Where(s => s.Name.Equals(property))
							.ToArray();

						if (!properties.Any())
						{
								sourceProperties = sourceType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic);
								properties = sourceProperties.Where(s => s.Name.Equals(property)).ToArray();
						}

						if (properties.Any())
						{
								var propertyValue = properties
									.Select(s => s.GetValue(source, null))
									.FirstOrDefault();

								return (T)propertyValue;
						}

						return default(T);
				}
		}
}