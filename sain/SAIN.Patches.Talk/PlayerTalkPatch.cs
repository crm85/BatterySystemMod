using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SAIN.Components;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Talk;

public class PlayerTalkPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(Player), "Say", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool PatchPrefix(Player __instance, EPhraseTrigger phrase, ETagStatus mask, bool aggressive)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Invalid comparison between Unknown and I4
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Invalid comparison between Unknown and I4
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		if ((int)phrase <= 15)
		{
			if ((int)phrase == 9 || (int)phrase == 15)
			{
				goto IL_0026;
			}
		}
		else if ((int)phrase == 26 || (int)phrase == 29)
		{
			goto IL_0026;
		}
		if (__instance.IsAI)
		{
			if (!SAINPlugin.LoadedPreset.GlobalSettings.Talk.DisableBotTalkPatching)
			{
				IAIData aIData = __instance.AIData;
				if (!SAINPlugin.IsBotExluded((aIData != null) ? aIData.BotOwner : null))
				{
					return false;
				}
			}
			BotManagerComponent.Instance?.BotHearing.PlayerTalked(phrase, mask, __instance);
			return true;
		}
		BotManagerComponent.Instance?.BotHearing.PlayerTalked(phrase, mask, __instance);
		return true;
		IL_0026:
		BotManagerComponent.Instance?.BotHearing.PlayerTalked(phrase, mask, __instance);
		return true;
	}
}
