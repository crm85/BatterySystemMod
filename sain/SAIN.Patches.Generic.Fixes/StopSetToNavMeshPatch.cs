using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Generic.Fixes;

public class StopSetToNavMeshPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotMover), "method_9", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool PatchPrefix(BotMover __instance, ref BotOwner ___botOwner_0)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (SAINEnableClass.IsBotInCombat((IPlayer)(object)((GClass419)__instance).botOwner_0))
		{
			__instance.PositionOnWayInner = ___botOwner_0.Position;
			___botOwner_0.Mover.LocalAvoidance.DropOffset();
			return false;
		}
		return true;
	}
}
