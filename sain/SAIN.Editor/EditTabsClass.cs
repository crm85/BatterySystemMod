using System.Collections.Generic;
using UnityEngine;

namespace SAIN.Editor;

internal class EditTabsClass
{
	private const float TabMenuHeight = 60f;

	private const float TabMenuVerticalMargin = 2f;

	private static Rect[] TabRects;

	public static Rect TabMenuRect;

	public static EEditorTab SelectedTab;

	public static readonly string[] Tabs;

	public static readonly string[] TabTooltips;

	public static readonly Dictionary<EEditorTab, TabClass> TabClasses;

	static EditTabsClass()
	{
		SelectedTab = EEditorTab.Home;
		TabClasses = new Dictionary<EEditorTab, TabClass>
		{
			{
				EEditorTab.Home,
				new TabClass
				{
					Name = "Home",
					ToolTip = "Select preset and modify global SAIN settings."
				}
			},
			{
				EEditorTab.BotSettings,
				new TabClass
				{
					Name = "Bot Settings",
					ToolTip = "Modify Settings that are unique to particular bot types for individual difficulties. Difficulty is determined on spawn by EFT, and is changed by selecting the Difficulty value when starting a raid. As Online is a mix of all difficulties."
				}
			},
			{
				EEditorTab.Personalities,
				new TabClass
				{
					Name = "Personalities",
					ToolTip = "Modify Individual Personality settings for how they are assigned to bots, and what each personality does for a bot's behavior."
				}
			},
			{
				EEditorTab.EquipmentStealth,
				new TabClass
				{
					Name = "Equipment Stealth",
					ToolTip = "Modify the stealth value that certain pieces of equipment provide."
				}
			},
			{
				EEditorTab.Advanced,
				new TabClass
				{
					Name = "Advanced Options",
					ToolTip = "Edit at your own risk. Enable additional advanced config options here"
				}
			}
		};
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		foreach (KeyValuePair<EEditorTab, TabClass> tabClass in TabClasses)
		{
			list.Add(tabClass.Value.Name);
			list2.Add(tabClass.Value.ToolTip);
		}
		Tabs = list.ToArray();
		TabTooltips = list2.ToArray();
	}

	public static EEditorTab TabSelectMenu(float minHeight = 30f, float speed = 3f, float closeSpeedMulti = 0.66f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		_ = TabMenuRect;
		if (TabRects == null)
		{
			TabMenuRect = new Rect(0f, ((Rect)(ref RectLayout.ExitRect)).height + 2f, ((Rect)(ref RectLayout.MainWindow)).width, 60f);
			TabRects = BuilderClass.HorizontalGridRects(TabMenuRect, Tabs.Length, minHeight);
		}
		string text = BuilderClass.SelectionGridExpandHeight(TabMenuRect, Tabs, TabClasses[SelectedTab].Name, TabRects, minHeight, speed, closeSpeedMulti, TabTooltips);
		foreach (KeyValuePair<EEditorTab, TabClass> tabClass in TabClasses)
		{
			if (tabClass.Value.Name == text)
			{
				SelectedTab = tabClass.Key;
			}
		}
		return SelectedTab;
	}

	public static void BeginScrollView()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		TabClasses[SelectedTab].Scroll = SAINLayout.BeginScrollView(TabClasses[SelectedTab].Scroll, ((Rect)(ref RectLayout.MainWindow)).width - 20f);
		SAINLayout.BeginVertical();
	}

	public static void EndScrollView()
	{
		SAINLayout.EndVertical();
		SAINLayout.EndScrollView();
	}

	public static bool IsTabSelected(EEditorTab tab)
	{
		return SelectedTab == tab;
	}
}
