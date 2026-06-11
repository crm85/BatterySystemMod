using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SAIN.Components;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SAIN.Patches.Components;

internal class GetBotController : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotsController), "Init", (Type[])null, (Type[])null);
	}

	[PatchPostfix]
	public static void Patch(BotsController __instance)
	{
		GameWorldComponent instance = GameWorldComponent.Instance;
		if ((Object)(object)instance == (Object)null)
		{
			ModulePatch.Logger.LogError((object)"gameWorld Null");
		}
		else
		{
			instance.Activate(__instance);
		}
	}
}
