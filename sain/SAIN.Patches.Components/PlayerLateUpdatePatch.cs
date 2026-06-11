using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SAIN.Components;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Components;

internal class PlayerLateUpdatePatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(Player), "LateUpdate", (Type[])null, (Type[])null);
	}

	[PatchPostfix]
	public static void Patch(Player __instance)
	{
		if (GameWorldComponent.TryGetPlayerComponent((IPlayer)(object)__instance, out var PlayerComponent))
		{
			PlayerComponent.ManualLateUpdate();
		}
	}
}
