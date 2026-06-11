using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SAIN.Components;
using SAIN.Preset.GlobalSettings;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Vision;

public class ToggleNightVisionPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotNightVisionData), "method_0", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool PatchPrefix(BotOwner ___botOwner_0, bool ____nightVisionAtPocket, BotNightVisionData __instance)
	{
		if (___botOwner_0.FlashGrenade.IsFlashed)
		{
			return false;
		}
		float timeVisionDistanceModifier = BotManagerComponent.Instance.TimeVision.TimeVisionDistanceModifier;
		LightNVGSettings light = GlobalSettingsClass.Instance.Look.Light;
		float nightVisionOnRatio = light.NightVisionOnRatio;
		float nightVisionOffRatio = light.NightVisionOffRatio;
		if (____nightVisionAtPocket)
		{
			if (timeVisionDistanceModifier < nightVisionOnRatio)
			{
				__instance.method_4();
				return false;
			}
		}
		else
		{
			if (timeVisionDistanceModifier < nightVisionOnRatio)
			{
				__instance.method_5();
			}
			if (timeVisionDistanceModifier >= nightVisionOffRatio)
			{
				__instance.method_1();
			}
		}
		return false;
	}
}
