using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Movement;

public class BotMoverManualUpdatePatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotMover), "ManualUpdate", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool PatchPrefix(BotMover __instance, BotOwner ___botOwner_0)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (!SAINEnableClass.IsBotInCombat((IPlayer)(object)___botOwner_0))
		{
			return true;
		}
		__instance.LocalAvoidance.DropOffset();
		__instance.PositionOnWayInner = ___botOwner_0.Position;
		__instance.method_11();
		return false;
	}
}
