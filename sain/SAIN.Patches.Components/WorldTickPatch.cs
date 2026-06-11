using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SAIN.Components;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Components;

internal class WorldTickPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(GameWorld), "DoWorldTick", (Type[])null, (Type[])null);
	}

	[PatchPostfix]
	public static void Patch(GameWorld __instance, float dt)
	{
		GameWorldComponent.Instance?.WorldTick(dt);
	}
}
