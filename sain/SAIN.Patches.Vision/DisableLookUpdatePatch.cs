using System;
using System.Reflection;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Vision;

public class DisableLookUpdatePatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(LookSensor), "CheckAllEnemies", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool Patch(LookSensor __instance)
	{
		return SAINEnableClass.IsBotExcluded(__instance._botOwner);
	}
}
