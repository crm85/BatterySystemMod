using System;
using System.Reflection;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Vision;

public class WeatherVisionPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(EnemyInfo), "method_8", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool PatchPrefix(EnemyInfo __instance, ref float __result)
	{
		if (SAINEnableClass.IsBotExcluded(__instance.Owner))
		{
			return true;
		}
		__result = 1f;
		return false;
	}
}
