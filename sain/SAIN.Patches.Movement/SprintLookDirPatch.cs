using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SAIN.Patches.Movement;

public class SprintLookDirPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotMover), "Sprint", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool Patch(BotMover __instance, bool val)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		if (__instance.Sprinting == val)
		{
			return false;
		}
		BotOwner botOwner_ = ((GClass419)__instance).botOwner_0;
		if (__instance.NoSprint)
		{
			__instance._player.EnableSprint(false);
			return false;
		}
		if (val && botOwner_.Mover.HasPathAndNoComplete)
		{
			Vector3 val2 = botOwner_.Mover.RealDestPoint - botOwner_.Position;
			val2.y = 0f;
			((Vector3)(ref val2)).Normalize();
			Vector3 lookDirection = botOwner_.LookDirection;
			lookDirection.y = 0f;
			((Vector3)(ref lookDirection)).Normalize();
			if (Vector3.Angle(lookDirection, val2) > 20f)
			{
				val = false;
			}
		}
		__instance.Sprinting = val;
		if (val)
		{
			botOwner_.SetTargetMoveSpeed(1f);
		}
		__instance._player.EnableSprint(val);
		return false;
	}
}
