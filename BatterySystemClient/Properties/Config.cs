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
		public static ConfigEntry<bool> EnableSmokeOccluders { get; private set; }
		public static ConfigEntry<int> SmokeOccluderDensity { get; private set; }
		public static ConfigEntry<float> SmokeOccluderRadiusMultiplier { get; private set; }
		public static ConfigEntry<bool> SmokeOccluderDebugVisuals { get; private set; }
		public static ConfigEntry<bool> ReplaceM18SmokeVisual { get; private set; }
		public static ConfigEntry<int> M18AirdropPlumeCount { get; private set; }
		public static ConfigEntry<float> M18AirdropPlumeScale { get; private set; }
		public static ConfigEntry<float> M18AirdropSmokeAmount { get; private set; }
		public static ConfigEntry<float> M18SmokeOccluderRadiusMultiplier { get; private set; }
		public static ConfigEntry<bool> EnableRetreatSmokeGrenades { get; private set; }
		public static ConfigEntry<float> RetreatSmokeChance { get; private set; }
		public static ConfigEntry<float> RetreatSmokeCooldown { get; private set; }
		public static ConfigEntry<float> RetreatSmokeThrowDistance { get; private set; }
		public static ConfigEntry<bool> RetreatSmokeAllowEmergencyToss { get; private set; }
		public static ConfigEntry<bool> RetreatSmokeDebugLogging { get; private set; }
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

				EnableSmokeOccluders = Config.Bind(generalSettings, "Enable Smoke Occluders", true,
					new ConfigDescription("Spawn invisible foliage occluders inside active smoke grenades so bots lose sight through smoke.",
					null,
					new ConfigurationManagerAttributes { IsAdvanced = false, Order = 24 }));

				SmokeOccluderDensity = Config.Bind(generalSettings, "Smoke Occluder Density", 16,
					new ConfigDescription("How many invisible AI sight occluders to spawn per active smoke grenade.",
					new AcceptableValueRange<int>(4, 48),
					new ConfigurationManagerAttributes { IsAdvanced = true, Order = 23 }));

				SmokeOccluderRadiusMultiplier = Config.Bind(generalSettings, "Smoke Occluder Radius Multiplier", 1f,
					new ConfigDescription("Scales the invisible AI sight occluder field around active smoke grenades.",
					new AcceptableValueRange<float>(0.25f, 2.5f),
					new ConfigurationManagerAttributes { IsAdvanced = true, Order = 22 }));

				SmokeOccluderDebugVisuals = Config.Bind(generalSettings, "Smoke Occluder Debug Visuals", false,
					new ConfigDescription("Show translucent green cylinders for active smoke AI occluders.",
					null,
					new ConfigurationManagerAttributes { IsAdvanced = false, Order = 21 }));

				ReplaceM18SmokeVisual = Config.Bind(generalSettings, "Replace M18 Smoke Visual", true,
					new ConfigDescription("Replace the M18 grenade's vanilla smoke visual with larger airdrop-style smoke plumes.",
					null,
					new ConfigurationManagerAttributes { IsAdvanced = false, Order = 20 }));

				M18AirdropPlumeCount = Config.Bind(generalSettings, "M18 Airdrop Plume Count", 5,
					new ConfigDescription("How many airdrop-style smoke plume instances to spawn for M18 smoke grenades.",
					new AcceptableValueRange<int>(1, 8),
					new ConfigurationManagerAttributes { IsAdvanced = true, Order = 19 }));

				M18AirdropPlumeScale = Config.Bind(generalSettings, "M18 Airdrop Plume Scale", 2.25f,
					new ConfigDescription("Scales the airdrop-style smoke plumes used for M18 smoke grenades.",
					new AcceptableValueRange<float>(0.5f, 4f),
					new ConfigurationManagerAttributes { IsAdvanced = true, Order = 18 }));

				M18AirdropSmokeAmount = Config.Bind(generalSettings, "M18 Airdrop Smoke Amount", 2.25f,
					new ConfigDescription("Multiplies the amount and size of smoke particles in the M18 airdrop-style replacement.",
					new AcceptableValueRange<float>(0.5f, 6f),
					new ConfigurationManagerAttributes { IsAdvanced = true, Order = 17 }));

				M18SmokeOccluderRadiusMultiplier = Config.Bind(generalSettings, "M18 Smoke Occluder Radius Multiplier", 1.6f,
					new ConfigDescription("Extra radius multiplier for invisible AI sight occluders on M18 smoke grenades only.",
					new AcceptableValueRange<float>(0.5f, 3.5f),
					new ConfigurationManagerAttributes { IsAdvanced = true, Order = 16 }));

				EnableRetreatSmokeGrenades = Config.Bind(generalSettings, "Enable Retreat Smoke Grenades", true,
					new ConfigDescription("When SAIN is installed, bots that enter SAIN's Retreat decision will try to throw a smoke grenade before running.",
					null,
					new ConfigurationManagerAttributes { IsAdvanced = false, Order = 15 }));

				RetreatSmokeChance = Config.Bind(generalSettings, "Retreat Smoke Chance", 1f,
					new ConfigDescription("Chance that a bot with smoke will throw one when entering SAIN's Retreat decision.",
					new AcceptableValueRange<float>(0f, 1f),
					new ConfigurationManagerAttributes { IsAdvanced = true, Order = 14 }));

				RetreatSmokeCooldown = Config.Bind(generalSettings, "Retreat Smoke Cooldown", 45f,
					new ConfigDescription("Per-bot cooldown in seconds between forced retreat smoke attempts.",
					new AcceptableValueRange<float>(5f, 180f),
					new ConfigurationManagerAttributes { IsAdvanced = true, Order = 13 }));

				RetreatSmokeThrowDistance = Config.Bind(generalSettings, "Retreat Smoke Throw Distance", 7f,
					new ConfigDescription("Maximum distance in meters for the forced retreat smoke target, placed between the bot and its enemy.",
					new AcceptableValueRange<float>(2f, 20f),
					new ConfigurationManagerAttributes { IsAdvanced = true, Order = 12 }));

				RetreatSmokeAllowEmergencyToss = Config.Bind(generalSettings, "Retreat Smoke Allow Emergency Toss", true,
					new ConfigDescription("If no clean smoke trajectory is found, allow a short low-power toss so the retreat smoke still happens.",
					null,
					new ConfigurationManagerAttributes { IsAdvanced = true, Order = 11 }));

				RetreatSmokeDebugLogging = Config.Bind(generalSettings, "Retreat Smoke Debug Logging", false,
					new ConfigDescription("Log SAIN retreat smoke attempts and rejection reasons.",
					null,
					new ConfigurationManagerAttributes { IsAdvanced = true, Order = 10 }));

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
