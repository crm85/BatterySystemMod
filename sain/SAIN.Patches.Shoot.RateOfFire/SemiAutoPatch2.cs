using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SAIN.Components;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Shoot.RateOfFire;

public class SemiAutoPatch2 : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(GClass453), "method_6", (Type[])null, (Type[])null);
	}

	[PatchPostfix]
	public static void PatchPostfix(BotOwner ___botOwner_0, ref float __result)
	{
		if (!SAINPlugin.IsBotExluded(___botOwner_0) && BotManagerComponent.Instance.GetSAIN(___botOwner_0, out var bot))
		{
			__result = bot.Info.WeaponInfo.Firerate.SemiAutoROF();
		}
	}
}
