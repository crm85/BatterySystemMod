using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EFT.UI;
using SAIN.Helpers;
using SAIN.Plugin;
using SAIN.Preset;
using UnityEngine;

namespace SAIN.Editor.GUISections;

public static class PresetSelection
{
	private static readonly List<SAINPresetDefinition> defaultPresets = SAINDifficultyClass.DefaultPresetDefinitions.Values.ToList();

	private const float PRESET_LABEL_HEIGHT = 55f;

	private const float PRESET_OPTION_HEIGHT = 25f;

	private const float PRESET_OPTION_WIDTH = 500f;

	private const float PRESET_BASE_OPTION_WIDTH = 150f;

	private const float PRESET_ALERT_HEIGHT = 30f;

	private static bool _deletePresetConfirmation1 = false;

	private static bool _deletePresetConfirmation2 = false;

	private static bool _makeNewPresetMenuToggle;

	private static string NewName = "Enter Name Here";

	private static string NewDescription = "Enter Description Here";

	private static string NewCreator = "Your Name Here";

	public static void PresetSelectionMenu()
	{
		SAINPresetDefinition info = SAINPlugin.LoadedPreset.Info;
		checkCreateWarning(info);
		SAINLayout.BeginHorizontal();
		baseSelectionOptions();
		info = selectDefault(info);
		info = selectCustom(info);
		checkCreateNew();
		if (checkDeletePreset())
		{
			info = SAINPresetClass.Instance.Info;
		}
		SAINLayout.FlexibleSpace();
		SAINLayout.EndHorizontal();
		if (info.Name != SAINPlugin.LoadedPreset.Info.Name)
		{
			PresetHandler.InitPresetFromDefinition(info);
		}
	}

	private static void checkCreateWarning(SAINPresetDefinition selectedPreset)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected O, but got Unknown
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		string text = selectedPreset.SAINPresetVersion;
		if (string.IsNullOrEmpty(text))
		{
			text = selectedPreset.SAINVersion;
		}
		GUIContent val = new GUIContent("Warning: The selected preset version is: [" + text + "], but current SAIN preset version is: [4.0.0] (SAIN version [4.0.3]), default bot config values may be set incorrectly due to updates to SAIN. THIS DOESN'T MEAN YOUR GAME IS BROKEN, just be aware bots might not act as intended.");
		Rect rect = GUILayoutUtility.GetRect(val, SAINLayout.GetStyle(Style.alert), (GUILayoutOption[])(object)new GUILayoutOption[1] { SAINLayout.Height(30f) });
		if (selectedPreset.IsCustom && text != "4.0.0")
		{
			GUI.Box(rect, val, SAINLayout.GetStyle(Style.alert));
		}
		else
		{
			GUI.Box(rect, new GUIContent(""), SAINLayout.GetStyle(Style.blankbox));
		}
	}

	private static void baseSelectionOptions()
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		SAINLayout.BeginVertical();
		SAINLayout.Box("Presets", "Select an Installed preset for SAIN Settings", SAINLayout.Height(55f), SAINLayout.Width(150f));
		if (SAINLayout.Button("Refresh", "Refresh installed Presets", (EUISoundType)3, SAINLayout.Height(55f), SAINLayout.Width(150f)))
		{
			PresetHandler.LoadCustomPresetOptions();
		}
		_makeNewPresetMenuToggle = SAINLayout.Toggle(_makeNewPresetMenuToggle, new GUIContent("Create New Preset"), (EUISoundType)3, SAINLayout.Height(55f), SAINLayout.Width(150f));
		SAINLayout.EndVertical();
	}

	private static SAINPresetDefinition selectDefault(SAINPresetDefinition selectedPreset)
	{
		SAINLayout.BeginVertical();
		SAINLayout.Label("Default Presets", SAINLayout.Width(500f));
		SAINDifficulty sAINDifficulty = default(SAINDifficulty);
		for (int i = 0; i < defaultPresets.Count; i++)
		{
			SAINPresetDefinition sAINPresetDefinition = defaultPresets[i];
			if (GClass835.TryGetKey<SAINDifficulty, SAINPresetDefinition>((IDictionary<SAINDifficulty, SAINPresetDefinition>)SAINDifficultyClass.DefaultPresetDefinitions, sAINPresetDefinition, ref sAINDifficulty))
			{
				bool flag = SAINPlugin.EditorDefaults.SelectedDefaultPreset == sAINDifficulty;
				if (SAINLayout.Toggle(flag, sAINPresetDefinition.Name ?? "", sAINPresetDefinition.Description, (EUISoundType)9, SAINLayout.Height(25f), SAINLayout.Width(500f)) && !flag)
				{
					SAINPlugin.EditorDefaults.SelectedDefaultPreset = sAINDifficulty;
					selectedPreset = sAINPresetDefinition;
				}
			}
		}
		SAINLayout.EndVertical();
		return selectedPreset;
	}

	private static SAINPresetDefinition selectCustom(SAINPresetDefinition selectedPreset)
	{
		SAINLayout.BeginVertical();
		SAINLayout.Label("Custom Presets", SAINLayout.Width(500f));
		for (int i = 0; i < PresetHandler.CustomPresetOptions.Count; i++)
		{
			SAINPresetDefinition sAINPresetDefinition = PresetHandler.CustomPresetOptions[i];
			if (sAINPresetDefinition.IsCustom)
			{
				bool flag = SAINPlugin.EditorDefaults.SelectedDefaultPreset == SAINDifficulty.none && selectedPreset.Name == sAINPresetDefinition.Name;
				if (SAINLayout.Toggle(flag, sAINPresetDefinition.Name ?? "", sAINPresetDefinition.Description, (EUISoundType)9, SAINLayout.Height(25f), SAINLayout.Width(500f)) && !flag)
				{
					selectedPreset = sAINPresetDefinition;
				}
			}
		}
		SAINLayout.EndVertical();
		return selectedPreset;
	}

	private static void checkCreateNew()
	{
		if (_makeNewPresetMenuToggle)
		{
			SAINLayout.BeginVertical();
			SAINLayout.BeginHorizontal();
			SAINLayout.Space(25f);
			SAINPresetDefinition info = SAINPlugin.LoadedPreset.Info;
			if (info.CanEditName && SAINLayout.Button("Save Info", "Update the selected presets name, description, and creator.", (EUISoundType)7, SAINLayout.Height(30f)))
			{
				string name = info.Name;
				SAINPresetDefinition sAINPresetDefinition = info.Clone();
				sAINPresetDefinition.Name = NewName;
				sAINPresetDefinition.Description = NewDescription;
				sAINPresetDefinition.Creator = NewCreator;
				JsonUtility.DeletePreset(info);
				PresetHandler.SavePresetDefinition(sAINPresetDefinition);
				PresetHandler.InitPresetFromDefinition(sAINPresetDefinition, isCopy: true);
				PresetHandler.LoadCustomPresetOptions();
			}
			if (SAINLayout.Button("Save A New Preset", (EUISoundType)7, (GUILayoutOption[])(object)new GUILayoutOption[1] { SAINLayout.Height(30f) }))
			{
				SAINPresetDefinition sAINPresetDefinition2 = SAINPlugin.LoadedPreset.Info.Clone();
				sAINPresetDefinition2.Name = NewName;
				sAINPresetDefinition2.Description = NewDescription;
				sAINPresetDefinition2.Creator = NewCreator;
				sAINPresetDefinition2.SAINVersion = "4.0.0";
				sAINPresetDefinition2.DateCreated = DateTime.Today.ToString();
				PresetHandler.SavePresetDefinition(sAINPresetDefinition2);
				PresetHandler.InitPresetFromDefinition(sAINPresetDefinition2, isCopy: true);
			}
			SAINLayout.Space(25f);
			SAINLayout.EndHorizontal();
			SAINLayout.Space(3f);
			NewName = LabeledTextField(NewName, "Name");
			NewDescription = LabeledTextField(NewDescription, "Description");
			NewCreator = LabeledTextField(NewCreator, "Creator");
			SAINLayout.EndVertical();
		}
	}

	private static bool checkDeletePreset()
	{
		if (SAINPresetClass.Instance.Info.IsCustom)
		{
			SAINLayout.BeginVertical();
			_deletePresetConfirmation1 = SAINLayout.Toggle(_deletePresetConfirmation1, "Delete Selected Preset", null, SAINLayout.Height(30f), SAINLayout.Width(250f));
			if (_deletePresetConfirmation1)
			{
				_deletePresetConfirmation2 = SAINLayout.Toggle(_deletePresetConfirmation2, "Are you Sure?", null, SAINLayout.Height(30f), SAINLayout.Width(250f));
				if (_deletePresetConfirmation2 && SAINLayout.Button("CONFIRM DELETE OF " + SAINPresetClass.Instance.Info.Name + " ?", SAINLayout.Height(60f), SAINLayout.Width(250f)))
				{
					SAINPresetDefinition info = SAINPresetClass.Instance.Info;
					PresetHandler.loadDefault();
					JsonUtility.DeletePreset(info);
					Sounds.PlaySound((EUISoundType)40);
					PresetHandler.LoadCustomPresetOptions();
					_deletePresetConfirmation2 = false;
					_deletePresetConfirmation1 = false;
					return true;
				}
			}
			SAINLayout.EndVertical();
		}
		return false;
	}

	private static string LabeledTextField(string value, string label)
	{
		SAINLayout.BeginHorizontal();
		SAINLayout.Box(label, SAINLayout.Width(125f), SAINLayout.Height(PresetHandler.EditorDefaults.ConfigEntryHeight));
		value = SAINLayout.TextField(value, null, SAINLayout.Width(350f), SAINLayout.Height(PresetHandler.EditorDefaults.ConfigEntryHeight));
		SAINLayout.EndHorizontal();
		return Regex.Replace(value, "[^\\w \\-]", "");
	}
}
