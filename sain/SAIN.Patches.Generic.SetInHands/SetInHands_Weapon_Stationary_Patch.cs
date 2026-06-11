using System;
using System.Reflection;
using EFT;
using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Generic.SetInHands;

public class SetInHands_Weapon_Stationary_Patch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(Player), "SetStationaryWeapon", (Type[])null, (Type[])null);
	}

	[PatchPostfix]
	public static void Patch(Player __instance, Weapon weapon)
	{
		Helpers.SetItemEquiped((IPlayer)(object)__instance, (Item)(object)weapon);
	}
}
