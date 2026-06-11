using System.Collections.Generic;
using UnityEngine;

namespace SAIN.Editor.Util;

public static class ColorsClass
{
	private static readonly Dictionary<string, Color> RandomColors = new Dictionary<string, Color>();

	public static readonly string SchemeName;

	public static readonly Dictionary<ColorNames, Color> ColorSchemeDictionary = new Dictionary<ColorNames, Color>();

	public static readonly Dictionary<EGraynessLevel, Color> GrayColorScheme = new Dictionary<EGraynessLevel, Color>();

	private static float Randomize => Random.Range(0.81f, 1.21f);

	public static void CreateCache()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		if (ColorSchemeDictionary.Count == 0)
		{
			AddColor(ColorNames.White, Color.white);
			AddColor(ColorNames.Black, Color.black);
			AddColor(ColorNames.Clear, Color.clear);
			AddColor(ColorNames.LightRed, new Color(0.8f, 0.35f, 0.35f, 0.85f));
			AddColor(ColorNames.MidRed, new Color(0.7f, 0.25f, 0.25f, 0.85f));
			AddColor(ColorNames.DarkRed, new Color(0.6f, 0.15f, 0.15f, 0.85f));
			AddColor(ColorNames.VeryDarkRed, new Color(0.8f, 0.35f, 0.35f, 0.85f));
			AddColor(ColorNames.LightBlue, new Color(0.4f, 0.4f, 0.9f));
			AddColor(ColorNames.MidBlue, new Color(0.3f, 0.3f, 0.8f));
			AddColor(ColorNames.DarkBlue, new Color(0.2f, 0.2f, 0.6f));
			AddColor(ColorNames.VeryDarkBlue, new Color(0.1f, 0.1f, 0.5f));
			AddColor(ColorNames.Gold, new Color(0.9f, 0.8f, 0f, 0.9f));
			AddColor(EGraynessLevel.VeryLight, Gray(0.35f));
			AddColor(EGraynessLevel.Light, Gray(0.275f));
			AddColor(EGraynessLevel.BrightMid, Gray(0.225f));
			AddColor(EGraynessLevel.Mid, Gray(0.175f));
			AddColor(EGraynessLevel.DarkMid, Gray(0.125f));
			AddColor(EGraynessLevel.Dark, Gray(0.1f));
			AddColor(EGraynessLevel.Darker, Gray(0.055f));
			AddColor(EGraynessLevel.VeryDark, Gray(0.04f));
			AddColor(EGraynessLevel.AlmostBlack, Gray(0.02f));
		}
	}

	public static Color GetRandomColor(string key)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (!RandomColors.ContainsKey(key))
		{
			RandomColors.Add(key, CreateRandom());
		}
		return RandomColors[key];
	}

	private static Color CreateRandom()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		float num = Random.Range(0.3f, 2f) * 0.151f;
		return new Color(num * Randomize, num, num);
	}

	public static Color GetColor(ColorNames name)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (ColorSchemeDictionary.ContainsKey(name))
		{
			return ColorSchemeDictionary[name];
		}
		return Color.green;
	}

	public static Color GetColor(EGraynessLevel level)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (GrayColorScheme.ContainsKey(level))
		{
			return GrayColorScheme[level];
		}
		return Color.green;
	}

	public static void AddColor(EGraynessLevel name, Color color)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (!GrayColorScheme.ContainsKey(name))
		{
			GrayColorScheme.Add(name, color);
		}
		else
		{
			GrayColorScheme[name] = color;
		}
	}

	public static void AddColor(ColorNames name, Color color)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (!ColorSchemeDictionary.ContainsKey(name))
		{
			ColorSchemeDictionary.Add(name, color);
		}
		else
		{
			ColorSchemeDictionary[name] = color;
		}
	}

	private static Color Gray(float brightness)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return new Color(brightness, brightness, brightness);
	}
}
