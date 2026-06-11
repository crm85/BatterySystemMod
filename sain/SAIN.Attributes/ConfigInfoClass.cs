using System;
using System.Reflection;
using SAIN.Helpers;
using SAIN.Plugin;
using SAIN.Preset.GlobalSettings;

namespace SAIN.Attributes;

public sealed class ConfigInfoClass
{
	public readonly MemberInfo MemberInfo;

	private string DictionaryString;

	public bool MenuOpen;

	public Type ValueType => MemberInfo.MemberType switch
	{
		MemberTypes.Field => (MemberInfo as FieldInfo).FieldType, 
		MemberTypes.Property => (MemberInfo as PropertyInfo).PropertyType, 
		_ => null, 
	};

	public Type DeclaringType => MemberInfo.DeclaringType;

	public object DefaultDictionary => Reflection.GetStaticValue(DeclaringType, DictionaryString);

	public string Name { get; private set; }

	public string Description { get; private set; }

	public string Category { get; private set; }

	public float Min { get; private set; } = 0f;

	public float Max { get; private set; } = 300f;

	public float Rounding { get; private set; } = 10f;

	public bool Hidden { get; private set; }

	public bool AdvancedOption { get; private set; }

	public bool DeveloperOption { get; private set; }

	public bool Debug { get; private set; }

	public bool SimpleValueEdit { get; private set; }

	public float? DefaultFloatValue { get; private set; }

	public bool CopyValue { get; private set; }

	public bool DoNotShowGUI => Hidden || (AdvancedOption && !PresetHandler.EditorDefaults.AdvancedBotConfigs) || (DeveloperOption && !PresetHandler.EditorDefaults.DevBotConfigs);

	public EListType EListType { get; private set; } = EListType.None;

	public Type ListType { get; private set; }

	public Type SecondaryListType { get; private set; }

	public ConfigInfoClass(MemberInfo member)
	{
		MemberInfo = member;
		GetInfo(member);
	}

	public ConfigInfoClass(string name)
	{
		Name = name;
	}

	public object GetValue(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		return MemberInfo.MemberType switch
		{
			MemberTypes.Field => (MemberInfo as FieldInfo).GetValue(obj), 
			MemberTypes.Property => (MemberInfo as PropertyInfo).GetValue(obj), 
			_ => null, 
		};
	}

	public void SetValue(object obj, object value)
	{
		switch (MemberInfo.MemberType)
		{
		case MemberTypes.Field:
			(MemberInfo as FieldInfo).SetValue(obj, value);
			break;
		case MemberTypes.Property:
			(MemberInfo as PropertyInfo).SetValue(obj, value);
			break;
		}
	}

	public object Clamp(object value)
	{
		return MathHelpers.ClampObject(value, Min, Max);
	}

	private void GetInfo(MemberInfo member)
	{
		Hidden = Get<HiddenAttribute>() != null;
		AdvancedOption = Get<AdvancedAttribute>() != null;
		DeveloperOption = Get<DeveloperOptionAttribute>() != null;
		Debug = Get<DebugAttribute>() != null;
		CopyValue = Get<CopyValueAttribute>() != null;
		SimpleValueEdit = Get<SimpleValueAttribute>() != null;
		if (!Hidden)
		{
			NameAndDescriptionAttribute nameAndDescriptionAttribute = Get<NameAndDescriptionAttribute>();
			Name = nameAndDescriptionAttribute?.Name ?? Get<NameAttribute>()?.Value ?? member.Name;
			Description = nameAndDescriptionAttribute?.Description ?? Get<DescriptionAttribute>()?.Value ?? string.Empty;
			Category = Get<CategoryAttribute>()?.Value ?? "None";
			GUIValuesAttribute gUIValuesAttribute = Get<GUIValuesAttribute>();
			if (gUIValuesAttribute != null)
			{
				Min = gUIValuesAttribute.Min;
				Max = gUIValuesAttribute.Max;
				Rounding = gUIValuesAttribute.Rounding;
			}
			DictionaryString = Get<DefaultDictionaryAttribute>()?.Value;
			DefaultFloatAttribute defaultFloatAttribute = Get<DefaultFloatAttribute>();
			if (defaultFloatAttribute != null)
			{
				DefaultFloatValue = defaultFloatAttribute.Value;
			}
		}
	}

	private T Get<T>() where T : Attribute
	{
		return MemberInfo.GetCustomAttribute<T>();
	}

	public object GetDefault(object settingsObject)
	{
		if (DefaultFloatValue.HasValue)
		{
			return DefaultFloatValue.Value;
		}
		if (settingsObject is ISAINSettings iSAINSettings)
		{
			object defaults = iSAINSettings.GetDefaults();
			return GetValue(defaults);
		}
		return null;
	}
}
