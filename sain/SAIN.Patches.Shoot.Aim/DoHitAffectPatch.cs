using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SAIN.Preset.GlobalSettings;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Shoot.Aim;

internal class DoHitAffectPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(GClass583), "DoAffection", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool Patch(BotOwner ___botOwner_0)
	{
		if (!GlobalSettingsClass.Instance.Aiming.HitEffects.HIT_REACTION_TOGGLE)
		{
			return true;
		}
		if (SAINEnableClass.GetSAIN(___botOwner_0, out var _))
		{
			return false;
		}
		return true;
	}
}
