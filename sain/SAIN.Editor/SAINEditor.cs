using System;
using System.Runtime.CompilerServices;
using BepInEx;
using BepInEx.Configuration;
using EFT.Console.Core;
using EFT.UI;
using SAIN.Editor.Util;
using SAIN.Plugin;
using SAIN.Preset;
using UnityEngine;

namespace SAIN.Editor;

public static class SAINEditor
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static WindowFunction _003C0_003E__MainWindowFunc;
	}

	private static float CheckKeyLimiter;

	public static bool ShiftKeyPressed;

	public static bool CtrlKeyPressed;

	private static bool ToggleKeyPressed;

	private static bool EscapeKeyPressed;

	private static bool CacheCreated;

	public static string ExceptionString;

	private static readonly GUIContent SaveContent;

	public static Rect OpenTabRect;

	public static bool AdvancedBotConfigs => PresetHandler.EditorDefaults.AdvancedBotConfigs;

	public static bool DisplayingWindow
	{
		get
		{
			return CursorSettings.DisplayingWindow;
		}
		set
		{
			CursorSettings.DisplayingWindow = value;
		}
	}

	private static Texture2D DragBackgroundTexture => TexturesClass.GetTexture(EGraynessLevel.Mid);

	static SAINEditor()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		ExceptionString = string.Empty;
		SaveContent = new GUIContent("Save All Changes", "Export All Changes to SAIN/Presets/" + SAINPlugin.LoadedPreset.Info.Name);
		OpenTabRect = new Rect(0f, 0f, ((Rect)(ref RectLayout.MainWindow)).width, 1000f);
		ConsoleScreen.Processor.RegisterCommand("saineditor", (Action)ToggleGUI, (string)null);
	}

	public static void Init()
	{
		CursorSettings.InitCursor();
	}

	[ConsoleCommand("Toggle SAIN GUI Editor", "", null, "", new string[] { })]
	private static void ToggleGUI()
	{
		DisplayingWindow = !DisplayingWindow;
	}

	private static void CheckKeys()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (CheckKeyLimiter < Time.time)
		{
			CheckKeyLimiter = Time.time + 0.1f;
			ShiftKeyPressed = Input.GetKey((KeyCode)304);
			CtrlKeyPressed = Input.GetKey((KeyCode)306);
			KeyboardShortcut value = SAINPlugin.OpenEditorConfigEntry.Value;
			ToggleKeyPressed = Input.GetKeyDown(((KeyboardShortcut)(ref value)).MainKey);
			EscapeKeyPressed = Input.GetKeyDown((KeyCode)27);
		}
	}

	public static void Update()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (DisplayingWindow)
		{
			CursorSettings.SetUnlockCursor(0, cursorVisible: true);
			MouseFunctions.Update();
		}
		else
		{
			CheckKeys();
		}
		KeyboardShortcut value = SAINPlugin.OpenEditorConfigEntry.Value;
		if ((((KeyboardShortcut)(ref value)).IsDown() && !DisplayingWindow) || SAINPlugin.OpenEditorButton.Value)
		{
			if (SAINPlugin.OpenEditorButton.Value)
			{
				((ConfigEntryBase)SAINPlugin.OpenEditorButton).BoxedValue = false;
				SAINPlugin.OpenEditorButton.Value = false;
			}
			ToggleGUI();
		}
	}

	public static void LateUpdate()
	{
		if (DisplayingWindow)
		{
			CursorSettings.SetUnlockCursor(0, cursorVisible: true);
		}
	}

	public static void OnGUI()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected O, but got Unknown
		if (DisplayingWindow)
		{
			if (!CacheCreated)
			{
				CacheCreated = true;
				ColorsClass.CreateCache();
				TexturesClass.CreateCache();
				StylesClass.CreateCache();
			}
			MouseFunctions.OnGUI();
			CursorSettings.SetUnlockCursor(0, cursorVisible: true);
			GUIUtility.ScaleAroundPivot(RectLayout.ScaledPivot, Vector2.zero);
			Rect mainWindow = RectLayout.MainWindow;
			object obj = _003C_003EO._003C0_003E__MainWindowFunc;
			if (obj == null)
			{
				WindowFunction val = MainWindowFunc;
				_003C_003EO._003C0_003E__MainWindowFunc = val;
				obj = (object)val;
			}
			RectLayout.MainWindow = GUI.Window(0, mainWindow, (WindowFunction)obj, "SAIN AI Settings Editor", SAINLayout.GetStyle(Style.window));
			UnityInput.Current.ResetInputAxes();
			ConfigEditingTracker.Update();
		}
	}

	private static void MainWindowFunc(int TWCWindowID)
	{
		GUI.FocusWindow(TWCWindowID);
		CheckKeys();
		if (ToggleKeyPressed || EscapeKeyPressed)
		{
			ToggleGUI();
			return;
		}
		CreateDragBar();
		CreateTopBarOptions();
		EEditorTab selectedTab = EditTabsClass.TabSelectMenu(35f, 3f, 0.5f);
		float value = ((Rect)(ref RectLayout.DragRect)).height + ((Rect)(ref EditTabsClass.TabMenuRect)).height;
		SAINLayout.Space(value);
		GUITabs.CreateTabs(selectedTab);
		MouseFunctions.OnGUI();
		DrawTooltip();
	}

	private static void CreateDragBar()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		GUI.DrawTexture(RectLayout.DragRect, (Texture)(object)DragBackgroundTexture, (ScaleMode)0, true, 0f);
		GUI.Box(RectLayout.DragRect, "SAIN 4.0.3 GUI Editor | Preset: " + SAINPlugin.LoadedPreset.Info.Name, SAINLayout.GetStyle(Style.dragBar));
		GUI.DragWindow(RectLayout.DragRect);
	}

	private static void CreateTopBarOptions()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		SaveContent.tooltip = ConfigEditingTracker.GetUnsavedValuesString();
		GUIStyle style = SAINLayout.GetStyle(Style.botTypeGrid);
		TextAnchor alignment = style.alignment;
		style.alignment = (TextAnchor)4;
		bool advancedBotConfigs = PresetHandler.EditorDefaults.AdvancedBotConfigs;
		string text = (advancedBotConfigs ? "ON" : "OFF");
		bool flag = GUI.Toggle(RectLayout.AdvRect, advancedBotConfigs, "Advanced Settings: [" + text + "]", SAINLayout.GetStyle(Style.botTypeGrid));
		if (advancedBotConfigs != flag)
		{
			Sounds.PlaySound((EUISoundType)13);
			PresetHandler.EditorDefaults.AdvancedBotConfigs = flag;
			PresetHandler.ExportEditorDefaults();
		}
		if (GUI.Button(RectLayout.SaveAllRect, SaveContent, SAINLayout.GetStyle(Style.botTypeGrid)))
		{
			Sounds.PlaySound((EUISoundType)7);
			SAINPresetClass.ExportAll(SAINPlugin.LoadedPreset);
		}
		if (GUI.Button(RectLayout.ExitRect, "X", SAINLayout.GetStyle(Style.botTypeGrid)))
		{
			Sounds.PlaySound((EUISoundType)13);
			ToggleGUI();
		}
		style.alignment = alignment;
	}

	private static void DrawTooltip()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		if (!string.IsNullOrEmpty(GUI.tooltip))
		{
			float num = Event.current.mousePosition.x;
			float num2 = Event.current.mousePosition.y + 15f;
			if (num > (float)(Screen.width / 3))
			{
				num -= 250f;
			}
			GUIStyle style = SAINLayout.GetStyle(Style.tooltip);
			float num3 = style.CalcHeight(new GUIContent(GUI.tooltip), 250f) + 10f;
			GUI.Box(new Rect(num, num2, 250f, num3), GUI.tooltip, style);
		}
	}
}
