using System;
using System.Reflection;
using CommonAssets.Scripts.Audio;
using EFT;
using HarmonyLib;
using SAIN.Components;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SAIN.Patches.Hearing;

public class SpecificStepAudioControllerPatch : ModulePatch
{
	protected static readonly FieldInfo NestedStepSoundSourceField = AccessTools.Field(typeof(Player), "NestedStepSoundSource");

	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(GClass1117), "Play", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool Patch(GClass1117 __instance, IPlayer ___iplayer_0, EAudioMovementState movementState, EnvironmentType environment, float distance, float baseStepVolume, float blendParameter, bool stereo)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Invalid comparison between Unknown and I4
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Invalid comparison between Unknown and I4
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Invalid comparison between Unknown and I4
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected I4, but got Unknown
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		if ((int)movementState == 0)
		{
			return false;
		}
		float num = baseStepVolume;
		if (!__instance.bool_0 && (int)environment != 1 && (int)movementState > 0)
		{
			SoundBank val = default(SoundBank);
			if (__instance.method_3(movementState, ref val))
			{
				num = __instance.CalculateFinalVolume(baseStepVolume, val);
				val.Play(__instance.betterSource_0, (EnvironmentType)0, distance, num, blendParameter, stereo, true);
			}
			else
			{
				Debug.LogError((object)$"Can't find bank for movement state: {movementState}");
			}
		}
		if (1 == 0)
		{
		}
		SAINSoundType sAINSoundType = (movementState - 1) switch
		{
			0 => SAINSoundType.FootStep, 
			1 => SAINSoundType.Sprint, 
			3 => SAINSoundType.Land, 
			4 => SAINSoundType.TurnSound, 
			7 => SAINSoundType.Land, 
			_ => SAINSoundType.Generic, 
		};
		if (1 == 0)
		{
		}
		SAINSoundType soundType = sAINSoundType;
		Player val2 = (Player)(object)((___iplayer_0 is Player) ? ___iplayer_0 : null);
		if (val2 != null)
		{
			if (NestedStepSoundSourceField != null)
			{
				object value = NestedStepSoundSourceField.GetValue(val2);
				if (value != null)
				{
					BetterSource val3 = (BetterSource)((value is BetterSource) ? value : null);
					if (val3 != null)
					{
						BotManagerComponent.Instance?.BotHearing.PlayAISound(___iplayer_0.ProfileId, soundType, ___iplayer_0.Position, val3.MaxDistance, num);
					}
					else
					{
						ModulePatch.Logger.LogError((object)"StepSourceObj is not BetterSource NestedStepSoundSource");
					}
				}
				else
				{
					ModulePatch.Logger.LogError((object)"StepSourceObj is null");
				}
			}
			else
			{
				ModulePatch.Logger.LogError((object)"NestedStepSoundSourceField is null");
			}
		}
		else
		{
			ModulePatch.Logger.LogError((object)"___iplayer_0 is not Player player");
		}
		return false;
	}
}
