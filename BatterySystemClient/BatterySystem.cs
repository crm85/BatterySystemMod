using System.Linq;
using System.Reflection;
using SPT.Reflection.Patching;
using HarmonyLib;
using Comfort.Common;
using UnityEngine;
using EFT;
using EFT.InventoryLogic;
using BSG.CameraEffects;
using BatterySystem.Configs;
using System.Threading.Tasks;
using BepInEx.Logging;
using System.Collections.Generic;
using EFT.CameraControl;
using EFT.Animations;
using System.Collections;
using EFT.Visual;

namespace BatterySystem
{
	public class BatterySystem
	{
		public static readonly string[] BatteryTemplateIds =
		{
			BatterySystemPlugin.AABatteryId,
			BatterySystemPlugin.CR2032BatteryId,
			BatterySystemPlugin.CR123BatteryId,
			BatterySystemPlugin.CarBatteryId
		};

        public static void UpdateBatteryDictionary()
		{
			if (BatterySystemPlugin.localInventory == null) return;

			// Remove unequipped items
			var batteryKeys = BatterySystemPlugin.batteryDictionary.Keys.ToArray();
            foreach (Item key in batteryKeys)
			{
				if (IsInLocalEquipment(key)) continue;

				BatterySystemPlugin.batteryDictionary.Remove(key);
			}

			HeadsetBatteries.TrackBatteries();
			NightVisionBatteries.TrackBatteries();
			SightBatteries.TrackBatteries();
			TacticalDeviceBatteries.TrackBatteries();
		}

        public static bool IsInSlot(Item item, Slot slot)
        {
            if (item == null) return false;
            if (slot == null) return false;
            if (slot.ContainedItem == null) return false;

            return item.IsChildOf(slot.ContainedItem);
        }

        public static bool IsInLocalEquipment(Item item)
        {
	        if (item == null) return false;
	        if (BatterySystemPlugin.localInventory == null) return false;

	        if (IsInSlot(item, BatterySystemPlugin.localInventory.Equipment.GetSlot(EquipmentSlot.Earpiece))) return true;
	        if (IsInSlot(item, BatterySystemPlugin.localInventory.Equipment.GetSlot(EquipmentSlot.Headwear))) return true;
	        if (IsInSlot(item, Singleton<GameWorld>.Instance?.MainPlayer?.ActiveSlot)) return true;

	        return false;
        }

        public static bool IsLocalEquipmentSlot(Slot slot)
        {
	        if (slot == null) return false;

	        return IsInLocalEquipment(slot.ContainedItem) || IsInLocalEquipment(slot.ParentItem);
        }

        public static bool HasBatterySlot(Item item)
        {
	        return item is CompoundItem compoundItem && compoundItem.AllSlots.Any(IsBatterySlot);
        }

        public static bool IsBatterySlot(Slot slot)
        {
	        return GetBatteryTemplateId(slot) != null;
        }

        public static string GetBatteryTemplateId(Slot slot)
        {
	        var filter = slot?.Filters?.FirstOrDefault()?.Filter;
	        if (filter == null) return null;

	        return BatteryTemplateIds.FirstOrDefault(templateId => filter.Contains(templateId));
        }

        public static bool IsBatteryItem(Item item)
        {
	        return item != null && BatteryTemplateIds.Contains(item.StringTemplateId);
        }
    }
}
