using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SAIN.Components;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Hearing;

public class DryShotPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(FirearmController), "DryShot", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static void PatchPrefix(Player ____player)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		float baseSoundRange_DryFire = SAINPlugin.LoadedPreset.GlobalSettings.Hearing.BaseSoundRange_DryFire;
		BotManagerComponent.Instance?.BotHearing.PlayAISound(____player.ProfileId, SAINSoundType.DryFire, ____player.WeaponRoot.position, baseSoundRange_DryFire, 1f);
	}
}
