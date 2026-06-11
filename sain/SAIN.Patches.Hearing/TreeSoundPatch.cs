using System;
using System.Reflection;
using EFT;
using EFT.Interactive;
using HarmonyLib;
using SAIN.Components;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SAIN.Patches.Hearing;

public class TreeSoundPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(TreeInteractive), "method_0", (Type[])null, (Type[])null);
	}

	[PatchPostfix]
	public static void Patch(Vector3 soundPosition, BetterSource source, IPlayerOwner player, SoundBank ____soundBank)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (player.iPlayer != null)
		{
			float range = 50f;
			if ((Object)(object)____soundBank != (Object)null)
			{
				range = ____soundBank.Rolloff * player.SoundRadius * 0.8f;
			}
			BotManagerComponent.Instance?.BotHearing.PlayAISound(player.iPlayer.ProfileId, SAINSoundType.Bush, soundPosition, range, 1f);
		}
	}
}
