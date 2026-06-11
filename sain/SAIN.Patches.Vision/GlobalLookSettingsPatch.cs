using System;
using System.Reflection;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Vision;

public class GlobalLookSettingsPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotGlobalLookData), "Update", (Type[])null, (Type[])null);
	}

	[PatchPostfix]
	public static void Patch(BotGlobalLookData __instance)
	{
		__instance.CHECK_HEAD_ANY_DIST = true;
		__instance.MIDDLE_DIST_CAN_SHOOT_HEAD = true;
		__instance.SHOOT_FROM_EYES = false;
	}
}
