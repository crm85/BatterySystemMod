using System;
using System.Reflection;
using HarmonyLib;
using SAIN.SAINComponent.Classes.EnemyClasses;
using SAIN.SAINComponent.Classes.Mover;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SAIN.Patches.Generic.Fixes;

internal class RunToEnemyUpdatePatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotMeleeWeaponData), "RunToEnemyUpdate", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool Patch(BotMeleeWeaponData __instance)
	{
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		if (SAINEnableClass.GetSAIN(((GClass419)__instance).botOwner_0, out var sain) && sain.SAINLayersActive)
		{
			Enemy enemy = sain.Enemy;
			if (enemy == null)
			{
				return false;
			}
			__instance.ShallEndRun = false;
			if (!((GClass419)__instance).botOwner_0.WeaponManager.IsMelee)
			{
				if (!((GClass419)__instance).botOwner_0.WeaponManager.Selector.CanChangeToMeleeWeapons)
				{
					__instance.ShallEndRun = true;
					return false;
				}
				((GClass419)__instance).botOwner_0.WeaponManager.Selector.ChangeToMelee();
			}
			if (((GClass419)__instance).botOwner_0.BotLay.IsLay)
			{
				((GClass419)__instance).botOwner_0.BotLay.GetUp(false);
			}
			sain.Mover.SetTargetPose(1f);
			EnemyInfo enemyInfo = enemy.EnemyInfo;
			bool flag;
			if (flag = enemyInfo.Distance < __instance.Single_0)
			{
				((GClass419)__instance).botOwner_0.Steering.LookToPoint(enemyInfo.BodyData().Key.Position);
				if (enemyInfo.Person.AIData.Player.MovementContext.IsInPronePose)
				{
					sain.Mover.SetTargetPose(0f);
				}
			}
			else
			{
				sain.Mover.SetTargetPose(1f);
				sain.Steering.LookToMovingDirection(sprint: true);
			}
			sain.Mover.Sprint(__instance.Running && enemyInfo.Distance > __instance.Single_1);
			if (__instance._nextTryHitTime < Time.time)
			{
				__instance.method_0((flag && __instance.method_2(enemyInfo)) ? 10f : __instance.TRY_HIT_PERIOD_FALSE);
			}
			if (sain.Mover.PathFollower.Running)
			{
				if (__instance._runPathCheck < Time.time)
				{
					float num = ((!__instance._useZigZag) ? ((enemyInfo.Distance > __instance.FAR_DIST) ? __instance.farRecalc : ((enemyInfo.Distance > __instance.MID_DIST) ? __instance.midRecalc : __instance.closeRecalc)) : ((enemyInfo.Distance > __instance.FAR_DIST) ? __instance.farRecalc : ((enemyInfo.Distance > __instance.MID_DIST) ? __instance.midRecalcZZ : __instance.closeRecalcZZ)));
					__instance._runPathCheck = Time.time + num;
					Vector3[] way = default(Vector3[]);
					if (!__instance.CanRunToEnemyToHit(enemyInfo, ref way))
					{
						__instance.ShallEndRun = true;
						return false;
					}
					if (enemyInfo.Distance < ((GClass419)__instance).botOwner_0.Settings.FileSettings.Shoot.MELEE_STOP_MOVE_DISTANCE)
					{
						sain.Mover.PathFollower.Cancel(0.1f);
					}
					else
					{
						sain.Mover.PathFollower.RunToPointByWay(way, ESprintUrgency.High, stopSprintEnemyVisible: false);
					}
				}
			}
			else
			{
				Vector3[] way2 = default(Vector3[]);
				if (!__instance.CanRunToEnemyToHit(enemyInfo, ref way2))
				{
					__instance.ShallEndRun = true;
					return false;
				}
				if (enemyInfo.Distance < ((GClass419)__instance).botOwner_0.Settings.FileSettings.Shoot.MELEE_STOP_MOVE_DISTANCE)
				{
					sain.Mover.PathFollower.Cancel(0.1f);
				}
				else
				{
					sain.Mover.PathFollower.RunToPointByWay(way2, ESprintUrgency.High, stopSprintEnemyVisible: false);
				}
			}
			return false;
		}
		return false;
	}
}
