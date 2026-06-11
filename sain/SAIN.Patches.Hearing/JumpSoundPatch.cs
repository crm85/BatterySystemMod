using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Hearing;

public class JumpSoundPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(MovementContext), "method_2", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool PatchPrefix(Player ____player, ref float ____nextJumpNoise)
	{
		return false;
	}
}
