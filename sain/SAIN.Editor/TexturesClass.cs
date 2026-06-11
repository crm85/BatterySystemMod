using System.Collections.Generic;
using SAIN.Editor.Util;
using UnityEngine;

namespace SAIN.Editor;

public static class TexturesClass
{
	private static readonly Dictionary<string, Texture2D> RandomColors = new Dictionary<string, Texture2D>();

	public static readonly Dictionary<string, Texture2D> ColorTextures = new Dictionary<string, Texture2D>();

	public static readonly Dictionary<string, Texture2D> CustomTextures = new Dictionary<string, Texture2D>();

	public static void CreateCache()
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		if (ColorTextures.Count != 0)
		{
			return;
		}
		foreach (KeyValuePair<ColorNames, Color> item in ColorsClass.ColorSchemeDictionary)
		{
			if (item.Key == ColorNames.Clear)
			{
				ColorTextures.Add(item.Key.ToString(), null);
			}
			else
			{
				ColorTextures.Add(item.Key.ToString(), NewTexture(item.Value));
			}
		}
		foreach (KeyValuePair<EGraynessLevel, Color> item2 in ColorsClass.GrayColorScheme)
		{
			ColorTextures.Add(item2.Key.ToString(), NewTexture(item2.Value));
		}
	}

	public static Texture2D GetRandomGray(string key)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (!RandomColors.ContainsKey(key))
		{
			Texture2D value = NewTexture(ColorsClass.GetRandomColor(key));
			RandomColors.Add(key, value);
		}
		return RandomColors[key];
	}

	public static Texture2D GetTexture<T>(T name)
	{
		if (ColorTextures.TryGetValue(name.ToString(), out var value))
		{
			return value;
		}
		return Texture2D.redTexture;
	}

	public static Texture2D GetCustom(ColorNames name)
	{
		if (CustomTextures.TryGetValue(name.ToString(), out var value))
		{
			return value;
		}
		return Texture2D.redTexture;
	}

	public static Texture2D NewTexture(Color color, int width = 2, int height = 2)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Expected O, but got Unknown
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		Texture2D val = new Texture2D(width, height);
		Color[] array = (Color[])(object)new Color[((Texture)val).width * ((Texture)val).height];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = color;
		}
		val.SetPixels(array);
		val.Apply();
		return val;
	}

	public static Rect DrawSliderBackGrounds(float progressRatio, Rect lastRect)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		float num = 5f;
		bool flag = MouseFunctions.IsMouseInside(lastRect);
		drawTexture(lastRect, flag ? EGraynessLevel.Dark : EGraynessLevel.Darker, null);
		Rect rect = lastRect;
		((Rect)(ref rect)).height = num;
		((Rect)(ref rect)).center = ((Rect)(ref lastRect)).center;
		drawTexture(rect, flag ? EGraynessLevel.Mid : EGraynessLevel.Dark, null);
		Rect val = lastRect;
		((Rect)(ref val)).height = num * 2f;
		((Rect)(ref val)).center = ((Rect)(ref lastRect)).center;
		((Rect)(ref val)).width = Mathf.Lerp(0f, ((Rect)(ref lastRect)).width, progressRatio);
		drawTexture(val, null, flag ? ColorNames.MidRed : ColorNames.DarkRed);
		drawSliderThumb(val, lastRect, flag, progressRatio);
		return lastRect;
	}

	private static void drawSliderThumb(Rect Filled, Rect lastRect, bool mouseInsideSlider, float progressRatio)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		Rect val = lastRect;
		((Rect)(ref val)).width = 12f;
		((Rect)(ref val)).x = Mathf.Lerp(((Rect)(ref lastRect)).x, ((Rect)(ref lastRect)).x + ((Rect)(ref lastRect)).width, progressRatio);
		if (((Rect)(ref val)).x + ((Rect)(ref val)).width > ((Rect)(ref lastRect)).x + ((Rect)(ref lastRect)).width)
		{
			((Rect)(ref val)).x = ((Rect)(ref lastRect)).x + ((Rect)(ref lastRect)).width - ((Rect)(ref val)).width;
		}
		getThumbColor(mouseInsideSlider, val, out var gray, out var colorName);
		drawTexture(val, gray, colorName);
	}

	private static void getThumbColor(bool mouseInSlider, Rect Thumb, out EGraynessLevel? gray, out ColorNames? color)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (MouseFunctions.IsMouseInside(Thumb))
		{
			color = null;
			gray = EGraynessLevel.VeryLight;
		}
		else if (mouseInSlider)
		{
			color = null;
			gray = EGraynessLevel.BrightMid;
		}
		else
		{
			color = null;
			gray = EGraynessLevel.DarkMid;
		}
	}

	private static Color color(ColorNames name)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		return ColorsClass.GetColor(name);
	}

	private static Color color(EGraynessLevel name)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		return ColorsClass.GetColor(name);
	}

	private static void drawTexture(Rect rect, EGraynessLevel? gray, ColorNames? colorName)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (gray.HasValue)
		{
			drawTexture(rect, color(gray.Value));
		}
		else if (colorName.HasValue)
		{
			drawTexture(rect, color(colorName.Value));
		}
	}

	private static void drawTexture(Rect rect, Color color)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		GUI.DrawTexture(rect, (Texture)(object)Texture2D.whiteTexture, (ScaleMode)0, true, 0f, color, 0f, 0f);
	}
}
