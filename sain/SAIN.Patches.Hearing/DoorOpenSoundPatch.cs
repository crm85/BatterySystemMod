using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SAIN.Components;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Hearing;

public class DoorOpenSoundPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(MovementContext), "StartInteraction", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static void PatchPrefix(Player ____player)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		float dOOR_OPEN_SOUND_RANGE = SAINPlugin.LoadedPreset.GlobalSettings.Hearing.DOOR_OPEN_SOUND_RANGE;
		BotManagerComponent.Instance?.BotHearing.PlayAISound(____player.ProfileId, SAINSoundType.Door, ____player.Position, dOOR_OPEN_SOUND_RANGE, 1f);
	}
}
