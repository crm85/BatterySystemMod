using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Generic.Fixes;

internal class EnableVaultPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(Player), "InitVaultingComponent", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static void Patch(Player __instance, ref bool aiControlled)
	{
		if (!__instance.UsedSimplifiedSkeleton)
		{
			aiControlled = false;
		}
	}
}
