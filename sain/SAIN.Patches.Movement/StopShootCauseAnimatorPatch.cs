using System;
using System.Reflection;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Movement;

public class StopShootCauseAnimatorPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(ShootData), "method_1", (Type[])null, (Type[])null);
	}

	[PatchPostfix]
	public static bool PatchPostfix(EPlayerState nextstate, ShootData __instance)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected I4, but got Unknown
		switch (nextstate - 6)
		{
		case 0:
		case 1:
		case 5:
		case 6:
		case 7:
		case 8:
		case 10:
		case 12:
		case 13:
			__instance.CanShootByState = true;
			return false;
		default:
			return true;
		}
	}
}
