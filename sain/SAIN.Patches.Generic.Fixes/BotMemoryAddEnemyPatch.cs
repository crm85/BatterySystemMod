using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SAIN.Patches.Generic.Fixes;

internal class BotMemoryAddEnemyPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotMemoryClass), "AddEnemy", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool PatchPrefix(IPlayer enemy)
	{
		if (enemy != null)
		{
			if (enemy.IsAI)
			{
				IAIData aIData = enemy.AIData;
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
