using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SAIN.Patches.Components;

internal class AddGameWorldPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(GameWorldUnityTickListener), "Create", (Type[])null, (Type[])null);
	}

	[PatchPostfix]
	public static void PatchPostfix(GameObject gameObject, GameWorld gameWorld)
	{
		if (gameWorld is HideoutGameWorld)
		{
			return;
		}
		try
		{
			GameWorldHandler.Create(gameWorld);
		}
		catch (Exception arg)
		{
			ModulePatch.Logger.LogError((object)$" SAIN Init Gameworld Error: {arg}");
		}
	}
}
