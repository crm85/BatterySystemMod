using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SAIN.Components;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Hearing;

public class ProneSoundPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(Player), "PlaySoundBank", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static void PatchPrefix(Player __instance, ref string soundBank, float ____runSurfaceCheck)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if (soundBank == "Prone" && __instance.SinceLastStep >= 0.5f && __instance.CheckSurface(____runSurfaceCheck))
		{
			float baseSoundRange_Prone = SAINPlugin.LoadedPreset.GlobalSettings.Hearing.BaseSoundRange_Prone;
			BotManagerComponent.Instance?.BotHearing.PlayAISound(__instance.ProfileId, SAINSoundType.Prone, __instance.Position, baseSoundRange_Prone, 1f);
		}
	}
}
