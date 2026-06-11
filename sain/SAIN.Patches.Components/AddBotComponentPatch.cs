using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SAIN.Components.BotController;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Components;

internal class AddBotComponentPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotOwner), "PreActivate", (Type[])null, (Type[])null);
	}

	[PatchPostfix]
	public static void PatchPostfix(ref BotOwner __instance)
	{
		try
		{
			BotSpawnController.Instance.AddBot(__instance);
		}
		catch (Exception arg)
		{
			ModulePatch.Logger.LogError((object)$" SAIN Add Bot Error: {arg}");
		}
	}
}
