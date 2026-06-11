using System.Collections.Generic;
using System.Text;
using EFT.UI;
using SAIN.Attributes;
using SAIN.Plugin;
using UnityEngine;

namespace SAIN.Editor.GUISections;

public static class BotSettingsEditor
{
	private static readonly StringBuilder _stringBuilder = new StringBuilder();

	public static bool WasEdited;

	private static readonly GUIEntryConfig EntryConfig = new GUIEntryConfig();

	public static void ShowAllSettingsGUI(object settings, out bool wasEdited, string name, string savePath, float height, out bool Saved)
	{
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		SAINLayout.BeginHorizontal();
		SAINLayout.Box(name, SAINLayout.Height(height));
		SAINLayout.Space(10f);
		SAINLayout.Label("Search", SAINLayout.Width(125f), SAINLayout.Height(height));
		SettingsContainer container = SettingsContainers.GetContainer(settings.GetType(), name);
		container.SearchPattern = SAINLayout.TextField(container.SearchPattern, null, SAINLayout.Width(250f), SAINLayout.Height(height));
		if (SAINLayout.Button("Clear", (EUISoundType)10, (GUILayoutOption[])(object)new GUILayoutOption[2]
		{
			SAINLayout.Width(80f),
			SAINLayout.Height(height)
		}))
		{
			container.SearchPattern = string.Empty;
		}
		SAINLayout.Space(10f);
		if (ConfigEditingTracker.UnsavedChanges)
		{
			BuilderClass.Alert("Click Save to export changes, and send changes to bots if in-game", "YOU HAVE UNSAVED CHANGES!", height, ColorNames.DarkRed);
		}
		else
		{
			BuilderClass.Alert(null, null, height, null);
		}
		Saved = SAINLayout.Button("Save and Export", ConfigEditingTracker.GetUnsavedValuesString(), (EUISoundType)7, SAINLayout.Height(height));
		SAINLayout.EndHorizontal();
		container.Scroll = SAINLayout.BeginScrollView(container.Scroll);
		CategoryOpenable(container.Categories, settings, out wasEdited, container.SearchPattern);
		SAINLayout.EndScrollView();
	}

	public static bool CheckIfOpen(SettingsContainer container, float height = 30f)
	{
		SAINLayout.BeginHorizontal();
		container.Open = BuilderClass.ExpandableMenu(container.Name, container.Open, null, height);
		if (SAINLayout.Button("Clear", "Clear Selected Options in this Menu", (EUISoundType)12, SAINLayout.Width(100f), SAINLayout.Height(height)))
		{
			container.SelectedCategories.Clear();
			foreach (Category category in container.Categories)
			{
				category.SelectedList.Clear();
			}
		}
		SAINLayout.EndHorizontal();
		return container.Open;
	}

	private static void CategoryOpenable(List<Category> categories, object settingsObject, out bool wasEdited, string search = null)
	{
		wasEdited = false;
		foreach (Category category in categories)
		{
			if (category.OptionCount(out var _) == 0)
			{
				continue;
			}
			ConfigInfoClass categoryInfo = category.CategoryInfo;
			object value = category.GetValue(settingsObject);
			SAINLayout.BeginHorizontal(30f);
			bool flag = true;
			if (string.IsNullOrEmpty(search))
			{
				category.Open = BuilderClass.ExpandableMenu(categoryInfo.Name, category.Open, categoryInfo.Description, EntryConfig.EntryHeight);
				flag = category.Open;
			}
			else
			{
				SAINLayout.Box(categoryInfo.Name, categoryInfo.Description, SAINLayout.Height(PresetHandler.EditorDefaults.ConfigEntryHeight));
			}
			SAINLayout.EndHorizontal(30f);
			if (flag)
			{
				AttributesGUI.EditAllValuesInObj(category, value, out var wasEdited2, search);
				if (wasEdited2)
				{
					wasEdited = true;
				}
			}
		}
	}
}
