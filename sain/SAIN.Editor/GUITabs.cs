using System;
using System.Runtime.CompilerServices;
using EFT.UI;
using SAIN.Attributes;
using SAIN.Components;
using SAIN.Editor.GUISections;
using SAIN.Helpers;
using SAIN.Plugin;
using SAIN.Preset;
using UnityEngine;

namespace SAIN.Editor;

public static class GUITabs
{
	private static bool _withGroupDelay;

	private static bool _forceTagStatusToggle;

	private static ETagStatus _forcedTagStatus;

	private static bool _forceTalkMenuOpen;

	private static bool _forceDecisionMenuOpen;

	private static bool ForceSoloOpen;

	private static bool ForceSquadOpen;

	private static bool ForceSelfOpen;

	public static void CreateTabs(EEditorTab selectedTab)
	{
		EditTabsClass.BeginScrollView();
		switch (selectedTab)
		{
		case EEditorTab.Home:
			Home();
			break;
		case EEditorTab.BotSettings:
			BotSettings();
			break;
		case EEditorTab.Personalities:
			Personality();
			break;
		case EEditorTab.EquipmentStealth:
			Stealth();
			break;
		case EEditorTab.Advanced:
			Advanced();
			break;
		}
		EditTabsClass.EndScrollView();
	}

	public static void Home()
	{
		PresetSelection.PresetSelectionMenu();
		SAINLayout.Space(20f);
		BotSettingsEditor.ShowAllSettingsGUI(SAINPlugin.LoadedPreset.GlobalSettings, out var _, "Global Settings", "SAIN/Presets/" + SAINPlugin.LoadedPreset.Info.Name, 35f, out var Saved);
		if (Saved)
		{
			SAINPresetClass.ExportAll(SAINPlugin.LoadedPreset);
			ConfigEditingTracker.Clear();
		}
	}

	public static void BotSettings()
	{
		BotSelectionClass.Menu();
	}

	public static void Personality()
	{
		BotPersonalityEditor.PersonalityMenu();
	}

	private static void Stealth()
	{
		SAINLayout.BeginVertical();
		SAINLayout.BeginHorizontal();
		if (ConfigEditingTracker.UnsavedChanges)
		{
			BuilderClass.Alert("Click Save to export changes, and send changes to bots if in-game", "YOU HAVE UNSAVED CHANGES!", 35f, ColorNames.DarkRed);
		}
		else
		{
			BuilderClass.Alert(null, null, 25f, null);
		}
		if (SAINLayout.Button("Save and Export", ConfigEditingTracker.GetUnsavedValuesString(), (EUISoundType)7, SAINLayout.Height(25f)))
		{
			SAINPresetClass.ExportAll(SAINPlugin.LoadedPreset);
		}
		SAINLayout.EndHorizontal();
		AttributesGUI.EditAllStealthValues(SAINPlugin.LoadedPreset.GearStealthValuesClass);
		SAINLayout.EndVertical();
	}

	private static void ForceDecisions(int spacing)
	{
		SAINLayout.Space(spacing);
		_forceDecisionMenuOpen = BuilderClass.ExpandableMenu("Force SAIN Bot Decisions", _forceDecisionMenuOpen);
		if (!_forceDecisionMenuOpen)
		{
			return;
		}
		SAINLayout.Space(spacing);
		ForceSoloOpen = BuilderClass.ExpandableMenu("Force Solo Decision", ForceSoloOpen);
		if (ForceSoloOpen)
		{
			SAINLayout.Space((float)spacing / 2f);
			if (SAINLayout.Button("Reset"))
			{
				SAINPlugin.ForceSoloDecision = ECombatDecision.None;
			}
			SAINLayout.Space((float)spacing / 2f);
			SAINPlugin.ForceSoloDecision = BuilderClass.SelectionGrid(SAINPlugin.ForceSoloDecision, EnumValues.GetEnum<ECombatDecision>());
		}
		SAINLayout.Space(spacing);
		ForceSquadOpen = BuilderClass.ExpandableMenu("Force Squad Decision", ForceSquadOpen);
		if (ForceSquadOpen)
		{
			SAINLayout.Space((float)spacing / 2f);
			if (SAINLayout.Button("Reset"))
			{
				SAINPlugin.ForceSquadDecision = ESquadDecision.None;
			}
			SAINLayout.Space((float)spacing / 2f);
			SAINPlugin.ForceSquadDecision = BuilderClass.SelectionGrid(SAINPlugin.ForceSquadDecision, EnumValues.GetEnum<ESquadDecision>());
		}
		SAINLayout.Space(spacing);
		ForceSelfOpen = BuilderClass.ExpandableMenu("Force Self Decision", ForceSelfOpen);
		if (ForceSelfOpen)
		{
			SAINLayout.Space((float)spacing / 2f);
			if (SAINLayout.Button("Reset"))
			{
				SAINPlugin.ForceSelfDecision = ESelfDecision.None;
			}
			SAINLayout.Space((float)spacing / 2f);
			SAINPlugin.ForceSelfDecision = BuilderClass.SelectionGrid(SAINPlugin.ForceSelfDecision, EnumValues.GetEnum<ESelfDecision>());
		}
	}

	public static void Advanced()
	{
		AttributesGUI.EditAllValuesInObj(PresetHandler.EditorDefaults, out var wasEdited);
		if (wasEdited)
		{
			PresetHandler.ExportEditorDefaults();
		}
		if (SAINPlugin.DebugMode)
		{
			ForceDecisions(4);
			ForceTalk(4);
		}
	}

	private static void ForceTalk(int spacing)
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Invalid comparison between Unknown and I4
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Invalid comparison between Unknown and I4
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		SAINLayout.Space(spacing);
		_forceTalkMenuOpen = BuilderClass.ExpandableMenu("Force Bots to Say Phrase", _forceTalkMenuOpen);
		if (!_forceTalkMenuOpen)
		{
			return;
		}
		SAINLayout.Space(5f);
		_forceTagStatusToggle = SAINLayout.Toggle(_forceTagStatusToggle, "Force ETagStatus for Phrase", null, Array.Empty<GUILayoutOption>());
		if (_forceTagStatusToggle)
		{
			ETagStatus[] array = EnumValues.GetEnum<ETagStatus>();
			for (int i = 0; i < array.Length; i++)
			{
				if (SAINLayout.Toggle((int)_forcedTagStatus == (int)array[i], ((object)System.Runtime.CompilerServices.Unsafe.As<ETagStatus, ETagStatus>(ref array[i])/*cast due to .constrained prefix*/).ToString(), null, Array.Empty<GUILayoutOption>()) && (int)_forcedTagStatus != (int)array[i])
				{
					_forcedTagStatus = array[i];
				}
			}
		}
		SAINLayout.Space(5f);
		_withGroupDelay = SAINLayout.Toggle(_withGroupDelay, "With Group Delay?", null, Array.Empty<GUILayoutOption>());
		SAINLayout.Space(5f);
		SAINLayout.Label("Say Phrase");
		EPhraseTrigger[] array2 = EnumValues.GetEnum<EPhraseTrigger>();
		for (int j = 0; j < array2.Length; j++)
		{
			if (!SAINLayout.Button(((object)System.Runtime.CompilerServices.Unsafe.As<EPhraseTrigger, EPhraseTrigger>(ref array2[j])/*cast due to .constrained prefix*/).ToString()) || BotManagerComponent.Instance?.Bots == null)
			{
				continue;
			}
			foreach (BotComponent value in BotManagerComponent.Instance.Bots.Values)
			{
				if ((Object)(object)value != (Object)null)
				{
					if (_forceTagStatusToggle)
					{
						value.Talk.Say(array2[j], _forcedTagStatus, _withGroupDelay);
					}
					else
					{
						value.Talk.Say(array2[j], null, _withGroupDelay);
					}
				}
			}
		}
	}
}
