using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SAIN.Components;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Hearing;

public class GrenadeCollisionPatch : ModulePatch
{
	private static float _defaultRolloff = 40f;

	private const float ROLLOFF_MULTI = 1.5f;

	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(Grenade), "OnCollisionHandler", (Type[])null, (Type[])null);
	}

	[PatchPostfix]
	public static void Patch(Grenade __instance, SoundBank ___soundBank_0)
	{
		___soundBank_0.Rolloff = _defaultRolloff * 1.5f;
		BotManagerComponent.Instance?.GrenadeController.GrenadeCollided(__instance, 35f);
	}
}
