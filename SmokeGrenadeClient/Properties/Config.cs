using BepInEx.Configuration;

namespace SmokeGrenadeClient.Configs
{
	internal class SmokeGrenadeConfig
	{
		public static ConfigEntry<bool> EnableMod { get; private set; }
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

		private const string GeneralSettings = "General Settings";

		public static void Init(ConfigFile config)
		{
			EnableMod = config.Bind(GeneralSettings, "Enable Mod", true,
				new ConfigDescription("Enable or disable the smoke grenade mod. Requires the game to be restarted.",
				null,
				new ConfigurationManagerAttributes { IsAdvanced = false, Order = 100 }));

			EnableSmokeOccluders = config.Bind(GeneralSettings, "Enable Smoke Occluders", true,
				new ConfigDescription("Spawn invisible foliage occluders inside active smoke grenades so bots lose sight through smoke.",
				null,
				new ConfigurationManagerAttributes { IsAdvanced = false, Order = 90 }));

			SmokeOccluderDensity = config.Bind(GeneralSettings, "Smoke Occluder Density", 16,
				new ConfigDescription("How many invisible AI sight occluders to spawn per active smoke grenade.",
				new AcceptableValueRange<int>(4, 48),
				new ConfigurationManagerAttributes { IsAdvanced = true, Order = 89 }));

			SmokeOccluderRadiusMultiplier = config.Bind(GeneralSettings, "Smoke Occluder Radius Multiplier", 1f,
				new ConfigDescription("Scales the invisible AI sight occluder field around active smoke grenades.",
				new AcceptableValueRange<float>(0.25f, 2.5f),
				new ConfigurationManagerAttributes { IsAdvanced = true, Order = 88 }));

			SmokeOccluderDebugVisuals = config.Bind(GeneralSettings, "Smoke Occluder Debug Visuals", false,
				new ConfigDescription("Show translucent green cylinders for active smoke AI occluders.",
				null,
				new ConfigurationManagerAttributes { IsAdvanced = false, Order = 87 }));

			ReplaceM18SmokeVisual = config.Bind(GeneralSettings, "Replace M18 Smoke Visual", true,
				new ConfigDescription("Replace the M18 grenade's vanilla smoke visual with larger airdrop-style smoke plumes.",
				null,
				new ConfigurationManagerAttributes { IsAdvanced = false, Order = 80 }));

			M18AirdropPlumeCount = config.Bind(GeneralSettings, "M18 Airdrop Plume Count", 5,
				new ConfigDescription("How many airdrop-style smoke plume instances to spawn for M18 smoke grenades.",
				new AcceptableValueRange<int>(1, 8),
				new ConfigurationManagerAttributes { IsAdvanced = true, Order = 79 }));

			M18AirdropPlumeScale = config.Bind(GeneralSettings, "M18 Airdrop Plume Scale", 2.25f,
				new ConfigDescription("Scales the airdrop-style smoke plumes used for M18 smoke grenades.",
				new AcceptableValueRange<float>(0.5f, 4f),
				new ConfigurationManagerAttributes { IsAdvanced = true, Order = 78 }));

			M18AirdropSmokeAmount = config.Bind(GeneralSettings, "M18 Airdrop Smoke Amount", 2.25f,
				new ConfigDescription("Multiplies the amount and size of smoke particles in the M18 airdrop-style replacement.",
				new AcceptableValueRange<float>(0.5f, 6f),
				new ConfigurationManagerAttributes { IsAdvanced = true, Order = 77 }));

			M18SmokeOccluderRadiusMultiplier = config.Bind(GeneralSettings, "M18 Smoke Occluder Radius Multiplier", 1.6f,
				new ConfigDescription("Extra radius multiplier for invisible AI sight occluders on M18 smoke grenades only.",
				new AcceptableValueRange<float>(0.5f, 3.5f),
				new ConfigurationManagerAttributes { IsAdvanced = true, Order = 76 }));

			EnableRetreatSmokeGrenades = config.Bind(GeneralSettings, "Enable Retreat Smoke Grenades", true,
				new ConfigDescription("When SAIN is installed, bots that enter SAIN's Retreat decision will try to throw a smoke grenade before running.",
				null,
				new ConfigurationManagerAttributes { IsAdvanced = false, Order = 70 }));

			RetreatSmokeChance = config.Bind(GeneralSettings, "Retreat Smoke Chance", 1f,
				new ConfigDescription("Chance that a bot with smoke will throw one when entering SAIN's Retreat decision.",
				new AcceptableValueRange<float>(0f, 1f),
				new ConfigurationManagerAttributes { IsAdvanced = true, Order = 69 }));

			RetreatSmokeCooldown = config.Bind(GeneralSettings, "Retreat Smoke Cooldown", 45f,
				new ConfigDescription("Per-bot cooldown in seconds between forced retreat smoke attempts.",
				new AcceptableValueRange<float>(5f, 180f),
				new ConfigurationManagerAttributes { IsAdvanced = true, Order = 68 }));

			RetreatSmokeThrowDistance = config.Bind(GeneralSettings, "Retreat Smoke Throw Distance", 7f,
				new ConfigDescription("Maximum distance in meters for the forced retreat smoke target, placed between the bot and its enemy.",
				new AcceptableValueRange<float>(2f, 20f),
				new ConfigurationManagerAttributes { IsAdvanced = true, Order = 67 }));

			RetreatSmokeAllowEmergencyToss = config.Bind(GeneralSettings, "Retreat Smoke Allow Emergency Toss", true,
				new ConfigDescription("If no clean smoke trajectory is found, allow a short low-power toss so the retreat smoke still happens.",
				null,
				new ConfigurationManagerAttributes { IsAdvanced = true, Order = 66 }));

			RetreatSmokeDebugLogging = config.Bind(GeneralSettings, "Retreat Smoke Debug Logging", false,
				new ConfigDescription("Log SAIN retreat smoke attempts and rejection reasons.",
				null,
				new ConfigurationManagerAttributes { IsAdvanced = true, Order = 65 }));
		}
	}
}
