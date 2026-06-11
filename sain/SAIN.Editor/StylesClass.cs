using System.Collections.Generic;
using SAIN.Editor.Util;
using SAIN.Helpers;
using UnityEngine;

namespace SAIN.Editor;

public static class StylesClass
{
	private sealed class DynamicStyle
	{
		public GUIStyle Normal;

		public GUIStyle Active;
	}

	private static readonly Dictionary<Style, GUIStyle> Styles = new Dictionary<Style, GUIStyle>();

	private static readonly Dictionary<Style, DynamicStyle> DynamicStyles = new Dictionary<Style, DynamicStyle>();

	public static void CreateCache()
	{
		if (Styles.Count == 0)
		{
			CreateStyles();
		}
	}

	public static GUIStyle GetStyle(Style key)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		if (!Styles.ContainsKey(key))
		{
			Styles.Add(key, new GUIStyle(GUI.skin.box));
		}
		return Styles[key];
	}

	public static GUIStyle GetFontStyleDynamic(Style key, bool active)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (!DynamicStyles.ContainsKey(key))
		{
			GUIStyle style = GetStyle(key);
			GUIStyle val = new GUIStyle(style)
			{
				fontStyle = (FontStyle)0,
				alignment = (TextAnchor)4
			};
			GUIStyle val2 = new GUIStyle(style)
			{
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4
			};
			Color color = ColorsClass.GetColor(ColorNames.Gold);
			ApplyToStyle.TextColorAllStates(Color.white, val);
			ApplyToStyle.TextColorAllStates(color, val2);
			DynamicStyles.Add(key, new DynamicStyle
			{
				Normal = val,
				Active = val2
			});
		}
		DynamicStyle dynamicStyle = DynamicStyles[key];
		return active ? dynamicStyle.Active : dynamicStyle.Normal;
	}

	private static void CreateStyles()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Expected O, but got Unknown
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Expected O, but got Unknown
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Expected O, but got Unknown
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Expected O, but got Unknown
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Expected O, but got Unknown
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Expected O, but got Unknown
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Expected O, but got Unknown
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Expected O, but got Unknown
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Expected O, but got Unknown
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Expected O, but got Unknown
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Expected O, but got Unknown
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Expected O, but got Unknown
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Expected O, but got Unknown
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Expected O, but got Unknown
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Expected O, but got Unknown
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Expected O, but got Unknown
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Expected O, but got Unknown
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Expected O, but got Unknown
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Expected O, but got Unknown
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Expected O, but got Unknown
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f2: Expected O, but got Unknown
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_056f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0713: Unknown result type (might be due to invalid IL or missing references)
		//IL_0718: Unknown result type (might be due to invalid IL or missing references)
		//IL_0720: Unknown result type (might be due to invalid IL or missing references)
		//IL_072a: Expected O, but got Unknown
		//IL_0746: Unknown result type (might be due to invalid IL or missing references)
		//IL_074b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0753: Unknown result type (might be due to invalid IL or missing references)
		//IL_075a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0764: Expected O, but got Unknown
		//IL_0767: Expected O, but got Unknown
		//IL_0768: Unknown result type (might be due to invalid IL or missing references)
		//IL_076d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0775: Unknown result type (might be due to invalid IL or missing references)
		//IL_077f: Expected O, but got Unknown
		//IL_0780: Unknown result type (might be due to invalid IL or missing references)
		//IL_0785: Unknown result type (might be due to invalid IL or missing references)
		//IL_078d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0797: Expected O, but got Unknown
		//IL_0798: Unknown result type (might be due to invalid IL or missing references)
		//IL_079d: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07af: Expected O, but got Unknown
		GUIStyle val = new GUIStyle(GUI.skin.label)
		{
			padding = new RectOffset(4, 4, 0, 0),
			margin = new RectOffset(4, 4, 4, 4),
			border = new RectOffset(4, 4, 0, 0),
			alignment = (TextAnchor)3,
			fontStyle = (FontStyle)0
		};
		GUIStyle val2 = new GUIStyle(GUI.skin.button)
		{
			padding = val.padding,
			margin = val.margin,
			border = val.border,
			overflow = val.overflow,
			contentOffset = val.contentOffset,
			alignment = (TextAnchor)3,
			fontStyle = (FontStyle)0
		};
		GUIStyle val3 = new GUIStyle(GUI.skin.box)
		{
			padding = val.padding,
			margin = val.margin,
			border = val.border,
			overflow = val.overflow,
			contentOffset = val.contentOffset,
			alignment = (TextAnchor)3,
			fontStyle = (FontStyle)0
		};
		GUIStyle val4 = new GUIStyle(GUI.skin.toggle)
		{
			padding = val.padding,
			margin = val.margin,
			border = val.border,
			overflow = val.overflow,
			contentOffset = val.contentOffset,
			alignment = (TextAnchor)3,
			fontStyle = (FontStyle)0
		};
		GUIStyle val5 = new GUIStyle(GUI.skin.textArea)
		{
			padding = val.padding,
			margin = val.margin,
			border = val.border,
			overflow = val.overflow,
			contentOffset = val.contentOffset,
			alignment = (TextAnchor)3,
			fontStyle = (FontStyle)0
		};
		GUIStyle val6 = new GUIStyle(GUI.skin.textField)
		{
			padding = val.padding,
			margin = val.margin,
			border = val.border,
			overflow = val.overflow,
			contentOffset = val.contentOffset,
			alignment = (TextAnchor)3,
			fontStyle = (FontStyle)0
		};
		GUIStyle val7 = new GUIStyle(GUI.skin.scrollView)
		{
			padding = val.padding,
			margin = val.margin,
			border = val.border,
			overflow = val.overflow,
			contentOffset = val.contentOffset
		};
		GUIStyle val8 = new GUIStyle(GUI.skin.window);
		GUIStyle val9 = new GUIStyle(GUI.skin.verticalScrollbarDownButton);
		GUIStyle val10 = new GUIStyle(GUI.skin.verticalScrollbar);
		GUIStyle val11 = new GUIStyle(GUI.skin.verticalScrollbarThumb);
		GUIStyle val12 = new GUIStyle(GUI.skin.verticalScrollbarUpButton);
		GUIStyle val13 = new GUIStyle(GUI.skin.horizontalSlider)
		{
			padding = val.padding,
			margin = val.margin,
			border = val.border,
			overflow = val.overflow,
			contentOffset = val.contentOffset,
			alignment = (TextAnchor)3,
			fontStyle = (FontStyle)0
		};
		GUIStyle val14 = new GUIStyle(GUI.skin.horizontalSliderThumb)
		{
			padding = val.padding,
			margin = val.margin,
			border = val.border,
			overflow = val.overflow,
			contentOffset = val.contentOffset,
			alignment = (TextAnchor)3,
			fontStyle = (FontStyle)0
		};
		GUIStyle value = new GUIStyle(GUI.skin.verticalSlider);
		GUIStyle value2 = new GUIStyle(GUI.skin.verticalSliderThumb);
		GUIStyle val15 = new GUIStyle(GUI.skin.toggle)
		{
			padding = val.padding,
			margin = val.margin,
			border = val.border,
			overflow = val.overflow,
			contentOffset = val.contentOffset,
			alignment = (TextAnchor)3,
			fontStyle = (FontStyle)0
		};
		GUIStyle val16 = new GUIStyle(GUI.skin.box)
		{
			padding = new RectOffset(4, 4, 4, 4),
			border = new RectOffset(4, 4, 4, 4),
			wordWrap = true,
			clipping = (TextClipping)1,
			alignment = (TextAnchor)3,
			fontStyle = (FontStyle)0
		};
		GUIStyle val17 = new GUIStyle(val)
		{
			alignment = (TextAnchor)3,
			fontStyle = (FontStyle)0
		};
		GUIStyle val18 = new GUIStyle(GUI.skin.box)
		{
			padding = val.padding,
			margin = val.margin,
			border = val.border,
			overflow = val.overflow,
			contentOffset = val.contentOffset,
			alignment = (TextAnchor)4,
			fontStyle = (FontStyle)1
		};
		Texture2D texture = TexturesClass.GetTexture(EGraynessLevel.Mid);
		Texture2D texture2 = TexturesClass.GetTexture(EGraynessLevel.Dark);
		Texture2D texture3 = TexturesClass.GetTexture(EGraynessLevel.VeryDark);
		Texture2D texture4 = TexturesClass.GetTexture(ColorNames.MidRed);
		Texture2D texture5 = TexturesClass.GetTexture(ColorNames.DarkRed);
		Color color = ColorsClass.GetColor(ColorNames.Gold);
		Color normal = default(Color);
		((Color)(ref normal))._002Ector(0.9f, 0.9f, 0.9f, 0.9f);
		ApplyToStyle.TextColorAllStates(normal, color, val8, val15, val2, val4, val6, val5, val18);
		ApplyToStyle.TextColorHover(Color.white, val8, val15, val2, val4, val6, val5, val18);
		ApplyToStyle.BackgroundAllStates(null, val17);
		ApplyToStyle.TextColorAllStates(normal, val17, val16, val3, val);
		ApplyToStyle.TextColorHover(Color.white, val17, val16, val3, val);
		ApplyToStyle.BackgroundAllStates(texture4, val18);
		ApplyToStyle.BackgroundAllStates(texture, texture5, val15);
		GUIStyle[] styles = (GUIStyle[])(object)new GUIStyle[2] { val4, val2 };
		ApplyToStyle.BackgroundNormal(texture, texture5, styles);
		ApplyToStyle.BackgroundActive(texture4, texture5, styles);
		ApplyToStyle.BackgroundHover(texture4, texture5, styles);
		ApplyToStyle.BackgroundFocused(texture4, texture5, styles);
		ApplyToStyle.BackgroundAllStates(texture2, texture, val6, val5);
		ApplyToStyle.BackgroundAllStates(null, val13, val14, val7);
		ApplyToStyle.BackgroundAllStates(texture3, val8);
		ApplyToStyle.BackgroundAllStates(texture2, val10, val12, val9, val3, val, val16);
		ApplyToStyle.BackgroundHover(TexturesClass.GetTexture(EGraynessLevel.DarkMid), val10, val12, val9, val3, val, val16);
		ApplyToStyle.BackgroundAllStates(texture5, val11);
		GUIStyle value3 = new GUIStyle(val4)
		{
			alignment = (TextAnchor)4,
			fontStyle = (FontStyle)0
		};
		val.margin = val3.margin;
		val.padding = val3.margin;
		GUIStyle value4 = new GUIStyle(val17)
		{
			alignment = (TextAnchor)3,
			padding = new RectOffset(10, 10, 3, 3)
		};
		GUIStyle value5 = new GUIStyle(val4)
		{
			fontStyle = (FontStyle)0,
			alignment = (TextAnchor)3
		};
		GUIStyle value6 = new GUIStyle(val4)
		{
			fontStyle = (FontStyle)0,
			alignment = (TextAnchor)3
		};
		GUIStyle value7 = new GUIStyle(val4)
		{
			alignment = (TextAnchor)3,
			fontStyle = (FontStyle)0
		};
		Styles.Add(Style.botTypeSection, value7);
		Styles.Add(Style.scrollView, val7);
		Styles.Add(Style.selectionList, value6);
		Styles.Add(Style.alert, val18);
		Styles.Add(Style.botTypeGrid, value5);
		Styles.Add(Style.dragBar, value4);
		Styles.Add(Style.selectionGrid, value3);
		Styles.Add(Style.horizontalSliderThumb, val14);
		Styles.Add(Style.button, val2);
		Styles.Add(Style.box, val3);
		Styles.Add(Style.toggle, val4);
		Styles.Add(Style.textField, val6);
		Styles.Add(Style.textArea, val5);
		Styles.Add(Style.window, val8);
		Styles.Add(Style.verticalScrollbarUpButton, val12);
		Styles.Add(Style.verticalScrollbarThumb, val11);
		Styles.Add(Style.verticalScrollbar, val10);
		Styles.Add(Style.verticalScrollbarDownButton, val9);
		Styles.Add(Style.horizontalSlider, val13);
		Styles.Add(Style.label, val);
		Styles.Add(Style.list, val15);
		Styles.Add(Style.verticalSlider, value);
		Styles.Add(Style.verticalSliderThumb, value2);
		Styles.Add(Style.blankbox, val17);
		Styles.Add(Style.tooltip, val16);
		Style[] array = EnumValues.GetEnum<Style>();
		foreach (Style key in array)
		{
			if (!Styles.ContainsKey(key))
			{
			}
		}
	}
}
