using System;
using System.Reflection;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Movement;

public class GlobalShootSettingsPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotGlobalShootData), "Update", (Type[])null, (Type[])null);
	}

	[PatchPostfix]
	public static void PatchPrefix(BotGlobalShootData __instance)
	{
		__instance.CAN_STOP_SHOOT_CAUSE_ANIMATOR = false;
		__instance.MAX_DIST_COEF = 100f;
	}
}
