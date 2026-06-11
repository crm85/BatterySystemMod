using System;
using System.Reflection;
using EFT.Interactive;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SAIN.Patches.Movement;

public class DoorDisabledPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(WorldInteractiveObject), "method_4", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool PatchPrefix(WorldInteractiveObject __instance)
	{
		if (!((Behaviour)__instance).enabled || !((Component)__instance).gameObject.activeInHierarchy)
		{
			return false;
		}
		return true;
	}
}
