using System;
using System.Reflection;
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using HarmonyLib;
using SAIN.Components;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Generic.SetInHands;

public class SetInHands_Food_Patch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		Type[] array = new Type[4]
		{
			typeof(FoodDrinkItemClass),
			typeof(float),
			typeof(int),
			typeof(Callback<GInterface176>)
		};
		return AccessTools.Method(typeof(Player), "SetInHands", array, (Type[])null);
	}

	[PatchPrefix]
	public static void PatchPrefix(Player __instance, FoodDrinkItemClass foodDrink)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		float baseSoundRange_EatDrink = SAINPlugin.LoadedPreset.GlobalSettings.Hearing.BaseSoundRange_EatDrink;
		BotManagerComponent.Instance?.BotHearing.PlayAISound(__instance.ProfileId, SAINSoundType.Food, __instance.Position, baseSoundRange_EatDrink, 1f);
		Helpers.SetItemEquiped((IPlayer)(object)__instance, (Item)(object)foodDrink);
	}
}
