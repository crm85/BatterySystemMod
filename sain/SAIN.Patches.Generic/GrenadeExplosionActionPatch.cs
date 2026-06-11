using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Generic;

public class GrenadeExplosionActionPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotsController), "method_3", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool PatchPrefix()
	{
		return false;
	}
}
