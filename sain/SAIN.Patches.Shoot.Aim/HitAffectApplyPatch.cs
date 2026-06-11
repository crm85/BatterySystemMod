using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SAIN.Preset.GlobalSettings;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SAIN.Patches.Shoot.Aim;

internal class HitAffectApplyPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(GClass583), "Affect", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool Patch(BotOwner ___botOwner_0, ref Vector3 __result, Vector3 dir)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (!GlobalSettingsClass.Instance.Aiming.HitEffects.HIT_REACTION_TOGGLE)
		{
			return true;
		}
		if (SAINEnableClass.GetSAIN(___botOwner_0, out var sain))
		{
			__result = sain.Medical.HitReaction.AimHitEffect.ApplyEffect(dir);
			return false;
		}
		return true;
	}
}
