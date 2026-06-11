using System;
using System.Reflection;
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Generic.SetInHands;

public class SetInHands_QuickUse_Patch2 : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		Type[] array = new Type[2]
		{
			typeof(ThrowWeapItemClass),
			typeof(Callback<GInterface179>)
		};
		return AccessTools.Method(typeof(Player), "SetInHandsForQuickUse", array, (Type[])null);
	}

	[PatchPrefix]
	public static void PatchPrefix(ThrowWeapItemClass throwWeap, Player __instance)
	{
		Helpers.SetItemEquiped((IPlayer)(object)__instance, (Item)(object)throwWeap);
	}
}
