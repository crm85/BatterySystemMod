using System;
using System.Collections.Generic;
using System.Linq;
using EFT.UI;
using SAIN.Attributes;
using SAIN.Editor.Util;
using SAIN.Helpers;
using SAIN.Plugin;
using UnityEngine;

namespace SAIN.Editor;

public static class BuilderClass
{
	public sealed class SearchParams : GUIParams
	{
		public float labelWidth = 150f;

		public float textFieldWidth = 250f;

		public float clearWidth = 75f;

		public GUILayoutOption[] Label => (GUILayoutOption[])(object)new GUILayoutOption[2]
		{
			GUILayout.Width(labelWidth),
			base.Height
		};

		public GUILayoutOption[] TextField => (GUILayoutOption[])(object)new GUILayoutOption[2]
		{
			GUILayout.Width(textFieldWidth),
			base.Height
		};

		public GUILayoutOption[] Clear => (GUILayoutOption[])(object)new GUILayoutOption[2]
		{
			GUILayout.Width(labelWidth),
			base.Height
		};

		public SearchParams(float height = 30f)
			: base(height)
		{
			Options = new EGUIConfig[2]
			{
				EGUIConfig.horizontal,
				EGUIConfig.startFlexSpace
			};
		}
	}

	public class GUIParams
	{
		public EGUIConfig[] Options = new EGUIConfig[2]
		{
			EGUIConfig.beginHorizontal,
			EGUIConfig.endHorizontal
		};

		public float optionHeight = 30f;

		public float optionSpacing = 5f;

		public float FixedSpaceWidth = 10f;

		public GUILayoutOption Height => GUILayout.Height(optionHeight);

		public bool StartFlexSpace => Options.Contains(EGUIConfig.startFlexSpace);

		public bool EndFlexSpace => Options.Contains(EGUIConfig.endFlexSpace);

		public bool Horizontal => Options.Contains(EGUIConfig.horizontal);

		public bool Vertical => Options.Contains(EGUIConfig.vertical);

		public bool FixedSpace => Options.Contains(EGUIConfig.fixedSpace);

		public GUIParams(float height)
		{
			optionHeight = height;
		}

		public void Spacing()
		{
			GUILayout.Space(optionSpacing);
		}

		public void Start()
		{
			if (Horizontal)
			{
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			}
			else if (Vertical)
			{
				GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
			}
			if (FixedSpace)
			{
				GUILayout.Space(FixedSpaceWidth);
			}
			else if (StartFlexSpace)
			{
				GUILayout.FlexibleSpace();
			}
		}

		public void End()
		{
			if (FixedSpace)
			{
				GUILayout.Space(FixedSpaceWidth);
			}
			else if (EndFlexSpace)
			{
				GUILayout.FlexibleSpace();
			}
			if (Horizontal)
			{
				GUILayout.EndHorizontal();
			}
			else if (Vertical)
			{
				GUILayout.EndVertical();
			}
		}
	}

	public enum EGUIConfig
	{
		startFlexSpace,
		endFlexSpace,
		beginHorizontal,
		endHorizontal,
		fixedSpace,
		horizontal,
		vertical
	}

	private static readonly EUISoundType SelectionSound = (EUISoundType)9;

	private static float ExpandMenuWidth => 250f;

	public static string SearchBox(SettingsContainer container, float height = 30f, SearchParams config = null)
	{
		container.SearchPattern = SearchBox(container.SearchPattern, height, config);
		return container.SearchPattern;
	}

	public static string SearchBox(string search, float height = 30f, SearchParams config = null)
	{
		config = config ?? new SearchParams
		{
			optionHeight = height
		};
		config.Start();
		SAINLayout.Label("Search", config.Label);
		config.Spacing();
		search = SAINLayout.TextField(search, null, config.TextField);
		config.Spacing();
		if (SAINLayout.Button("Clear", (EUISoundType)10, config.Clear))
		{
			search = string.Empty;
		}
		config.End();
		return search;
	}

	public static bool SaveChanges(string toolTip, float height = 35f)
	{
		SAINLayout.BeginHorizontal();
		bool result = false;
		if (SAINLayout.Button("Save and Export", toolTip, (EUISoundType)7, SAINLayout.Height(height), SAINLayout.Width(500f)))
		{
			result = true;
		}
		if (ConfigEditingTracker.UnsavedChanges)
		{
			Alert("Click Save to export changes, and send changes to bots if in-game", "YOU HAVE UNSAVED CHANGES", height, 250f, ColorNames.LightRed);
		}
		else
		{
			Alert(null, null, height, 250f);
		}
		SAINLayout.EndHorizontal();
		return result;
	}

	public static void Alert(string toolTip, string text = null, float height = 25f, float width = 25f, ColorNames? colorName = null)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		if (GClass1437.IsNullOrEmpty(toolTip) && GClass1437.IsNullOrEmpty(text))
		{
			SAINLayout.Box(string.Empty, SAINLayout.Height(height));
			return;
		}
		GUIStyle val = SAINLayout.GetStyle(Style.alert);
		if (colorName.HasValue)
		{
			val = new GUIStyle(val);
			ApplyToStyle.BackgroundAllStates(TexturesClass.GetTexture(colorName.Value), val);
		}
		text = text ?? "";
		GUIContent content = new GUIContent(text, toolTip);
		SAINLayout.Box(content, val, SAINLayout.Height(height), SAINLayout.Width(width));
	}

	public static void Alert(string toolTip, string text = null, float height = 25f, ColorNames? colorName = null)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		if (GClass1437.IsNullOrEmpty(toolTip) && GClass1437.IsNullOrEmpty(text))
		{
			SAINLayout.Box(string.Empty, SAINLayout.Height(height));
			return;
		}
		GUIStyle val = SAINLayout.GetStyle(Style.alert);
		if (colorName.HasValue)
		{
			val = new GUIStyle(val);
			ApplyToStyle.BackgroundAllStates(TexturesClass.GetTexture(colorName.Value), val);
		}
		text = text ?? string.Empty;
		GUIContent content = new GUIContent(text, toolTip);
		SAINLayout.Box(content, val, SAINLayout.Height(height));
	}

	public static void MinValueBox(object value, params GUILayoutOption[] options)
	{
		if (value != null)
		{
			SAINLayout.Box(value.ToString(), "Minimum", options);
		}
	}

	public static void MaxValueBox(object value, params GUILayoutOption[] options)
	{
		if (value != null)
		{
			SAINLayout.Box(value.ToString(), "Maximum", options);
		}
	}

	public static object ResultBox(object value, params GUILayoutOption[] options)
	{
		if (value != null)
		{
			SAINLayout.Box(value.ToString(), "The Rounding this option is set to", options);
			string input = SAINLayout.TextField(value.ToString(), null, options);
			value = CleanString(input, value);
		}
		return value;
	}

	public static object CleanString(string input, object currentValue)
	{
		if (currentValue is float currentValue2)
		{
			currentValue = CleanString(input, currentValue2);
		}
		if (currentValue is int currentValue3)
		{
			currentValue = CleanString(input, currentValue3);
		}
		if (currentValue is bool currentValue4)
		{
			currentValue = CleanString(input, currentValue4);
		}
		return currentValue;
	}

	public static float CleanString(string input, float currentValue)
	{
		if (float.TryParse(input, out var result))
		{
			return result;
		}
		return currentValue;
	}

	public static int CleanString(string input, int currentValue)
	{
		if (int.TryParse(input, out var result))
		{
			return result;
		}
		if (float.TryParse(input, out var result2))
		{
			return Mathf.RoundToInt(result2);
		}
		return currentValue;
	}

	public static bool CleanString(string input, bool currentValue)
	{
		if (input == true.ToString() || input == false.ToString())
		{
			currentValue = bool.Parse(input);
		}
		return currentValue;
	}

	public static T SelectionGrid<T>(T value, float height, int optionsPerLine, params T[] valueOptions)
	{
		if (valueOptions.Length == 0)
		{
			return value;
		}
		GUILayoutOption[] options;
		int count = StartSelection(optionsPerLine, height, out options);
		for (int i = 0; i < valueOptions.Length; i++)
		{
			value = CheckToggle(value, valueOptions[i], options);
			count = HorizontalSpacing(count, optionsPerLine);
		}
		SAINLayout.EndHorizontal();
		return value;
	}

	public static T SelectionGrid<T>(T value, params T[] valueOptions)
	{
		return SelectionGrid(value, 25f, 3, valueOptions);
	}

	public static T SelectionGrid<T>(T value, List<T> list)
	{
		return SelectionGrid(value, 25f, 3, list);
	}

	public static T SelectionGrid<T>(T value, float height, int optionsPerLine, List<T> list)
	{
		GUILayoutOption[] options;
		int count = StartSelection(optionsPerLine, height, out options);
		for (int i = 0; i < list.Count; i++)
		{
			value = CheckToggle(value, list[i], options);
			count = HorizontalSpacing(count, optionsPerLine);
		}
		SAINLayout.EndHorizontal();
		return value;
	}

	public static T SelectionGrid<T>(T value, List<T> list, float height = 25f, int optionsPerLine = 3)
	{
		GUILayoutOption[] options;
		int count = StartSelection(optionsPerLine, height, out options);
		for (int i = 0; i < list.Count; i++)
		{
			value = CheckToggle(value, list[i], options);
			count = HorizontalSpacing(count, optionsPerLine);
		}
		SAINLayout.EndHorizontal();
		return value;
	}

	private static int StartSelection(int optionsCount, float height, out GUILayoutOption[] options)
	{
		SAINLayout.BeginHorizontalSpace();
		float width = 1850f / (float)optionsCount;
		options = (GUILayoutOption[])(object)new GUILayoutOption[2]
		{
			SAINLayout.Height(height),
			SAINLayout.Width(width)
		};
		return 0;
	}

	private static int HorizontalSpacing(int count, int max)
	{
		count++;
		if (count >= max)
		{
			count = 0;
			SAINLayout.EndHorizontal();
			SAINLayout.BeginHorizontalSpace();
		}
		return count;
	}

	private static T CheckToggle<T>(T value, T newValue, params GUILayoutOption[] options)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		string text = newValue.ToString();
		bool value2 = text == value.ToString();
		if (SAINLayout.Toggle(value2, text, SelectionSound, options))
		{
			value = newValue;
		}
		return value;
	}

	public static string SelectionGridExpandHeight(Rect menuRect, string[] options, string selectedOption, Rect[] optionRects, float min = 15f, float incPerFrame = 3f, float closeMulti = 0.66f, string[] toolTips = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		SAINLayout.BeginGroup(menuRect);
		string text = string.Empty;
		for (int i = 0; i < options.Length; i++)
		{
			if (toolTips != null)
			{
				text = toolTips[i];
			}
			string text2 = options[i];
			bool flag = selectedOption == text2;
			optionRects[i] = AnimateHeight(optionRects[i], flag, ((Rect)(ref menuRect)).height, out var hovering, min, incPerFrame, closeMulti);
			GUIStyle val = StyleHandler(flag, hovering);
			bool flag2 = GUI.Button(optionRects[i], new GUIContent(text2, text), val);
			if (flag2 && flag)
			{
				Sounds.PlaySound((EUISoundType)3);
				selectedOption = "None";
			}
			if (flag2 && !flag)
			{
				Sounds.PlaySound((EUISoundType)3);
				selectedOption = text2;
			}
		}
		SAINLayout.EndGroup();
		return selectedOption;
	}

	public static GUIStyle StyleHandler(bool selected, bool hovering)
	{
		GUIStyle fontStyleDynamic = StylesClass.GetFontStyleDynamic(Style.selectionGrid, selected);
		Texture2D normal = (selected ? TexturesClass.GetTexture(ColorNames.DarkRed) : ((!hovering) ? TexturesClass.GetTexture(EGraynessLevel.Mid) : TexturesClass.GetTexture(ColorNames.LightRed)));
		ApplyToStyle.BackgroundAllStates(normal, fontStyleDynamic);
		return fontStyleDynamic;
	}

	public static string SelectionGridExpandWidth(Rect menuRect, string[] options, string selectedOption, Rect[] optionRects, float min = 15f, float incPerFrame = 3f, float closeMulti = 0.66f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		SAINLayout.BeginGroup(menuRect);
		for (int i = 0; i < options.Length; i++)
		{
			string text = options[i];
			bool flag = selectedOption == text;
			optionRects[i] = AnimateWidth(optionRects[i], flag, ((Rect)(ref menuRect)).width, out var hovering, min, incPerFrame, closeMulti);
			GUIStyle val = StyleHandler(flag, hovering);
			bool flag2 = GUI.Button(optionRects[i], text, val);
			if (flag2 && flag)
			{
				Sounds.PlaySound((EUISoundType)3);
				selectedOption = "None";
			}
			if (flag2 && !flag)
			{
				Sounds.PlaySound((EUISoundType)3);
				selectedOption = text;
			}
		}
		SAINLayout.EndGroup();
		return selectedOption;
	}

	public static void SelectionGridExpandWidth(Rect menuRect, string[] options, List<string> selectedList, Rect[] optionRects, float min = 15f, float incPerFrame = 3f, float closeMulti = 0.66f)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < options.Length; i++)
		{
			string text = options[i];
			bool flag = selectedList.Contains(text);
			optionRects[i] = AnimateWidth(optionRects[i], flag, ((Rect)(ref menuRect)).width, out var hovering, min, incPerFrame, closeMulti);
			GUIStyle val = StyleHandler(flag, hovering);
			bool flag2 = GUI.Toggle(optionRects[i], flag, text, val);
			if (flag2 != flag)
			{
				Sounds.PlaySound((EUISoundType)12);
				if (flag)
				{
					selectedList.Remove(text);
				}
				else
				{
					selectedList.Add(text);
				}
			}
		}
	}

	public static Rect[] VerticalGridRects(Rect MenuRect, int count, float startWidth)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		Rect[] array = (Rect[])(object)new Rect[count];
		float num = ((Rect)(ref MenuRect)).height / (float)count;
		float x = 0f;
		for (int i = 0; i < array.Length; i++)
		{
			float y = num * (float)i;
			int num2 = i;
			Rect val = default(Rect);
			((Rect)(ref val)).x = x;
			((Rect)(ref val)).y = y;
			((Rect)(ref val)).width = startWidth;
			((Rect)(ref val)).height = num;
			array[num2] = val;
		}
		return array;
	}

	public static Rect[] HorizontalGridRects(Rect MenuRect, int count, float startHeight)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		Rect[] array = (Rect[])(object)new Rect[count];
		float num = ((Rect)(ref MenuRect)).width / (float)count;
		float y = 0f;
		for (int i = 0; i < array.Length; i++)
		{
			float x = num * (float)i;
			int num2 = i;
			Rect val = default(Rect);
			((Rect)(ref val)).x = x;
			((Rect)(ref val)).y = y;
			((Rect)(ref val)).width = num;
			((Rect)(ref val)).height = startHeight;
			array[num2] = val;
		}
		return array;
	}

	private static Rect AnimateHeight(Rect rect, bool selected, float max, out bool hovering, float min = 15f, float incPerFrame = 3f, float closeMulti = 0.66f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		Rect rect2 = rect;
		((Rect)(ref rect2)).height = max;
		hovering = MouseFunctions.IsMouseInside(rect2);
		((Rect)(ref rect)).height = Animate(((Rect)(ref rect)).height, hovering, selected, max, min, incPerFrame, closeMulti);
		return rect;
	}

	private static Rect AnimateWidth(Rect rect, bool selected, float max, out bool hovering, float min = 15f, float incPerFrame = 3f, float closeMulti = 0.66f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		Rect rect2 = rect;
		((Rect)(ref rect2)).width = max;
		hovering = MouseFunctions.IsMouseInside(rect2);
		((Rect)(ref rect)).width = Animate(((Rect)(ref rect)).width, hovering, selected, max, min, incPerFrame, closeMulti);
		return rect;
	}

	private static float Animate(float current, bool mouseHover, bool selected, float max, float min = 15f, float incPerFrame = 3f, float closeMulti = 0.66f)
	{
		current = ((!(mouseHover || selected)) ? (current - incPerFrame * closeMulti) : (current + incPerFrame));
		current = Mathf.Clamp(current, min, max);
		return current;
	}

	public static bool ExpandableMenu(string name, bool value, string description = null, float height = 20f)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Expected O, but got Unknown
		SAINLayout.BeginHorizontal();
		value = SAINLayout.Toggle(value, new GUIContent(value ? "-" : "+", value ? "Collapse" : "Expand"), (EUISoundType)11, SAINLayout.Width(17.5f), SAINLayout.Height(height));
		value = SAINLayout.Toggle(value, new GUIContent(name, description), (EUISoundType)11, SAINLayout.Height(height));
		SAINLayout.EndHorizontal();
		return value;
	}

	public static float CreateSlider(float value, float min, float max, float rounding, params GUILayoutOption[] options)
	{
		float value2 = value;
		value = SAINLayout.HorizontalSlider(value2, min, max, null, options);
		Backgrounds(value, min, max);
		if (!MouseFunctions.MouseIsMoving)
		{
			value = value.Round(rounding);
		}
		return value;
	}

	public static float CreateSlider(float value, ConfigInfoClass info, GUIEntryConfig config)
	{
		float num = value;
		value = SAINLayout.HorizontalSlider(value, info.Min, info.Max, null, config.Toggle);
		Backgrounds(value, info.Min, info.Max);
		SAINLayout.Box(value.Round(info.Rounding).ToString(), config.Result);
		if (!MouseFunctions.MouseIsMoving)
		{
			value = value.Round(info.Rounding);
		}
		return value;
	}

	private static void Backgrounds(float value, float min, float max)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Rect lastRect = GUILayoutUtility.GetLastRect();
		float progressRatio = (value - min) / (max - min);
		TexturesClass.DrawSliderBackGrounds(progressRatio, lastRect);
	}
}
