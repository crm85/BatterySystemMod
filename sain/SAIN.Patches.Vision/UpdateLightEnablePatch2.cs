using System;
using System.Reflection;
using HarmonyLib;
using SAIN.Components;
using SAIN.Preset.GlobalSettings;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Vision;

public class UpdateLightEnablePatch2 : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotLight), "method_0", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool PatchPrefix(BotLight __instance)
	{
		if (!__instance.IsEnable)
		{
			return false;
		}
		float timeVisionDistanceModifier = BotManagerComponent.Instance.TimeVision.TimeVisionDistanceModifier;
		float lightOffRatio = GlobalSettingsClass.Instance.Look.Light.LightOffRatio;
		if (timeVisionDistanceModifier >= lightOffRatio)
		{
			__instance.TurnOff(true, true);
		}
		return false;
	}
}
