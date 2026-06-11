using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SAIN.Components;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Hearing;

public class OnWeaponModifiedPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(FirearmController), "WeaponModified", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static void PatchPrefix(FirearmController __instance, Player ____player)
	{
		if (GameWorldComponent.TryGetPlayerComponent((IPlayer)(object)____player, out var PlayerComponent))
		{
			PlayerComponent.Equipment.WeaponModified(__instance.Weapon);
		}
	}
}
