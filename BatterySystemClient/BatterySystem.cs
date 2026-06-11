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
				BatterySystemPlugin.batteryDrainMultipliers.Remove(key);
			}

			HeadsetBatteries.TrackBatteries();
			RealismAnalyzerBatteries.TrackBatteries();
			NightVisionBatteries.TrackBatteries();
			SightBatteries.TrackBatteries();
			TacticalDeviceBatteries.TrackBatteries();
		}

		public static void RefreshBatteryDrainStates()
		{
			if (BatterySystemPlugin.localInventory == null) return;

			HeadsetBatteries.SetEarPieceComponents();
			RealismAnalyzerBatteries.RefreshAnalyzerStates();
			NightVisionBatteries.SetHeadWearComponents();
			TacticalDeviceBatteries.CheckDeviceIfDraining();
			SightBatteries.CheckSightIfDraining();
			UpdateBatteryDictionary();
		}

		public static IEnumerable<Item> GetItemsInSlot(Slot slot)
		{
			Item containedItem = slot?.ContainedItem;
			if (containedItem == null) yield break;

			yield return containedItem;

			if (containedItem is CompoundItem compoundItem)
			{
				foreach (Item childItem in compoundItem.GetAllItems())
				{
					if (childItem != null && childItem != containedItem)
						yield return childItem;
				}
			}
		}

		public static ResourceComponent GetBatteryResource(Item item)
		{
			if (item == null) return null;

			return item.GetItemComponentsInChildren<ResourceComponent>(false)
				.FirstOrDefault(component => IsBatteryItem(component?.Item));
		}

		public static bool HasChargedBattery(Item item)
		{
			ResourceComponent batteryResource = GetBatteryResource(item);
			return batteryResource != null && batteryResource.Value > 0f;
		}

		public static bool DrainBattery(Item item, float drainMultiplier = 1f)
		{
			float drainAmount = 1 / 100f * BatterySystemConfig.DrainMultiplier.Value * Mathf.Max(0f, drainMultiplier);
			return DrainBatteryCharge(item, drainAmount);
		}

        public static bool DrainBatteryCharge(Item item, float drainAmount)
		{
			ResourceComponent batteryResource = GetBatteryResource(item);
			if (batteryResource == null) return false;

			float oldValue = batteryResource.Value;
			batteryResource.Value = Mathf.Clamp(oldValue - Mathf.Max(0f, drainAmount), 0f, batteryResource.MaxResource);
			if (!Mathf.Approximately(oldValue, batteryResource.Value))
			{
				batteryResource.Item?.UpdateAttributes();
				item?.UpdateAttributes();
			}

			return batteryResource.Value > 0f;
		}

		public static bool TrySetBatteryDrain(Item item, bool isDraining, float drainMultiplier = 1f)
		{
			if (item == null) return false;
			if (!HasBatterySlot(item) && GetBatteryResource(item) == null) return false;
			if (!IsInLocalEquipment(item)) return false;

			BatterySystemPlugin.batteryDictionary[item] = isDraining;
			BatterySystemPlugin.batteryDrainMultipliers[item] = Mathf.Max(0f, drainMultiplier);
			return true;
		}

		public static Slot GetHeadwearSlot()
		{
			return BatterySystemPlugin.localInventory?.Equipment.GetSlot(EquipmentSlot.Headwear);
		}

        public static bool IsInSlot(Item item, Slot slot)
        {
            if (item == null) return false;
            if (slot == null) return false;
            if (slot.ContainedItem == null) return false;

            return item == slot.ContainedItem || item.IsChildOf(slot.ContainedItem);
        }

        public static bool IsInLocalEquipment(Item item)
        {
	        if (item == null) return false;
	        if (BatterySystemPlugin.localInventory == null) return false;

	        if (IsInSlot(item, BatterySystemPlugin.localInventory.Equipment.GetSlot(EquipmentSlot.Earpiece))) return true;
	        if (IsInSlot(item, BatterySystemPlugin.localInventory.Equipment.GetSlot(EquipmentSlot.Headwear))) return true;
	        if (IsInSlot(item, BatterySystemPlugin.localInventory.Equipment.GetSlot(EquipmentSlot.TacticalVest))) return true;
	        if (IsInSlot(item, BatterySystemPlugin.localInventory.Equipment.GetSlot(EquipmentSlot.ArmBand))) return true;
	        if (IsInSlot(item, BatterySystemPlugin.localInventory.Equipment.GetSlot(EquipmentSlot.Pockets))) return true;
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
