using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SAIN.Components;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SAIN.Patches.Components;

internal class ActivateBotComponentPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotOwner), "method_10", (Type[])null, (Type[])null);
	}

	[PatchPostfix]
	public static void PatchPostfix(ref BotOwner __instance)
	{
		((Component)__instance).GetComponent<BotComponent>()?.Activate(__instance);
	}
}
