using BepInEx.Bootstrap;
using SAIN.Editor;
using SAIN.Helpers;
using UnityEngine;

namespace SAIN;

public static class ModDetection
{
	private static readonly float ModsCheckTimer;

	private static bool ModsChecked;

	public static bool LootingBotsLoaded { get; private set; }

	public static bool RealismLoaded { get; private set; }

	public static bool QuestingBotsLoaded { get; private set; }

	public static bool ProjectFikaLoaded { get; private set; }

	static ModDetection()
	{
		ModsCheckTimer = -1f;
		ModsChecked = false;
		ModsCheckTimer = Time.time + 5f;
	}

	public static void Update()
	{
		if (!ModsChecked && ModsCheckTimer < Time.time && ModsCheckTimer > 0f)
		{
			ModsChecked = true;
			CheckPlugins();
		}
	}

	public static void CheckPlugins()
	{
		if (Chainloader.PluginInfos.ContainsKey("com.fika.core") || Chainloader.PluginInfos.ContainsKey("com.mpt.core"))
		{
			ProjectFikaLoaded = true;
			Logger.LogInfo("SAIN: Project Fika Detected.");
		}
		if (Chainloader.PluginInfos.ContainsKey("com.DanW.QuestingBots"))
		{
			QuestingBotsLoaded = true;
			Logger.LogInfo("SAIN: Questing Bots Detected.");
		}
		if (Chainloader.PluginInfos.ContainsKey("me.skwizzy.lootingbots"))
		{
			LootingBotsLoaded = true;
			Logger.LogInfo("SAIN: Looting Bots Detected.");
		}
		if (Chainloader.PluginInfos.ContainsKey("RealismMod"))
		{
			RealismLoaded = true;
			Logger.LogInfo("SAIN: Realism Detected.");
		}
	}

	public static void UpdateArmorClassCoef()
	{
		if (RealismLoaded)
		{
			EFTCoreSettings.UpdateArmorClassCoef(3.5f);
			Logger.LogInfo("Realism Detected, updating armor class number to reflect new armor classes...");
		}
		else
		{
			EFTCoreSettings.UpdateArmorClassCoef(6f);
		}
	}

	public static void ModDetectionGUI()
	{
		SAINLayout.BeginVertical();
		SAINLayout.BeginHorizontal();
		IsDetected(LootingBotsLoaded, "Looting Bots");
		IsDetected(QuestingBotsLoaded, "Questing Bots");
		IsDetected(RealismLoaded, "Realism Mod");
		SAINLayout.EndHorizontal();
		SAINLayout.EndVertical();
	}

	private static void IsDetected(bool value, string name)
	{
		SAINLayout.Label(name);
		SAINLayout.Box(value ? "Detected" : "Not Detected");
	}
}
