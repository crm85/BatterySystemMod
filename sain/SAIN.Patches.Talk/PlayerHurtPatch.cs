using System;
using System.Reflection;
using EFT;
using EFT.HealthSystem;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Talk;

public class PlayerHurtPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(Player), "ApplyHitDebuff", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static void PatchPrefix(Player __instance, float damage)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (__instance == null)
		{
			return;
		}
		IHealthController healthController = __instance.HealthController;
		if (((healthController != null) ? new bool?(healthController.IsAlive) : ((bool?)null)) == true && __instance.IsAI && (!__instance.MovementContext.PhysicalConditionIs((EPhysicalCondition)1) || damage > 4f))
		{
			PhraseSpeakerClass speaker = __instance.Speaker;
			if (speaker != null)
			{
				speaker.Play((EPhraseTrigger)15, __instance.HealthStatus, true, (int?)null);
			}
		}
	}
}
