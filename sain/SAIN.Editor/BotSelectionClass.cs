using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EFT.UI;
using SAIN.Attributes;
using SAIN.Editor.Util;
using SAIN.Plugin;
using SAIN.Preset;
using SAIN.Preset.BotSettings.SAINSettings;
using UnityEngine;

namespace SAIN.Editor;

public static class BotSelectionClass
{
	private static GUIStyle botTypeSectionStyle;

	private static bool[] SectionOpens;

	public static readonly string[] Sections;

	private static readonly List<BotType> SelectedBotTypes;

	public static readonly BotDifficulty[] BotDifficultyOptions;

	public static readonly List<BotDifficulty> SelectedDifficulties;

	public static bool BotSettingsWereEdited;

	private static GUIEntryConfig entryConfig;

	static BotSelectionClass()
	{
		SelectedBotTypes = new List<BotType>();
		BotDifficulty[] array = new BotDifficulty[4];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		BotDifficultyOptions = (BotDifficulty[])(object)array;
		SelectedDifficulties = new List<BotDifficulty>();
		List<string> list = new List<string>();
		foreach (BotType value in BotTypeDefinitions.BotTypes.Values)
		{
			if (!list.Contains(value.Section))
			{
				list.Add(value.Section);
			}
		}
		Sections = list.ToArray();
		SectionOpens = new bool[Sections.Length];
	}

	public static void Menu()
	{
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Expected O, but got Unknown
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Expected O, but got Unknown
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Expected O, but got Unknown
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Expected O, but got Unknown
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Expected O, but got Unknown
		SAINLayout.BeginHorizontal();
		SAINLayout.FlexibleSpace();
		string text = "Apply Values set below to selected Bot Type. Exports edited values to SAIN/Presets/" + SAINPlugin.LoadedPreset.Info.Name + "/BotSettings folder";
		if (BuilderClass.SaveChanges(ConfigEditingTracker.GetUnsavedValuesString()))
		{
			SAINPresetClass.ExportAll(SAINPlugin.LoadedPreset);
		}
		SAINLayout.FlexibleSpace();
		SAINLayout.EndHorizontal();
		SAINLayout.BeginHorizontal();
		SAINLayout.FlexibleSpace();
		SAINLayout.Space(3f);
		float width = 1850f / (float)Sections.Length;
		for (int i = 0; i < Sections.Length; i++)
		{
			SAINLayout.BeginVertical();
			if (botTypeSectionStyle == null)
			{
				botTypeSectionStyle = new GUIStyle(SAINLayout.GetStyle(Style.toggle))
				{
					alignment = (TextAnchor)3,
					padding = new RectOffset(5, 5, 0, 0),
					margin = new RectOffset(5, 5, 0, 0),
					border = new RectOffset(5, 5, 0, 0),
					fontStyle = (FontStyle)1
				};
			}
			string text2 = Sections[i];
			SectionOpens[i] = SAINLayout.Toggle(SectionOpens[i], new GUIContent(text2), botTypeSectionStyle, (EUISoundType)11, SAINLayout.Height(35f), SAINLayout.Width(width));
			if (SectionOpens[i])
			{
				ModifyLists.AddOrRemove(SelectedBotTypes, text2, 27.5f, width);
			}
			SAINLayout.EndVertical();
		}
		SAINLayout.FlexibleSpace();
		SAINLayout.EndHorizontal();
		SAINLayout.Space(3f);
		if (SAINLayout.Button("Clear Bot Types", "Clear all selected bot types", (EUISoundType)2))
		{
			SelectedBotTypes.Clear();
		}
		SAINLayout.Space(3f);
		SAINLayout.BeginHorizontal();
		SAINLayout.Label("Difficulties", "Select which difficulties you wish to modify.", SAINLayout.Height(25f));
		SAINLayout.Space(3f);
		ModifyLists.AddOrRemove(SelectedDifficulties, out var _, 4, 1200f, 35f);
		SAINLayout.Space(3f);
		if (SAINLayout.Button("Clear Difficulties", "Clear all selected difficulties", null, SAINLayout.Height(25f), SAINLayout.Width(150f)))
		{
			SelectedDifficulties.Clear();
		}
		SAINLayout.EndHorizontal();
		SAINLayout.Space(5f);
		SelectProperties();
	}

	private static void SelectProperties()
	{
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		if (SelectedBotTypes.Count == 0 || SelectedDifficulties.Count == 0)
		{
			if (SelectedBotTypes.Count == 0)
			{
				SAINLayout.Box("No Bot Types Selected, please select at least one above.");
			}
			else
			{
				SAINLayout.Box("No Bot Difficulties Selected, please select at least one above.");
			}
			return;
		}
		SettingsContainer container = SettingsContainers.GetContainer(typeof(SAINSettingsClass), "Select Options to Edit");
		string value = BuilderClass.SearchBox(container);
		try
		{
			foreach (Category category in container.Categories)
			{
				GUIStyle style = SAINLayout.GetStyle(Style.toggle);
				TextAnchor alignment = style.alignment;
				style.alignment = (TextAnchor)3;
				category.CategoryInfo.MenuOpen = SAINLayout.Toggle(category.CategoryInfo.MenuOpen, category.CategoryInfo.Name, null, SAINLayout.Height(PresetHandler.EditorDefaults.ConfigEntryHeight));
				style.alignment = alignment;
				if (!category.CategoryInfo.MenuOpen)
				{
					continue;
				}
				for (int i = 0; i < category.FieldAttributesList.Count; i++)
				{
					ConfigInfoClass configInfoClass = category.FieldAttributesList[i];
					if (!string.IsNullOrEmpty(value) && !configInfoClass.Name.ToLower().Contains(value))
					{
						continue;
					}
					SAINLayout.BeginHorizontal();
					SAINLayout.Space(30f);
					style.alignment = (TextAnchor)3;
					configInfoClass.MenuOpen = SAINLayout.Toggle(configInfoClass.MenuOpen, configInfoClass.Name, configInfoClass.Description, null, SAINLayout.Height(PresetHandler.EditorDefaults.ConfigEntryHeight), SAINLayout.Width(500f));
					style.alignment = alignment;
					SAINLayout.EndHorizontal();
					if (!configInfoClass.MenuOpen)
					{
						continue;
					}
					for (int j = 0; j < SelectedBotTypes.Count; j++)
					{
						BotType botType = SelectedBotTypes[j];
						if (!SAINPlugin.LoadedPreset.BotSettings.SAINSettings.TryGetValue(botType.WildSpawnType, out var value2))
						{
							continue;
						}
						if (entryConfig == null)
						{
							entryConfig = new GUIEntryConfig();
						}
						for (int k = 0; k < SelectedDifficulties.Count; k++)
						{
							BotDifficulty val = SelectedDifficulties[k];
							if (value2.Settings.TryGetValue(val, out var value3))
							{
								SAINLayout.BeginHorizontal();
								SAINLayout.Space(60f);
								object value4 = category.GetValue(value3);
								object value5 = configInfoClass.GetValue(value4);
								SAINLayout.Label($"{botType.Name} : {val}", SAINLayout.Height(PresetHandler.EditorDefaults.ConfigEntryHeight), SAINLayout.Width(200f));
								value5 = AttributesGUI.EditFloatBoolInt(ref value5, value4, configInfoClass, entryConfig, 0, out var wasEdited, showLabel: false, beginHoriz: false);
								if (wasEdited)
								{
									ConfigEditingTracker.Add(configInfoClass.Name, value5);
								}
								configInfoClass.SetValue(value4, value5);
								SAINLayout.EndHorizontal();
							}
						}
					}
				}
			}
		}
		catch (Exception data)
		{
			Logger.LogError(data);
		}
	}
}
