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
	public class SpawnPatch : ModulePatch
	{
		protected override MethodBase GetTargetMethod()
		{
			return AccessTools.Method(typeof(Player), "Init");
		}

		[PatchPostfix]
		public static async void Postfix(Player __instance, Task __result)
		{
			await __result;

			if (__instance.IsYourPlayer)
			{
                BatterySystemPlugin.localInventory = __instance.InventoryController.Inventory; //Player Inventory
                BatterySystemPlugin.batteryDictionary.Clear();
                BatterySystemPlugin.batteryDrainMultipliers.Clear();
                SightBatteries.sightMods.Clear(); // remove old sight entries that were saved from previous raid
                TacticalDeviceBatteries.lightMods.Clear(); // same for tactical devices
                HeadsetBatteries.SetEarPieceComponents();
				//__instance.OnSightChangedEvent -= sight => BatterySystem.CheckSightIfDraining();
			}
			else//Spawned bots have their batteries drained
            {
                //Delay draining batteries a bit, to allow mods like Realism-Mod to generate them first
                await Task.Delay(1000);

				AddBatteriesToBot(__instance);
			}
		}
		
        private static void AddBatteriesToBot(Player botPlayer)
        {
            Inventory _botInventory = botPlayer.InventoryController.Inventory;
            foreach (Item item in _botInventory.Equipment.GetAllItems().ToArray())
            {
	            if (!(item is CompoundItem compoundItem)) continue;
	            
                foreach (Slot slot in compoundItem.AllSlots.ToArray())
                {
                    if (slot.ContainedItem != null) continue;

                    string batteryTemplateId = BatterySystem.GetBatteryTemplateId(slot);
                    if (batteryTemplateId == null) continue;

					Item battery = Singleton<ItemFactoryClass>.Instance.CreateItem(MongoID.Generate(), batteryTemplateId, null);
                    if (battery == null || !slot.CheckCompatibility(battery)) continue;

                    battery.StackObjectsCount = 1;
                    battery.SpawnedInSession = true;
                    slot.Add(battery, false);
                    DrainSpawnedBattery(battery, botPlayer);
                }
            }
        }

		private static void DrainSpawnedBattery(Item spawnedBattery, Player botPlayer)
        {
            System.Random random = new System.Random();
            //batteries charge depends on their max charge and bot level
            foreach (ResourceComponent batteryResource in spawnedBattery.GetItemComponentsInChildren<ResourceComponent>())
			{
				if (batteryResource.MaxResource <= 0) continue;
				
				//Boss always have full battery
				if(botPlayer.AIData?.BotOwner?.Boss?.IamBoss == true)
                {
                    batteryResource.Value = batteryResource.MaxResource;
                    continue;
                }

                int configuredMin = Mathf.Clamp(Mathf.Min(BatterySystemConfig.SpawnDurabilityMin.Value, BatterySystemConfig.SpawnDurabilityMax.Value), 0, 100);
                int configuredMax = Mathf.Clamp(Mathf.Max(BatterySystemConfig.SpawnDurabilityMin.Value, BatterySystemConfig.SpawnDurabilityMax.Value), 0, 100);
                int chargePercent = random.Next(configuredMin, configuredMax + 1);

                batteryResource.Value = Mathf.Clamp(chargePercent / 100f * batteryResource.MaxResource, 0, batteryResource.MaxResource);
			}
		}
	}
}
