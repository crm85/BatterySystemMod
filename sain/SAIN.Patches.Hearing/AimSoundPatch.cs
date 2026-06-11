using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SAIN.Components;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Hearing;

public class AimSoundPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(Player), "method_58", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static void PatchPrefix(float volume, Player __instance)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		float baseSoundRange_AimingandGearRattle = SAINPlugin.LoadedPreset.GlobalSettings.Hearing.BaseSoundRange_AimingandGearRattle;
		BotManagerComponent.Instance?.BotHearing.PlayAISound(__instance.ProfileId, SAINSoundType.GearSound, __instance.Position, baseSoundRange_AimingandGearRattle, volume);
	}
}
