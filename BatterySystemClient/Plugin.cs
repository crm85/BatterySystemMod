using BepInEx;
using System;
using System.Collections.Generic;
using Comfort.Common;
using UnityEngine;
using EFT;
using BatterySystem.Configs;
using EFT.InventoryLogic;
using System.Linq;

namespace BatterySystem
{
	/*TODO: 
	 * Enable switching to iron sights when battery runs out
	 * Change background color of empty items
	 * equipping and removing headwear gives infinite nvg
	 * Sound when toggling battery runs out or is removed or added
	 * flir does not require batteries, make recharge craft
	 * battery recharger - idea by Props
	 */
	[BepInPlugin("com.jiro.batterysystem", "BatterySystem", "1.7.0")]
	[BepInDependency("RealismMod", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("me.sol.sain", BepInDependency.DependencyFlags.SoftDependency)]
	//[BepInDependency("com.AKI.core", "3.8.0")]
	public class BatterySystemPlugin : BaseUnityPlugin
	{
		public const string AABatteryId = "5672cb124bdc2d1a0f8b4568";
		public const string CR2032BatteryId = "5672cb304bdc2dc2088b456a";
		public const string CR123BatteryId = "590a358486f77429692b2790";
		public const string CarBatteryId = "5733279d245977289b77ec24";
        public static Dictionary<Item, bool> batteryDictionary = new Dictionary<Item, bool>();
        public static Dictionary<Item, float> batteryDrainMultipliers = new Dictionary<Item, float>();
        //resource drain all batteries that are on // using dictionary to help and sync draining batteries

        public static Inventory localInventory;

		public void Awake()
		{
			BatterySystemConfig.Init(Config);
			if (!BatterySystemConfig.EnableMod.Value) return;

			if (BatterySystemConfig.EnableQuestPresenceDetector.Value)
			{
				QuestPresenceDetector.LoadInterfaceBundle(Logger);
				new QuestPresenceLootItemPatch().Enable();
				new QuestPresencePlayerPatch().Enable();
			}
			new SpawnPatch().Enable();
			new AimSightPatch().Enable();
			if (BatterySystemConfig.EnableHeadsets.Value)
			{
				new UpdatePhonesPatch().Enable();
				if (HeadsetBatteries.HasRealismDeafenController())
				{
					new RealismHeadsetGainPatch().Enable();
					new RealismDeafeningPatch().Enable();
				}
			}
			if (RealismAnalyzerBatteries.HasRealismAnalyzerSupport())
			{
				new RealismCheckForDevicesPatch().Enable();
				new RealismGasAnalyserAudioPatch().Enable();
				new RealismGeigerAudioPatch().Enable();
			}
			if (RealismAquapepsPurification.HasRealismFoodPoisoningSupport())
			{
				EnablePatchSafe("RealismAquapepsDrinkCombinePatch", () => new RealismAquapepsDrinkCombinePatch().Enable());
				EnablePatchSafe("RealismAquapepsContextCombinePatch", () => new RealismAquapepsContextCombinePatch().Enable());
				EnablePatchSafe("RealismAquapepsPoisoningPatch", () => new RealismAquapepsPoisoningPatch().Enable());
			}
			new ApplyItemPatch().Enable();
			new SightDevicePatch().Enable();
			new TacticalDevicePatch().Enable();
			new NvgHeadWearPatch().Enable();
			new ThermalHeadWearPatch().Enable();
			new TrainSummonPatch().Enable();
			EnablePatchSafe("SmokeGrenadeExplosionPatch", () => new SmokeGrenadeExplosionPatch().Enable());
			EnablePatchSafe("M18SmokeEffectSuppressionPatch", () => new M18SmokeEffectSuppressionPatch().Enable());
			EnablePatchSafe("SainRetreatDecisionSmokePatch", SainRetreatDecisionSmokePatch.TryEnable);
            //new FoldableSightPatch().Enable();

            InvokeRepeating(nameof(Heartbeat), 1, 1);
		}

		private void EnablePatchSafe(string patchName, Action enable)
		{
			try
			{
				enable();
			}
			catch (Exception ex)
			{
				Logger.LogWarning($"BatterySystem patch {patchName} failed to enable; continuing without it. {ex}");
			}
		}

		//Gets called every second
		private void Heartbeat()
		{
			BatterySystem.RefreshBatteryDrainStates();
			DrainBatteries();
		}

        private static void DrainBatteries()
		{
			if (!InGame()) return;

			//here?
			var poweredItems = batteryDictionary.Keys.ToArray();
            foreach (Item poweredItem in poweredItems)
			{
				//Is draining disabled on this battery?
				if (!batteryDictionary[poweredItem]) continue;
				
				//for sights, earpiece and tactical devices
				if (BatterySystem.GetBatteryResource(poweredItem) is ResourceComponent batteryResource)
				{
					float drainMultiplier = batteryDrainMultipliers.TryGetValue(poweredItem, out float configuredMultiplier)
						? configuredMultiplier
						: 1f;
					BatterySystem.DrainBattery(poweredItem, drainMultiplier);

					//when battery has no charge left
					if (batteryResource.Value <= 0f)
					{
						BatterySystem.RefreshBatteryDrainStates();
					}
				}
			}
		}

        public static bool InGame()
        {
            return Singleton<GameWorld>.Instance?.MainPlayer?.HealthController.IsAlive == true
                    && !(Singleton<GameWorld>.Instance.MainPlayer is HideoutPlayer);
        }
    }
}
