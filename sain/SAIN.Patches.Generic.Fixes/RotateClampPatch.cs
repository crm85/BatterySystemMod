using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Generic.Fixes;

internal class RotateClampPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(Player), "Rotate", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static void PatchPrefix(Player __instance, ref bool ignoreClamp)
	{
		if (__instance != null && __instance.IsAI && __instance.IsSprintEnabled && SAINEnableClass.IsBotInCombat((IPlayer)(object)__instance))
		{
			ignoreClamp = true;
		}
	}
}
