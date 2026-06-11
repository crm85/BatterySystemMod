using System;
using System.Reflection;
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using HarmonyLib;
using SAIN.Components;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Generic.SetInHands;

public class SetInHands_Grenade_Patch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		Type[] array = new Type[2]
		{
			typeof(ThrowWeapItemClass),
			typeof(Callback<IHandsThrowController>)
		};
		return AccessTools.Method(typeof(Player), "SetInHands", array, (Type[])null);
	}

	[PatchPrefix]
	public static void PatchPrefix(Player __instance, ThrowWeapItemClass throwWeap)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		float baseSoundRange_GrenadePinDraw = SAINPlugin.LoadedPreset.GlobalSettings.Hearing.BaseSoundRange_GrenadePinDraw;
		BotManagerComponent.Instance?.BotHearing.PlayAISound(__instance.ProfileId, SAINSoundType.GrenadeDraw, __instance.Position, baseSoundRange_GrenadePinDraw, 1f);
		Helpers.SetItemEquiped((IPlayer)(object)__instance, (Item)(object)throwWeap);
	}
}
