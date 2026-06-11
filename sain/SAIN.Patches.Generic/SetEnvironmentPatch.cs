using System;
using System.Reflection;
using EFT;
using EFT.EnvironmentEffect;
using HarmonyLib;
using SAIN.Components;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Generic;

public class SetEnvironmentPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(GClass567), "SetEnvironment", (Type[])null, (Type[])null);
	}

	[PatchPostfix]
	public static void Patch(GClass567 __instance, IndoorTrigger trigger)
	{
		BotManagerComponent instance = BotManagerComponent.Instance;
		if (instance != null)
		{
			object profileID;
			if (__instance == null)
			{
				profileID = null;
			}
			else
			{
				Player player = __instance.Player;
				profileID = ((player != null) ? player.ProfileId : null);
			}
			instance.PlayerEnviromentChanged((string)profileID, trigger);
		}
	}
}
