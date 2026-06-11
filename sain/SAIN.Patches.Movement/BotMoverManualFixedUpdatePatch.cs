using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Movement;

public class BotMoverManualFixedUpdatePatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotMover), "ManualFixedUpdate", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool PatchPrefix(BotMover __instance, BotOwner ___botOwner_0)
	{
		if (SAINEnableClass.IsBotInCombat((IPlayer)(object)___botOwner_0))
		{
			return false;
		}
		return true;
	}
}
