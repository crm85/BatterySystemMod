using System;
using System.Reflection;
using HarmonyLib;
using RootMotion.FinalIK;
using SAIN.Preset.GlobalSettings;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Shoot.Aim;

internal class PlayerHitReactionDisablePatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(HitReaction), "Hit", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool Patch()
	{
		if (!GlobalSettingsClass.Instance.Aiming.HitEffects.HIT_REACTION_TOGGLE)
		{
			return true;
		}
		return false;
	}
}
