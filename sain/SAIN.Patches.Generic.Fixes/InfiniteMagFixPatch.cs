using System;
using System.Reflection;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Generic.Fixes;

internal class InfiniteMagFixPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotReload), "TryUploadMagazine", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool Patch(BotReload __instance)
	{
		return false;
	}
}
