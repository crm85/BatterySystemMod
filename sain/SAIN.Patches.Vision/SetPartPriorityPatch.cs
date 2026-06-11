using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SAIN.SAINComponent.Classes.EnemyClasses;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SAIN.Patches.Vision;

public class SetPartPriorityPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(EnemyInfo), "method_1", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool PatchPrefix(EnemyInfo __instance)
	{
		IPlayer person = __instance.Person;
		bool flag = person != null && person.IsAI;
		if (flag)
		{
			if (!__instance.HaveSeenPersonal || Time.time - __instance.TimeLastSeenReal > 5f)
			{
				__instance.SetFarParts();
			}
			else
			{
				__instance.SetMiddleParts();
			}
			return false;
		}
		if (!flag && SAINEnableClass.GetSAIN(__instance.Owner, out var sain))
		{
			Enemy enemy = sain.EnemyController.CheckAddEnemy(__instance.Person);
			if (enemy != null)
			{
				if (enemy.IsCurrentEnemy)
				{
					__instance.SetCloseParts();
					return false;
				}
				if (enemy.Status.ShotAtMeRecently || enemy.Status.PositionalFlareEnabled)
				{
					__instance.SetCloseParts();
					return false;
				}
			}
		}
		return true;
	}
}
