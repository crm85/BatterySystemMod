using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Generic;

public class TurnDamnLightOffPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotWeaponSelector), "TryChangeToSlot", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static void PatchPrefix(ref BotOwner ___botOwner_0)
	{
		BotOwner obj = ___botOwner_0;
		if (obj != null)
		{
			BotLight botLight = obj.BotLight;
			if (botLight != null)
			{
				botLight.TurnOff(false, true);
			}
		}
	}
}
