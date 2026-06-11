using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SAIN.Components;
using SAIN.Components.Helpers;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SAIN.Patches.Hearing;

public class SoundClipNameCheckerPatch : ModulePatch
{
	private static MethodInfo _Player;

	private static FieldInfo _PlayerBridge;

	protected override MethodBase GetTargetMethod()
	{
		_PlayerBridge = AccessTools.Field(typeof(BaseSoundPlayer), "playersBridge");
		_Player = AccessTools.PropertyGetter(_PlayerBridge.FieldType, "iPlayer");
		return AccessTools.Method(typeof(BaseSoundPlayer), "SoundEventHandler", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static void PatchPrefix(string soundName, BaseSoundPlayer __instance)
	{
		if ((Object)(object)BotManagerComponent.Instance != (Object)null)
		{
			object value = _PlayerBridge.GetValue(__instance);
			object obj = _Player.Invoke(value, null);
			Player player = (Player)((obj is Player) ? obj : null);
			SAINSoundTypeHandler.AISoundFileChecker(soundName, player);
		}
	}
}
