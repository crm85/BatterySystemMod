using System;
using System.Reflection;
using HarmonyLib;
using SAIN.SAINComponent.Classes.EnemyClasses;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Vision;

public class VisionDistancePatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(EnemyInfo), "CheckPartLineOfSight", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static void PatchPrefix(ref float addSensorDistance, EnemyInfo __instance)
	{
		if (!SAINEnableClass.GetSAIN(__instance.Owner, out var sain))
		{
			return;
		}
		Enemy enemy = sain.EnemyController.GetEnemy(__instance.ProfileId, mustBeActive: true);
		if (enemy != null)
		{
			if (!enemy.Vision.Angles.CanBeSeen)
			{
				addSensorDistance = float.MinValue;
			}
			else
			{
				addSensorDistance += enemy.Vision.VisionDistance;
			}
		}
	}
}
