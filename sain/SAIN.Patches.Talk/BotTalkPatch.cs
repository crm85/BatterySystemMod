using System;
using System.Reflection;
using EFT;
using EFT.HealthSystem;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Talk;

public class BotTalkPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotTalk), "Say", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool PatchPrefix(BotOwner ___botOwner_0, EPhraseTrigger type, ETagStatus? additionalMask = null)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Invalid comparison between Unknown and I4
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Invalid comparison between Unknown and I4
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Invalid comparison between Unknown and I4
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Invalid comparison between Unknown and I4
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Invalid comparison between Unknown and I4
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Invalid comparison between Unknown and I4
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		if (SAINPlugin.LoadedPreset.GlobalSettings.Talk.DisableBotTalkPatching)
		{
			return true;
		}
		if (___botOwner_0 != null)
		{
			IHealthController healthController = ___botOwner_0.HealthController;
			if (((healthController != null) ? new bool?(healthController.IsAlive) : ((bool?)null)) == false)
			{
				return true;
			}
		}
		if ((int)type <= 15)
		{
			if ((int)type == 9 || (int)type == 15)
			{
				goto IL_0095;
			}
		}
		else if ((int)type == 26 || (int)type == 29)
		{
			goto IL_0095;
		}
		if (!SAINEnableClass.GetSAIN(___botOwner_0, out var sain))
		{
			return true;
		}
		if (type - 48 <= 1)
		{
			sain.Talk.GroupSay(type);
		}
		return false;
		IL_0095:
		return true;
	}
}
