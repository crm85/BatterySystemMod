using System.Collections.Generic;
using SAIN.Attributes;
using SAIN.Helpers;
using SAIN.Plugin;
using SAIN.Preset;

namespace SAIN.Editor.GUISections;

public static class BotPersonalityEditor
{
	private static EPersonality _selected = EPersonality.None;

	private static List<EPersonality> _options = new List<EPersonality>();

	private static readonly Dictionary<string, bool> OpenPersMenus = new Dictionary<string, bool>();

	public static bool PersonalitiesWereEdited => ConfigEditingTracker.UnsavedChanges;

	public static void ClearCache()
	{
		ListHelpers.ClearCache(OpenPersMenus);
	}

	public static void PersonalityMenu()
	{
		string text = "Apply Values set below to Personalities. Exports edited values to SAIN/Presets/" + SAINPlugin.LoadedPreset.Info.Name + "/Personalities folder";
		if (BuilderClass.SaveChanges(ConfigEditingTracker.GetUnsavedValuesString()))
		{
			SAINPresetClass.ExportAll(SAINPlugin.LoadedPreset);
		}
		_selected = SelectPersonality(_selected, 35f, 4);
		if (_selected != EPersonality.None && SAINPresetClass.Instance.PersonalityManager.PersonalityDictionary.TryGetValue(_selected, out var value))
		{
			AttributesGUI.EditAllValuesInObj(value, out var _, null, null, 1);
		}
	}

	public static EPersonality SelectPersonality(EPersonality selected, float height, int optionsPerLine)
	{
		if (_options.Count == 0)
		{
			_options.AddRange(SAINPresetClass.Instance.PersonalityManager.PersonalityDictionary.Keys);
		}
		return BuilderClass.SelectionGrid(selected, height, optionsPerLine, _options);
	}
}
