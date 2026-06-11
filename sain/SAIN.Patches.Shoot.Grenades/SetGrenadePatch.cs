using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SAIN.Components;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Shoot.Grenades;

public class SetGrenadePatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotGrenadeController), "method_3", (Type[])null, (Type[])null);
	}

	[PatchPostfix]
	public static void Patch(BotOwner ___botOwner_0, ThrowWeapItemClass potentialGrenade, BotGrenadeController __instance)
	{
		if (potentialGrenade != null && BotManagerComponent.Instance.GetSAIN(___botOwner_0, out var bot))
		{
			bot.Grenade.MyGrenade = potentialGrenade;
		}
	}
}
