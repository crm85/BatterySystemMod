using System;
using System.Reflection;
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using HarmonyLib;
using SAIN.Components;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Generic.SetInHands;

public class SetInHands_Meds_Patch2 : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		Type[] array = new Type[4]
		{
			typeof(MedsItemClass),
			typeof(GStruct353<EBodyPart>),
			typeof(int),
			typeof(Callback<GInterface176>)
		};
		return AccessTools.Method(typeof(Player), "SetInHands", array, (Type[])null);
	}

	[PatchPrefix]
	public static void PatchPrefix(MedsItemClass meds, Player __instance)
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		SAINSoundType soundType;
		float range;
		if (meds != null && meds.HealthEffectsComponent.AffectsAny((EDamageEffectType[])(object)new EDamageEffectType[1] { (EDamageEffectType)8 }))
		{
			soundType = SAINSoundType.Surgery;
			range = SAINPlugin.LoadedPreset.GlobalSettings.Hearing.BaseSoundRange_Surgery;
		}
		else
		{
			soundType = SAINSoundType.Heal;
			range = SAINPlugin.LoadedPreset.GlobalSettings.Hearing.BaseSoundRange_Healing;
		}
		BotManagerComponent.Instance?.BotHearing.PlayAISound(__instance.ProfileId, soundType, __instance.Position, range, 1f);
		Helpers.SetItemEquiped((IPlayer)(object)__instance, (Item)(object)meds);
	}
}
