using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SAIN.Patches.Hearing;

public class SprintSoundPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(MovementContext), "method_1", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool PatchPrefix(Player ____player, Vector3 motion, MovementContext __instance, ref float ____nextStepNoise)
	{
		return false;
	}
}
