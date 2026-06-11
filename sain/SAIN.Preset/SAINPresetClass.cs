using System;
using System.Collections.Generic;
using SAIN.Helpers;
using SAIN.Plugin;
using SAIN.Preset.BotSettings;
using SAIN.Preset.BotSettings.SAINSettings;
using SAIN.Preset.GearStealthValues;
using SAIN.Preset.GlobalSettings;
using SAIN.Preset.Personalities;

namespace SAIN.Preset;

public class SAINPresetClass
{
	public static SAINPresetClass Instance { get; private set; }

	public SAINPresetDefinition Info { get; private set; }

	public GlobalSettingsClass GlobalSettings { get; private set; }

	public SAINBotSettingsClass BotSettings { get; private set; }

	public PersonalityManagerClass PersonalityManager { get; private set; }

	public GearStealthValuesClass GearStealthValuesClass { get; private set; }

	public SAINPresetClass(SAINPresetDefinition preset, bool isCopy = false)
	{
		Instance = this;
		SAINPlugin.EditorDefaults.SelectedDefaultPreset = SAINDifficulty.none;
		if (isCopy)
		{
			CopyPreset(preset);
		}
		CreateSettings(preset, preset.BaseSAINDifficulty);
	}

	private void CopyPreset(SAINPresetDefinition preset)
	{
		if (Instance != null)
		{
			SAINPresetDefinition info = SAINPlugin.LoadedPreset.Info;
			SAINPlugin.LoadedPreset.Info = preset;
			ExportAll(SAINPlugin.LoadedPreset);
			SAINPlugin.LoadedPreset.Info = info;
		}
	}

	public SAINPresetClass(SAINDifficulty sainDifficulty)
	{
		Instance = this;
		SAINPlugin.EditorDefaults.SelectedCustomPreset = string.Empty;
		SAINPlugin.EditorDefaults.SelectedDefaultPreset = sainDifficulty;
		PresetHandler.ExportEditorDefaults();
		CreateSettings(null, sainDifficulty);
	}

	private void CreateSettings(SAINPresetDefinition preset, SAINDifficulty difficulty)
	{
		if (preset == null)
		{
			Info = SAINDifficultyClass.DefaultPresetDefinitions[difficulty];
			GlobalSettings = new GlobalSettingsClass();
		}
		else
		{
			Info = preset;
			GlobalSettings = GlobalSettingsClass.ImportGlobalSettings(preset);
		}
		BotSettings = new SAINBotSettingsClass(this);
		PersonalityManager = new PersonalityManagerClass(this);
		GearStealthValuesClass = new GearStealthValuesClass(Info);
	}

	public void Init()
	{
		GlobalSettings.Init();
		BotSettings.Init();
		PersonalityManager.Init();
	}

	public void UpdateDefaults(SAINPresetClass preset = null)
	{
		GlobalSettings.UpdateDefaults(preset?.GlobalSettings);
		BotSettings.UpdateDefaults(preset?.BotSettings);
		PersonalityManager.UpdateDefaults(preset?.PersonalityManager);
	}

	public static void ExportAll(SAINPresetClass preset)
	{
		ConfigEditingTracker.Clear();
		if (!preset.Info.IsCustom)
		{
			SAINPresetDefinition sAINPresetDefinition = preset.Info.Clone();
			sAINPresetDefinition.Name += " [Modified]";
			sAINPresetDefinition.Creator = "user";
			sAINPresetDefinition.Description = "[Modified] " + sAINPresetDefinition.Description;
			sAINPresetDefinition.DateCreated = DateTime.Today.ToString();
			PresetHandler.SavePresetDefinition(sAINPresetDefinition);
			PresetHandler.InitPresetFromDefinition(sAINPresetDefinition, isCopy: true);
			PresetHandler.UpdateExistingBots();
		}
		else
		{
			ExportDefinition(preset.Info);
			ExportGlobalSettings(preset.GlobalSettings, preset.Info.Name);
			ExportPersonalities(preset.PersonalityManager, preset.Info.Name);
			ExportBotSettings(preset.BotSettings, preset.Info.Name);
			GearStealthValuesClass.Export(preset.GearStealthValuesClass, preset.Info);
			PresetHandler.UpdateExistingBots();
		}
	}

	private static void ExportDefinition(SAINPresetDefinition info)
	{
		if (!info.IsCustom)
		{
			return;
		}
		try
		{
			Export(info, info.Name, "Info");
		}
		catch (Exception ex)
		{
			LogExportError(ex);
		}
	}

	private static bool ExportGlobalSettings(GlobalSettingsClass globalSettings, string presetName)
	{
		bool result = false;
		try
		{
			Export(globalSettings, presetName, "GlobalSettings");
			result = true;
		}
		catch (Exception ex)
		{
			LogExportError(ex);
		}
		return result;
	}

	private static bool ExportPersonalities(PersonalityManagerClass personClass, string presetName)
	{
		bool result = false;
		try
		{
			foreach (KeyValuePair<EPersonality, PersonalitySettingsClass> item in personClass.PersonalityDictionary)
			{
				if (item.Value == null || !Export(item.Value, presetName, item.Key.ToString(), "Personalities"))
				{
					if (item.Value == null)
					{
						Logger.LogError("Personality Settings Are Null");
					}
					else
					{
						Logger.LogError($"Failed to Export {item.Key}");
					}
				}
			}
			result = true;
		}
		catch (Exception ex)
		{
			LogExportError(ex);
		}
		return result;
	}

	private static bool ExportBotSettings(SAINBotSettingsClass botSettings, string presetName)
	{
		bool result = false;
		try
		{
			foreach (SAINSettingsGroupClass value in botSettings.SAINSettings.Values)
			{
				Export(value, presetName, value.Name, "BotSettings");
			}
			result = true;
		}
		catch (Exception ex)
		{
			LogExportError(ex);
		}
		return result;
	}

	public static bool Export(object obj, string presetName, string fileName, string subFolder = null)
	{
		bool result = false;
		try
		{
			string[] array = Folders(presetName, subFolder);
			JsonUtility.SaveObjectToJson(obj, fileName, array);
			result = true;
			string text = string.Empty;
			for (int i = 0; i < array.Length; i++)
			{
				text = text + "/" + array[i];
			}
			Logger.LogDebug("Successfully Exported [" + obj.GetType().Name + "] : Name: [" + fileName + "] To: [" + text + "]");
		}
		catch (Exception ex)
		{
			Logger.LogError("Failed Export of Type [" + obj.GetType().Name + "] Name: [" + fileName + "]");
			LogExportError(ex);
		}
		return result;
	}

	public static bool Import<T>(out T result, string presetName, string fileName, string subFolder = null)
	{
		string[] array = Folders(presetName, subFolder);
		if (JsonUtility.Load.LoadJsonFile(out var json, fileName, array))
		{
			try
			{
				result = JsonUtility.Load.DeserializeObject<T>(json);
				string text = string.Empty;
				for (int i = 0; i < array.Length; i++)
				{
					text = text + "/" + array[i];
				}
				Logger.LogDebug("Successfully Imported [" + typeof(T).Name + "] File Name: [" + fileName + "] To Path: [" + text + "]");
				return true;
			}
			catch (Exception ex)
			{
				Logger.LogError($"Failed import Item of Type {typeof(T)}");
				LogExportError(ex);
			}
		}
		result = default(T);
		return false;
	}

	public static string[] Folders(string presetName, string subFolder = null)
	{
		string text = "Presets";
		if (subFolder == null)
		{
			return new string[2] { text, presetName };
		}
		return new string[3] { text, presetName, subFolder };
	}

	private static void LogExportError(Exception ex)
	{
		Logger.LogError($"Export Error: {ex}");
	}
}
