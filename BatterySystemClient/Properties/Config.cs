using BepInEx.Configuration;

namespace BatterySystem.Configs
{
	internal class BatterySystemConfig
	{
		public static ConfigEntry<bool> EnableMod { get; private set; }
		public static ConfigEntry<float> DrainMultiplier { get; private set; }
		public static ConfigEntry<bool> EnableHeadsets { get; private set; }
		public static ConfigEntry<bool> EnableQuestPresenceDetector { get; private set; }
		public static ConfigEntry<bool> QuestPresenceShowArrow { get; private set; }
		public static ConfigEntry<float> QuestPresenceAreaSize { get; private set; }
		public static ConfigEntry<bool> QuestPresenceShowNotification { get; private set; }
		public static ConfigEntry<bool> EnableWhiteFlareTrainSummon { get; private set; }
		public static ConfigEntry<bool> AutoUnfold { get; private set; }
        public static ConfigEntry<bool> IsRealism { get; private set; }
        public static ConfigEntry<int> SpawnDurabilityMin { get; private set; }
        public static ConfigEntry<int> SpawnDurabilityMax { get; private set; }

        //public static ConfigEntry<float> CompressorMixerVolume { get; private set; }
        //public static ConfigEntry<float> MainMixerVolume { get; private set; }
        //public static ConfigEntry<float> CompressorGain { get; private set; }

        private static string generalSettings = "General Settings";

		public static void Init(ConfigFile Config)
		{
			{
				EnableMod = Config.Bind(generalSettings, "Enable Mod", true,
					new ConfigDescription("Enable or disable the mod. Requires the game to be restarted.",
					null,
					new ConfigurationManagerAttributes { IsAdvanced = false, Order = 100 }));

				EnableHeadsets = Config.Bind(generalSettings, "Enable Headsets", true,
					new ConfigDescription("Enable BatterySystem for headsets. Disable this if your headsets behave weirdly with other mods such as Realism. Requires restart.",
					null,
					new ConfigurationManagerAttributes { IsAdvanced = false, Order = 75 }));

				EnableQuestPresenceDetector = Config.Bind(generalSettings, "Enable Quest Presence Detector", true,
					new ConfigDescription("Show a proximity indicator when you are near loose quest items. Requires restart.",
					null,
					new ConfigurationManagerAttributes { IsAdvanced = false, Order = 70 }));

				QuestPresenceShowArrow = Config.Bind(generalSettings, "Quest Presence Show Arrow", true,
					new ConfigDescription("Show an arrow pointing toward nearby loose quest items.",
					null,
					new ConfigurationManagerAttributes { IsAdvanced = false, Order = 69 }));

				QuestPresenceAreaSize = Config.Bind(generalSettings, "Quest Presence Area Size", 5f,
					new ConfigDescription("The detection radius around loose quest items in meters. Cannot be changed mid-raid.",
					new AcceptableValueRange<float>(5f, 25f),
					new ConfigurationManagerAttributes { IsAdvanced = false, Order = 68 }));

				QuestPresenceShowNotification = Config.Bind(generalSettings, "Quest Presence Show Notification", true,
					new ConfigDescription("Show a notification when entering a loose quest item's detection area.",
					null,
					new ConfigurationManagerAttributes { IsAdvanced = false, Order = 67 }));

				EnableWhiteFlareTrainSummon = Config.Bind(generalSettings, "White Flare Train Summon", true,
					new ConfigDescription("Summon map trains when the local player fires a successful white flare.",
					null,
					new ConfigurationManagerAttributes { IsAdvanced = false, Order = 25 }));

				DrainMultiplier = Config.Bind(generalSettings, "Battery Drain Multiplier", 1f,
					new ConfigDescription("Adjust the drain multiplier when NVG is on. By default a battery lasts an hour on NVGs and 2.5 hours on collimators.",
					new AcceptableValueRange<float>(0f, 10f),
					new ConfigurationManagerAttributes { IsAdvanced = false, Order = 0 }));

				AutoUnfold = Config.Bind(generalSettings, "Auto Unfold (NOT IMPLEMENTED)", true,
					new ConfigDescription("Automatically unfold iron sights when the main sight runs out of battery. Doesn't do anything yet.",
					null,
					new ConfigurationManagerAttributes { IsAdvanced = true, Order = -50 }));

                IsRealism = Config.Bind(generalSettings, "Using Realism Mod", true,
                    new ConfigDescription("Toggle for Realism users; affects how headset volume is handled. CAUTION: if using Realism and you don't toggle this on, the mod will throw an error!",
                    null,
                    new ConfigurationManagerAttributes { IsAdvanced = false, Order = 50 }));


                SpawnDurabilityMin = Config.Bind(generalSettings, "Spawn Durability Min", 5,
					new ConfigDescription("Adjust the minimum durability a battery can spawn with on bots.",
					new AcceptableValueRange<int>(0, 100),
					new ConfigurationManagerAttributes { IsAdvanced = false, Order = -50 }));

				SpawnDurabilityMax = Config.Bind(generalSettings, "Spawn Durability Max", 15,
					new ConfigDescription("Adjust the maximum durability a battery can spawn with on bots. This must be ATLEAST the same value as Spawn Durability Minimum.",
					new AcceptableValueRange<int>(0, 100),
					new ConfigurationManagerAttributes { IsAdvanced = false, Order = -100 }));
				
				/*
				CompressorMixerVolume = Config.Bind(generalSettings, "CompressorMixerVolume", -3f,
					new ConfigDescription("",
					new AcceptableValueRange<float>(-30f, 10f),
					new ConfigurationManagerAttributes { IsAdvanced = false, Order = -180 }));

				MainMixerVolume = Config.Bind(generalSettings, "MainMixerVolume", -0f,
					new ConfigDescription("",
					new AcceptableValueRange<float>(-30f, 10f),
					new ConfigurationManagerAttributes { IsAdvanced = false, Order = -230 }));
				*/
            }
		}
	}
}
