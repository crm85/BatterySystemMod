using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Generic;

public class SetPanicPointPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotMemoryClass), "SetPanicPoint", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool PatchPrefix(BotOwner ___botOwner_0)
	{
		return SAINPlugin.IsBotExluded(___botOwner_0);
	}
}
