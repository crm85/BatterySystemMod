using System;
using EFT.UI;
using UnityEngine;

namespace SAIN.Editor;

public static class SAINLayout
{
	public static void Box(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
	{
		GUILayout.Box(content, style, options);
	}

	public static void Box(string text, params GUILayoutOption[] options)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		GUILayout.Box(new GUIContent(text), GetStyle(Style.box), options);
	}

	public static void Box(string text, GUIStyle style, params GUILayoutOption[] options)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Expected O, but got Unknown
		GUILayout.Box(new GUIContent(text), style, options);
	}

	public static void Box(string text, string tooltip, params GUILayoutOption[] options)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		GUILayout.Box(new GUIContent(text, tooltip), GetStyle(Style.box), options);
	}

	public static void Box(string text, string tooltip, GUIStyle style, params GUILayoutOption[] options)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Expected O, but got Unknown
		GUILayout.Box(new GUIContent(text, tooltip), style, options);
	}

	public static void BlankBox(string text, params GUILayoutOption[] options)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		Box(new GUIContent(text), GetStyle(Style.blankbox), options);
	}

	public static void BlankBox(string text, string tooltip, params GUILayoutOption[] options)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected O, but got Unknown
		Box(new GUIContent(text, tooltip), GetStyle(Style.blankbox), options);
	}

	public static void ToolTip(Rect rect, GUIContent text)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		GUI.Box(rect, text, GetStyle(Style.tooltip));
	}

	public static void Label(string text, GUIStyle style, params GUILayoutOption[] options)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Expected O, but got Unknown
		Label(new GUIContent(text), style, options);
	}

	public static void Label(string text, string tooltip, GUIStyle style, params GUILayoutOption[] options)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Expected O, but got Unknown
		Label(new GUIContent(text, tooltip), style, options);
	}

	public static void Label(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
	{
		GUILayout.Label(content, style, options);
	}

	public static void Label(Rect rect, GUIContent content, GUIStyle style = null)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (style == null)
		{
			style = GetStyle(Style.label);
		}
		GUI.Label(rect, content, style);
	}

	public static void Label(Rect rect, string text, GUIStyle style = null)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (style == null)
		{
			style = GetStyle(Style.label);
		}
		GUI.Label(rect, text, style);
	}

	public static void Label(string text, params GUILayoutOption[] options)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		Label(new GUIContent(text), GetStyle(Style.label), options);
	}

	public static void Label(string text, string tooltip, params GUILayoutOption[] options)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		Label(new GUIContent(text, tooltip), GetStyle(Style.label), options);
	}

	public static void Label(GUIContent content, params GUILayoutOption[] options)
	{
		Label(content, GetStyle(Style.label), options);
	}

	public static string TextField(string value, EUISoundType? sound = null, params GUILayoutOption[] options)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		string text = GUILayout.TextField(value, GetStyle(Style.textField), options);
		if (CompareValuePlaySound(value, text, sound) && SAINPlugin.DebugMode)
		{
			Logger.LogDebug($"Toggle {sound.Value}");
		}
		return text;
	}

	public static string TextArea(string value, EUISoundType? sound = null, params GUILayoutOption[] options)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		string text = GUILayout.TextArea(value, GetStyle(Style.textField), options);
		if (CompareValuePlaySound(value, text, sound) && SAINPlugin.DebugMode)
		{
			Logger.LogDebug($"Toggle {sound.Value}");
		}
		return text;
	}

	public static bool Button(string text, params GUILayoutOption[] options)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		return Button(new GUIContent(text), null, options);
	}

	public static bool Button(string text, EUISoundType? sound, params GUILayoutOption[] options)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Expected O, but got Unknown
		return Button(new GUIContent(text), sound, options);
	}

	public static bool Button(string text, string tooltip, EUISoundType? sound, params GUILayoutOption[] options)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Expected O, but got Unknown
		return Button(new GUIContent(text, tooltip), sound, options);
	}

	public static bool Button(string text, string tooltip, EUISoundType? sound, GUIStyle style, params GUILayoutOption[] options)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		return Button(new GUIContent(text, tooltip), sound, options);
	}

	public static bool Button(GUIContent content, EUISoundType? sound, params GUILayoutOption[] options)
	{
		return Button(content, GetStyle(Style.button), sound, options);
	}

	public static bool Button(GUIContent content, GUIStyle style, EUISoundType? sound, params GUILayoutOption[] options)
	{
		if (GUILayout.Button(content, style, options))
		{
			CompareValuePlaySound(true, false, sound);
			return true;
		}
		return false;
	}

	public static bool Toggle(bool value, string text, EUISoundType? sound = null, params GUILayoutOption[] options)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Expected O, but got Unknown
		return Toggle(value, new GUIContent(text), sound, options);
	}

	public static bool Toggle(bool value, string text, string tooltip, EUISoundType? sound = null, params GUILayoutOption[] options)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return Toggle(value, new GUIContent(text, tooltip), sound, options);
	}

	public static bool Toggle(bool value, string text, string tooltip, GUIStyle style, EUISoundType? sound = null, params GUILayoutOption[] options)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		return Toggle(value, new GUIContent(text, tooltip), sound, options);
	}

	public static bool Toggle(bool value, GUIContent content, EUISoundType? sound = null, params GUILayoutOption[] options)
	{
		return Toggle(value, content, GetStyle(Style.toggle), sound, options);
	}

	public static bool Toggle(bool value, GUIContent content, GUIStyle style, EUISoundType? sound = null, params GUILayoutOption[] options)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		bool flag = GUILayout.Toggle(value, content, style, options);
		if (CompareValuePlaySound(value, flag, sound) && SAINPlugin.DebugMode)
		{
			Logger.LogDebug($"Toggle {sound.Value}");
		}
		return flag;
	}

	private static bool CompareValuePlaySound(object oldValue, object newValue, EUISoundType? sound = null, float volume = 1f)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (oldValue.ToString() != newValue.ToString() && sound.HasValue)
		{
			Sounds.PlaySound(sound.Value, volume);
			return true;
		}
		return false;
	}

	public static float HorizontalSlider(float value, float min, float max, EUISoundType? sound = null, params GUILayoutOption[] options)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		float num = GUILayout.HorizontalSlider(value, min, max, GetStyle(Style.horizontalSlider), GetStyle(Style.horizontalSliderThumb), options);
		float num2 = (num - min) / (max - min);
		sound = (EUISoundType)(((_003F?)sound) ?? 4);
		num2 = Mathf.Clamp(num2, 0.33f, 1f);
		if (!CompareValuePlaySound(value, num, sound, num2) || SAINPlugin.DebugMode)
		{
		}
		return num;
	}

	public static void BeginHorizontalSpace(float space = 10f)
	{
		BeginHorizontal();
		Space(space);
	}

	public static void EndHorizontalSpace(float space = 10f)
	{
		Space(space);
		EndHorizontal();
	}

	public static void BeginHorizontal(float indent = 0f)
	{
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		if (indent > 0f)
		{
			GUILayout.Space(indent);
		}
	}

	public static void BeginHorizontal(bool flexibleSpace)
	{
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		if (flexibleSpace)
		{
			GUILayout.FlexibleSpace();
		}
	}

	public static void EndHorizontal(float indent = 0f)
	{
		if (indent > 0f)
		{
			GUILayout.Space(indent);
		}
		GUILayout.EndHorizontal();
	}

	public static void EndHorizontal(bool flexibleSpace)
	{
		if (flexibleSpace)
		{
			GUILayout.FlexibleSpace();
		}
		GUILayout.EndHorizontal();
	}

	public static void BeginVertical(float indent = 0f)
	{
		GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
		if (indent > 0f)
		{
			GUILayout.Space(indent);
		}
	}

	public static void EndVertical(float indent = 0f)
	{
		if (indent > 0f)
		{
			GUILayout.Space(indent);
		}
		GUILayout.EndVertical();
	}

	public static void BeginArea(Rect rect)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		GUILayout.BeginArea(rect);
	}

	public static void EndArea()
	{
		GUILayout.EndArea();
	}

	public static void Space(float value, bool enable = true)
	{
		if (enable && value > 0f)
		{
			GUILayout.Space(value);
		}
	}

	public static void BeginGroup(Rect rect)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		GUI.BeginGroup(rect, GetStyle(Style.blankbox));
	}

	public static void EndGroup()
	{
		GUI.EndGroup();
	}

	public static GUILayoutOption ExpandHeight(bool value)
	{
		return GUILayout.ExpandHeight(value);
	}

	public static GUILayoutOption ExpandWidth(bool value)
	{
		return GUILayout.ExpandWidth(value);
	}

	public static void FlexibleSpace(bool value = true)
	{
		if (value)
		{
			GUILayout.FlexibleSpace();
		}
	}

	public static Vector2 BeginScrollView(Vector2 scrollPos, float width)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		return GUILayout.BeginScrollView(scrollPos, GetStyle(Style.scrollView), GetStyle(Style.verticalScrollbar), (GUILayoutOption[])(object)new GUILayoutOption[1] { Width(width) });
	}

	public static Vector2 BeginScrollView(Vector2 scrollPos, params GUILayoutOption[] options)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		return GUILayout.BeginScrollView(scrollPos, GetStyle(Style.scrollView), GetStyle(Style.verticalScrollbar), options);
	}

	public static Vector2 BeginScrollView(Rect rect, Vector2 scrollPos, Rect viewRect)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		return GUI.BeginScrollView(rect, scrollPos, viewRect, GetStyle(Style.scrollView), GetStyle(Style.verticalScrollbar));
	}

	public static void EndScrollView()
	{
		GUILayout.EndScrollView();
	}

	public static void EndScrollView(bool handleScrollWheel)
	{
		GUI.EndScrollView(handleScrollWheel);
	}

	public static GUIStyle GetStyle(Style key)
	{
		return StylesClass.GetStyle(key);
	}

	public static GUILayoutOption Height(float height)
	{
		return GUILayout.Height(height);
	}

	public static GUILayoutOption Width(float width)
	{
		return GUILayout.Width(width);
	}

	public static Rect NewWindow(int id, Rect viewRect, WindowFunction func, string title)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		return GUI.Window(id, viewRect, func, title, GetStyle(Style.window));
	}
}
