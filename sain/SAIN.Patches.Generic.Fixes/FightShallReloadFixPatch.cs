using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Generic.Fixes;

internal class FightShallReloadFixPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotReload), "FightShallReload", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool Patch(BotOwner ___botOwner_0, ref bool __result)
	{
		if (SAINEnableClass.IsBotInCombat((IPlayer)(object)___botOwner_0))
		{
			__result = true;
			return false;
		}
		return true;
	}
}
