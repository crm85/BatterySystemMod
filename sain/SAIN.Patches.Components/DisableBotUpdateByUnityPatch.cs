using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SAIN.Components;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SAIN.Patches.Components;

internal class DisableBotUpdateByUnityPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotsController), "method_0", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool Patch()
	{
		if ((Object)(object)GameWorldComponent.Instance == (Object)null)
		{
			return true;
		}
		return false;
	}
}
