using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Generic;

public class StopRefillMagsPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotReload), "method_1", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool Patch(BotOwner ___botOwner_0)
	{
		return SAINPlugin.IsBotExluded(___botOwner_0);
	}
}
