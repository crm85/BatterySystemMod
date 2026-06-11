using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Shoot.Aim;

internal class SetAimStatusPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.PropertySetter(typeof(BotAimingClass), "Status");
	}

	[PatchPrefix]
	public static bool PatchPrefix(BotAimingClass __instance, AimStatus value, BotOwner ___botOwner_0, ref AimStatus ___aimStatus_0, float ___float_7)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Invalid comparison between I4 and Unknown
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected I4, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Invalid comparison between Unknown and I4
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if ((int)___aimStatus_0 == (int)value || (int)___botOwner_0.BotState != 2)
		{
			return false;
		}
		if (SAINEnableClass.GetSAIN(___botOwner_0, out var _))
		{
			___aimStatus_0 = (AimStatus)(int)value;
			if ((int)value == 2)
			{
				___botOwner_0.BotPersonalStats.Aim(__instance.EndTargetPoint, ___float_7);
			}
			return false;
		}
		return true;
	}
}
