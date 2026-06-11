using UnityEngine;

namespace SAIN.Editor;

public static class ButtonsClass
{
	private const float InfoWidth = 25f;

	public static void InfoBox(string description, float height)
	{
		InfoBox(description, SAINLayout.Width(25f), SAINLayout.Height(height));
	}

	public static void InfoBox(string description, float height, float width)
	{
		InfoBox(description, SAINLayout.Width(width), SAINLayout.Height(height));
	}

	public static void InfoBox(string description, params GUILayoutOption[] options)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		GUIStyle style = SAINLayout.GetStyle(Style.alert);
		SAINLayout.Box(new GUIContent("?", description), style, options);
	}

	public static string Toggle(bool value, string on, string off)
	{
		return value ? on : off;
	}

	public static void SingleTextBool(string text, bool value, params GUILayoutOption[] options)
	{
		string text2 = (value ? ": Detected" : ": Not Detected");
		SAINLayout.Box(text + text2, options);
	}
}
