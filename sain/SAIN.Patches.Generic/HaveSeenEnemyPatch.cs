using System.Reflection;
using HarmonyLib;
using SAIN.SAINComponent.Classes.EnemyClasses;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Generic;

public class HaveSeenEnemyPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.PropertyGetter(typeof(EnemyInfo), "HaveSeen");
	}

	[PatchPostfix]
	public static void PatchPostfix(ref bool __result, EnemyInfo __instance)
	{
		if (!__result && SAINEnableClass.GetSAIN(__instance.Owner, out var sain))
		{
			Enemy enemy = sain.EnemyController.CheckAddEnemy(__instance.Person);
			if (enemy != null && enemy.EnemyKnown)
			{
				__result = true;
			}
		}
	}
}
