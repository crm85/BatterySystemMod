using System.Linq;
using UnityEngine;

namespace SAIN.Editor.Util;

internal class ApplyToStyle
{
	public static void BackgroundAllStates(Texture2D normal, params GUIStyle[] styles)
	{
		BackgroundOn(normal, styles);
		BackgroundNotOn(normal, styles);
	}

	public static void BackgroundAllStates(Texture2D normal, Texture2D active, params GUIStyle[] styles)
	{
		BackgroundOn(normal, styles);
		BackgroundNotOn(active, styles);
	}

	public static void BackgroundNotOn(Texture2D texture, params GUIStyle[] styles)
	{
		for (int i = 0; i < styles.Length; i++)
		{
			Background(texture, styles[i], StyleState.normal, StyleState.hover, StyleState.focused, StyleState.active);
		}
	}

	public static void BackgroundOn(Texture2D texture, params GUIStyle[] styles)
	{
		for (int i = 0; i < styles.Length; i++)
		{
			Background(texture, styles[i], StyleState.onNormal, StyleState.onHover, StyleState.onFocused, StyleState.onActive);
		}
	}

	public static void BackgroundNormal(Texture2D normal, Texture2D onNormal, params GUIStyle[] styles)
	{
		for (int i = 0; i < styles.Length; i++)
		{
			Background(normal, styles[i], default(StyleState));
			Background(onNormal, styles[i], StyleState.onNormal);
		}
	}

	public static void BackgroundActive(Texture2D active, Texture2D onActive, params GUIStyle[] styles)
	{
		for (int i = 0; i < styles.Length; i++)
		{
			Background(active, styles[i], StyleState.active);
			Background(onActive, styles[i], StyleState.onActive);
		}
	}

	public static void BackgroundFocused(Texture2D focused, Texture2D onFocused, params GUIStyle[] styles)
	{
		for (int i = 0; i < styles.Length; i++)
		{
			Background(focused, styles[i], StyleState.focused);
			Background(onFocused, styles[i], StyleState.onFocused);
		}
	}

	public static void BackgroundHover(Texture2D hover, Texture2D onHover, params GUIStyle[] styles)
	{
		for (int i = 0; i < styles.Length; i++)
		{
			Background(hover, styles[i], StyleState.hover);
			Background(onHover, styles[i], StyleState.onHover);
		}
	}

	public static void BackgroundNormal(Texture2D normal, params GUIStyle[] styles)
	{
		BackgroundNormal(normal, normal, styles);
	}

	public static void BackgroundActive(Texture2D active, params GUIStyle[] styles)
	{
		BackgroundActive(active, active, styles);
	}

	public static void BackgroundFocused(Texture2D focused, params GUIStyle[] styles)
	{
		BackgroundFocused(focused, focused, styles);
	}

	public static void BackgroundHover(Texture2D hover, params GUIStyle[] styles)
	{
		BackgroundHover(hover, hover, styles);
	}

	public static void Background(Texture2D texture, GUIStyle style, params StyleState[] states)
	{
		if (states.Contains(StyleState.normal))
		{
			style.normal.background = texture;
		}
		if (states.Contains(StyleState.onNormal))
		{
			style.onNormal.background = texture;
		}
		if (states.Contains(StyleState.active))
		{
			style.active.background = texture;
		}
		if (states.Contains(StyleState.onActive))
		{
			style.onActive.background = texture;
		}
		if (states.Contains(StyleState.hover))
		{
			style.hover.background = texture;
		}
		if (states.Contains(StyleState.onHover))
		{
			style.onHover.background = texture;
		}
		if (states.Contains(StyleState.focused))
		{
			style.focused.background = texture;
		}
		if (states.Contains(StyleState.onFocused))
		{
			style.onFocused.background = texture;
		}
	}

	public static void TextColorAllStates(Color normal, params GUIStyle[] styles)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		TextColorNotOn(normal, styles);
		TextColorOn(normal, styles);
	}

	public static void TextColorAllStates(Color normal, Color active, params GUIStyle[] styles)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		TextColorNotOn(normal, styles);
		TextColorOn(active, styles);
	}

	public static void TextColorNotOn(Color texture, params GUIStyle[] styles)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < styles.Length; i++)
		{
			TextColor(texture, styles[i], StyleState.normal, StyleState.hover, StyleState.focused, StyleState.active);
		}
	}

	public static void TextColorOn(Color texture, params GUIStyle[] styles)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < styles.Length; i++)
		{
			TextColor(texture, styles[i], StyleState.onNormal, StyleState.onHover, StyleState.onFocused, StyleState.onActive);
		}
	}

	public static void TextColorNormal(Color normal, Color onNormal, params GUIStyle[] styles)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < styles.Length; i++)
		{
			TextColor(normal, styles[i], default(StyleState));
			TextColor(onNormal, styles[i], StyleState.onNormal);
		}
	}

	public static void TextColorActive(Color active, Color onActive, params GUIStyle[] styles)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < styles.Length; i++)
		{
			TextColor(active, styles[i], StyleState.active);
			TextColor(onActive, styles[i], StyleState.onActive);
		}
	}

	public static void TextColorFocused(Color focused, Color onFocused, params GUIStyle[] styles)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < styles.Length; i++)
		{
			TextColor(focused, styles[i], StyleState.focused);
			TextColor(onFocused, styles[i], StyleState.onFocused);
		}
	}

	public static void TextColorHover(Color hover, Color onHover, params GUIStyle[] styles)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < styles.Length; i++)
		{
			TextColor(hover, styles[i], StyleState.hover);
			TextColor(onHover, styles[i], StyleState.onHover);
		}
	}

	public static void TextColorNormal(Color normal, params GUIStyle[] styles)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		TextColorNormal(normal, normal, styles);
	}

	public static void TextColorActive(Color active, params GUIStyle[] styles)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		TextColorActive(active, active, styles);
	}

	public static void TextColorFocused(Color focused, params GUIStyle[] styles)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		TextColorFocused(focused, focused, styles);
	}

	public static void TextColorHover(Color hover, params GUIStyle[] styles)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		TextColorHover(hover, hover, styles);
	}

	public static void TextColor(Color color, GUIStyle style, params StyleState[] states)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		if (states.Contains(StyleState.normal))
		{
			style.normal.textColor = color;
		}
		if (states.Contains(StyleState.onNormal))
		{
			style.onNormal.textColor = color;
		}
		if (states.Contains(StyleState.active))
		{
			style.active.textColor = color;
		}
		if (states.Contains(StyleState.onActive))
		{
			style.onActive.textColor = color;
		}
		if (states.Contains(StyleState.hover))
		{
			style.hover.textColor = color;
		}
		if (states.Contains(StyleState.onHover))
		{
			style.onHover.textColor = color;
		}
		if (states.Contains(StyleState.focused))
		{
			style.focused.textColor = color;
		}
		if (states.Contains(StyleState.onFocused))
		{
			style.onFocused.textColor = color;
		}
	}
}
