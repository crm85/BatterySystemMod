using SPT.Reflection.Patching;
using BatterySystem.Configs;
using BSG.CameraEffects;
using EFT.Animations;
using EFT.InventoryLogic;
using EFT;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Comfort.Common;
using SPT.Reflection.Utils;

namespace BatterySystem
{
	public class ApplyItemPatch : ModulePatch
	{
		protected override MethodBase GetTargetMethod()
		{
			return typeof(Slot).GetMethod(nameof(Slot.ApplyContainedItem));
		}

		[PatchPostfix]
		static void Postfix(ref Slot __instance) // limit to only player asap
		{
			if (!BatterySystemPlugin.InGame()) return;
			if (!BatterySystem.IsLocalEquipmentSlot(__instance)) return;
			
			HeadsetBatteries.SetEarPieceComponents();
			NightVisionBatteries.SetHeadWearComponents();
			TacticalDeviceBatteries.CheckDeviceIfDraining();
			SightBatteries.CheckSightIfDraining();
		}
	}
}
