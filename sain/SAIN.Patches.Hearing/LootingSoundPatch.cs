using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SAIN.Components;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SAIN.Patches.Hearing;

public class LootingSoundPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(Player), "method_46", (Type[])null, (Type[])null);
	}

	[PatchPostfix]
	public static void PatchPostfix(Player __instance, BetterSource ____searchSource)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)____searchSource == (Object)null))
		{
			float baseSoundRange_Looting = SAINPlugin.LoadedPreset.GlobalSettings.Hearing.BaseSoundRange_Looting;
			BotManagerComponent.Instance?.BotHearing.PlayAISound(__instance.ProfileId, SAINSoundType.Looting, __instance.Position, baseSoundRange_Looting, 1f);
		}
	}
}
