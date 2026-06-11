using System;
using System.Reflection;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Movement;

public class GlobalLookPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotGlobalLookData), "Update", (Type[])null, (Type[])null);
	}

	[PatchPostfix]
	public static void PatchPrefix(BotGlobalLookData __instance)
	{
		__instance.SHOOT_FROM_EYES = false;
	}
}
