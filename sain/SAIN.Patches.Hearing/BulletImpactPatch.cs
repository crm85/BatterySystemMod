using System;
using System.Reflection;
using Systems.Effects;
using HarmonyLib;
using SAIN.Components;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SAIN.Patches.Hearing;

public class BulletImpactPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(EffectsCommutator), "PlayHitEffect", (Type[])null, (Type[])null);
	}

	[PatchPostfix]
	public static void PatchPostfix(EftBulletClass info)
	{
		if ((Object)(object)BotManagerComponent.Instance != (Object)null)
		{
			BotManagerComponent.Instance.BotHearing.BulletImpacted(info);
		}
	}
}
