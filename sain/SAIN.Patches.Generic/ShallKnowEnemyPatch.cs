using System;
using System.Reflection;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Generic;

internal class ShallKnowEnemyPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(EnemyInfo), "ShallISuppress", (Type[])null, (Type[])null);
	}

	[PatchPostfix]
	public static void PatchPostfix(EnemyInfo __instance, ref bool __result)
	{
		if (SAINEnableClass.GetSAIN(__instance.Owner, out var sain))
		{
			__result = sain.EnemyController.CheckAddEnemy(__instance.Person)?.EnemyKnown ?? false;
		}
	}
}
