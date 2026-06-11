using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Generic.SetInHands;

public class SetInHands_Empty : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(Player), "SetEmptyHands", (Type[])null, (Type[])null);
	}

	[PatchPostfix]
	public static void Patch(Player __instance)
	{
		Helpers.SetItemEquiped((IPlayer)(object)__instance, null);
	}
}
