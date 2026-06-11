using System;
using System.Reflection;

namespace SAIN.Helpers;

internal class Reflection
{
	public static FieldInfo[] GetFieldsInType(Type type, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public)
	{
		return type.GetFields(flags);
	}

	public static object GetStaticValue(Type type, string name)
	{
		if (name == null)
		{
			return null;
		}
		FieldInfo field = type.GetField(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		if (field != null)
		{
			return field.GetValue(null);
		}
		PropertyInfo property = type.GetProperty(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		if (property != null)
		{
			return property.GetValue(null);
		}
		return null;
	}

	public static FieldInfo FindFieldByName(string name, FieldInfo[] fields)
	{
		foreach (FieldInfo fieldInfo in fields)
		{
			if (fieldInfo.Name == name)
			{
				return fieldInfo;
			}
		}
		return null;
	}
}
