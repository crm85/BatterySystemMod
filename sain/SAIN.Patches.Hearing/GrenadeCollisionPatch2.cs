using System;
using System.Reflection;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SAIN.Patches.Hearing;

public class GrenadeCollisionPatch2 : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(Throwable), "OnCollisionHandler", (Type[])null, (Type[])null);
	}

	[PatchPostfix]
	public static void Patch(ref float ___IgnoreCollisionTrackingTimer, Throwable __instance)
	{
		___IgnoreCollisionTrackingTimer = Time.time + 0.35f;
	}
}
