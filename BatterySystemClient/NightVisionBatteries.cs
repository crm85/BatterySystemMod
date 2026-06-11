using BatterySystem.Configs;
using BepInEx.Configuration;
using BSG.CameraEffects;
using EFT;
using EFT.InventoryLogic;
using SPT.Reflection.Patching;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BatterySystem
{
    public class NightVisionBatteries
    {
        private static Dictionary<string, float> deviceDrainMultiplier = new Dictionary<string, float>
        {
            { "5c0696830db834001d23f5da", 1f },// PNV-10T Night Vision Goggles, AA Battery
            { "5c0558060db834001b735271", 2f },// GPNVG-18 Night Vision goggles, CR123 battery pack
            { "5c066e3a0db834001b7353f0", 1f },// Armasight N-15 Night Vision Goggles, single CR123A lithium battery
            { "57235b6f24597759bf5a30f1", 0.5f },// AN/PVS-14 Night Vision Monocular, AA Battery
            { "5c110624d174af029e69734c", 3f },// T-7 Thermal Goggles with a Night Vision mount, Double AA
        };

        public static Item NightVisionItem = null;
        private static NightVisionComponent _nvgDevice = null;
        private static ThermalVisionComponent _thermalDevice = null;
        private static bool _drainingNightVisionBattery = false;
        public static ResourceComponent NightVisionBattery = null;

        public static void SetHeadWearComponents()
        {
            Item headwearItem = BatterySystem.GetHeadwearSlot()?.ContainedItem;
            _nvgDevice = headwearItem?.GetItemComponentsInChildren<NightVisionComponent>().FirstOrDefault(); //default null else nvg item
            _thermalDevice = headwearItem?.GetItemComponentsInChildren<ThermalVisionComponent>().FirstOrDefault(); //default null else thermal item
            NightVisionItem = GetHeadwearSight();
            NightVisionBattery = BatterySystem.GetBatteryResource(NightVisionItem); //default null else resource

            CheckHeadWearIfDraining();
            BatterySystem.UpdateBatteryDictionary();
        }

        public static void TrackBatteries()
        {
            Item headwearSight = GetHeadwearSight();
            if (headwearSight == null) return;
            
            BatterySystem.TrySetBatteryDrain(headwearSight, _drainingNightVisionBattery, GetDrainMultiplier(headwearSight));
        }

        public static Item GetHeadwearSight() // returns the special device goggles that are equipped
        {
            if (_nvgDevice != null)
                return _nvgDevice.Item;
            if (_thermalDevice != null)
                return _thermalDevice.Item;

            return null;
        }

        public static void CheckHeadWearIfDraining()
        {
            bool hasChargedBattery = NightVisionBattery != null && NightVisionBattery.Value > 0;
            bool nvgShouldRun = hasChargedBattery
                && _nvgDevice != null
                && ((ITogglableComponentContainer)_nvgDevice).Togglable.On
                && CameraClass.Instance?.NightVision?.InProcessSwitching == false;
            bool thermalShouldRun = hasChargedBattery
                && _thermalDevice != null
                && ((ITogglableComponentContainer)_thermalDevice).Togglable.On
                && CameraClass.Instance?.ThermalVision?.InProcessSwitching == false;

            _drainingNightVisionBattery = nvgShouldRun || thermalShouldRun;

            Item headwearSight = GetHeadwearSight();
            BatterySystem.TrySetBatteryDrain(headwearSight, _drainingNightVisionBattery, GetDrainMultiplier(headwearSight));

            if (CameraClass.Instance?.NightVision != null)
                CameraClass.Instance.NightVision.On = nvgShouldRun;
            if (CameraClass.Instance?.ThermalVision != null)
                CameraClass.Instance.ThermalVision.On = thermalShouldRun;
        }

        private static float GetDrainMultiplier(Item item)
        {
            if (item != null && deviceDrainMultiplier.TryGetValue(item.StringTemplateId, out float drainMultiplier))
                return drainMultiplier;

            return 1f;
        }
    }

    public class NvgHeadWearPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(NightVision).GetMethod(nameof(NightVision.StartSwitch));
        }

        [PatchPostfix]
        static void Postfix(ref NightVision __instance)
        {
            if (!BatterySystemPlugin.InGame()) return;
            if (__instance.name != "FPS Camera") return;
            
            if (__instance.InProcessSwitching)
                StaticManager.BeginCoroutine(IsNVSwitching(__instance));
            else 
                NightVisionBatteries.SetHeadWearComponents();
        }
        //waits until InProcessSwitching is false and then 
        private static IEnumerator IsNVSwitching(NightVision nv)
        {
            while (nv.InProcessSwitching)
                yield return new WaitForSeconds(1f / 100f);
                
            NightVisionBatteries.SetHeadWearComponents();
        }
    }

    public class ThermalHeadWearPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(ThermalVision).GetMethod(nameof(ThermalVision.StartSwitch));
        }

        [PatchPostfix]
        static void Postfix(ref ThermalVision __instance)
        {
            if (!BatterySystemPlugin.InGame()) return;
            if (__instance.name != "FPS Camera") return;

            if (__instance.InProcessSwitching)
                StaticManager.BeginCoroutine(IsThermalSwitching(__instance));
            else 
                NightVisionBatteries.SetHeadWearComponents();
        }
        private static IEnumerator IsThermalSwitching(ThermalVision tv)
        {
            while (tv.InProcessSwitching)
                yield return new WaitForSeconds(1f / 100f);
            
            NightVisionBatteries.SetHeadWearComponents();
        }
    }
}
