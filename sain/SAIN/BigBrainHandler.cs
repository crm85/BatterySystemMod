using System;
using System.Collections.Generic;
using System.Reflection;
using DrakiaXYZ.BigBrain.Brains;
using EFT;
using SAIN.Layers;
using SAIN.Layers.Combat.Run;
using SAIN.Layers.Combat.Solo;
using SAIN.Layers.Combat.Squad;
using SAIN.Preset.GlobalSettings;
using SAIN.Preset.GlobalSettings.Categories;

namespace SAIN;

public class BigBrainHandler
{
	public class BrainAssignment
	{
		private static VanillaBotSettings _vanillaBotSettings => SAINPlugin.LoadedPreset.GlobalSettings.General.VanillaBots;

		public static void Init()
		{
			addCustomLayersToPMCs();
			addCustomLayersToScavs();
			addCustomLayersToRaiders(new List<WildSpawnType> { (WildSpawnType)9 });
			addCustomLayersToRogues();
			addCustomLayersToBloodHounds();
			addCustomLayersToBosses();
			addCustomLayersToFollowers();
			addCustomLayersToGoons();
			addCustomLayersToOthers();
			ToggleVanillaLayersForPMCs(useVanillaLayers: false);
			ToggleVanillaLayersForOthers(useVanillaLayers: false);
			ToggleVanillaLayersForAllBots();
		}

		public static void ToggleVanillaLayersForAllBots()
		{
			ToggleVanillaLayersForScavs(_vanillaBotSettings.VanillaScavs);
			ToggleVanillaLayersForRogues(_vanillaBotSettings.VanillaRogues);
			ToggleVanillaLayersForRaiders(new List<WildSpawnType> { (WildSpawnType)9 }, useVanillaLayers: false);
			ToggleVanillaLayersForBloodHounds(_vanillaBotSettings.VanillaBloodHounds);
			ToggleVanillaLayersForBosses(_vanillaBotSettings.VanillaBosses);
			ToggleVanillaLayersForFollowers(_vanillaBotSettings.VanillaFollowers);
			ToggleVanillaLayersForGoons(_vanillaBotSettings.VanillaGoons);
		}

		public static void ToggleVanillaLayersForPMCs(bool useVanillaLayers)
		{
			List<string> brainList = getBrainList(AIBrains.PMCs);
			List<string> list = new List<string> { "Request", "KnightFight", "PmcBear", "PmcUsec" };
			list.AddRange(commonVanillaLayersToRemove);
			toggleVanillaLayers(brainList, list, useVanillaLayers);
			bool flag = true;
			ToggleVanillaLayersForRaiders(new List<WildSpawnType>
			{
				(WildSpawnType)51,
				(WildSpawnType)52
			}, useVanillaLayers);
		}

		public static void ToggleVanillaLayersForScavs(bool useVanillaLayers)
		{
			List<string> brainList = getBrainList(AIBrains.Scavs);
			List<string> list = new List<string> { "PmcBear", "PmcUsec" };
			list.AddRange(commonVanillaLayersToRemove);
			toggleVanillaLayers(brainList, list, useVanillaLayers);
			ToggleVanillaLayersForRaiders(new List<WildSpawnType> { (WildSpawnType)19 }, useVanillaLayers);
		}

		public static void ToggleVanillaLayersForRaiders(List<WildSpawnType> roles, bool useVanillaLayers)
		{
			List<string> brainNames = new List<string> { Brain.PMC.ToString() };
			List<string> list = new List<string> { "Request", "KnightFight", "PmcBear", "PmcUsec" };
			list.AddRange(commonVanillaLayersToRemove);
			toggleVanillaLayers(brainNames, list, roles, useVanillaLayers);
		}

		public static void ToggleVanillaLayersForOthers(bool useVanillaLayers)
		{
			List<string> brainList = getBrainList(AIBrains.Others);
			List<string> list = new List<string> { "Request", "KnightFight", "PmcBear", "PmcUsec" };
			list.AddRange(commonVanillaLayersToRemove);
			toggleVanillaLayers(brainList, list, useVanillaLayers);
		}

		public static void ToggleVanillaLayersForRogues(bool useVanillaLayers)
		{
			List<string> brainNames = new List<string> { Brain.ExUsec.ToString() };
			List<string> list = new List<string> { "Request", "KnightFight", "PmcBear", "PmcUsec" };
			list.AddRange(commonVanillaLayersToRemove);
			toggleVanillaLayers(brainNames, list, useVanillaLayers);
		}

		public static void ToggleVanillaLayersForBloodHounds(bool useVanillaLayers)
		{
			List<string> brainNames = new List<string> { Brain.ArenaFighter.ToString() };
			List<string> list = new List<string> { "Request", "KnightFight", "PmcBear", "PmcUsec" };
			list.AddRange(commonVanillaLayersToRemove);
			toggleVanillaLayers(brainNames, list, useVanillaLayers);
		}

		public static void ToggleVanillaLayersForBosses(bool useVanillaLayers)
		{
			List<string> brainList = getBrainList(AIBrains.Bosses);
			List<string> list = new List<string> { "KnightFight", "BirdEyeFight", "BossBoarFight" };
			list.AddRange(commonVanillaLayersToRemove);
			toggleVanillaLayers(brainList, list, useVanillaLayers);
		}

		public static void ToggleVanillaLayersForFollowers(bool useVanillaLayers)
		{
			List<string> brainList = getBrainList(AIBrains.Followers);
			List<string> list = new List<string> { "KnightFight", "BoarGrenadeDanger" };
			list.AddRange(commonVanillaLayersToRemove);
			toggleVanillaLayers(brainList, list, useVanillaLayers);
		}

		public static void ToggleVanillaLayersForGoons(bool useVanillaLayers)
		{
			List<string> brainList = getBrainList(AIBrains.Goons);
			List<string> list = new List<string> { "KnightFight", "BirdEyeFight", "Kill logic" };
			list.AddRange(commonVanillaLayersToRemove);
			toggleVanillaLayers(brainList, list, useVanillaLayers);
		}

		private static void toggleVanillaLayers(List<string> brainNames, List<string> layerNames, bool useVanillaLayers)
		{
			if (useVanillaLayers)
			{
				BrainManager.RemoveLayers(SAINLayerNames, brainNames);
				BrainManager.RestoreLayers(layerNames, brainNames);
			}
			else
			{
				checkExtractEnabled(layerNames);
				BrainManager.RestoreLayers(SAINLayerNames, brainNames);
				BrainManager.RemoveLayers(layerNames, brainNames);
			}
		}

		private static void toggleVanillaLayers(List<string> brainNames, List<string> layerNames, List<WildSpawnType> roles, bool useVanillaLayers)
		{
			if (useVanillaLayers)
			{
				BrainManager.RemoveLayers(SAINLayerNames, brainNames, roles);
				BrainManager.RestoreLayers(layerNames, brainNames, roles);
			}
			else
			{
				checkExtractEnabled(layerNames);
				BrainManager.RestoreLayers(SAINLayerNames, brainNames, roles);
				BrainManager.RemoveLayers(layerNames, brainNames, roles);
			}
		}

		private static void addCustomLayersToPMCs()
		{
			List<string> brainList = getBrainList(AIBrains.PMCs);
			LayerSettings layers = SAINPlugin.LoadedPreset.GlobalSettings.General.Layers;
			BrainManager.AddCustomLayer(typeof(DebugLayer), brainList, 99);
			BrainManager.AddCustomLayer(typeof(SAINAvoidThreatLayer), brainList, 80);
			BrainManager.AddCustomLayer(typeof(ExtractLayer), brainList, layers.SAINExtractLayerPriority);
			BrainManager.AddCustomLayer(typeof(CombatSquadLayer), brainList, layers.SAINCombatSquadLayerPriority);
			BrainManager.AddCustomLayer(typeof(CombatSoloLayer), brainList, layers.SAINCombatSoloLayerPriority);
			bool flag = true;
			addCustomLayersToRaiders(new List<WildSpawnType>
			{
				(WildSpawnType)51,
				(WildSpawnType)52
			});
		}

		private static void addCustomLayersToScavs()
		{
			List<string> brainList = getBrainList(AIBrains.Scavs);
			LayerSettings layers = SAINPlugin.LoadedPreset.GlobalSettings.General.Layers;
			BrainManager.AddCustomLayer(typeof(DebugLayer), brainList, 99);
			BrainManager.AddCustomLayer(typeof(SAINAvoidThreatLayer), brainList, 80);
			BrainManager.AddCustomLayer(typeof(ExtractLayer), brainList, layers.SAINExtractLayerPriority);
			BrainManager.AddCustomLayer(typeof(CombatSquadLayer), brainList, layers.SAINCombatSquadLayerPriority);
			BrainManager.AddCustomLayer(typeof(CombatSoloLayer), brainList, layers.SAINCombatSoloLayerPriority);
			addCustomLayersToRaiders(new List<WildSpawnType> { (WildSpawnType)19 });
		}

		private static void addCustomLayersToRaiders(List<WildSpawnType> roles)
		{
			LayerSettings layers = SAINPlugin.LoadedPreset.GlobalSettings.General.Layers;
			List<string> list = new List<string> { Brain.PMC.ToString() };
			BrainManager.AddCustomLayer(typeof(DebugLayer), list, 99, roles);
			BrainManager.AddCustomLayer(typeof(SAINAvoidThreatLayer), list, 80, roles);
			BrainManager.AddCustomLayer(typeof(ExtractLayer), list, layers.SAINExtractLayerPriority, roles);
			BrainManager.AddCustomLayer(typeof(CombatSquadLayer), list, layers.SAINCombatSquadLayerPriority, roles);
			BrainManager.AddCustomLayer(typeof(CombatSoloLayer), list, layers.SAINCombatSoloLayerPriority, roles);
		}

		private static void addCustomLayersToOthers()
		{
			List<string> brainList = getBrainList(AIBrains.Others);
			LayerSettings layers = SAINPlugin.LoadedPreset.GlobalSettings.General.Layers;
			BrainManager.AddCustomLayer(typeof(DebugLayer), brainList, 99);
			BrainManager.AddCustomLayer(typeof(SAINAvoidThreatLayer), brainList, 80);
			BrainManager.AddCustomLayer(typeof(ExtractLayer), brainList, layers.SAINExtractLayerPriority);
			BrainManager.AddCustomLayer(typeof(CombatSquadLayer), brainList, layers.SAINCombatSquadLayerPriority);
			BrainManager.AddCustomLayer(typeof(CombatSoloLayer), brainList, layers.SAINCombatSoloLayerPriority);
		}

		private static void addCustomLayersToRogues()
		{
			List<string> list = new List<string>();
			list.Add(Brain.ExUsec.ToString());
			LayerSettings layers = SAINPlugin.LoadedPreset.GlobalSettings.General.Layers;
			BrainManager.AddCustomLayer(typeof(DebugLayer), list, 99);
			BrainManager.AddCustomLayer(typeof(SAINAvoidThreatLayer), list, 80);
			BrainManager.AddCustomLayer(typeof(ExtractLayer), list, layers.SAINExtractLayerPriority);
			BrainManager.AddCustomLayer(typeof(CombatSquadLayer), list, layers.SAINCombatSquadLayerPriority);
			BrainManager.AddCustomLayer(typeof(CombatSoloLayer), list, layers.SAINCombatSoloLayerPriority);
		}

		private static void addCustomLayersToBloodHounds()
		{
			List<string> list = new List<string>();
			list.Add(Brain.ArenaFighter.ToString());
			LayerSettings layers = SAINPlugin.LoadedPreset.GlobalSettings.General.Layers;
			BrainManager.AddCustomLayer(typeof(DebugLayer), list, 99);
			BrainManager.AddCustomLayer(typeof(SAINAvoidThreatLayer), list, 80);
			BrainManager.AddCustomLayer(typeof(ExtractLayer), list, layers.SAINExtractLayerPriority);
			BrainManager.AddCustomLayer(typeof(CombatSquadLayer), list, layers.SAINCombatSquadLayerPriority);
			BrainManager.AddCustomLayer(typeof(CombatSoloLayer), list, layers.SAINCombatSoloLayerPriority);
		}

		private static void addCustomLayersToBosses()
		{
			List<string> brainList = getBrainList(AIBrains.Bosses);
			GeneralSettings general = SAINPlugin.LoadedPreset.GlobalSettings.General;
			BrainManager.AddCustomLayer(typeof(DebugLayer), brainList, 99);
			BrainManager.AddCustomLayer(typeof(SAINAvoidThreatLayer), brainList, 80);
			BrainManager.AddCustomLayer(typeof(CombatSquadLayer), brainList, 70);
			BrainManager.AddCustomLayer(typeof(CombatSoloLayer), brainList, 69);
		}

		private static void addCustomLayersToFollowers()
		{
			List<string> brainList = getBrainList(AIBrains.Followers);
			GeneralSettings general = SAINPlugin.LoadedPreset.GlobalSettings.General;
			BrainManager.AddCustomLayer(typeof(DebugLayer), brainList, 99);
			BrainManager.AddCustomLayer(typeof(SAINAvoidThreatLayer), brainList, 80);
			BrainManager.AddCustomLayer(typeof(CombatSquadLayer), brainList, 70);
			BrainManager.AddCustomLayer(typeof(CombatSoloLayer), brainList, 69);
		}

		private static void addCustomLayersToGoons()
		{
			List<string> brainList = getBrainList(AIBrains.Goons);
			BrainManager.AddCustomLayer(typeof(DebugLayer), brainList, 99);
			BrainManager.AddCustomLayer(typeof(SAINAvoidThreatLayer), brainList, 80);
			BrainManager.AddCustomLayer(typeof(CombatSquadLayer), brainList, 64);
			BrainManager.AddCustomLayer(typeof(CombatSoloLayer), brainList, 62);
		}

		private static void checkExtractEnabled(List<string> layersToRemove)
		{
			if (GlobalSettingsClass.Instance.General.Extract.SAIN_EXTRACT_TOGGLE)
			{
				layersToRemove.Add("Exfiltration");
			}
		}

		private static List<string> getBrainList(List<Brain> brains)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < brains.Count; i++)
			{
				list.Add(brains[i].ToString());
			}
			return list;
		}
	}

	public const bool INCLUDE_RAIDER_BRAIN_FOR_PMCS = true;

	private static readonly string[] commonVanillaLayersToRemove = new string[9] { "Help", "AdvAssaultTarget", "Hit", "Simple Target", "Pmc", "AssaultHaveEnemy", "Assault Building", "Enemy Building", "PushAndSup" };

	private static List<Type> _SAINLayers = new List<Type>();

	private static List<string> _SAINLayerNames = new List<string>();

	public static bool BigBrainInitialized;

	public static List<string> SAINLayerNames => findAllSAINLayers();

	public static List<Type> SAINLayers
	{
		get
		{
			if (_SAINLayers.Count == 0)
			{
				Type[] types = typeof(SAINPlugin).Assembly.GetTypes();
				foreach (Type type in types)
				{
					if (type.IsSubclassOf(typeof(SAINLayer)))
					{
						_SAINLayers.Add(type);
					}
				}
			}
			return _SAINLayers;
		}
	}

	public static void Init()
	{
		BrainAssignment.Init();
	}

	private static List<string> findAllSAINLayers()
	{
		if (_SAINLayerNames.Count != 0)
		{
			return _SAINLayerNames;
		}
		foreach (Type sAINLayer in SAINLayers)
		{
			FieldInfo field = sAINLayer.GetField("Name", BindingFlags.Static | BindingFlags.Public);
			if (field == null)
			{
				Logger.LogError(sAINLayer.Name + " does not have a public static Name field. This is required for enabling vanilla layers!");
			}
			else
			{
				_SAINLayerNames.Add((string)field.GetValue(null));
			}
		}
		return _SAINLayerNames;
	}
}
