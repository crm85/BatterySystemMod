using System;
using System.Reflection;
using CommonAssets.Scripts.Audio;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Hearing;

public class GenericMovementSoundPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(Player), "DefaultPlay", (Type[])null, (Type[])null);
	}

	[PatchPostfix]
	public static void Patch(Player __instance, SoundBank bank, float volume, EAudioMovementState movementState)
	{
	}
}
