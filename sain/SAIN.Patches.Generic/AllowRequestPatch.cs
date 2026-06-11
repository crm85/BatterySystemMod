using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Generic;

public class AllowRequestPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotRequestController), "method_2", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool Patch(BotOwner ____owner)
	{
		BotRequest curRequest = ____owner.BotRequestController.CurRequest;
		if (curRequest == null)
		{
			return false;
		}
		if (!SAINEnableClass.GetSAIN(____owner, out var sain))
		{
			return true;
		}
		if (sain.HasEnemy)
		{
			IPlayer requester = curRequest.Requester;
			if (requester != null && requester.IsAI)
			{
				curRequest.Dispose();
				return false;
			}
		}
		return true;
	}
}
