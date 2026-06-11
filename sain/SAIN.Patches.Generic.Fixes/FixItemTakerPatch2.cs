using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Generic.Fixes;

internal class FixItemTakerPatch2 : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotItemTaker), "RefreshClosestItems", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool PatchPrefix(BotOwner ___botOwner_0)
	{
		return GenericHelpers.CheckNotNull(___botOwner_0);
	}
}
