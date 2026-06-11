using System;
using System.Collections.Generic;

namespace SAIN.Helpers;

internal class ListHelpers
{
	public static bool ClearCache<T>(List<T> list)
	{
		if (list != null && list.Count > 0)
		{
			list.Clear();
			return true;
		}
		return false;
	}

	public static bool ClearCache<T, V>(Dictionary<T, V> list)
	{
		if (list != null && list.Count > 0)
		{
			list.Clear();
			return true;
		}
		return false;
	}

	public static void PopulateKeys<T, K>(Dictionary<T, K> dictionary, K defaultVal) where T : Enum
	{
		T[] array = EnumValues.GetEnum<T>();
		foreach (T key in array)
		{
			if (!dictionary.ContainsKey(key))
			{
				dictionary.Add(key, defaultVal);
			}
		}
	}

	public static void CloneEntries<T, K>(Dictionary<T, K> source, Dictionary<T, K> destination) where T : Enum
	{
		foreach (KeyValuePair<T, K> item in source)
		{
			if (!destination.ContainsKey(item.Key))
			{
				destination.Add(item.Key, item.Value);
			}
		}
	}
}
