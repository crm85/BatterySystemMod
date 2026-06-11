using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Talk;

public class JumpPainPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(Player), "method_110", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static void PatchPrefix(Player __instance, EPlayerState nextState)
	{
	}
}
