using System.Collections.Generic;
using EFT;
using SAIN.Helpers;
using SAIN.Preset;
using SAIN.Preset.BotSettings;
using SAIN.Preset.BotSettings.SAINSettings;
using SAIN.Preset.GlobalSettings;
using UnityEngine;

namespace SAIN.Plugin;

internal static class SAINDifficultyClass
{
	private const string PresetNameEasy = "Baby Bots";

	private const string PresetNameNormal = "Less Difficult";

	private const string PresetNameHard = "Default";

	private const string PresetNameHarderPMCs = "Default with Harder PMCs";

	private const string DefaultPresetDescription = "Bots are difficult but fair, the way SAIN was meant to played.";

	private const string PresetNameVeryHard = "I Like Pain";

	private const string PresetNameImpossible = "Death Wish";

	public static readonly Dictionary<SAINDifficulty, SAINPresetDefinition> DefaultPresetDefinitions;

	static SAINDifficultyClass()
	{
		DefaultPresetDefinitions = new Dictionary<SAINDifficulty, SAINPresetDefinition>();
		DefaultPresetDefinitions.Add(SAINDifficulty.easy, SAINPresetDefinition.CreateDefaultDefinition("Baby Bots", SAINDifficulty.easy, "Bots react slowly and are incredibly inaccurate."));
		DefaultPresetDefinitions.Add(SAINDifficulty.lesshard, SAINPresetDefinition.CreateDefaultDefinition("Less Difficult", SAINDifficulty.lesshard, "Bots react more slowly, and are less accurate than usual."));
		DefaultPresetDefinitions.Add(SAINDifficulty.hard, SAINPresetDefinition.CreateDefaultDefinition("Default", SAINDifficulty.hard, "Bots are difficult but fair, the way SAIN was meant to played."));
		DefaultPresetDefinitions.Add(SAINDifficulty.harderpmcs, SAINPresetDefinition.CreateDefaultDefinition("Default with Harder PMCs", SAINDifficulty.harderpmcs, "Default Settings, but PMCs are harder than normal."));
		DefaultPresetDefinitions.Add(SAINDifficulty.veryhard, SAINPresetDefinition.CreateDefaultDefinition("I Like Pain", SAINDifficulty.veryhard, "Bots react faster, are more accurate, and can see further."));
		DefaultPresetDefinitions.Add(SAINDifficulty.deathwish, SAINPresetDefinition.CreateDefaultDefinition("Death Wish", SAINDifficulty.deathwish, "Prepare To Die. Bots have almost no scatter, get less recoil from their weapon while shooting, are more accurate, and react deadly fast."));
	}

	public static SAINPresetClass GetDefaultPreset(SAINDifficulty difficulty)
	{
		SAINPresetClass result;
		switch (difficulty)
		{
		case SAINDifficulty.easy:
			result = CreateEasyPreset();
			break;
		case SAINDifficulty.lesshard:
			result = CreateNormalPreset();
			break;
		case SAINDifficulty.hard:
			result = CreateHardPreset();
			break;
		case SAINDifficulty.harderpmcs:
			result = CreateHarderPMCsPreset();
			break;
		case SAINDifficulty.veryhard:
			result = CreateVeryHardPreset();
			break;
		case SAINDifficulty.deathwish:
			result = CreateImpossiblePreset();
			break;
		default:
			return null;
		}
		return result;
	}

	private static SAINPresetClass CreateEasyPreset()
	{
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Invalid comparison between Unknown and I4
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Invalid comparison between Unknown and I4
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Invalid comparison between Unknown and I4
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Invalid comparison between Unknown and I4
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Invalid comparison between Unknown and I4
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Invalid comparison between Unknown and I4
		SAINPresetClass sAINPresetClass = new SAINPresetClass(SAINDifficulty.easy);
		GlobalSettingsClass globalSettings = sAINPresetClass.GlobalSettings;
		globalSettings.Shoot.RecoilMultiplier = 3f;
		globalSettings.Difficulty.ScatteringCoef = 5f;
		globalSettings.Difficulty.PrecisionSpeedCoef = 2f;
		globalSettings.Difficulty.AccuracySpeedCoef = 2f;
		globalSettings.Difficulty.HearingDistanceCoef = 0.6f;
		globalSettings.Aiming.FasterCQBReactionsGlobal = false;
		globalSettings.Difficulty.VisibleDistCoef = 0.66f;
		globalSettings.Difficulty.GainSightCoef = 2.5f;
		foreach (KeyValuePair<WildSpawnType, SAINSettingsGroupClass> sAINSetting in sAINPresetClass.BotSettings.SAINSettings)
		{
			sAINSetting.Value.DifficultyModifier = Mathf.Clamp(sAINSetting.Value.DifficultyModifier * 0.5f, 0.01f, 2f).Round100();
			foreach (KeyValuePair<BotDifficulty, SAINSettingsClass> setting in sAINSetting.Value.Settings)
			{
				setting.Value.Core.VisibleAngle = 120f;
				setting.Value.Shoot.FireratMulti *= 0.4f;
				setting.Value.Shoot.BurstMulti *= 0.5f;
				setting.Value.Look.MinimumVisionSpeed = 0.4f;
				setting.Value.Aiming.DistanceAimTimeMultiplier = 1.5f;
				setting.Value.Aiming.AngleAimTimeMultiplier = 1.5f;
				if (setting.Value.Aiming.MAX_AIM_TIME < 1f)
				{
					setting.Value.Aiming.MAX_AIM_TIME = 1f;
				}
				if (setting.Value.Aiming.MAX_AIMING_UPGRADE_BY_TIME < 0.4f)
				{
					setting.Value.Aiming.MAX_AIMING_UPGRADE_BY_TIME = 0.4f;
				}
				setting.Value.Core.ScatteringPerMeter += 0.05f;
				setting.Value.Core.ScatteringClosePerMeter += 0.1f;
			}
		}
		foreach (KeyValuePair<WildSpawnType, SAINSettingsGroupClass> sAINSetting2 in sAINPresetClass.BotSettings.SAINSettings)
		{
			if (BotSettingsRepoClass.IsBossOrFollower(sAINSetting2.Key))
			{
				Dictionary<BotDifficulty, SAINSettingsClass> settings = sAINSetting2.Value.Settings;
				SAINSettingsClass sAINSettingsClass = settings[(BotDifficulty)0];
				sAINSettingsClass.Move.STRAFE_SPEED = 0.4f;
				SAINSettingsClass sAINSettingsClass2 = settings[(BotDifficulty)1];
				sAINSettingsClass2.Move.STRAFE_SPEED = 0.5f;
				SAINSettingsClass sAINSettingsClass3 = settings[(BotDifficulty)2];
				sAINSettingsClass3.Move.STRAFE_SPEED = 0.55f;
				SAINSettingsClass sAINSettingsClass4 = settings[(BotDifficulty)3];
				sAINSettingsClass4.Move.STRAFE_SPEED = 0.65f;
			}
			if (sAINSetting2.Key.IsPMC() || (int)sAINSetting2.Key == 24 || (int)sAINSetting2.Key == 9 || (int)sAINSetting2.Key == 34 || (int)sAINSetting2.Key == 35)
			{
				Dictionary<BotDifficulty, SAINSettingsClass> settings2 = sAINSetting2.Value.Settings;
				SAINSettingsClass sAINSettingsClass5 = settings2[(BotDifficulty)0];
				sAINSettingsClass5.Move.STRAFE_SPEED = 0.4f;
				SAINSettingsClass sAINSettingsClass6 = settings2[(BotDifficulty)1];
				sAINSettingsClass6.Move.STRAFE_SPEED = 0.5f;
				SAINSettingsClass sAINSettingsClass7 = settings2[(BotDifficulty)2];
				sAINSettingsClass7.Move.STRAFE_SPEED = 0.55f;
				SAINSettingsClass sAINSettingsClass8 = settings2[(BotDifficulty)3];
				sAINSettingsClass8.Move.STRAFE_SPEED = 0.75f;
			}
			if ((int)sAINSetting2.Key == 1 || (int)sAINSetting2.Key == 19)
			{
				Dictionary<BotDifficulty, SAINSettingsClass> settings3 = sAINSetting2.Value.Settings;
				SAINSettingsClass sAINSettingsClass9 = settings3[(BotDifficulty)0];
				sAINSettingsClass9.Move.STRAFE_SPEED = 0.4f;
				SAINSettingsClass sAINSettingsClass10 = settings3[(BotDifficulty)1];
				sAINSettingsClass10.Move.STRAFE_SPEED = 0.45f;
				SAINSettingsClass sAINSettingsClass11 = settings3[(BotDifficulty)2];
				sAINSettingsClass11.Move.STRAFE_SPEED = 0.45f;
				SAINSettingsClass sAINSettingsClass12 = settings3[(BotDifficulty)3];
				sAINSettingsClass12.Move.STRAFE_SPEED = 0.5f;
			}
		}
		return sAINPresetClass;
	}

	private static SAINPresetClass CreateNormalPreset()
	{
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Invalid comparison between Unknown and I4
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Invalid comparison between Unknown and I4
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Invalid comparison between Unknown and I4
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Invalid comparison between Unknown and I4
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Invalid comparison between Unknown and I4
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Invalid comparison between Unknown and I4
		SAINPresetClass sAINPresetClass = new SAINPresetClass(SAINDifficulty.lesshard);
		GlobalSettingsClass globalSettings = sAINPresetClass.GlobalSettings;
		globalSettings.Shoot.RecoilMultiplier = 1.6f;
		globalSettings.Difficulty.ScatteringCoef = 1.25f;
		globalSettings.Difficulty.PrecisionSpeedCoef = 1.25f;
		globalSettings.Difficulty.AccuracySpeedCoef = 1.25f;
		globalSettings.Difficulty.VisibleDistCoef = 0.85f;
		globalSettings.Difficulty.GainSightCoef = 1.25f;
		globalSettings.Difficulty.HearingDistanceCoef = 0.85f;
		globalSettings.Aiming.FasterCQBReactionsGlobal = false;
		foreach (KeyValuePair<WildSpawnType, SAINSettingsGroupClass> sAINSetting in sAINPresetClass.BotSettings.SAINSettings)
		{
			sAINSetting.Value.DifficultyModifier = Mathf.Clamp(sAINSetting.Value.DifficultyModifier * 0.85f, 0.01f, 2f).Round100();
			foreach (KeyValuePair<BotDifficulty, SAINSettingsClass> setting in sAINSetting.Value.Settings)
			{
				setting.Value.Core.VisibleAngle = 150f;
				setting.Value.Shoot.FireratMulti *= 0.8f;
				setting.Value.Look.MinimumVisionSpeed = 0.1f;
			}
		}
		foreach (KeyValuePair<WildSpawnType, SAINSettingsGroupClass> sAINSetting2 in sAINPresetClass.BotSettings.SAINSettings)
		{
			if (BotSettingsRepoClass.IsBossOrFollower(sAINSetting2.Key))
			{
				Dictionary<BotDifficulty, SAINSettingsClass> settings = sAINSetting2.Value.Settings;
				SAINSettingsClass sAINSettingsClass = settings[(BotDifficulty)0];
				sAINSettingsClass.Move.STRAFE_SPEED = 0.5f;
				SAINSettingsClass sAINSettingsClass2 = settings[(BotDifficulty)1];
				sAINSettingsClass2.Move.STRAFE_SPEED = 0.65f;
				SAINSettingsClass sAINSettingsClass3 = settings[(BotDifficulty)2];
				sAINSettingsClass3.Move.STRAFE_SPEED = 0.8f;
				SAINSettingsClass sAINSettingsClass4 = settings[(BotDifficulty)3];
				sAINSettingsClass4.Move.STRAFE_SPEED = 1f;
			}
			if (sAINSetting2.Key.IsPMC() || (int)sAINSetting2.Key == 24 || (int)sAINSetting2.Key == 9 || (int)sAINSetting2.Key == 34 || (int)sAINSetting2.Key == 35)
			{
				Dictionary<BotDifficulty, SAINSettingsClass> settings2 = sAINSetting2.Value.Settings;
				SAINSettingsClass sAINSettingsClass5 = settings2[(BotDifficulty)0];
				sAINSettingsClass5.Move.STRAFE_SPEED = 0.4f;
				SAINSettingsClass sAINSettingsClass6 = settings2[(BotDifficulty)1];
				sAINSettingsClass6.Move.STRAFE_SPEED = 0.6f;
				SAINSettingsClass sAINSettingsClass7 = settings2[(BotDifficulty)2];
				sAINSettingsClass7.Move.STRAFE_SPEED = 0.7f;
				SAINSettingsClass sAINSettingsClass8 = settings2[(BotDifficulty)3];
				sAINSettingsClass8.Move.STRAFE_SPEED = 0.9f;
			}
			if ((int)sAINSetting2.Key == 1 || (int)sAINSetting2.Key == 19)
			{
				Dictionary<BotDifficulty, SAINSettingsClass> settings3 = sAINSetting2.Value.Settings;
				SAINSettingsClass sAINSettingsClass9 = settings3[(BotDifficulty)0];
				sAINSettingsClass9.Move.STRAFE_SPEED = 0.35f;
				SAINSettingsClass sAINSettingsClass10 = settings3[(BotDifficulty)1];
				sAINSettingsClass10.Move.STRAFE_SPEED = 0.45f;
				SAINSettingsClass sAINSettingsClass11 = settings3[(BotDifficulty)2];
				sAINSettingsClass11.Move.STRAFE_SPEED = 0.5f;
				SAINSettingsClass sAINSettingsClass12 = settings3[(BotDifficulty)3];
				sAINSettingsClass12.Move.STRAFE_SPEED = 0.65f;
			}
		}
		return sAINPresetClass;
	}

	private static SAINPresetClass CreateHardPreset()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Invalid comparison between Unknown and I4
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Invalid comparison between Unknown and I4
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Invalid comparison between Unknown and I4
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Invalid comparison between Unknown and I4
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Invalid comparison between Unknown and I4
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Invalid comparison between Unknown and I4
		SAINPresetClass sAINPresetClass = new SAINPresetClass(SAINDifficulty.hard);
		foreach (KeyValuePair<WildSpawnType, SAINSettingsGroupClass> sAINSetting in sAINPresetClass.BotSettings.SAINSettings)
		{
			if (BotSettingsRepoClass.IsBossOrFollower(sAINSetting.Key))
			{
				Dictionary<BotDifficulty, SAINSettingsClass> settings = sAINSetting.Value.Settings;
				SAINSettingsClass sAINSettingsClass = settings[(BotDifficulty)0];
				sAINSettingsClass.Move.STRAFE_SPEED = 0.6f;
				SAINSettingsClass sAINSettingsClass2 = settings[(BotDifficulty)1];
				sAINSettingsClass2.Move.STRAFE_SPEED = 0.75f;
				SAINSettingsClass sAINSettingsClass3 = settings[(BotDifficulty)2];
				sAINSettingsClass3.Move.STRAFE_SPEED = 0.85f;
				SAINSettingsClass sAINSettingsClass4 = settings[(BotDifficulty)3];
				sAINSettingsClass4.Move.STRAFE_SPEED = 1f;
			}
			if (sAINSetting.Key.IsPMC() || (int)sAINSetting.Key == 24 || (int)sAINSetting.Key == 9 || (int)sAINSetting.Key == 34 || (int)sAINSetting.Key == 35)
			{
				Dictionary<BotDifficulty, SAINSettingsClass> settings2 = sAINSetting.Value.Settings;
				SAINSettingsClass sAINSettingsClass5 = settings2[(BotDifficulty)0];
				sAINSettingsClass5.Move.STRAFE_SPEED = 0.5f;
				SAINSettingsClass sAINSettingsClass6 = settings2[(BotDifficulty)1];
				sAINSettingsClass6.Move.STRAFE_SPEED = 0.65f;
				SAINSettingsClass sAINSettingsClass7 = settings2[(BotDifficulty)2];
				sAINSettingsClass7.Move.STRAFE_SPEED = 0.75f;
				SAINSettingsClass sAINSettingsClass8 = settings2[(BotDifficulty)3];
				sAINSettingsClass8.Move.STRAFE_SPEED = 0.9f;
			}
			if ((int)sAINSetting.Key == 1 || (int)sAINSetting.Key == 19)
			{
				Dictionary<BotDifficulty, SAINSettingsClass> settings3 = sAINSetting.Value.Settings;
				SAINSettingsClass sAINSettingsClass9 = settings3[(BotDifficulty)0];
				sAINSettingsClass9.Move.STRAFE_SPEED = 0.5f;
				SAINSettingsClass sAINSettingsClass10 = settings3[(BotDifficulty)1];
				sAINSettingsClass10.Move.STRAFE_SPEED = 0.55f;
				SAINSettingsClass sAINSettingsClass11 = settings3[(BotDifficulty)2];
				sAINSettingsClass11.Move.STRAFE_SPEED = 0.6f;
				SAINSettingsClass sAINSettingsClass12 = settings3[(BotDifficulty)3];
				sAINSettingsClass12.Move.STRAFE_SPEED = 0.65f;
			}
		}
		return sAINPresetClass;
	}

	private static SAINPresetClass CreateHarderPMCsPreset()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Invalid comparison between Unknown and I4
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Invalid comparison between Unknown and I4
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Invalid comparison between Unknown and I4
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Invalid comparison between Unknown and I4
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Invalid comparison between Unknown and I4
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Invalid comparison between Unknown and I4
		SAINPresetClass sAINPresetClass = new SAINPresetClass(SAINDifficulty.harderpmcs);
		foreach (KeyValuePair<WildSpawnType, SAINSettingsGroupClass> sAINSetting in sAINPresetClass.BotSettings.SAINSettings)
		{
			if (BotSettingsRepoClass.IsBossOrFollower(sAINSetting.Key))
			{
				Dictionary<BotDifficulty, SAINSettingsClass> settings = sAINSetting.Value.Settings;
				SAINSettingsClass sAINSettingsClass = settings[(BotDifficulty)0];
				sAINSettingsClass.Move.STRAFE_SPEED = 0.5f;
				SAINSettingsClass sAINSettingsClass2 = settings[(BotDifficulty)1];
				sAINSettingsClass2.Move.STRAFE_SPEED = 0.65f;
				SAINSettingsClass sAINSettingsClass3 = settings[(BotDifficulty)2];
				sAINSettingsClass3.Move.STRAFE_SPEED = 0.8f;
				SAINSettingsClass sAINSettingsClass4 = settings[(BotDifficulty)3];
				sAINSettingsClass4.Move.STRAFE_SPEED = 1f;
			}
			if (sAINSetting.Key.IsPMC() || (int)sAINSetting.Key == 24 || (int)sAINSetting.Key == 9 || (int)sAINSetting.Key == 34 || (int)sAINSetting.Key == 35)
			{
				Dictionary<BotDifficulty, SAINSettingsClass> settings2 = sAINSetting.Value.Settings;
				SAINSettingsClass sAINSettingsClass5 = settings2[(BotDifficulty)0];
				sAINSettingsClass5.Move.STRAFE_SPEED = 0.4f;
				SAINSettingsClass sAINSettingsClass6 = settings2[(BotDifficulty)1];
				sAINSettingsClass6.Move.STRAFE_SPEED = 0.6f;
				SAINSettingsClass sAINSettingsClass7 = settings2[(BotDifficulty)2];
				sAINSettingsClass7.Move.STRAFE_SPEED = 0.7f;
				SAINSettingsClass sAINSettingsClass8 = settings2[(BotDifficulty)3];
				sAINSettingsClass8.Move.STRAFE_SPEED = 0.9f;
			}
			if ((int)sAINSetting.Key == 1 || (int)sAINSetting.Key == 19)
			{
				Dictionary<BotDifficulty, SAINSettingsClass> settings3 = sAINSetting.Value.Settings;
				SAINSettingsClass sAINSettingsClass9 = settings3[(BotDifficulty)0];
				sAINSettingsClass9.Move.STRAFE_SPEED = 0.35f;
				SAINSettingsClass sAINSettingsClass10 = settings3[(BotDifficulty)1];
				sAINSettingsClass10.Move.STRAFE_SPEED = 0.45f;
				SAINSettingsClass sAINSettingsClass11 = settings3[(BotDifficulty)2];
				sAINSettingsClass11.Move.STRAFE_SPEED = 0.5f;
				SAINSettingsClass sAINSettingsClass12 = settings3[(BotDifficulty)3];
				sAINSettingsClass12.Move.STRAFE_SPEED = 0.65f;
			}
		}
		ApplyHarderPMCs(sAINPresetClass);
		return sAINPresetClass;
	}

	private static void ApplyHarderPMCs(SAINPresetClass preset)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Invalid comparison between Unknown and I4
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Invalid comparison between Unknown and I4
		SAINBotSettingsClass botSettings = preset.BotSettings;
		foreach (KeyValuePair<WildSpawnType, SAINSettingsGroupClass> sAINSetting in botSettings.SAINSettings)
		{
			if ((int)sAINSetting.Key != 52 && (int)sAINSetting.Key != 51)
			{
				continue;
			}
			Dictionary<BotDifficulty, SAINSettingsClass> settings = sAINSetting.Value.Settings;
			foreach (SAINSettingsClass value in settings.Values)
			{
				value.Mind.WeaponProficiency = 0.75f;
				value.Difficulty.ScatteringCoef = 0.8f;
				value.Difficulty.PrecisionSpeedCoef = 0.8f;
				value.Difficulty.AccuracySpeedCoef = 0.8f;
				value.Difficulty.GainSightCoef = 0.8f;
				value.Difficulty.VisibleDistCoef = 1.25f;
				value.Difficulty.AggressionCoef = 1.2f;
			}
			SAINSettingsClass sAINSettingsClass = settings[(BotDifficulty)0];
			sAINSettingsClass.Aiming.FasterCQBReactionsDistance = 20f;
			sAINSettingsClass.Aiming.FasterCQBReactionsMinimum = 0.3f;
			sAINSettingsClass.Aiming.MAX_AIMING_UPGRADE_BY_TIME = 0.35f;
			sAINSettingsClass.Aiming.MAX_AIM_TIME = 1.5f;
			sAINSettingsClass.Aiming.BASE_HIT_AFFECTION_DELAY_SEC = 0.65f;
			sAINSettingsClass.Core.VisibleDistance = 200f;
			SAINSettingsClass sAINSettingsClass2 = settings[(BotDifficulty)1];
			sAINSettingsClass2.Aiming.FasterCQBReactionsDistance = 35f;
			sAINSettingsClass2.Aiming.FasterCQBReactionsMinimum = 0.25f;
			sAINSettingsClass2.Aiming.MAX_AIMING_UPGRADE_BY_TIME = 0.4f;
			sAINSettingsClass2.Aiming.MAX_AIM_TIME = 1.35f;
			sAINSettingsClass2.Aiming.BASE_HIT_AFFECTION_DELAY_SEC = 0.5f;
			sAINSettingsClass2.Core.VisibleDistance = 225f;
			SAINSettingsClass sAINSettingsClass3 = settings[(BotDifficulty)2];
			sAINSettingsClass3.Aiming.FasterCQBReactionsDistance = 50f;
			sAINSettingsClass3.Aiming.FasterCQBReactionsMinimum = 0.2f;
			sAINSettingsClass3.Aiming.MAX_AIMING_UPGRADE_BY_TIME = 0.2f;
			sAINSettingsClass3.Aiming.MAX_AIM_TIME = 1.15f;
			sAINSettingsClass3.Aiming.BASE_HIT_AFFECTION_DELAY_SEC = 0.35f;
			sAINSettingsClass3.Core.VisibleDistance = 250f;
			SAINSettingsClass sAINSettingsClass4 = settings[(BotDifficulty)3];
			sAINSettingsClass4.Aiming.FasterCQBReactionsDistance = 60f;
			sAINSettingsClass4.Aiming.FasterCQBReactionsMinimum = 0.15f;
			sAINSettingsClass4.Aiming.MAX_AIMING_UPGRADE_BY_TIME = 0.15f;
			sAINSettingsClass4.Aiming.MAX_AIM_TIME = 1f;
			sAINSettingsClass4.Aiming.BASE_HIT_AFFECTION_DELAY_SEC = 0.25f;
			sAINSettingsClass4.Core.VisibleDistance = 275f;
		}
	}

	private static SAINPresetClass CreateVeryHardPreset()
	{
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Invalid comparison between Unknown and I4
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Invalid comparison between Unknown and I4
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Invalid comparison between Unknown and I4
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Invalid comparison between Unknown and I4
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Invalid comparison between Unknown and I4
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Invalid comparison between Unknown and I4
		SAINPresetClass sAINPresetClass = new SAINPresetClass(SAINDifficulty.veryhard);
		GlobalSettingsClass globalSettings = sAINPresetClass.GlobalSettings;
		globalSettings.Shoot.RecoilMultiplier = 0.66f;
		globalSettings.Difficulty.ScatteringCoef = 0.85f;
		globalSettings.Aiming.AimCenterMassGlobal = false;
		globalSettings.Difficulty.VisibleDistCoef = 1.33f;
		globalSettings.Difficulty.GainSightCoef = 0.8f;
		globalSettings.Difficulty.PrecisionSpeedCoef = 0.8f;
		globalSettings.Difficulty.AccuracySpeedCoef = 0.8f;
		foreach (KeyValuePair<WildSpawnType, SAINSettingsGroupClass> sAINSetting in sAINPresetClass.BotSettings.SAINSettings)
		{
			sAINSetting.Value.DifficultyModifier = Mathf.Clamp(sAINSetting.Value.DifficultyModifier * 1.33f, 0.01f, 2f).Round100();
			foreach (KeyValuePair<BotDifficulty, SAINSettingsClass> setting in sAINSetting.Value.Settings)
			{
				setting.Value.Core.VisibleAngle = 170f;
				setting.Value.Shoot.FireratMulti = 1.5f;
				setting.Value.Shoot.BurstMulti = 2f;
				setting.Value.Aiming.AimCenterMass = false;
			}
		}
		foreach (KeyValuePair<WildSpawnType, SAINSettingsGroupClass> sAINSetting2 in sAINPresetClass.BotSettings.SAINSettings)
		{
			if (BotSettingsRepoClass.IsBossOrFollower(sAINSetting2.Key))
			{
				Dictionary<BotDifficulty, SAINSettingsClass> settings = sAINSetting2.Value.Settings;
				SAINSettingsClass sAINSettingsClass = settings[(BotDifficulty)0];
				sAINSettingsClass.Move.STRAFE_SPEED = 0.75f;
				SAINSettingsClass sAINSettingsClass2 = settings[(BotDifficulty)1];
				sAINSettingsClass2.Move.STRAFE_SPEED = 0.85f;
				SAINSettingsClass sAINSettingsClass3 = settings[(BotDifficulty)2];
				sAINSettingsClass3.Move.STRAFE_SPEED = 0.9f;
				SAINSettingsClass sAINSettingsClass4 = settings[(BotDifficulty)3];
				sAINSettingsClass4.Move.STRAFE_SPEED = 1f;
			}
			if (sAINSetting2.Key.IsPMC() || (int)sAINSetting2.Key == 24 || (int)sAINSetting2.Key == 9 || (int)sAINSetting2.Key == 34 || (int)sAINSetting2.Key == 35)
			{
				Dictionary<BotDifficulty, SAINSettingsClass> settings2 = sAINSetting2.Value.Settings;
				SAINSettingsClass sAINSettingsClass5 = settings2[(BotDifficulty)0];
				sAINSettingsClass5.Move.STRAFE_SPEED = 0.75f;
				SAINSettingsClass sAINSettingsClass6 = settings2[(BotDifficulty)1];
				sAINSettingsClass6.Move.STRAFE_SPEED = 0.85f;
				SAINSettingsClass sAINSettingsClass7 = settings2[(BotDifficulty)2];
				sAINSettingsClass7.Move.STRAFE_SPEED = 0.9f;
				SAINSettingsClass sAINSettingsClass8 = settings2[(BotDifficulty)3];
				sAINSettingsClass8.Move.STRAFE_SPEED = 1f;
			}
			if ((int)sAINSetting2.Key == 1 || (int)sAINSetting2.Key == 19)
			{
				Dictionary<BotDifficulty, SAINSettingsClass> settings3 = sAINSetting2.Value.Settings;
				SAINSettingsClass sAINSettingsClass9 = settings3[(BotDifficulty)0];
				sAINSettingsClass9.Move.STRAFE_SPEED = 0.65f;
				SAINSettingsClass sAINSettingsClass10 = settings3[(BotDifficulty)1];
				sAINSettingsClass10.Move.STRAFE_SPEED = 0.7f;
				SAINSettingsClass sAINSettingsClass11 = settings3[(BotDifficulty)2];
				sAINSettingsClass11.Move.STRAFE_SPEED = 0.75f;
				SAINSettingsClass sAINSettingsClass12 = settings3[(BotDifficulty)3];
				sAINSettingsClass12.Move.STRAFE_SPEED = 0.9f;
			}
		}
		return sAINPresetClass;
	}

	private static SAINPresetClass CreateImpossiblePreset()
	{
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Invalid comparison between Unknown and I4
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Invalid comparison between Unknown and I4
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Invalid comparison between Unknown and I4
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Invalid comparison between Unknown and I4
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Invalid comparison between Unknown and I4
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Invalid comparison between Unknown and I4
		SAINPresetClass sAINPresetClass = new SAINPresetClass(SAINDifficulty.deathwish);
		GlobalSettingsClass globalSettings = sAINPresetClass.GlobalSettings;
		globalSettings.Shoot.RecoilMultiplier = 0.25f;
		globalSettings.Difficulty.ScatteringCoef = 0.01f;
		globalSettings.Difficulty.VisibleDistCoef = 3f;
		globalSettings.Difficulty.GainSightCoef = 0.65f;
		globalSettings.Difficulty.PrecisionSpeedCoef = 0.5f;
		globalSettings.Difficulty.AccuracySpeedCoef = 0.5f;
		globalSettings.Aiming.AimCenterMassGlobal = false;
		globalSettings.Look.NotLooking.NotLookingToggle = false;
		globalSettings.Aiming.PMCSAimForHead = true;
		foreach (KeyValuePair<WildSpawnType, SAINSettingsGroupClass> sAINSetting in sAINPresetClass.BotSettings.SAINSettings)
		{
			foreach (KeyValuePair<BotDifficulty, SAINSettingsClass> setting in sAINSetting.Value.Settings)
			{
				setting.Value.Core.VisibleAngle = 180f;
				setting.Value.Shoot.FireratMulti = 3f;
				setting.Value.Shoot.BurstMulti = 3f;
				setting.Value.Aiming.AimCenterMass = false;
				setting.Value.Core.VisibleAngle = 180f;
				setting.Value.Core.GainSightCoef *= 0.66f;
			}
		}
		foreach (KeyValuePair<WildSpawnType, SAINSettingsGroupClass> sAINSetting2 in sAINPresetClass.BotSettings.SAINSettings)
		{
			if (BotSettingsRepoClass.IsBossOrFollower(sAINSetting2.Key))
			{
				Dictionary<BotDifficulty, SAINSettingsClass> settings = sAINSetting2.Value.Settings;
				SAINSettingsClass sAINSettingsClass = settings[(BotDifficulty)0];
				sAINSettingsClass.Move.STRAFE_SPEED = 0.85f;
				SAINSettingsClass sAINSettingsClass2 = settings[(BotDifficulty)1];
				sAINSettingsClass2.Move.STRAFE_SPEED = 0.9f;
				SAINSettingsClass sAINSettingsClass3 = settings[(BotDifficulty)2];
				sAINSettingsClass3.Move.STRAFE_SPEED = 1f;
				SAINSettingsClass sAINSettingsClass4 = settings[(BotDifficulty)3];
				sAINSettingsClass4.Move.STRAFE_SPEED = 1f;
			}
			if (sAINSetting2.Key.IsPMC() || (int)sAINSetting2.Key == 24 || (int)sAINSetting2.Key == 9 || (int)sAINSetting2.Key == 34 || (int)sAINSetting2.Key == 35)
			{
				Dictionary<BotDifficulty, SAINSettingsClass> settings2 = sAINSetting2.Value.Settings;
				SAINSettingsClass sAINSettingsClass5 = settings2[(BotDifficulty)0];
				sAINSettingsClass5.Move.STRAFE_SPEED = 0.75f;
				SAINSettingsClass sAINSettingsClass6 = settings2[(BotDifficulty)1];
				sAINSettingsClass6.Move.STRAFE_SPEED = 0.9f;
				SAINSettingsClass sAINSettingsClass7 = settings2[(BotDifficulty)2];
				sAINSettingsClass7.Move.STRAFE_SPEED = 1f;
				SAINSettingsClass sAINSettingsClass8 = settings2[(BotDifficulty)3];
				sAINSettingsClass8.Move.STRAFE_SPEED = 1f;
			}
			if ((int)sAINSetting2.Key == 1 || (int)sAINSetting2.Key == 19)
			{
				Dictionary<BotDifficulty, SAINSettingsClass> settings3 = sAINSetting2.Value.Settings;
				SAINSettingsClass sAINSettingsClass9 = settings3[(BotDifficulty)0];
				sAINSettingsClass9.Move.STRAFE_SPEED = 0.65f;
				SAINSettingsClass sAINSettingsClass10 = settings3[(BotDifficulty)1];
				sAINSettingsClass10.Move.STRAFE_SPEED = 0.75f;
				SAINSettingsClass sAINSettingsClass11 = settings3[(BotDifficulty)2];
				sAINSettingsClass11.Move.STRAFE_SPEED = 0.9f;
				SAINSettingsClass sAINSettingsClass12 = settings3[(BotDifficulty)3];
				sAINSettingsClass12.Move.STRAFE_SPEED = 1f;
			}
		}
		return sAINPresetClass;
	}
}
