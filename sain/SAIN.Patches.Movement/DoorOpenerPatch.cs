using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SAIN.Preset.GlobalSettings;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Movement;

public class DoorOpenerPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotDoorOpener), "Update", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool PatchPrefix(ref BotOwner ____owner, ref bool __result)
	{
		DoorSettings doors = GlobalSettingsClass.Instance.General.Doors;
		if (doors.DisableAllDoors)
		{
			__result = false;
			return false;
		}
		if (doors.NewDoorOpening && SAINEnableClass.GetSAIN(____owner, out var sain) && sain.SAINLayersActive)
		{
			__result = sain.DoorOpener.FindDoorsToOpen();
			return false;
		}
		return true;
	}
}
