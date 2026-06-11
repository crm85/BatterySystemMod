using System.Collections.Generic;
using System.Reflection;
using SAIN.Components;
using SAIN.Helpers;
using SAIN.Preset;
using SAIN.Preset.BotSettings.SAINSettings;
using SAIN.Preset.Personalities;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Info;

public class SAINBotInfoClass : BotComponentClassBase
{
	private SAINSettingsClass _fileSettings;

	private static FieldInfo[] EFTSettingsCategories;

	private static FieldInfo[] SAINSettingsCategories;

	private static readonly Dictionary<FieldInfo, FieldInfo[]> EFTSettingsFields = new Dictionary<FieldInfo, FieldInfo[]>();

	private static readonly Dictionary<FieldInfo, FieldInfo[]> SAINSettingsFields = new Dictionary<FieldInfo, FieldInfo[]>();

	public SAINSettingsClass FileSettings
	{
		get
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			if (_fileSettings == null)
			{
				_fileSettings = SAINPresetClass.Instance.BotSettings.GetSAINSettings(Profile.WildSpawnType, Profile.BotDifficulty);
			}
			return _fileSettings;
		}
	}

	public BotDifficultyClass Difficulty { get; }

	public BotProfile Profile { get; private set; }

	public WeaponInfoClass WeaponInfo { get; private set; }

	public EPersonality Personality { get; private set; }

	public PersonalityBehaviorSettings PersonalitySettings => PersonalitySettingsClass?.Behavior;

	public PersonalitySettingsClass PersonalitySettingsClass { get; private set; }

	public float TimeBeforeSearch { get; private set; } = 0f;

	public float HoldGroundDelay { get; private set; }

	public float PercentageBeforeExtract { get; set; } = -1f;

	public bool ForceExtract { get; set; } = false;

	public float ForgetEnemyTime { get; private set; }

	public float AggressionMultiplier => Difficulty.AggressionModifier;

	public SAINBotInfoClass(BotComponent sain)
		: base(sain)
	{
		base.TickRequirement = ESAINTickState.OnlyNoSleep;
		Profile = new BotProfile(sain);
		WeaponInfo = new WeaponInfoClass(sain);
		Personality = GetPersonality(out var settings);
		PersonalitySettingsClass = settings;
		Difficulty = new BotDifficultyClass(sain);
	}

	public override void Init()
	{
		ConfigureBot(SAINPlugin.LoadedPreset);
		WeaponInfo.Init();
		Difficulty.Init();
		base.Init();
	}

	public override void ManualUpdate()
	{
		WeaponInfo.ManualUpdate();
		base.ManualUpdate();
	}

	public override void Dispose()
	{
		WeaponInfo.Dispose();
		Difficulty.Dispose();
	}

	public void SetPersonality(EPersonality personality)
	{
		if (SAINPlugin.LoadedPreset.PersonalityManager.PersonalityDictionary.TryGetValue(personality, out var value))
		{
			PersonalitySettingsClass = value;
			Personality = personality;
		}
	}

	private void ConfigureBot(SAINPresetClass preset)
	{
		Difficulty.UpdateSettings(preset);
		CalcTimeBeforeSearch();
		CalcHoldGroundDelay();
		UpdateExtractTime();
		SetConfigValues(FileSettings);
	}

	protected override void UpdatePresetSettings(SAINPresetClass preset)
	{
		ConfigureBot(preset);
		base.UpdatePresetSettings(preset);
	}

	public void CalcHoldGroundDelay()
	{
		PersonalityBehaviorSettings personalitySettings = PersonalitySettings;
		float value = personalitySettings.General.HoldGroundBaseTime * AggressionMultiplier;
		float holdGroundMinRandom = personalitySettings.General.HoldGroundMinRandom;
		float holdGroundMaxRandom = personalitySettings.General.HoldGroundMaxRandom;
		HoldGroundDelay = value.Randomize(holdGroundMinRandom, holdGroundMaxRandom).Round100();
	}

	public void CalcTimeBeforeSearch()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Invalid comparison between Unknown and I4
		float value = (((int)Profile.WildSpawnType == 6 || (int)Profile.WildSpawnType == 22) ? 0.1f : ((!Profile.IsFollower || !base.Bot.Squad.BotInGroup) ? PersonalitySettings.Search.SearchBaseTime : 10f));
		value = (value.Randomize(0.66f, 1.33f) / AggressionMultiplier).Round100();
		if (value < 0.1f)
		{
			value = 0.1f;
		}
		TimeBeforeSearch = value;
		float num = 30f.Randomize(0.75f, 1.25f).Round100();
		float num2 = value + num;
		if (num2 < 240f)
		{
			num2 = 240f.Randomize(0.9f, 1.1f).Round100();
		}
		base.BotOwner.Settings.FileSettings.Mind.TIME_TO_FORGOR_ABOUT_ENEMY_SEC = num2;
		ForgetEnemyTime = num2;
	}

	private void UpdateExtractTime()
	{
		float percentageBeforeExtract = Random.Range(FileSettings.Mind.MinExtractPercentage, FileSettings.Mind.MaxExtractPercentage);
		SAINSquadClass sAINSquadClass = base.Bot?.Squad;
		Dictionary<string, BotComponent> dictionary = sAINSquadClass?.Members;
		if (sAINSquadClass != null && sAINSquadClass.BotInGroup && dictionary != null && dictionary.Count > 0)
		{
			if (sAINSquadClass.IAmLeader)
			{
				PercentageBeforeExtract = percentageBeforeExtract;
				{
					foreach (KeyValuePair<string, BotComponent> item in dictionary)
					{
						SAINBotInfoClass sAINBotInfoClass = item.Value?.Info;
						if (sAINBotInfoClass != null)
						{
							sAINBotInfoClass.PercentageBeforeExtract = percentageBeforeExtract;
						}
					}
					return;
				}
			}
			if (PercentageBeforeExtract == -1f)
			{
				SAINBotInfoClass sAINBotInfoClass2 = sAINSquadClass?.LeaderComponent?.Info;
				if (sAINBotInfoClass2 != null)
				{
					PercentageBeforeExtract = sAINBotInfoClass2.PercentageBeforeExtract;
				}
			}
		}
		else
		{
			PercentageBeforeExtract = percentageBeforeExtract;
		}
	}

	public EPersonality GetPersonality(out PersonalitySettingsClass settings)
	{
		return SAINPlugin.LoadedPreset.PersonalityManager.PersonalityDictionary.GetPersonality(this, out settings);
	}

	private void SetConfigValues(SAINSettingsClass sainFileSettings)
	{
		BotSettingsComponents fileSettings = base.BotOwner.Settings.FileSettings;
		fileSettings.Shoot.NOT_TO_SEE_ENEMY_TO_WANT_RELOAD_SEC = float.MaxValue;
		if (EFTSettingsCategories == null)
		{
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public;
			EFTSettingsCategories = ((object)fileSettings).GetType().GetFields(bindingAttr);
			FieldInfo[] eFTSettingsCategories = EFTSettingsCategories;
			foreach (FieldInfo fieldInfo in eFTSettingsCategories)
			{
				EFTSettingsFields.Add(fieldInfo, fieldInfo.FieldType.GetFields(bindingAttr));
			}
			SAINSettingsCategories = sainFileSettings.GetType().GetFields(bindingAttr);
			FieldInfo[] sAINSettingsCategories = SAINSettingsCategories;
			foreach (FieldInfo fieldInfo2 in sAINSettingsCategories)
			{
				SAINSettingsFields.Add(fieldInfo2, fieldInfo2.FieldType.GetFields(bindingAttr));
			}
		}
		FieldInfo[] sAINSettingsCategories2 = SAINSettingsCategories;
		foreach (FieldInfo fieldInfo3 in sAINSettingsCategories2)
		{
			FieldInfo fieldInfo4 = Reflection.FindFieldByName(fieldInfo3.Name, EFTSettingsCategories);
			if (!(fieldInfo4 != null))
			{
				continue;
			}
			object value = fieldInfo3.GetValue(sainFileSettings);
			object value2 = fieldInfo4.GetValue(fileSettings);
			FieldInfo[] array = SAINSettingsFields[fieldInfo3];
			FieldInfo[] fields = EFTSettingsFields[fieldInfo4];
			FieldInfo[] array2 = array;
			foreach (FieldInfo fieldInfo5 in array2)
			{
				FieldInfo fieldInfo6 = Reflection.FindFieldByName(fieldInfo5.Name, fields);
				if (fieldInfo6 != null)
				{
					object value3 = fieldInfo5.GetValue(value);
					if (SAINPlugin.DebugMode)
					{
					}
					fieldInfo6.SetValue(value2, value3);
				}
			}
		}
	}
}
