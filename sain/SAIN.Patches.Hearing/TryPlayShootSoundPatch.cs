using System;
using System.Reflection;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Hearing;

public class TryPlayShootSoundPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(GClass567), "TryPlayShootSound", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool PatchPrefix(GClass567 __instance)
	{
		__instance.Boolean_0 = true;
		return false;
	}
}
