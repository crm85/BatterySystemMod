using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SAIN.Helpers;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SAIN.Patches.Generic;

public class GrenadeThrownActionPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotsController), "method_5", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool PatchPrefix(BotsController __instance, Grenade grenade, Vector3 position, Vector3 force, float mass)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector.DangerPoint(position, force, mass);
		foreach (BotOwner botOwner in __instance.Bots.BotOwners)
		{
			if (SAINPlugin.IsBotExluded(botOwner))
			{
				botOwner.BewareGrenade.AddGrenadeDanger(val, grenade);
			}
		}
		return false;
	}
}
