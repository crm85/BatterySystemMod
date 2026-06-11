using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SAIN.Patches.Generic.Fixes;

internal class BotGroupAddEnemyPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotsGroup), "AddEnemy", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool PatchPrefix(IPlayer person)
	{
		if (person != null)
		{
			if (person.IsAI)
			{
				IAIData aIData = person.AIData;
				object obj;
				if (aIData == null)
				{
					obj = null;
				}
				else
				{
					BotOwner botOwner = aIData.BotOwner;
					obj = ((botOwner != null) ? botOwner.GetPlayer : null);
				}
				if ((Object)obj == (Object)null)
				{
					goto IL_003a;
				}
			}
			return true;
		}
		goto IL_003a;
		IL_003a:
		return false;
	}
}
