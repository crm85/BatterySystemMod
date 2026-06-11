using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SAIN.Components;
using SAIN.Components.PlayerComponentSpace;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SAIN.Patches.Vision;

public class CheckFlashlightPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(FirearmController), "SetLightsState", (Type[])null, (Type[])null);
	}

	[PatchPostfix]
	public static void PatchPostfix(Player ____player)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		PlayerComponent playerComponent = GameWorldComponent.Instance?.PlayerTracker.GetPlayerComponent((____player != null) ? ____player.ProfileId : null);
		if ((Object)(object)playerComponent != (Object)null)
		{
			BotManagerComponent.Instance.BotHearing.PlayAISound(playerComponent, SAINSoundType.GearSound, playerComponent.Player.WeaponRoot.position, 35f, 1f, limitFreq: true);
			FlashLightClass flashlight = playerComponent.Flashlight;
			flashlight.CheckDevice();
			if (!flashlight.WhiteLight && !flashlight.Laser)
			{
				IAIData aIData = ____player.AIData;
				((GClass567)((aIData is GClass567) ? aIData : null)).UsingLight = false;
			}
		}
	}
}
