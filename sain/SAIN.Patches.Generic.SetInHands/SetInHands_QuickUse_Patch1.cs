using System;
using System.Reflection;
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Generic.SetInHands;

public class SetInHands_QuickUse_Patch1 : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		Type[] array = new Type[2]
		{
			typeof(Item),
			typeof(Callback<IOnHandsUseCallback>)
		};
		return AccessTools.Method(typeof(Player), "SetInHandsForQuickUse", array, (Type[])null);
	}

	[PatchPrefix]
	public static void PatchPrefix(Item quickUseItem, Player __instance)
	{
		Helpers.SetItemEquiped((IPlayer)(object)__instance, quickUseItem);
	}
}
