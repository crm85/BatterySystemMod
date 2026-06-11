using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Talk;

public class BotTalkManualUpdatePatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotTalk), "ManualUpdate", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool PatchPrefix(BotOwner ___botOwner_0)
	{
		return SAINPlugin.LoadedPreset.GlobalSettings.Talk.DisableBotTalkPatching || SAINPlugin.IsBotExluded(___botOwner_0);
	}
}
