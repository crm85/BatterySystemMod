using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SAIN.Helpers;
using SAIN.Preset.GlobalSettings;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Vision;

public class BotLightTurnOnPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotLight), "TurnOn", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool PatchPrefix(BotOwner ___botOwner_0, ref bool ____isInDarkPlace)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (____isInDarkPlace && !SAINPlugin.LoadedPreset.GlobalSettings.General.Flashlight.AllowLightOnForDarkBuildings)
		{
			____isInDarkPlace = false;
		}
		if (____isInDarkPlace || ___botOwner_0.Memory.GoalEnemy != null)
		{
			return true;
		}
		if (!ShallTurnLightOff(___botOwner_0.Profile.Info.Settings.Role))
		{
			return true;
		}
		___botOwner_0.BotLight.TurnOff(false, true);
		return false;
	}

	private static bool ShallTurnLightOff(WildSpawnType wildSpawnType)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		FlashlightSettings flashlight = SAINPlugin.LoadedPreset.GlobalSettings.General.Flashlight;
		if (EnumValues.WildSpawn.IsScav(wildSpawnType))
		{
			return flashlight.TurnLightOffNoEnemySCAV;
		}
		if (EnumValues.WildSpawn.IsPMC(wildSpawnType))
		{
			return flashlight.TurnLightOffNoEnemyPMC;
		}
		if (EnumValues.WildSpawn.IsGoons(wildSpawnType))
		{
			return flashlight.TurnLightOffNoEnemyGOONS;
		}
		if (EnumValues.WildSpawn.IsBoss(wildSpawnType))
		{
			return flashlight.TurnLightOffNoEnemyBOSS;
		}
		if (EnumValues.WildSpawn.IsFollower(wildSpawnType))
		{
			return flashlight.TurnLightOffNoEnemyFOLLOWER;
		}
		return flashlight.TurnLightOffNoEnemyRAIDERROGUE;
	}
}
