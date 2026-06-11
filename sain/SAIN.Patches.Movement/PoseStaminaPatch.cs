using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Movement;

public class PoseStaminaPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(PlayerPhysicalClass), "ConsumePoseLevelChange", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool PatchPrefix(Player ___player_0)
	{
		if (___player_0.IsAI)
		{
			return false;
		}
		return true;
	}
}
