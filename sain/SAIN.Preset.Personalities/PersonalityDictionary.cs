using System.Collections.Generic;
using EFT;
using SAIN.Helpers;
using SAIN.Preset.GlobalSettings;
using SAIN.SAINComponent.Classes.Info;
using UnityEngine;

namespace SAIN.Preset.Personalities;

public class PersonalityDictionary : Dictionary<EPersonality, PersonalitySettingsClass>
{
	private class NickNames
	{
		public string Description = "Names are not case sensitive. Any bot nick name that contains one of the entries here will be forced to use the matching personality.";

		public Dictionary<string, EPersonality> NicknamePersonalityMatches = new Dictionary<string, EPersonality>
		{
			{
				"steve",
				EPersonality.Wreckless
			},
			{
				"solarint",
				EPersonality.GigaChad
			},
			{
				"lvndmark",
				EPersonality.SnappingTurtle
			},
			{
				"chomp",
				EPersonality.Chad
			},
			{
				"senko",
				EPersonality.Chad
			},
			{
				"kaeno",
				EPersonality.Timmy
			},
			{
				"justnu",
				EPersonality.Timmy
			},
			{
				"ratthew",
				EPersonality.Rat
			},
			{
				"choccy",
				EPersonality.Rat
			}
		};
	}

	private static readonly NickNames _nicknames;

	public Dictionary<EPersonality, List<string>> Nickname_Personalities = new Dictionary<EPersonality, List<string>>();

	static PersonalityDictionary()
	{
		if (!JsonUtility.Load.LoadObject<NickNames>(out _nicknames, "NicknamePersonalities"))
		{
			_nicknames = new NickNames();
			JsonUtility.SaveObjectToJson(_nicknames, "NicknamePersonalities");
		}
	}

	public EPersonality GetPersonality(SAINBotInfoClass infoClass, out PersonalitySettingsClass settings)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (checkForcePersonality(out var personality))
		{
			settings = base[personality];
			return personality;
		}
		personality = setNicknamePersonality(infoClass.Profile.NickName);
		if (personality != EPersonality.Normal)
		{
			settings = base[personality];
			return personality;
		}
		personality = setBossPersonality(infoClass.Profile.WildSpawnType);
		if (personality != EPersonality.Normal)
		{
			settings = base[personality];
			return personality;
		}
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				KeyValuePair<EPersonality, PersonalitySettingsClass> current = enumerator.Current;
				if (canBotBePersonality(infoClass, current.Key))
				{
					settings = current.Value;
					return current.Key;
				}
			}
		}
		personality = ((!infoClass.Profile.IsPMC || !EFTMath.RandomBool(33f)) ? EPersonality.Normal : EPersonality.Chad);
		settings = base[personality];
		return personality;
	}

	public PersonalitySettingsClass GetSettings(EPersonality personality)
	{
		if (TryGetValue(personality, out var value))
		{
			return value;
		}
		return null;
	}

	private bool checkForcePersonality(out EPersonality personality)
	{
		foreach (KeyValuePair<EPersonality, bool> item in SAINPlugin.LoadedPreset.GlobalSettings.Mind.ForcePersonality)
		{
			if (item.Value)
			{
				personality = item.Key;
				return true;
			}
		}
		personality = EPersonality.Normal;
		return false;
	}

	private EPersonality setNicknamePersonality(string nickname)
	{
		if (GClass1437.IsNullOrEmpty(nickname))
		{
			return EPersonality.Normal;
		}
		string text = nickname.ToLower();
		foreach (KeyValuePair<string, EPersonality> nicknamePersonalityMatch in _nicknames.NicknamePersonalityMatches)
		{
			if (text.Contains(nicknamePersonalityMatch.Key.ToLower()))
			{
				return nicknamePersonalityMatch.Value;
			}
		}
		return EPersonality.Normal;
	}

	private EPersonality setBossPersonality(WildSpawnType wildSpawnType)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (GlobalSettingsClass.Instance.Mind.PERS_BOSSES.TryGetValue(wildSpawnType, out var value))
		{
			return value;
		}
		return EPersonality.Normal;
	}

	private bool canBotBePersonality(SAINBotInfoClass infoClass, EPersonality personality)
	{
		if (!TryGetValue(personality, out var value))
		{
			return false;
		}
		PersonalityAssignmentSettings assignment = value.Assignment;
		if (!assignment.Enabled)
		{
			return false;
		}
		if (checkRandomAssignment(value))
		{
			return true;
		}
		if (meetsRequirements(infoClass, value))
		{
			float chance = getChance(infoClass.Profile.PowerLevel, value);
			if (EFTMath.RandomBool(chance))
			{
				return true;
			}
		}
		return false;
	}

	private bool checkRandomAssignment(PersonalitySettingsClass settings)
	{
		return settings.Assignment.CanBeRandomlyAssigned && EFTMath.RandomBool(settings.Assignment.RandomlyAssignedChance);
	}

	private bool meetsRequirements(SAINBotInfoClass infoClass, PersonalitySettingsClass settings)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		PersonalityAssignmentSettings assignment = settings.Assignment;
		return assignment.AllowedTypes.Contains(infoClass.Profile.WildSpawnType) && infoClass.Profile.PowerLevel <= assignment.PowerLevelMax && infoClass.Profile.PowerLevel > assignment.PowerLevelMin && (float)infoClass.Profile.PlayerLevel <= assignment.MaxLevel && (float)infoClass.Profile.PlayerLevel > assignment.MinLevel;
	}

	private float getChance(float powerLevel, PersonalitySettingsClass settings)
	{
		PersonalityAssignmentSettings assignment = settings.Assignment;
		powerLevel = Mathf.Clamp(powerLevel, 0f, 1000f);
		float num = (powerLevel - assignment.PowerLevelScaleStart) / (assignment.PowerLevelScaleEnd - assignment.PowerLevelScaleStart);
		if (assignment.InverseScale)
		{
			num = 1f - num;
		}
		float num2 = assignment.MaxChanceIfMeetRequirements * num;
		return Mathf.Clamp(num2, 0f, 100f);
	}
}
