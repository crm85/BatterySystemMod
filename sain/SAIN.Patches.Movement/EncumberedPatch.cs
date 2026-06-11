using System;
using System.Reflection;
using Comfort.Common;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SAIN.Patches.Movement;

public class EncumberedPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BasePhysicalClass), "UpdateWeightLimits", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool PatchPrefix(bool ___bool_7, IObserverToPlayerBridge ___iobserverToPlayerBridge_0, BasePhysicalClass __instance)
	{
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		IPlayer iPlayer = ___iobserverToPlayerBridge_0.iPlayer;
		if (iPlayer == null)
		{
			ModulePatch.Logger.LogWarning((object)"Player is Null, can't set weight limits for AI.");
			return true;
		}
		if (!iPlayer.IsAI)
		{
			return true;
		}
		if (SAINPlugin.IsBotExluded(iPlayer.AIData.BotOwner))
		{
			return true;
		}
		GClass1499 stamina = Singleton<BackendConfigSettingsClass>.Instance.Stamina;
		float num = ___iobserverToPlayerBridge_0.Skills.CarryingWeightRelativeModifier * ___iobserverToPlayerBridge_0.iPlayer.HealthController.CarryingWeightRelativeModifier;
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(___iobserverToPlayerBridge_0.iPlayer.HealthController.CarryingWeightAbsoluteModifier, ___iobserverToPlayerBridge_0.iPlayer.HealthController.CarryingWeightAbsoluteModifier);
		InertiaSettings inertia = Singleton<BackendConfigSettingsClass>.Instance.Inertia;
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector(inertia.InertiaLimitsStep * (float)((AbstractSkillClass)___iobserverToPlayerBridge_0.Skills.Strength).SummaryLevel, inertia.InertiaLimitsStep * (float)((AbstractSkillClass)___iobserverToPlayerBridge_0.Skills.Strength).SummaryLevel, 0f);
		__instance.BaseInertiaLimits = inertia.InertiaLimits + val2;
		__instance.WalkOverweightLimits = stamina.WalkOverweightLimits * num + val;
		__instance.BaseOverweightLimits = stamina.BaseOverweightLimits * num + val;
		__instance.SprintOverweightLimits = stamina.SprintOverweightLimits * num + val;
		__instance.WalkSpeedOverweightLimits = stamina.WalkSpeedOverweightLimits * num + val;
		return false;
	}
}
