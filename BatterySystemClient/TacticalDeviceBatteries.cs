using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using SPT.Reflection.Patching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine;

namespace BatterySystem
{
    public class TacticalDeviceBatteries
    {
        public static Dictionary<TacticalComboVisualController, ResourceComponent> lightMods = new Dictionary<TacticalComboVisualController, ResourceComponent>();
        private static bool _drainingTacDeviceBattery;

        public static void TrackBatteries()
        {
            var lightModKeys = lightMods.Keys.ToArray();
            foreach (TacticalComboVisualController deviceController in lightModKeys) // tactical devices on active weapon
            {
                if (deviceController?.LightMod?.Item == null || !IsInActiveSlot(deviceController.LightMod.Item)) continue;

                BatterySystem.TrySetBatteryDrain(
                    deviceController.LightMod.Item,
                    deviceController.LightMod.IsActive && lightMods[deviceController]?.Value > 0);
            }

            TrackLightComponents(BatterySystemPlugin.localInventory?.Equipment.GetSlot(EquipmentSlot.Headwear));
            TrackLightComponents(Singleton<GameWorld>.Instance?.MainPlayer?.ActiveSlot);
        }

        public static void SetDeviceComponents(TacticalComboVisualController deviceInstance)
        {
            if (deviceInstance?.LightMod?.Item == null) return;

            var lightModKeys = lightMods.Keys.ToArray();
            foreach (TacticalComboVisualController deviceController in lightModKeys)
                if (deviceController?.LightMod?.Item == null || !IsInActiveSlot(deviceController.LightMod.Item))
                    lightMods.Remove(deviceController);

            if (IsInActiveSlot(deviceInstance.LightMod.Item) && BatterySystem.HasBatterySlot(deviceInstance.LightMod.Item))
            {
                // if sight is already in dictionary, dont add it
                if (!lightMods.Keys.Any(key => key?.LightMod?.Item == deviceInstance.LightMod.Item))
                    lightMods.Add(deviceInstance, BatterySystem.GetBatteryResource(deviceInstance.LightMod.Item));
            }
            CheckDeviceIfDraining();
            BatterySystem.UpdateBatteryDictionary();
        }
        
        public static void CheckDeviceIfDraining()
        {
            var lightModKeys = lightMods.Keys.ToArray();
            foreach (TacticalComboVisualController deviceController in lightModKeys) {
                if (deviceController?.LightMod?.Item == null || !IsInActiveSlot(deviceController.LightMod.Item))
                {
                    lightMods.Remove(deviceController);
                    continue;
                }

                ResourceComponent deviceBattery = BatterySystem.GetBatteryResource(deviceController.LightMod.Item);
                lightMods[deviceController] = deviceBattery;
                _drainingTacDeviceBattery = (deviceBattery != null && deviceController.LightMod.IsActive && deviceBattery.Value > 0 && IsInActiveSlot(deviceController.LightMod.Item));

                BatterySystem.TrySetBatteryDrain(deviceController.LightMod.Item, _drainingTacDeviceBattery);

                SetDeviceActive(deviceController, _drainingTacDeviceBattery);
            }

            TrackLightComponents(BatterySystemPlugin.localInventory?.Equipment.GetSlot(EquipmentSlot.Headwear));
            TrackLightComponents(Singleton<GameWorld>.Instance?.MainPlayer?.ActiveSlot);
        }

        private static void TrackLightComponents(Slot slot)
        {
            foreach (Item item in BatterySystem.GetItemsInSlot(slot))
            {
                LightComponent lightComponent = item.GetItemComponent<LightComponent>();
                if (lightComponent == null || !BatterySystem.HasBatterySlot(item)) continue;

                bool hasChargedBattery = BatterySystem.HasChargedBattery(item);
                if (lightComponent.IsActive && !hasChargedBattery)
                    lightComponent.IsActive = false;

                BatterySystem.TrySetBatteryDrain(item, lightComponent.IsActive && hasChargedBattery);
            }
        }

        private static void SetDeviceActive(TacticalComboVisualController deviceController, bool active)
        {
            //Also turn off the device when out of battery (so bots don't spot the player because the light is enabled)
            deviceController.LightMod.IsActive = active;
            
            foreach (LaserBeam laser in deviceController.gameObject.GetComponentsInChildren<LaserBeam>(true))
                laser.gameObject.gameObject.SetActive(active);
            foreach (Light light in deviceController.gameObject.GetComponentsInChildren<Light>(true))
                light.gameObject.gameObject.SetActive(active);
        }

        public static bool IsInActiveSlot(Item tacticalDevice)
        {
            if(BatterySystem.IsInSlot(tacticalDevice, Singleton<GameWorld>.Instance?.MainPlayer.ActiveSlot)) return true;
            if(BatterySystem.IsInSlot(tacticalDevice, Singleton<GameWorld>.Instance?.MainPlayer.Inventory.Equipment.GetSlot(EquipmentSlot.Headwear))) return true;

            return false;
        }
    }
    public class TacticalDevicePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(TacticalComboVisualController).GetMethod(nameof(TacticalComboVisualController.UpdateBeams));
        }

        [PatchPostfix]
        static void Postfix(ref TacticalComboVisualController __instance)
        {
            //only sights on equipped weapon are added
            if (!BatterySystemPlugin.InGame()) return;
            if (!TacticalDeviceBatteries.IsInActiveSlot(__instance?.LightMod?.Item)) return;

            TacticalDeviceBatteries.SetDeviceComponents(__instance);
        }
    }
}
