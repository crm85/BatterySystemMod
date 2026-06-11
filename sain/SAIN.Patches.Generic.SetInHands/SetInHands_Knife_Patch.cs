using System;
using System.Reflection;
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Generic.SetInHands;

public class SetInHands_Knife_Patch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		Type[] array = new Type[2]
		{
			typeof(KnifeComponent),
			typeof(Callback<IKnifeController>)
		};
		return AccessTools.Method(typeof(Player), "SetInHands", array, (Type[])null);
	}

	[PatchPostfix]
	public static void Patch(Player __instance, KnifeComponent knife)
	{
		Helpers.SetItemEquiped((IPlayer)(object)__instance, ((GClass3175)(knife?)).Item);
	}
}
