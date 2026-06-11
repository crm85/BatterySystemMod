using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SAIN.Patches.Hearing;

public class HearingSensorPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotHearingSensor), "method_0", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool PatchPrefix(BotOwner ____botOwner)
	{
		if ((Object)(object)____botOwner == (Object)null || (Object)(object)____botOwner.GetPlayer == (Object)null)
		{
			return false;
		}
		if (!SAINPlugin.IsBotExluded(____botOwner))
		{
			return false;
		}
		return true;
	}
}
