using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Hearing;

public class FootstepSoundPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(Player), "PlayStepSound", (Type[])null, (Type[])null);
	}

	[PatchPostfix]
	public static void Patch(Player __instance, BetterSource ___NestedStepSoundSource, SurfaceSet ____currentSet)
	{
	}
}
