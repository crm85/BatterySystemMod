using System;
using System.Reflection;
using HarmonyLib;
using SAIN.Helpers;
using SAIN.Preset.GlobalSettings;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Shoot.Aim;

internal class ForceNoHeadAimPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(EnemyInfo), "method_13", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static void PatchPrefix(ref bool withLegs, ref bool canBehead, EnemyInfo __instance)
	{
		if (!__instance.Person.IsAI)
		{
			AimSettings aiming = GlobalSettingsClass.Instance.Aiming;
			canBehead = EFTMath.RandomBool(aiming.PMCAimForHeadChance) && aiming.PMCSAimForHead && IsPMC(__instance);
			withLegs = true;
		}
		else
		{
			canBehead = true;
			withLegs = true;
		}
	}

	private static bool IsPMC(EnemyInfo __instance)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return EnumValues.WildSpawn.IsPMC(__instance.Owner.Profile.Info.Settings.Role);
	}
}
