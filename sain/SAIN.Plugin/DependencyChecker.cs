using System;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;

namespace SAIN.Plugin;

internal class DependencyChecker
{
	internal sealed class ConfigurationManagerAttributes
	{
		public delegate void CustomHotkeyDrawerFunc(ConfigEntryBase setting, ref bool isCurrentlyAcceptingInput);

		public bool? ShowRangeAsPercent;

		public Action<ConfigEntryBase> CustomDrawer;

		public CustomHotkeyDrawerFunc CustomHotkeyDrawer;

		public bool? Browsable;

		public string Category;

		public object DefaultValue;

		public bool? HideDefaultButton;

		public bool? HideSettingName;

		public string Description;

		public string DispName;

		public int? Order;

		public bool? ReadOnly;

		public bool? IsAdvanced;

		public Func<object, string> ObjToStr;

		public Func<string, object> StrToObj;
	}

	public static bool ValidateDependencies(ManualLogSource Logger, PluginInfo Info, Type pluginType, ConfigFile Config = null)
	{
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Expected O, but got Unknown
		Version version = new Version("0.0.0");
		BepInDependency[] array = pluginType.GetCustomAttributes(typeof(BepInDependency), inherit: true) as BepInDependency[];
		BepInDependency[] array2 = array;
		int num = 0;
		while (num < array2.Length)
		{
			BepInDependency val = array2[num];
			if (!Chainloader.PluginInfos.TryGetValue(val.DependencyGUID, out var value))
			{
				value = null;
			}
			if (value != null)
			{
				BaseUnityPlugin instance = value.Instance;
				if (instance == null || ((Behaviour)instance).enabled)
				{
					num++;
					continue;
				}
			}
			string text = ((value != null) ? value.Metadata.Name : null) ?? val.DependencyGUID;
			string text2 = "";
			if (val.MinimumVersion > version)
			{
				text2 = $" v{val.MinimumVersion}";
			}
			string text3 = $"ERROR: This version of {Info.Metadata.Name} v{Info.Metadata.Version} depends on {text}{text2}, but it was not loaded.";
			Logger.LogError((object)text3);
			Chainloader.DependencyErrors.Add(text3);
			if (Config != null)
			{
				Config.Bind<string>("", "MissingDeps", "", new ConfigDescription(text3, (AcceptableValueBase)null, new object[1]
				{
					new ConfigurationManagerAttributes
					{
						CustomDrawer = ErrorLabelDrawer,
						ReadOnly = true,
						HideDefaultButton = true,
						HideSettingName = true,
						Category = null
					}
				}));
			}
			return false;
		}
		return true;
	}

	private static void ErrorLabelDrawer(ConfigEntryBase entry)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		GUIStyle val = new GUIStyle(GUI.skin.label);
		val.wordWrap = true;
		val.stretchWidth = true;
		GUIStyle val2 = new GUIStyle(GUI.skin.label);
		val2.stretchWidth = true;
		val2.alignment = (TextAnchor)4;
		val2.normal.textColor = Color.red;
		val2.fontStyle = (FontStyle)1;
		GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
		GUILayout.Label(entry.Description.Description, val, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.ExpandWidth(true) });
		GUILayout.Label("Plugin has been disabled!", val2, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.ExpandWidth(true) });
		GUILayout.EndVertical();
	}
}
