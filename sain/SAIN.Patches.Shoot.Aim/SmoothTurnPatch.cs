using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SAIN.Components;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SAIN.Patches.Shoot.Aim;

public class SmoothTurnPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotSteering), "Steering", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool Patch(BotSteering __instance)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected I4, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		BotOwner botOwner_ = ((GClass419)__instance).botOwner_0;
		if (!GameWorldComponent.TryGetPlayerComponent((IPlayer)(object)botOwner_, out var PlayerComponent))
		{
			return true;
		}
		Vector3 val;
		if (botOwner_.Mover.Sprinting && botOwner_.Mover.HasPathAndNoComplete)
		{
			val = CalcLookPoint(botOwner_, botOwner_.Mover.RealDestPoint);
		}
		else
		{
			EBotSteering steeringMode = __instance.SteeringMode;
			EBotSteering val2 = steeringMode;
			val = (Vector3)((int)val2 switch
			{
				0 => (!botOwner_.Destination.HasValue) ? __instance.LookDirection : CalcLookPoint(botOwner_, botOwner_.Destination.Value), 
				1 => __instance.CanSteerToMovingDirection() ? CalcLookPoint(botOwner_, botOwner_.Mover.RealDestPoint) : __instance._customDirection, 
				2 => __instance._customPoint - botOwner_.WeaponRoot.position, 
				3 => __instance._customDirection, 
				_ => __instance._customDirection, 
			});
		}
		if (Mathf.Abs(val.y) < 0.001f)
		{
			val.y = 0f;
		}
		PlayerComponent.SmoothController.SetTargetLookDirection(val);
		__instance._lookDirection = PlayerComponent.SmoothController.CurrentControlLookDirection;
		__instance.Speed = float.MaxValue;
		__instance.SetXAngle(float.MaxValue);
		__instance.SetYByDir(__instance._lookDirection);
		return false;
	}

	private static Vector3 CalcLookPoint(BotOwner botOwner, Vector3 point)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = botOwner.Position;
		Vector3 position2 = botOwner.WeaponRoot.position;
		float num = position2.y - position.y;
		point.y += num;
		return point - position2;
	}
}
