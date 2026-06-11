using System.Collections.Generic;
using System.Reflection;
using SAIN.Attributes;
using UnityEngine;

namespace SAIN.Editor;

public sealed class Category
{
	public readonly ConfigInfoClass CategoryInfo;

	public readonly List<ConfigInfoClass> FieldAttributesList = new List<ConfigInfoClass>();

	public readonly List<ConfigInfoClass> SelectedList = new List<ConfigInfoClass>();

	public bool Open = false;

	public Vector2 Scroll = Vector2.zero;

	public Category(ConfigInfoClass attributes)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		CategoryInfo = attributes;
		FieldInfo[] fields = attributes.ValueType.GetFields(BindingFlags.Instance | BindingFlags.Public);
		foreach (FieldInfo member in fields)
		{
			ConfigInfoClass attributeInfo = AttributesGUI.GetAttributeInfo(member);
			if (attributeInfo != null && !attributeInfo.Hidden)
			{
				FieldAttributesList.Add(attributeInfo);
			}
		}
	}

	public object GetValue(object obj)
	{
		return CategoryInfo.GetValue(obj);
	}

	public void SetValue(object obj, object value)
	{
		CategoryInfo.SetValue(obj, value);
	}

	public int OptionCount(out int realCount)
	{
		realCount = 0;
		int num = 0;
		foreach (ConfigInfoClass fieldAttributes in FieldAttributesList)
		{
			if (!fieldAttributes.DoNotShowGUI)
			{
				num++;
			}
			realCount++;
		}
		return num;
	}
}
