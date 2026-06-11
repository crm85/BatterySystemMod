using System.Collections.Generic;
using EFT;
using EFT.UI;
using SAIN.Attributes;
using SAIN.Helpers;
using SAIN.Plugin;
using SAIN.Preset;
using SAIN.Preset.GlobalSettings.Categories;
using UnityEngine;

namespace SAIN.Editor.Util;

public static class ModifyLists
{
	public static void EditDictionary(Dictionary<ELocation, float> dictionary, out bool wasEdited)
	{
		wasEdited = false;
		if (dictionary != null)
		{
			ELocation[] array = EnumValues.GetEnum<ELocation>();
			foreach (ELocation eLocation in array)
			{
			}
		}
	}

	public static void AddOrRemove(List<WildSpawnType> list, out bool wasEdited, int optionsPerLine = 4)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		wasEdited = false;
		if (list == null)
		{
			return;
		}
		GUILayoutOption[] dimensions;
		int i = StartListEdit(optionsPerLine, out dimensions);
		foreach (BotType value in BotTypeDefinitions.BotTypes.Values)
		{
			AddOrRemove(value.WildSpawnType, list, out var wasEdited2, value.Name, value.Description, dimensions);
			if (wasEdited2)
			{
				wasEdited = true;
			}
			i = ListSpacing(i, optionsPerLine);
		}
		EndListEdit();
	}

	public static void AddOrRemoveConfigOptions(SettingsContainer container, out bool wasEdited, string search = null)
	{
		GUILayoutOption[] options = (GUILayoutOption[])(object)new GUILayoutOption[2]
		{
			SAINLayout.Height(PresetHandler.EditorDefaults.ConfigEntryHeight),
			SAINLayout.Width(500f)
		};
		wasEdited = false;
		foreach (Category category in container.Categories)
		{
			string name = category.CategoryInfo.Name;
			string description = category.CategoryInfo.Description;
			if (string.IsNullOrEmpty(search))
			{
				category.Open = BuilderClass.ExpandableMenu(name, category.Open, description, PresetHandler.EditorDefaults.ConfigEntryHeight);
				if (!category.Open)
				{
					continue;
				}
			}
			else
			{
				SAINLayout.Label(name, description, SAINLayout.Height(PresetHandler.EditorDefaults.ConfigEntryHeight));
			}
			bool wasEdited2;
			foreach (ConfigInfoClass fieldAttributes in category.FieldAttributesList)
			{
				if (string.IsNullOrEmpty(search) || fieldAttributes.Name.ToLower().Contains(search))
				{
					AddOrRemove(fieldAttributes, category.SelectedList, out wasEdited2, fieldAttributes.Name, fieldAttributes.Description, options);
					if (wasEdited2)
					{
						wasEdited = true;
					}
				}
			}
			AddOrRemove(category, container.SelectedCategories, category.SelectedList.Count > 0, out wasEdited2);
			if (wasEdited2)
			{
				wasEdited = true;
			}
		}
	}

	private static void AddOrRemove<T>(T item, List<T> list, bool value, out bool wasEdited)
	{
		wasEdited = false;
		if (value)
		{
			if (!list.Contains(item))
			{
				list.Add(item);
			}
		}
		else if (list.Contains(item))
		{
			list.Remove(item);
		}
	}

	public static void AddOrRemove(List<BotDifficulty> list, out bool wasEdited, int optionsPerLine = 4, float width = 1200f, float height = 20f)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		wasEdited = false;
		float width2 = (width / (float)optionsPerLine).Round10();
		GUILayoutOption[] options = (GUILayoutOption[])(object)new GUILayoutOption[2]
		{
			SAINLayout.Height(height),
			SAINLayout.Width(width2)
		};
		BotDifficulty[] difficulties = EnumValues.Difficulties;
		foreach (BotDifficulty value in difficulties)
		{
			AddOrRemove(value, list, out var wasEdited2, null, null, options);
			if (wasEdited2)
			{
				wasEdited = true;
			}
		}
	}

	public static void AddOrRemove(List<BotType> list, out bool wasEdited, int optionsPerLine = 5)
	{
		wasEdited = false;
		GUILayoutOption[] dimensions;
		int i = StartListEdit(optionsPerLine, out dimensions);
		List<BotType> botTypesList = BotTypeDefinitions.BotTypesList;
		for (int j = 0; j < botTypesList.Count; j++)
		{
			BotType botType = botTypesList[j];
			AddOrRemove(botType, list, out var wasEdited2, botType.Name, botType.Description, dimensions);
			if (wasEdited2)
			{
				wasEdited = true;
			}
			i = ListSpacing(i, optionsPerLine);
		}
		EndListEdit();
	}

	public static void AddOrRemove(List<Brain> list, out bool wasEdited, int optionsPerLine = 5)
	{
		wasEdited = false;
		GUILayoutOption[] dimensions;
		int i = StartListEdit(optionsPerLine, out dimensions);
		List<Brain> allBrainsList = BotBrains.AllBrainsList;
		for (int j = 0; j < allBrainsList.Count; j++)
		{
			Brain value = allBrainsList[j];
			AddOrRemove(value, list, out var wasEdited2, null, null, dimensions);
			if (wasEdited2)
			{
				wasEdited = true;
			}
			i = ListSpacing(i, optionsPerLine);
		}
		EndListEdit();
	}

	public static void AddOrRemove(List<BotType> list, string section, float height, float width)
	{
		foreach (BotType value in BotTypeDefinitions.BotTypes.Values)
		{
			if (value.Section == section)
			{
				AddOrRemove(value, list, out var _, value.Name, value.Description, SAINLayout.Height(height), SAINLayout.Width(width));
			}
		}
	}

	private static void AddOrRemove<T>(T value, List<T> list, out bool wasEdited, string name = null, string description = null, params GUILayoutOption[] options)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		wasEdited = false;
		if (list != null)
		{
			bool value2 = SAINLayout.Toggle(list.Contains(value), new GUIContent(name ?? value.ToString(), description), SAINLayout.GetStyle(Style.selectionList), (EUISoundType)9, options);
			AddOrRemove(value, list, value2, out var wasEdited2);
			if (wasEdited2)
			{
				wasEdited = true;
			}
		}
	}

	private static int StartListEdit(int optionsPerLine, out GUILayoutOption[] dimensions, float gridWidth = 1875f)
	{
		SAINLayout.BeginVertical();
		SAINLayout.BeginHorizontal();
		SAINLayout.Space(5f);
		float width = (gridWidth / (float)optionsPerLine).Round10();
		dimensions = (GUILayoutOption[])(object)new GUILayoutOption[2]
		{
			SAINLayout.Height(PresetHandler.EditorDefaults.ConfigEntryHeight),
			SAINLayout.Width(width)
		};
		return 0;
	}

	private static int ListSpacing(int i, int max)
	{
		i++;
		if (i >= max)
		{
			i = 0;
			SAINLayout.Space(5f);
			SAINLayout.EndHorizontal();
			SAINLayout.BeginHorizontal();
		}
		return i;
	}

	private static void EndListEdit()
	{
		SAINLayout.EndHorizontal();
		SAINLayout.EndVertical();
	}
}
