using System;
using System.Reflection;

namespace SAIN.Preset.GlobalSettings;

public static class CloneSettingsClass
{
	public static void CopyFields(object original, object clone)
	{
		Type type = original.GetType();
		FieldInfo[] fields = type.GetFields();
		FieldInfo[] array = fields;
		foreach (FieldInfo fieldInfo in array)
		{
			Type fieldType = fieldInfo.FieldType;
			if (!(fieldType == type))
			{
				object value = fieldInfo.GetValue(original);
				fieldInfo.SetValue(clone, value);
			}
		}
	}
}
