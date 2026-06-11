using System;
using System.Collections.Generic;
using EFT.UI;
using SAIN.Editor;
using SAIN.Helpers;
using SAIN.Preset;

namespace SAIN.Plugin;

internal class PresetHandler
{
	public const string DefaultPreset = "3. Default";

	public const string DefaultPresetDescription = "Bots are difficult but fair, the way SAIN was meant to played.";

	private const string Settings = "ConfigSettings";

	public static readonly List<SAINPresetDefinition> CustomPresetOptions = new List<SAINPresetDefinition>();

	public static SAINPresetClass LoadedPreset;

	public static PresetEditorDefaults EditorDefaults;

	public static event Action<SAINPresetClass> OnPresetUpdated;

	public static event Action<PresetEditorDefaults> OnEditorSettingsChanged;

	public static void LoadCustomPresetOptions()
	{
		JsonUtility.Load.LoadCustomPresetOptions(CustomPresetOptions);
	}

	public static void Init()
	{
		ImportEditorDefaults();
		LoadCustomPresetOptions();
		SAINPresetDefinition definition = null;
		if (!GClass1437.IsNullOrEmpty(EditorDefaults.SelectedCustomPreset))
		{
			CheckIfPresetLoaded(EditorDefaults.SelectedCustomPreset, out definition);
		}
		InitPresetFromDefinition(definition);
	}

	public static bool LoadPresetDefinition(string presetKey, out SAINPresetDefinition definition)
	{
		for (int i = 0; i < CustomPresetOptions.Count; i++)
		{
			SAINPresetDefinition sAINPresetDefinition = CustomPresetOptions[i];
			if (sAINPresetDefinition.IsCustom && sAINPresetDefinition.Name == presetKey)
			{
				definition = sAINPresetDefinition;
				return true;
			}
		}
		if (JsonUtility.Load.LoadObject<SAINPresetDefinition>(out definition, "Info", "Presets", presetKey) && definition.IsCustom)
		{
			CustomPresetOptions.Add(definition);
			return true;
		}
		return false;
	}

	public static void SavePresetDefinition(SAINPresetDefinition definition)
	{
		if (!definition.IsCustom)
		{
			return;
		}
		string name = definition.Name;
		for (int i = 0; i < 100; i++)
		{
			if (!JsonUtility.DoesFileExist("Info", "Presets", definition.Name))
			{
				break;
			}
			definition.Name = name + $" Copy({i})";
		}
		CustomPresetOptions.Add(definition);
		JsonUtility.SaveObjectToJson(definition, "Info", "Presets", definition.Name);
	}

	public static void loadDefault()
	{
		LoadedPreset = SAINDifficultyClass.GetDefaultPreset(EditorDefaults.SelectedDefaultPreset) ?? SAINDifficultyClass.GetDefaultPreset(SAINDifficulty.hard);
		LoadedPreset.Init();
		LoadedPreset.UpdateDefaults();
	}

	public static void InitPresetFromDefinition(SAINPresetDefinition def, bool isCopy = false)
	{
		if (def == null || !def.IsCustom)
		{
			loadDefault();
			UpdateExistingBots();
			ExportEditorDefaults();
			return;
		}
		try
		{
			SAINPresetClass defaultPreset = SAINDifficultyClass.GetDefaultPreset(def.BaseSAINDifficulty);
			LoadedPreset = new SAINPresetClass(def, isCopy);
			LoadedPreset.Init();
			if (defaultPreset != null)
			{
				LoadedPreset.UpdateDefaults(defaultPreset);
			}
		}
		catch (Exception data)
		{
			Sounds.PlaySound((EUISoundType)6);
			Logger.LogError(data);
			loadDefault();
		}
		UpdateExistingBots();
		ExportEditorDefaults();
	}

	public static void ExportEditorDefaults()
	{
		if (EditorDefaults.SelectedDefaultPreset == SAINDifficulty.none && LoadedPreset.Info.IsCustom)
		{
			EditorDefaults.SelectedCustomPreset = LoadedPreset.Info.Name;
		}
		else
		{
			EditorDefaults.SelectedCustomPreset = string.Empty;
		}
		JsonUtility.SaveObjectToJson(EditorDefaults, "ConfigSettings", "Presets");
		PresetHandler.OnEditorSettingsChanged?.Invoke(EditorDefaults);
	}

	public static void ImportEditorDefaults()
	{
		if (JsonUtility.Load.LoadObject<PresetEditorDefaults>(out var obj, "ConfigSettings", "Presets"))
		{
			EditorDefaults = obj;
		}
		else
		{
			EditorDefaults = new PresetEditorDefaults("3. Default");
		}
	}

	public static void UpdateExistingBots()
	{
		PresetHandler.OnPresetUpdated?.Invoke(LoadedPreset);
		LoadedPreset?.GlobalSettings.Update();
		LoadedPreset?.PersonalityManager.Update();
		LoadedPreset?.BotSettings.Update();
	}

	private static bool CheckIfPresetLoaded(string presetName, out SAINPresetDefinition definition)
	{
		definition = null;
		if (string.IsNullOrEmpty(presetName))
		{
			return false;
		}
		for (int i = 0; i < CustomPresetOptions.Count; i++)
		{
			SAINPresetDefinition sAINPresetDefinition = CustomPresetOptions[i];
			if (sAINPresetDefinition.Name.Contains(presetName) || sAINPresetDefinition.Name == presetName)
			{
				definition = sAINPresetDefinition;
				return true;
			}
		}
		return false;
	}
}
