using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SAIN.Components;
using SAIN.Components.PlayerComponentSpace;
using SAIN.Preset.GlobalSettings;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SAIN.Patches.Vision;

public class UpdateLightEnablePatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotLight), "UpdateLightEnable", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool PatchPrefix(BotOwner ___botOwner_0, float curLightDist, ref float __result, bool ____haveLight, ref float ____curLightDist, ref bool ____canUseNow, BotLight __instance)
	{
		__result = curLightDist;
		if (___botOwner_0.FlashGrenade.IsFlashed)
		{
			return false;
		}
		if (!____haveLight)
		{
			return false;
		}
		____curLightDist = curLightDist;
		float timeVisionDistanceModifier = BotManagerComponent.Instance.TimeVision.TimeVisionDistanceModifier;
		LightNVGSettings light = GlobalSettingsClass.Instance.Look.Light;
		float lightOnRatio = light.LightOnRatio;
		float lightOffRatio = light.LightOffRatio;
		bool isEnable = __instance.IsEnable;
		bool flag = !isEnable && timeVisionDistanceModifier <= lightOnRatio && ___botOwner_0.Memory.IsPeace;
		bool flag2 = isEnable && timeVisionDistanceModifier >= lightOffRatio;
		____canUseNow = timeVisionDistanceModifier < lightOffRatio;
		if (flag)
		{
			try
			{
				__instance.TurnOn(true);
			}
			catch (Exception ex)
			{
				if (SAINPlugin.DebugMode)
				{
					ModulePatch.Logger.LogError((object)ex);
				}
			}
		}
		if (flag2)
		{
			try
			{
				__instance.TurnOff(true, true);
			}
			catch (Exception ex2)
			{
				if (SAINPlugin.DebugMode)
				{
					ModulePatch.Logger.LogError((object)ex2);
				}
			}
		}
		if (__instance.IsEnable)
		{
			GameWorldComponent instance = GameWorldComponent.Instance;
			if ((Object)(object)instance == (Object)null)
			{
				ModulePatch.Logger.LogError((object)"GameWorldComponent is null, cannot check if bot has flashlight on!");
				return false;
			}
			PlayerComponent playerComponent = instance.PlayerTracker.GetPlayerComponent(___botOwner_0.ProfileId);
			if ((Object)(object)playerComponent == (Object)null)
			{
				ModulePatch.Logger.LogError((object)"Player Component is null, cannot check if bot has flashlight on!");
				return false;
			}
			if (playerComponent.Flashlight.WhiteLight || (___botOwner_0.NightVision.UsingNow && playerComponent.Flashlight.IRLight))
			{
				float vISIBLE_DISNACE_WITH_LIGHT = ___botOwner_0.Settings.FileSettings.Look.VISIBLE_DISNACE_WITH_LIGHT;
				__result = Mathf.Clamp(curLightDist, vISIBLE_DISNACE_WITH_LIGHT, float.MaxValue);
			}
		}
		return false;
	}
}
