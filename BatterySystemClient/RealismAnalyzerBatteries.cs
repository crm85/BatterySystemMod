using EFT;
using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Linq;
using System.Reflection;

namespace BatterySystem
{
    public static class RealismAnalyzerBatteries
    {
        private const string GasAnalyzerId = "590a3efd86f77437d351a25b";
        private const string GeigerId = "5672cb724bdc2dc2088b456b";

        private static readonly EquipmentSlot[] AnalyzerSlots =
        {
            EquipmentSlot.TacticalVest,
            EquipmentSlot.ArmBand,
            EquipmentSlot.Pockets
        };

        private static Type _gearControllerType;
        private static Type _realismAudioControllerType;
        private static Item _gasAnalyzerItem;
        private static Item _geigerItem;
        private static bool _gasAnalyzerPowered;
        private static bool _geigerPowered;

        public static bool HasRealismAnalyzerSupport()
        {
            return GetGearControllerType() != null
                && GetRealismAudioControllerType() != null;
        }

        public static bool HasPoweredGasAnalyzer()
        {
            RefreshAnalyzerStates();
            return _gasAnalyzerPowered;
        }

        public static bool HasPoweredGeiger()
        {
            RefreshAnalyzerStates();
            return _geigerPowered;
        }

        public static void TrackBatteries()
        {
            if (!HasRealismAnalyzerSupport()) return;

            RefreshAnalyzerStates();
        }

        public static void RefreshAnalyzerStates()
        {
            if (!HasRealismAnalyzerSupport()) return;
            if (BatterySystemPlugin.localInventory == null) return;

            _gasAnalyzerItem = FindEquippedAnalyzer(GasAnalyzerId);
            _geigerItem = FindEquippedAnalyzer(GeigerId);
            _gasAnalyzerPowered = BatterySystem.HasChargedBattery(_gasAnalyzerItem);
            _geigerPowered = BatterySystem.HasChargedBattery(_geigerItem);

            BatterySystem.TrySetBatteryDrain(_gasAnalyzerItem, _gasAnalyzerPowered);
            BatterySystem.TrySetBatteryDrain(_geigerItem, _geigerPowered);
            SetRealismDeviceState("HasGasAnalyser", _gasAnalyzerItem != null && _gasAnalyzerPowered);
            SetRealismDeviceState("HasGeiger", _geigerItem != null && _geigerPowered);
        }

        private static Item FindEquippedAnalyzer(string templateId)
        {
            foreach (EquipmentSlot slotName in AnalyzerSlots)
            {
                Slot slot = BatterySystemPlugin.localInventory?.Equipment.GetSlot(slotName);
                foreach (Item item in BatterySystem.GetItemsInSlot(slot))
                {
                    if (item?.StringTemplateId == templateId)
                        return item;
                }
            }

            return null;
        }

        private static void SetRealismDeviceState(string propertyName, bool value)
        {
            Type gearControllerType = GetGearControllerType();
            AccessTools.Property(gearControllerType, propertyName)?.SetValue(null, value, null);
        }

        private static Type GetGearControllerType()
        {
            if (_gearControllerType != null) return _gearControllerType;

            _gearControllerType = Type.GetType("RealismMod.GearController, RealismMod", false);
            return _gearControllerType;
        }

        private static Type GetRealismAudioControllerType()
        {
            if (_realismAudioControllerType != null) return _realismAudioControllerType;

            _realismAudioControllerType = Type.GetType("RealismMod.Audio.RealismAudioController, RealismMod", false);
            return _realismAudioControllerType;
        }
    }

    public class RealismCheckForDevicesPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(Type.GetType("RealismMod.GearController, RealismMod", false), "CheckForDevices");
        }

        [PatchPostfix]
        [HarmonyPriority(Priority.Last)]
        public static void PatchPostfix()
        {
            if (!BatterySystemPlugin.InGame()) return;

            RealismAnalyzerBatteries.RefreshAnalyzerStates();
        }
    }

    public class RealismGasAnalyserAudioPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(Type.GetType("RealismMod.Audio.RealismAudioController, RealismMod", false), "DoGasAnalyserAudio");
        }

        [PatchPrefix]
        [HarmonyPriority(Priority.First)]
        public static bool PatchPrefix()
        {
            if (!BatterySystemPlugin.InGame()) return true;

            return RealismAnalyzerBatteries.HasPoweredGasAnalyzer();
        }
    }

    public class RealismGeigerAudioPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(Type.GetType("RealismMod.Audio.RealismAudioController, RealismMod", false), "DoGeigerAudio");
        }

        [PatchPrefix]
        [HarmonyPriority(Priority.First)]
        public static bool PatchPrefix()
        {
            if (!BatterySystemPlugin.InGame()) return true;

            return RealismAnalyzerBatteries.HasPoweredGeiger();
        }
    }
}
