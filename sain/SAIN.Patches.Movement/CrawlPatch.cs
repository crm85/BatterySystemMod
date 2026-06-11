using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SAIN.Patches.Movement;

public class CrawlPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(GClass485), "method_0", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool PatchPrefix(GClass485 __instance, BotOwner ___botOwner_0, Vector3 pos, bool slowAtTheEnd, bool getUpWithCheck)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!SAINEnableClass.IsBotInCombat((IPlayer)(object)___botOwner_0))
		{
			return true;
		}
		if (___botOwner_0.BotLay.IsLay && getUpWithCheck)
		{
			Vector3 val = pos - ___botOwner_0.Mover.PositionOnWay;
			if (val.y < 0.5f)
			{
				val.y = 0f;
			}
			if (((Vector3)(ref val)).sqrMagnitude > 0.2f)
			{
				___botOwner_0.BotLay.GetUp(getUpWithCheck);
			}
			if (___botOwner_0.BotLay.IsLay)
			{
				return false;
			}
		}
		___botOwner_0.WeaponManager.Stationary.StartMove();
		__instance.SlowAtTheEnd = slowAtTheEnd;
		return false;
	}
}
