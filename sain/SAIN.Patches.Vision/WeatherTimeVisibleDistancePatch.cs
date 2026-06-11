using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SAIN.Patches.Vision;

public class WeatherTimeVisibleDistancePatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(LookSensor), "method_2", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool PatchPrefix(BotOwner ____botOwner, ref float ____nextUpdateVisibleDist)
	{
		if (____nextUpdateVisibleDist < Time.time)
		{
			if (SAINEnableClass.IsBotExcluded(____botOwner))
			{
				return true;
			}
			____nextUpdateVisibleDist = float.MaxValue;
			return false;
		}
		return false;
	}
}
