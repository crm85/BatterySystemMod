using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SAIN.Helpers;
using SAIN.SAINComponent.Classes;
using SAIN.SAINComponent.Classes.EnemyClasses;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SAIN.Patches.Shoot.Aim;

internal class AimOffsetPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotAimingClass), "method_13", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool PatchPrefix(BotAimingClass __instance)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		if (!SAINEnableClass.GetSAIN(__instance.botOwner_0, out var sain))
		{
			return true;
		}
		Vector3 realTargetPoint = __instance.botOwner_0.AimingManager.CurrentAiming.RealTargetPoint;
		if (sain.IsCheater)
		{
			__instance.RealTargetPoint = realTargetPoint;
			return false;
		}
		Enemy enemy = sain.Shoot.LastShotEnemy ?? sain.Enemy ?? sain.LastEnemy;
		if (enemy == null)
		{
			return true;
		}
		float float_ = __instance.float_13;
		Vector3 val = __instance.vector3_5;
		Vector3 currentRecoilOffset = sain.Info.WeaponInfo.Recoil.CurrentRecoilOffset;
		Vector3 val2;
		if (__instance.botOwner_0.Settings.FileSettings.Aiming.DIST_TO_SHOOT_NO_OFFSET > enemy.RealDistance)
		{
			val2 = Vector3.zero;
		}
		else
		{
			float num = float_ / enemy.Aim.AimAndScatterMultiplier;
			num = Mathf.Clamp(num, 0f, 3f);
			val2 = __instance.vector3_4 * num;
		}
		if (sain.Info.Profile.IsPMC || sain.Info.Profile.WildSpawnType.IsGoons())
		{
			val = Vector3.zero;
		}
		Vector3 val3 = val + val2 + currentRecoilOffset;
		if (!enemy.IsAI && SAINPlugin.LoadedPreset.GlobalSettings.Look.NotLooking.NotLookingToggle)
		{
			val3 += NotLookingOffset(enemy.EnemyPerson.IPlayer, __instance.botOwner_0);
		}
		Vector3 val4 = realTargetPoint + val3;
		if (SAINPlugin.LoadedPreset.GlobalSettings.General.Debug.Gizmos.DebugDrawAimGizmos && enemy.EnemyPerson.IPlayer.IsYourPlayer)
		{
			Vector3 position = __instance.botOwner_0.WeaponRoot.position;
			DebugGizmos.Line(position, val4, Color.red, 0.02f, 0.25f, taperLine: true);
			DebugGizmos.Sphere(val4, 0.025f, Color.red, 10f);
			DebugGizmos.Line(position, realTargetPoint, Color.white, 0.02f, 0.25f, taperLine: true);
			DebugGizmos.Sphere(realTargetPoint, 0.025f, Color.white, 10f);
		}
		__instance.EndTargetPoint = val4;
		return false;
	}

	private static Vector3 NotLookingOffset(IPlayer person, BotOwner botOwner)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		float spreadIncrease = SAINNotLooking.GetSpreadIncrease(person, botOwner);
		if (spreadIncrease > 0f)
		{
			Vector3 result = Random.insideUnitSphere * spreadIncrease;
			result.y = 0f;
			return result;
		}
		return Vector3.zero;
	}
}
