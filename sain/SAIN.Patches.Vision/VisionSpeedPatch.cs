using System;
using System.Reflection;
using HarmonyLib;
using SAIN.SAINComponent.Classes.EnemyClasses;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SAIN.Patches.Vision;

public class VisionSpeedPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(EnemyInfo), "method_7", (Type[])null, (Type[])null);
	}

	[PatchPostfix]
	public static void PatchPostfix(ref float __result, EnemyInfo __instance)
	{
		if (!SAINEnableClass.GetSAIN(__instance.Owner, out var sain))
		{
			return;
		}
		Enemy enemy = sain.EnemyController.GetEnemy(__instance.Person.ProfileId, mustBeActive: true);
		if (enemy != null)
		{
			if (!enemy.Vision.Angles.CanBeSeen)
			{
				__result = 0f;
			}
			else
			{
				__result /= enemy.Vision.GainSightCoef;
			}
			enemy.Vision.LastGainSightResult = __result;
		}
		float minimumVisionSpeed = sain.Info.FileSettings.Look.MinimumVisionSpeed;
		if (minimumVisionSpeed > 0f)
		{
			__result = Mathf.Min(__result, 1f / minimumVisionSpeed);
		}
	}
}
