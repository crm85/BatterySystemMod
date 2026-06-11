using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EFT;
using EFT.UI;
using SAIN.Components.RotationController;
using SAIN.Editor;
using SAIN.Editor.GUISections;
using SAIN.Editor.Util;
using SAIN.Helpers;
using SAIN.Models.Enums;
using SAIN.Models.Structs;
using SAIN.Plugin;
using SAIN.Preset;
using SAIN.Preset.GearStealthValues;
using SAIN.Preset.GlobalSettings;
using SAIN.Preset.GlobalSettings.Categories;
using SAIN.Preset.Personalities;
using SAIN.SAINComponent.Classes.WeaponFunction;
using UnityEngine;

namespace SAIN.Attributes;

public class AttributesGUI
{
	public struct ConfigParams
	{
		public object SettingsObject;

		public string Search;

		public GUIEntryConfig EntryConfig;

		public int ListDepth;

		public string Name;
	}

	private static Dictionary<string, bool> openedSelections = new Dictionary<string, bool>();

	private static Dictionary<WildSpawnType, EPersonality> _tempBossPersDict = new Dictionary<WildSpawnType, EPersonality>();

	private static string[] personalities_strings;

	private static readonly Dictionary<string, bool> _listOpen = new Dictionary<string, bool>();

	private static readonly List<string> _failedAdds = new List<string>();

	private static readonly GUIEntryConfig _defaultEntryConfig = new GUIEntryConfig();

	private static GUIStyle _labelStyle;

	private static readonly Dictionary<string, ConfigInfoClass> _attributeClasses = new Dictionary<string, ConfigInfoClass>();

	public static ConfigInfoClass GetAttributeInfo(MemberInfo member)
	{
		string text = member.Name + member.DeclaringType.Name;
		AddAttributesToDictionary(text, member);
		if (_attributeClasses.TryGetValue(text, out var value))
		{
			return value;
		}
		return null;
	}

	private static void AddAttributesToDictionary(string name, MemberInfo member)
	{
		if (!_attributeClasses.ContainsKey(name) && !_failedAdds.Contains(name))
		{
			ConfigInfoClass configInfoClass = new ConfigInfoClass(member);
			if (configInfoClass.ValueType != null)
			{
				_attributeClasses.Add(name, configInfoClass);
			}
			else
			{
				_failedAdds.Add(name);
			}
		}
	}

	public static object EditValue(ref object value, object settingsObject, ConfigInfoClass attributes, out bool wasEdited, int listDepth, GUIEntryConfig config = null, string search = null)
	{
		CheckEditValue(ref value, settingsObject, attributes, out wasEdited, listDepth, config, search);
		if (wasEdited && !(value is ISAINSettings) && !(value is ISettingsGroup))
		{
			ConfigEditingTracker.Add(attributes.Name, value);
		}
		return value;
	}

	private static object CheckEditValue(ref object value, object settingsObject, ConfigInfoClass info, out bool wasEdited, int listDepth, GUIEntryConfig config = null, string search = null)
	{
		wasEdited = false;
		if (value != null && info != null && !info.DoNotShowGUI)
		{
			config = config ?? _defaultEntryConfig;
			if (value is string value2)
			{
				DisplayString(value2, listDepth, config, info);
				return value;
			}
			if (value is float || value is bool || value is int)
			{
				value = EditFloatBoolInt(ref value, settingsObject, info, config, listDepth, out wasEdited);
				return value;
			}
			if (value is EHeardFromPeaceBehavior)
			{
				return value;
			}
			if (!ExpandableList(info, config.EntryHeight + 3f, listDepth++, config))
			{
				return value;
			}
			if (value is ISAINSettings obj)
			{
				listDepth++;
				EditAllValuesInObj(obj, out wasEdited, search, config, listDepth);
				return value;
			}
			if (value is ISettingsGroup obj2)
			{
				listDepth++;
				EditAllValuesInObj(obj2, out wasEdited, search, config, listDepth);
				return value;
			}
			value = FindListTypeAndEdit(ref value, settingsObject, info, listDepth, out wasEdited, config, search);
		}
		return value;
	}

	private static bool CheckEditDictionary(ref object value, ref bool wasEdited, int listDepth, GUIEntryConfig config, string search)
	{
		if (value is Dictionary<EBotLookMode, TurnSettings> dictionary)
		{
			listDepth++;
			EditGenericStructDictionary(dictionary, out wasEdited, config, listDepth, search);
			return true;
		}
		return false;
	}

	private static void EditGenericStructDictionary<T, K>(Dictionary<T, K> dictionary, out bool wasEdited, GUIEntryConfig entryConfig, int listDepth, string search) where T : Enum where K : struct
	{
		wasEdited = false;
		Dictionary<T, K> dictionary2 = new Dictionary<T, K>();
		foreach (KeyValuePair<T, K> item in dictionary)
		{
			object settingsObject = item.Value;
			if (ExpandableList(item.Key.ToString(), string.Empty, PresetHandler.EditorDefaults.ConfigEntryHeight + 3f, listDepth, entryConfig))
			{
				ConfigParams configParams = new ConfigParams
				{
					EntryConfig = entryConfig,
					ListDepth = listDepth,
					SettingsObject = settingsObject,
					Search = search
				};
				SAINLayout.BeginVertical();
				List<ConfigInfoClass> list = new List<ConfigInfoClass>();
				GetAllAttributeInfos(dictionary, list, search);
				DisplayOptionsByCategory(configParams, list, out var wasEdited2);
				if (wasEdited2)
				{
					wasEdited = true;
					dictionary2.Add(item.Key, item.Value);
				}
				list.Clear();
				SAINLayout.EndVertical();
			}
		}
		foreach (KeyValuePair<T, K> item2 in dictionary2)
		{
			dictionary[item2.Key] = item2.Value;
		}
	}

	private static void EditSuppressionDict(Dictionary<ESuppressionState, SuppressionConfig> suppDict, out bool wasEdited)
	{
		wasEdited = false;
		CreateLabelStyle();
		SAINLayout.BeginVertical(5f);
		foreach (KeyValuePair<ESuppressionState, SuppressionConfig> item in suppDict)
		{
			SAINLayout.BeginHorizontal(150f);
			string name = $"Suppression State: {item.Key}";
			if (ExpandableList(name, null, PresetHandler.EditorDefaults.ConfigEntryHeight, 1, _defaultEntryConfig))
			{
			}
			SAINLayout.EndHorizontal(150f);
		}
		SAINLayout.EndVertical(5f);
	}

	public static void DisplayString(string value, float listDepth, GUIEntryConfig entryConfig, ConfigInfoClass info)
	{
		if (value != null && info != null && !info.DoNotShowGUI)
		{
			if (entryConfig == null)
			{
				entryConfig = _defaultEntryConfig;
			}
			StartConfigEntry(listDepth, entryConfig, info);
			SAINLayout.Label(info.Name + ": ", SAINLayout.Width(80f), SAINLayout.Height(PresetHandler.EditorDefaults.ConfigEntryHeight));
			SAINLayout.Box(value, SAINLayout.Height(PresetHandler.EditorDefaults.ConfigEntryHeight));
			SAINLayout.EndHorizontal(100f);
		}
	}

	public static void DisplayString(string value, float listDepth, GUIEntryConfig entryConfig, float heightOverride = -1f)
	{
		if (value != null)
		{
			if (entryConfig == null)
			{
				entryConfig = _defaultEntryConfig;
			}
			StartConfigEntry(listDepth, entryConfig, null);
			if (heightOverride < 0f)
			{
				heightOverride = entryConfig.EntryHeight;
			}
			SAINLayout.Box(value, SAINLayout.Height(heightOverride));
			SAINLayout.EndHorizontal(100f);
		}
	}

	public static object FindListTypeAndEdit(ref object value, object settingsObject, ConfigInfoClass info, int listDepth, out bool wasEdited, GUIEntryConfig config = null, string search = null)
	{
		wasEdited = false;
		CreateLabelStyle();
		if (value is Dictionary<EBotLookMode, TurnSettings> dictionary)
		{
			listDepth++;
			EditGenericStructDictionary(dictionary, out wasEdited, config, listDepth, search);
			return value;
		}
		if (value is Dictionary<ELocation, DifficultySettings> dictionary2)
		{
			EditLocationDict(dictionary2, settingsObject, info, listDepth, config, out wasEdited, search);
			return value;
		}
		if (value is Dictionary<ESuppressionState, SuppressionConfig> suppDict)
		{
			EditSuppressionDict(suppDict, out wasEdited);
			return value;
		}
		if (value is Dictionary<string, EPersonality> persDictionary)
		{
			CreatePersonalityDict(persDictionary, out wasEdited);
			return value;
		}
		if (value is Dictionary<WildSpawnType, EPersonality> persDictionary2)
		{
			CreatePersonalityDict(persDictionary2, config, out wasEdited);
			return value;
		}
		if (value is Dictionary<ECaliber, float>)
		{
			EditFloatDictionary<ECaliber>(value, info, out wasEdited);
			return value;
		}
		if (value is Dictionary<SAINSoundType, float>)
		{
			EditFloatDictionary<SAINSoundType>(value, info, out wasEdited);
			return value;
		}
		if (value is Dictionary<EWeaponClass, float>)
		{
			EditFloatDictionary<EWeaponClass>(value, info, out wasEdited);
			return value;
		}
		if (value is Dictionary<ESoundDispersionType, DispersionValues> dictionary3)
		{
			EditDispersionDictionary(dictionary3, settingsObject, info, out wasEdited);
			return value;
		}
		if (value is Dictionary<AILimitSetting, float> dictionary4)
		{
			EditAILimitDictionary(dictionary4, settingsObject, info, out wasEdited);
			return value;
		}
		if (value is Dictionary<EPersonality, bool> dictValue)
		{
			EditBoolDictionary<EPersonality>(dictValue, info, out wasEdited);
			return value;
		}
		if (value is List<WildSpawnType> list)
		{
			ModifyLists.AddOrRemove(list, out wasEdited);
			return value;
		}
		if (value is List<BotType> list2)
		{
			ModifyLists.AddOrRemove(list2, out wasEdited);
			return value;
		}
		if (value is List<Brain> list3)
		{
			ModifyLists.AddOrRemove(list3, out wasEdited);
			return value;
		}
		return value;
	}

	private static void CreatePersonalityDict(Dictionary<string, EPersonality> persDictionary, out bool wasEdited)
	{
		CreateLabelStyle();
		SAINLayout.BeginVertical(5f);
		wasEdited = false;
		foreach (KeyValuePair<string, EPersonality> item in persDictionary)
		{
			SAINLayout.BeginHorizontal(150f);
			string text = SAINLayout.TextArea(item.Key, null, SAINLayout.Width(300f), SAINLayout.Height(PresetHandler.EditorDefaults.ConfigEntryHeight));
			string text2 = SAINLayout.TextArea(item.Value.ToString(), null, SAINLayout.Width(300f), SAINLayout.Height(PresetHandler.EditorDefaults.ConfigEntryHeight));
			SAINLayout.EndHorizontal(150f);
		}
		SAINLayout.EndVertical(5f);
	}

	private static void CreatePersonalityDict(Dictionary<WildSpawnType, EPersonality> persDictionary, GUIEntryConfig entryConfig, out bool wasEdited)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		CreateLabelStyle();
		_tempBossPersDict.Clear();
		GClass1835.AddRange<WildSpawnType, EPersonality>((IDictionary<WildSpawnType, EPersonality>)_tempBossPersDict, (IDictionary<WildSpawnType, EPersonality>)persDictionary);
		wasEdited = false;
		SAINLayout.BeginVertical(5f);
		foreach (KeyValuePair<WildSpawnType, EPersonality> item in _tempBossPersDict)
		{
			SAINLayout.BeginHorizontal(150f);
			string name = $"Boss Personality: {item.Key}";
			if (ExpandableList(name, null, 25f, 1, entryConfig))
			{
				EPersonality ePersonality = SelectPersonality(item.Value, entryConfig);
				if (ePersonality != item.Value)
				{
					persDictionary[item.Key] = ePersonality;
				}
			}
			SAINLayout.EndHorizontal(150f);
		}
		SAINLayout.EndVertical(5f);
		_tempBossPersDict.Clear();
	}

	private static EPersonality SelectPersonality(EPersonality selected, GUIEntryConfig entryConfig)
	{
		int num = 0;
		EPersonality[] array = EnumValues.GetEnum<EPersonality>();
		if (personalities_strings == null)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < array.Length; i++)
			{
				list.Add(array[i].ToString());
			}
			personalities_strings = list.ToArray();
		}
		for (int j = 0; j < personalities_strings.Length; j++)
		{
			if (personalities_strings[j] == selected.ToString())
			{
				num = j;
				break;
			}
		}
		SAINLayout.BeginVertical(5f);
		EPersonality ePersonality = BotPersonalityEditor.SelectPersonality(selected, 25f, 3);
		if (ePersonality != selected)
		{
			selected = ePersonality;
			Sounds.PlaySound((EUISoundType)9, 0.5f);
		}
		SAINLayout.EndVertical(10f);
		return selected;
	}

	private static void CreateLabelStyle()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		if (_labelStyle == null)
		{
			GUIStyle style = SAINLayout.GetStyle(Style.box);
			_labelStyle = new GUIStyle(SAINLayout.GetStyle(Style.label))
			{
				alignment = (TextAnchor)3,
				margin = style.margin,
				padding = style.padding
			};
		}
	}

	private static void StartConfigEntry(float listDepth, GUIEntryConfig entryConfig, ConfigInfoClass info)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		if (entryConfig == null)
		{
			entryConfig = _defaultEntryConfig;
		}
		float num = listDepth;
		num = ((entryConfig == null) ? (num * 25f) : (num * entryConfig.SubList_Indent_Horizontal));
		if (info != null && (info.AdvancedOption || info.DeveloperOption) && _labelStyle != null)
		{
			SAINLayout.BeginHorizontal(25f);
			TextAnchor alignment = _labelStyle.alignment;
			_labelStyle.alignment = (TextAnchor)4;
			SAINLayout.Space(num);
			SAINLayout.Box(info.AdvancedOption ? "Advanced" : "Developer", _labelStyle, SAINLayout.Width(70f), SAINLayout.Height(PresetHandler.EditorDefaults.ConfigEntryHeight));
			_labelStyle.alignment = alignment;
		}
		else
		{
			SAINLayout.BeginHorizontal(100f + num);
		}
	}

	public static object EditFloatBoolInt(ref object value, object settingsObject, ConfigInfoClass info, GUIEntryConfig entryConfig, int listDepth, out bool wasEdited, bool showLabel = true, bool beginHoriz = true)
	{
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Expected O, but got Unknown
		if (value == null)
		{
			wasEdited = false;
			return null;
		}
		if (beginHoriz)
		{
			StartConfigEntry(listDepth, entryConfig, info);
		}
		bool flag = info.SimpleValueEdit || !PresetHandler.EditorDefaults.SliderToggle;
		if (showLabel)
		{
			CreateLabelStyle();
			SAINLayout.Box(options: (GUILayoutOption[])(object)((!flag) ? new GUILayoutOption[1] { GUILayout.Height(PresetHandler.EditorDefaults.ConfigEntryHeight) } : new GUILayoutOption[2]
			{
				GUILayout.Width(450f),
				GUILayout.Height(PresetHandler.EditorDefaults.ConfigEntryHeight)
			}), content: new GUIContent(info.Name, info.Description), style: _labelStyle);
		}
		object obj = value;
		string text = string.Empty;
		if (info.ValueType == typeof(bool))
		{
			if (!flag)
			{
				value = SAINLayout.Toggle((bool)value, ((bool)value) ? "On" : "Off", (EUISoundType)9, entryConfig.Toggle);
			}
			text = value.ToString();
		}
		else if (info.ValueType == typeof(float))
		{
			float num = (float)value;
			if (!flag)
			{
				num = BuilderClass.CreateSlider(num, info.Min, info.Max, info.Rounding, entryConfig.Toggle);
			}
			value = num;
			text = num.Round(info.Rounding).ToString();
		}
		GUILayoutOption[] options = (GUILayoutOption[])((!flag) ? ((Array)entryConfig.Result) : ((Array)new GUILayoutOption[2]
		{
			GUILayout.Width(100f),
			GUILayout.Height(PresetHandler.EditorDefaults.ConfigEntryHeight)
		}));
		if (flag && info.ValueType == typeof(bool))
		{
			value = SAINLayout.Toggle((bool)value, ((bool)value) ? "On" : "Off", (EUISoundType)9, options);
		}
		else
		{
			string text2 = SAINLayout.TextField(text, null, options);
			if (text2 != text)
			{
				value = BuilderClass.CleanString(text2, value);
			}
			if (value is int || value is float)
			{
				value = info.Clamp(value);
			}
		}
		options = (GUILayoutOption[])((!flag) ? ((Array)entryConfig.Reset) : ((Array)new GUILayoutOption[2]
		{
			GUILayout.Width(100f),
			GUILayout.Height(PresetHandler.EditorDefaults.ConfigEntryHeight)
		}));
		object obj2 = info.GetDefault(settingsObject);
		if (obj2 != null)
		{
			if (SAINLayout.Button("Reset", "Reset To Default Value", (EUISoundType)3, options))
			{
				value = obj2;
				ConfigEditingTracker.Remove(info);
			}
		}
		else
		{
			SAINLayout.Box(" ", "No Default Value is assigned to this option.", options);
		}
		if (beginHoriz)
		{
			SAINLayout.EndHorizontal(100f);
		}
		wasEdited = obj.ToString() != value.ToString();
		return value;
	}

	public static void EditAllStealthValues(GearStealthValuesClass stealthClass)
	{
		SAINLayout.BeginVertical(5f);
		EEquipmentType[] array = EnumValues.GetEnum<EEquipmentType>();
		int num = array.Length;
		Dictionary<EEquipmentType, List<ItemStealthValue>> itemStealthValues = stealthClass.ItemStealthValues;
		List<ItemStealthValue> defaults = stealthClass.Defaults;
		for (int i = 0; i < num; i++)
		{
			EEquipmentType key = array[i];
			if (itemStealthValues.TryGetValue(key, out var value) && ExpandableList(key.ToString(), string.Empty, _defaultEntryConfig.EntryHeight + 5f, 0, _defaultEntryConfig))
			{
				EditStealthValueList(value, defaults);
			}
		}
		SAINLayout.EndVertical(5f);
	}

	private static void EditStealthValueList(List<ItemStealthValue> list, List<ItemStealthValue> defaults)
	{
		if (list.Count != 0)
		{
			SAINLayout.BeginVertical(10f);
			for (int i = 0; i < list.Count; i++)
			{
				ItemStealthValue itemStealthValue = list[i];
				ItemStealthValue defaultValue = GetDefault(itemStealthValue, defaults);
				EditStealthValue(itemStealthValue, defaultValue);
			}
			SAINLayout.EndVertical(10f);
		}
	}

	private static ItemStealthValue GetDefault(ItemStealthValue value, List<ItemStealthValue> defaults)
	{
		if (!defaults.Contains(value))
		{
			return null;
		}
		foreach (ItemStealthValue @default in defaults)
		{
			if (@default.Name == value.Name)
			{
				return @default;
			}
		}
		return null;
	}

	private static void EditStealthValue(ItemStealthValue stealthValue, ItemStealthValue defaultValue)
	{
		SAINLayout.BeginHorizontal(150f);
		string name = stealthValue.Name;
		string description = "The Stealth Value for " + name;
		float stealthValue2 = stealthValue.StealthValue;
		float min = 0.1f;
		float max = 2f;
		stealthValue2 = Slider(name, description, stealthValue2, min, max, 1000f);
		if (defaultValue != null && ResetButton())
		{
			stealthValue2 = defaultValue.StealthValue;
		}
		if (stealthValue2 != stealthValue.StealthValue)
		{
			stealthValue.StealthValue = stealthValue2;
			ConfigEditingTracker.Add(name, stealthValue2);
		}
		SAINLayout.EndHorizontal(150f);
	}

	private static bool ExpandableList(ConfigInfoClass info, float height, int listDepth, GUIEntryConfig config)
	{
		return ExpandableList(info.Name, info.Description, height, listDepth, config);
	}

	private static bool ExpandableList(string name, string description, float height, int listDepth, GUIEntryConfig config)
	{
		SAINLayout.BeginHorizontal(100f + (float)listDepth * config.SubList_Indent_Horizontal);
		if (!_listOpen.ContainsKey(name))
		{
			_listOpen.Add(name, value: false);
		}
		bool value = _listOpen[name];
		value = BuilderClass.ExpandableMenu(name, value, description, height);
		_listOpen[name] = value;
		SAINLayout.EndHorizontal(100f);
		return value;
	}

	public static void EditBoolDictionary<T>(object dictValue, ConfigInfoClass info, out bool edited) where T : Enum
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		edited = false;
		SAINLayout.BeginVertical(5f);
		Dictionary<T, bool> dictionary = info.DefaultDictionary as Dictionary<T, bool>;
		Dictionary<T, bool> dictionary2 = dictValue as Dictionary<T, bool>;
		List<T> list = dictionary2.Keys.ToList();
		CreateLabelStyle();
		for (int i = 0; i < list.Count; i++)
		{
			SAINLayout.BeginHorizontal(150f);
			T key = list[i];
			string text = key.ToString();
			SAINLayout.Box(new GUIContent(text), _labelStyle, SAINLayout.Height(PresetHandler.EditorDefaults.ConfigEntryHeight));
			if (SAINLayout.Toggle(dictionary2[key], dictionary2[key] ? "On" : "Off", (EUISoundType)9, _defaultEntryConfig.Toggle))
			{
				for (int j = 0; j < list.Count; j++)
				{
					T key2 = list[j];
					bool flag = key2.ToString() == text;
					if (!flag && dictionary2[key2])
					{
						dictionary2[key2] = false;
						edited = true;
					}
					if (flag && !dictionary2[key2])
					{
						dictionary2[key2] = true;
						edited = true;
					}
				}
			}
			else if (dictionary2[key])
			{
				dictionary2[key] = false;
				edited = true;
			}
			SAINLayout.EndHorizontal(150f);
		}
		list.Clear();
		SAINLayout.EndVertical(5f);
	}

	public static void EditDispersionDictionary(Dictionary<ESoundDispersionType, DispersionValues> dictionary, object settingsObject, ConfigInfoClass info, out bool wasEdited)
	{
		SAINLayout.BeginVertical(5f);
		Dictionary<ESoundDispersionType, DispersionValues> defaultDictionary = info.GetDefault(settingsObject) as Dictionary<ESoundDispersionType, DispersionValues>;
		ESoundDispersionType[] array = EnumValues.GetEnum<ESoundDispersionType>();
		wasEdited = false;
		foreach (ESoundDispersionType eSoundDispersionType in array)
		{
			if (dictionary.TryGetValue(eSoundDispersionType, out var value))
			{
				EditDispStruct(value, eSoundDispersionType, defaultDictionary, out var wasEdited2);
				if (wasEdited2)
				{
					wasEdited = true;
				}
			}
		}
		SAINLayout.EndVertical(5f);
	}

	private static void EditLocationDict(Dictionary<ELocation, DifficultySettings> dictionary, object settingsObject, ConfigInfoClass info, int listDepth, GUIEntryConfig config, out bool wasEdited, string search = null)
	{
		SAINLayout.BeginVertical(5f);
		Dictionary<ELocation, DifficultySettings> dictionary2 = info.GetDefault(settingsObject) as Dictionary<ELocation, DifficultySettings>;
		ELocation[] array = EnumValues.GetEnum<ELocation>();
		wasEdited = false;
		for (int i = 0; i < array.Length; i++)
		{
			ELocation key = array[i];
			if (!dictionary.TryGetValue(key, out var value))
			{
				continue;
			}
			string text = key.ToString();
			if (ExpandableList(text, string.Empty, PresetHandler.EditorDefaults.ConfigEntryHeight + 3f, listDepth, config))
			{
				SAINLayout.BeginHorizontal(100f + (float)listDepth * config.SubList_Indent_Horizontal);
				SAINLayout.Label(text, SAINLayout.Height(PresetHandler.EditorDefaults.ConfigEntryHeight));
				SAINLayout.EndHorizontal(100f);
				int listDepth2 = listDepth + 1;
				EditAllValuesInObj(value, out var wasEdited2, search, config, listDepth2);
				if (wasEdited2)
				{
					wasEdited = true;
				}
			}
		}
		SAINLayout.EndVertical(5f);
	}

	public static void EditAILimitDictionary(Dictionary<AILimitSetting, float> dictionary, object settingsObject, ConfigInfoClass info, out bool wasEdited)
	{
		SAINLayout.BeginVertical(5f);
		Dictionary<AILimitSetting, float> dictionary2 = info.GetDefault(settingsObject) as Dictionary<AILimitSetting, float>;
		AILimitSetting[] array = EnumValues.GetEnum<AILimitSetting>();
		wasEdited = false;
		for (int i = 0; i < array.Length; i++)
		{
			AILimitSetting key = array[i];
			if (dictionary.TryGetValue(key, out var value))
			{
				SAINLayout.BeginHorizontal(200f);
				string name = key.ToString();
				string description = "";
				float min = 5f;
				float max = 800f;
				float num = Slider(name, description, value, min, max, 10f);
				if (ResetButton())
				{
					num = (dictionary[key] = dictionary2[key]);
				}
				if (dictionary[key] != num)
				{
					dictionary[key] = num;
					wasEdited = true;
				}
				SAINLayout.EndHorizontal(200f);
			}
		}
		SAINLayout.EndVertical(5f);
	}

	private static void EditDispStruct(DispersionValues values, ESoundDispersionType soundType, Dictionary<ESoundDispersionType, DispersionValues> defaultDictionary, out bool wasEdited)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		wasEdited = false;
		SAINLayout.BeginVertical(2f);
		SAINLayout.BeginHorizontal(150f);
		SAINLayout.Box(new GUIContent(soundType.ToString()), _labelStyle, SAINLayout.Height(PresetHandler.EditorDefaults.ConfigEntryHeight));
		SAINLayout.EndHorizontal(150f);
		SAINLayout.BeginHorizontal(200f);
		string name = "DistanceModifier";
		string description = "How much to randomize the distance that a bot thinks a sound originated from.";
		float distanceModifier = values.DistanceModifier;
		float min = 0f;
		float max = 20f;
		distanceModifier = Slider(name, description, distanceModifier, min, max, 100f);
		if (ResetButton())
		{
			distanceModifier = defaultDictionary[soundType].DistanceModifier;
		}
		if (distanceModifier != values.DistanceModifier)
		{
			values.DistanceModifier = distanceModifier;
			wasEdited = true;
		}
		SAINLayout.EndHorizontal(200f);
		SAINLayout.BeginHorizontal(200f);
		name = "MinAngle";
		description = "";
		distanceModifier = values.MinAngle;
		min = 0f;
		max = 180f;
		distanceModifier = Slider(name, description, distanceModifier, min, max, 100f);
		if (ResetButton())
		{
			distanceModifier = defaultDictionary[soundType].MinAngle;
		}
		if (distanceModifier != values.MinAngle)
		{
			values.MinAngle = distanceModifier;
			wasEdited = true;
		}
		SAINLayout.EndHorizontal(200f);
		SAINLayout.BeginHorizontal(200f);
		name = "MaxAngle";
		description = "";
		distanceModifier = values.MaxAngle;
		min = 0f;
		max = 180f;
		distanceModifier = Slider(name, description, distanceModifier, min, max, 100f);
		if (ResetButton())
		{
			distanceModifier = defaultDictionary[soundType].MaxAngle;
		}
		if (distanceModifier != values.MaxAngle)
		{
			values.MaxAngle = distanceModifier;
			wasEdited = true;
		}
		SAINLayout.EndHorizontal(200f);
		SAINLayout.BeginHorizontal(200f);
		name = "VerticalModifier";
		description = "";
		distanceModifier = values.VerticalModifier;
		min = 0f;
		max = 0.5f;
		distanceModifier = Slider(name, description, distanceModifier, min, max, 100f);
		if (ResetButton())
		{
			distanceModifier = defaultDictionary[soundType].VerticalModifier;
		}
		if (distanceModifier != values.VerticalModifier)
		{
			values.VerticalModifier = distanceModifier;
			wasEdited = true;
		}
		SAINLayout.EndHorizontal(200f);
		SAINLayout.EndVertical(2f);
	}

	private static bool ResetButton()
	{
		return SAINLayout.Button("Reset", (EUISoundType)3, _defaultEntryConfig.Reset);
	}

	private static float Slider(string name, string description, float value, float min, float max, float rounding)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		SAINLayout.Box(new GUIContent(name, description), _labelStyle, SAINLayout.Height(PresetHandler.EditorDefaults.ConfigEntryHeight));
		value = BuilderClass.CreateSlider(value, min, max, rounding, _defaultEntryConfig.Toggle).Round(100f);
		SAINLayout.Box(value.Round(rounding).ToString(), _defaultEntryConfig.Result);
		return value;
	}

	public static void EditFloatDictionary<T>(object dictValue, ConfigInfoClass info, out bool wasEdited) where T : Enum
	{
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Expected O, but got Unknown
		SAINLayout.BeginVertical(5f);
		float min = info.Min;
		float max = info.Max;
		float rounding = info.Rounding;
		Dictionary<T, float> dictionary = info.DefaultDictionary as Dictionary<T, float>;
		Dictionary<T, float> dictionary2 = dictValue as Dictionary<T, float>;
		T[] array = EnumValues.GetEnum<T>();
		if (array != null && array.Length != 0)
		{
			for (int i = 0; i < array.Length; i++)
			{
			}
		}
		List<T> list = new List<T>();
		foreach (KeyValuePair<T, float> item in dictionary2)
		{
			if (!(item.Key.ToString() == "Default"))
			{
				list.Add(item.Key);
			}
		}
		CreateLabelStyle();
		wasEdited = false;
		for (int j = 0; j < list.Count; j++)
		{
			SAINLayout.BeginHorizontal(150f);
			T key = list[j];
			float num = dictionary2[key];
			float value = num;
			SAINLayout.Box(new GUIContent(key.ToString()), _labelStyle, SAINLayout.Height(PresetHandler.EditorDefaults.ConfigEntryHeight));
			value = BuilderClass.CreateSlider(value, min, max, rounding, _defaultEntryConfig.Toggle);
			SAINLayout.Box(value.Round(rounding).ToString(), _defaultEntryConfig.Result);
			if (ResetButton())
			{
				value = dictionary[key];
			}
			if (value != num)
			{
				wasEdited = true;
				dictionary2[key] = value;
			}
			SAINLayout.EndHorizontal(150f);
		}
		list.Clear();
		SAINLayout.EndVertical(5f);
	}

	public static void EditAllValuesInObj(object obj, out bool wasEdited, string search = null, GUIEntryConfig entryConfig = null, int listDepth = 0)
	{
		ConfigParams configParams = new ConfigParams
		{
			SettingsObject = obj,
			Search = search,
			EntryConfig = entryConfig,
			ListDepth = listDepth
		};
		EditAllValuesInObj(configParams, out wasEdited);
	}

	public static void EditAllValuesInObj(ConfigParams configParams, out bool wasEdited)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		float indentValue = GetIndentValue(configParams.EntryConfig);
		SAINLayout.BeginVertical();
		if (!GClass1437.IsNullOrEmpty(configParams.Name))
		{
			SAINLayout.Box(new GUIContent(configParams.Name), _labelStyle, SAINLayout.Height(PresetHandler.EditorDefaults.ConfigEntryHeight));
		}
		List<ConfigInfoClass> list = new List<ConfigInfoClass>();
		GetAllAttributeInfos(configParams.SettingsObject, list, configParams.Search);
		DisplayOptionsByCategory(configParams, list, out wasEdited);
		list.Clear();
		SAINLayout.EndVertical();
	}

	private static float GetIndentValue(GUIEntryConfig entryConfig)
	{
		return entryConfig?.SubList_Indent_Vertical ?? 5f;
	}

	private static void DisplayCategory(ConfigParams configParams, List<ConfigInfoClass> attributeInfos, string category, out bool wasEdited)
	{
		wasEdited = false;
		bool flag = false;
		int num = 0;
		foreach (ConfigInfoClass attributeInfo in attributeInfos)
		{
			if (!attributeInfo.AdvancedOption && !attributeInfo.DeveloperOption && !attributeInfo.DoNotShowGUI && !(attributeInfo.Category != category))
			{
				if (!flag)
				{
					flag = true;
					DrawCategory(configParams, attributeInfo, category);
				}
				DisplayConfigGUI(attributeInfo, configParams, num++, out var edited);
				if (edited)
				{
					wasEdited = true;
				}
			}
		}
		foreach (ConfigInfoClass attributeInfo2 in attributeInfos)
		{
			if (attributeInfo2.AdvancedOption && !attributeInfo2.DoNotShowGUI && !(attributeInfo2.Category != category))
			{
				if (!flag)
				{
					flag = true;
					DrawCategory(configParams, attributeInfo2, category);
				}
				DisplayConfigGUI(attributeInfo2, configParams, num++, out var edited2);
				if (edited2)
				{
					wasEdited = true;
				}
			}
		}
		foreach (ConfigInfoClass attributeInfo3 in attributeInfos)
		{
			if (attributeInfo3.DeveloperOption && !attributeInfo3.DoNotShowGUI && !(attributeInfo3.Category != category))
			{
				if (!flag)
				{
					flag = true;
					DrawCategory(configParams, attributeInfo3, category);
				}
				DisplayConfigGUI(attributeInfo3, configParams, num++, out var edited3);
				if (edited3)
				{
					wasEdited = true;
				}
			}
		}
		if (num > 0)
		{
			SAINLayout.Space(10f);
		}
	}

	private static void DrawCategory(ConfigParams configParams, ConfigInfoClass configInfo, string category)
	{
		if (!(category == "None"))
		{
			SAINLayout.BeginHorizontal();
			DisplayString("    Category: " + category + "    ", configParams.ListDepth, configParams.EntryConfig, 15f);
			SAINLayout.FlexibleSpace();
			SAINLayout.EndHorizontal();
		}
	}

	private static void DisplayOptionsByCategory(ConfigParams configParams, List<ConfigInfoClass> configInfos, out bool wasEdited)
	{
		wasEdited = false;
		List<string> list = new List<string>();
		GetCategories(configInfos, list);
		for (int i = 0; i < list.Count; i++)
		{
			DisplayCategory(configParams, configInfos, list[i], out var wasEdited2);
			if (wasEdited2)
			{
				wasEdited = true;
			}
		}
		list.Clear();
	}

	private static void GetCategories(List<ConfigInfoClass> configInfos, List<string> outputList)
	{
		outputList.Clear();
		for (int i = 0; i < configInfos.Count; i++)
		{
			ConfigInfoClass configInfoClass = configInfos[i];
			string category = configInfoClass.Category;
			if (!GClass1437.IsNullOrEmpty(category) && !outputList.Contains(category))
			{
				outputList.Add(category);
			}
		}
	}

	private static void DisplayConfigGUI(ConfigInfoClass configInfo, ConfigParams configParams, int count, out bool edited)
	{
		object value = GetConfigValue(configInfo, configParams.SettingsObject);
		bool wasEdited;
		object value2 = EditValue(ref value, configParams.SettingsObject, configInfo, out wasEdited, configParams.ListDepth, configParams.EntryConfig, configParams.Search);
		if (wasEdited)
		{
			SetConfigValue(value2, configInfo.MemberInfo, configParams.SettingsObject);
			edited = true;
		}
		else
		{
			edited = false;
		}
	}

	private static object GetConfigValue(ConfigInfoClass configInfo, object obj)
	{
		MemberInfo memberInfo = configInfo.MemberInfo;
		return memberInfo.MemberType switch
		{
			MemberTypes.Field => (memberInfo as FieldInfo).GetValue(obj), 
			MemberTypes.Property => (memberInfo as PropertyInfo).GetValue(obj), 
			_ => null, 
		};
	}

	private static void SetConfigValue(object value, MemberInfo memberInfo, object obj)
	{
		switch (memberInfo.MemberType)
		{
		case MemberTypes.Field:
			(memberInfo as FieldInfo).SetValue(obj, value);
			break;
		case MemberTypes.Property:
			(memberInfo as PropertyInfo).SetValue(obj, value);
			break;
		}
	}

	private static void GetAllAttributeInfos(object obj, List<ConfigInfoClass> outputList, string search)
	{
		outputList.Clear();
		FieldInfo[] fields = obj.GetType().GetFields();
		FieldInfo[] array = fields;
		foreach (FieldInfo member in array)
		{
			ConfigInfoClass attributeInfo = GetAttributeInfo(member);
			if (!SkipForSearch(attributeInfo, search))
			{
				outputList.Add(attributeInfo);
			}
		}
	}

	public static void EditAllValuesInObj(Category category, object categoryObject, out bool wasEdited, string search = null)
	{
		EditAllValuesInObj(categoryObject, out wasEdited, search);
	}

	public static bool SkipForSearch(ConfigInfoClass attributes, string searchQuerry)
	{
		int result;
		if (!string.IsNullOrEmpty(searchQuerry))
		{
			string name = attributes.Name;
			if (name != null && !name.ToLower().Contains(searchQuerry))
			{
				string description = attributes.Description;
				if (description != null && !description.ToLower().Contains(searchQuerry))
				{
					string category = attributes.Category;
					result = ((category != null && !category.ToLower().Contains(searchQuerry)) ? 1 : 0);
					goto IL_0064;
				}
			}
			result = 0;
		}
		else
		{
			result = 0;
		}
		goto IL_0064;
		IL_0064:
		return (byte)result != 0;
	}
}
