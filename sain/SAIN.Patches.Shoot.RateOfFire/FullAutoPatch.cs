using System;
using System.Reflection;
using EFT;
using EFT.InventoryLogic;
using HarmonyLib;
using SAIN.Helpers;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SAIN.Patches.Shoot.RateOfFire;

public class FullAutoPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(ShootData), "method_6", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool PatchPrefix(BotOwner ____owner, ref float ___nextFingerUpTime)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Invalid comparison between Unknown and I4
		if (SAINPlugin.IsBotExluded(____owner))
		{
			return true;
		}
		if (____owner.AimingManager.CurrentAiming == null)
		{
			return true;
		}
		Weapon currentWeapon = ____owner.WeaponManager.CurrentWeapon;
		if ((int)currentWeapon.SelectedFireMode == 0)
		{
			float lastDist2Target = ____owner.AimingManager.CurrentAiming.LastDist2Target;
			float num = SAIN.Helpers.Shoot.FullAutoBurstLength(____owner, lastDist2Target);
			___nextFingerUpTime = num + Time.time;
			return false;
		}
		___nextFingerUpTime = 0.001f + Time.time;
		return true;
	}
}
