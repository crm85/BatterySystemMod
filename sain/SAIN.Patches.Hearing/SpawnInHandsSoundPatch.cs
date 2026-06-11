using System;
using System.Reflection;
using EFT;
using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Hearing;

public class SpawnInHandsSoundPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(Player), "SpawnInHands", (Type[])null, (Type[])null);
	}

	[PatchPostfix]
	public static void PatchPostfix(Player __instance, Item item)
	{
	}
}
