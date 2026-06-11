using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Generic.Fixes;

public class StopSetToNavMeshPatch2 : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(GClass478), "method_18", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool PatchPrefix(GClass478 __instance)
	{
		if (SAINEnableClass.IsBotInCombat((IPlayer)(object)((GClass419)__instance).botOwner_0))
		{
			return false;
		}
		return true;
	}
}
