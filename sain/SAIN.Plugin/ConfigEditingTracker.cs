using System;
using System.Collections.Generic;
using System.Text;
using SAIN.Attributes;

namespace SAIN.Plugin;

internal static class ConfigEditingTracker
{
	private static readonly Type _float = typeof(float);

	private static readonly Type _bool = typeof(bool);

	private static string _unsavedValues = string.Empty;

	private static readonly StringBuilder _stringBuilder = new StringBuilder();

	public static readonly Dictionary<string, object> EditedConfigValues = new Dictionary<string, object>();

	public static bool SettingChangedThisFrame { get; private set; }

	public static bool UnsavedChanges => EditedConfigValues.Count > 0;

	public static void Update()
	{
		SettingChangedThisFrame = false;
	}

	public static void Add(string name, object value)
	{
		AddConfigValue(name, value);
		ClearAndCreateStringBuilder();
		SettingChangedThisFrame = true;
	}

	private static void AddConfigValue(string name, object value)
	{
		if (EditedConfigValues.ContainsKey(name))
		{
			EditedConfigValues[name] = value;
		}
		else
		{
			EditedConfigValues.Add(name, value);
		}
	}

	public static void Remove(ConfigInfoClass info)
	{
		EditedConfigValues.Remove(info.Name);
		ClearAndCreateStringBuilder();
	}

	public static bool WasEdited(ConfigInfoClass info)
	{
		return EditedConfigValues.ContainsKey(info.Name);
	}

	public static void Clear()
	{
		EditedConfigValues.Clear();
		_stringBuilder.Clear();
		_unsavedValues = string.Empty;
	}

	public static string GetUnsavedValuesString()
	{
		return _unsavedValues;
	}

	private static void AddToStringBuilder(string name, object value)
	{
		Type type = value.GetType();
		string value2 = ((!(type == _float) && !(type == _bool)) ? (name ?? "") : $"{name}: {value}");
		_stringBuilder.AppendLine(value2);
		_unsavedValues = _stringBuilder.ToString();
	}

	private static void ClearAndCreateStringBuilder()
	{
		_stringBuilder.Clear();
		_stringBuilder.AppendLine($"Unsaved Config Options: Count: [{EditedConfigValues.Count}]");
		foreach (KeyValuePair<string, object> editedConfigValue in EditedConfigValues)
		{
			AddToStringBuilder(editedConfigValue.Key, editedConfigValue.Value);
		}
		_unsavedValues = _stringBuilder.ToString();
	}
}
