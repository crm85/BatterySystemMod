using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SAIN.Components;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SAIN.Patches.Hearing;

public class OnMakingShotPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(Player), "OnMakingShot", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static void PatchPrefix(Player __instance, IWeapon weapon, Vector3 force)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (GameWorldComponent.TryGetPlayerComponent((IPlayer)(object)__instance, out var PlayerComponent))
		{
			PlayerComponent.OnMakingShot(weapon, force);
		}
	}
}
