using BatterySystem.Configs;
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace BatterySystem
{
    public class HeadsetBatteries
    {
        private static Item headsetItem = null;
        private static ResourceComponent headsetBattery = null;
        private static bool _drainingEarPieceBattery = false;
        public static float compressorMakeup;
        // compressor is used because the default 
        public static float compressor;
        private static float mainVolume;
        private static bool _hasAudioDefaults;
        private static Type _realismDeafenControllerType;

        public static void TrackBatteries()
        {
            if (!BatterySystemConfig.EnableHeadsets.Value) return;
            if (headsetItem == null) return;

            BatterySystem.TrySetBatteryDrain(headsetItem, _drainingEarPieceBattery);
        }
        
        public static void SetEarPieceComponents()
        {
            if (BatterySystemConfig.EnableHeadsets.Value)
            {
                headsetItem = GetEarpiece();
                headsetBattery = BatterySystem.GetBatteryResource(headsetItem);
                CheckEarPieceIfDraining();
                BatterySystem.UpdateBatteryDictionary();
            }
        }

        public static void CheckEarPieceIfDraining()
        {
            _drainingEarPieceBattery = false;
            if (!BatterySystemConfig.EnableHeadsets.Value) return;

            bool hasChargedBattery = headsetBattery != null && headsetBattery.Value > 0f;

            //headset has charged battery installed
            if (hasChargedBattery)
            {
                _drainingEarPieceBattery = true;
                if (!TrySetRealismHeadsetState(true))
                    RestoreVanillaAudio();
            }
            //headset has no battery
            else if (headsetItem != null)
            {
                if (!TrySetRealismHeadsetState(false))
                {
                    Singleton<BetterAudio>.Instance.Master.SetFloat("CompressorMakeup", 0f);
                    Singleton<BetterAudio>.Instance.Master.SetFloat("Compressor", compressor - 15f);
                    Singleton<BetterAudio>.Instance.Master.SetFloat("MainVolume", -10f);
                    _drainingEarPieceBattery = false;
                }
            }
            //no headset equipped
            else
            {
                if (!TrySetRealismHeadsetState(false))
                    RestoreVanillaAudio();
                _drainingEarPieceBattery = false;
            }

            BatterySystem.TrySetBatteryDrain(headsetItem, _drainingEarPieceBattery);
        }

        public static void EnforceRealismHeadsetState()
        {
            if (!BatterySystemConfig.EnableHeadsets.Value) return;
            if (!HasRealismDeafenController()) return;

            headsetItem = GetEarpiece();
            headsetBattery = BatterySystem.GetBatteryResource(headsetItem);
            _drainingEarPieceBattery = headsetBattery != null && headsetBattery.Value > 0f;

            BatterySystem.TrySetBatteryDrain(headsetItem, _drainingEarPieceBattery);
            TrySetRealismHeadsetState(_drainingEarPieceBattery);
        }

        public static void CaptureVanillaAudioDefaults()
        {
            Singleton<BetterAudio>.Instance.Master.GetFloat("Compressor", out compressor);
            Singleton<BetterAudio>.Instance.Master.GetFloat("CompressorMakeup", out compressorMakeup);
            Singleton<BetterAudio>.Instance.Master.GetFloat("MainVolume", out mainVolume);
            _hasAudioDefaults = true;
        }

        private static void RestoreVanillaAudio()
        {
            if (!_hasAudioDefaults) return;

            Singleton<BetterAudio>.Instance.Master.SetFloat("Compressor", compressor);
            Singleton<BetterAudio>.Instance.Master.SetFloat("CompressorMakeup", compressorMakeup);
            Singleton<BetterAudio>.Instance.Master.SetFloat("MainVolume", mainVolume);
        }

        private static bool TrySetRealismHeadsetState(bool hasPoweredHeadset)
        {
            Type deafenControllerType = GetRealismDeafenControllerType();
            if (deafenControllerType == null) return false;

            PropertyInfo hasHeadSetProperty = AccessTools.Property(deafenControllerType, "HasHeadSet");
            if (hasHeadSetProperty == null) return false;

            hasHeadSetProperty.SetValue(null, hasPoweredHeadset, null);
            if (!hasPoweredHeadset)
            {
                AccessTools.Property(deafenControllerType, "HeadSetGain")?.SetValue(null, GetRealismMinGain(deafenControllerType), null);
                AccessTools.Property(deafenControllerType, "EarProtectionFactor")?.SetValue(null, GetHelmetProtectionFactor(), null);
            }

            return true;
        }

        public static bool HasRealismDeafenController()
        {
            return GetRealismDeafenControllerType() != null;
        }

        private static Type GetRealismDeafenControllerType()
        {
            if (_realismDeafenControllerType != null) return _realismDeafenControllerType;

            _realismDeafenControllerType = Type.GetType("RealismMod.DeafenController, RealismMod", false);
            return _realismDeafenControllerType;
        }

        private static float GetRealismMinGain(Type deafenControllerType)
        {
            FieldInfo minGainField = AccessTools.Field(deafenControllerType, "MinGain");
            if (minGainField?.GetRawConstantValue() is object minGain)
                return Convert.ToSingle(minGain);

            return -10f;
        }

        private static float GetHelmetProtectionFactor()
        {
            Item headwear = BatterySystemPlugin.localInventory?.Equipment.GetSlot(EquipmentSlot.Headwear)?.ContainedItem;
            if (headwear is CompoundItem && headwear is ArmorItemClass armorItem && armorItem.Armor != null)
            {
                switch (armorItem.Armor.Deaf)
                {
                    case EDeafStrength.Low:
                        return 0.9f;
                    case EDeafStrength.High:
                        return 0.8f;
                }
            }

            return 1f;
        }

        private static Item GetEarpiece()
        {
            if (BatterySystemPlugin.localInventory == null) return null;
            //Try get headphones from "Earpiece" slot
            if(BatterySystemPlugin.localInventory.Equipment.GetSlot(EquipmentSlot.Earpiece).ContainedItem is Item headphones) return headphones;
            //Try get headphones from helmet attachment slot
            if (BatterySystemPlugin.localInventory.Equipment.GetSlot(EquipmentSlot.Headwear).ContainedItem is CompoundItem helmet)
            {
                foreach (Item helmetAttachment in helmet.GetAllItems())
                {
                    if (helmetAttachment is HeadphonesItemClass)
                        return helmetAttachment;
                }
            }

            return null;
        }
    }

    public class UpdatePhonesPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod(nameof(Player.UpdatePhones));
        }
        [PatchPostfix]
        [HarmonyPriority(Priority.Last)]
        public static void PatchPostfix(ref Player __instance) //BetterAudio __instance
        {
            if (!BatterySystemPlugin.InGame()) return;
            if (!__instance.IsYourPlayer) return;
            
            HeadsetBatteries.CaptureVanillaAudioDefaults();
            HeadsetBatteries.SetEarPieceComponents();
        }
    }

    public class RealismHeadsetGainPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            Type headsetGainControllerType = Type.GetType("RealismMod.HeadsetGainController, RealismMod", false);
            return AccessTools.Method(headsetGainControllerType, "AdjustHeadsetVolume");
        }

        [PatchPrefix]
        [HarmonyPriority(Priority.First)]
        public static void PatchPrefix()
        {
            if (!BatterySystemPlugin.InGame()) return;

            HeadsetBatteries.EnforceRealismHeadsetState();
        }
    }

    public class RealismDeafeningPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            Type deafenControllerType = Type.GetType("RealismMod.DeafenController, RealismMod", false);
            return AccessTools.Method(deafenControllerType, "DoDeafening");
        }

        [PatchPrefix]
        [HarmonyPriority(Priority.First)]
        public static void PatchPrefix()
        {
            if (!BatterySystemPlugin.InGame()) return;

            HeadsetBatteries.EnforceRealismHeadsetState();
        }
    }

}
