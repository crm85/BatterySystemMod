using System.Collections.Generic;
using System.Text;
using SAIN.Components;
using SAIN.Helpers;
using SAIN.Models.Enums;
using UnityEngine;

namespace SAIN.BotController.Classes;

public class SquadPersonalityManager
{
	private static readonly List<EPersonality> MemberPersonalities = new List<EPersonality>();

	private static readonly Dictionary<EPersonality, int> PersonalityCounts = new Dictionary<EPersonality, int>();

	private static readonly Dictionary<ESquadPersonality, SquadPersonalitySettings> SquadSettings = new Dictionary<ESquadPersonality, SquadPersonalitySettings>();

	public static ESquadPersonality GetSquadPersonality(Dictionary<string, BotComponent> Members, out SquadPersonalitySettings settings)
	{
		GetMemberPersonalities(Members);
		int count;
		EPersonality mostFrequentPersonality = GetMostFrequentPersonality(PersonalityCounts, out count);
		ESquadPersonality eSquadPersonality = PickSquadPersonality(mostFrequentPersonality);
		settings = GetSquadSettings(eSquadPersonality);
		return eSquadPersonality;
	}

	private static void GetMemberPersonalities(Dictionary<string, BotComponent> Members)
	{
		PersonalityCounts.Clear();
		MemberPersonalities.Clear();
		foreach (BotComponent value in Members.Values)
		{
			if ((Object)(object)value?.Player != (Object)null && value.Player.HealthController.IsAlive)
			{
				EPersonality personality = value.Info.Personality;
				MemberPersonalities.Add(personality);
				if (!PersonalityCounts.ContainsKey(personality))
				{
					PersonalityCounts.Add(personality, 1);
				}
				else
				{
					PersonalityCounts[personality]++;
				}
			}
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<EPersonality, int> personalityCount in PersonalityCounts)
		{
			stringBuilder.AppendLine($"[{personalityCount.Key}] : [{personalityCount.Value}]");
		}
	}

	private static EPersonality GetMostFrequentPersonality(Dictionary<EPersonality, int> PersonalityCounts, out int count)
	{
		count = 0;
		EPersonality result = EPersonality.Normal;
		foreach (KeyValuePair<EPersonality, int> PersonalityCount in PersonalityCounts)
		{
			if (PersonalityCount.Value > count)
			{
				count = PersonalityCount.Value;
				result = PersonalityCount.Key;
			}
		}
		return result;
	}

	private static ESquadPersonality PickSquadPersonality(EPersonality mostFrequentPersonality)
	{
		ESquadPersonality eSquadPersonality = ESquadPersonality.None;
		switch (mostFrequentPersonality)
		{
		case EPersonality.Wreckless:
		case EPersonality.GigaChad:
		case EPersonality.Chad:
			return EFTMath.RandomBool(66f) ? ESquadPersonality.GigaChads : ESquadPersonality.Elite;
		case EPersonality.Timmy:
		case EPersonality.Coward:
			return ESquadPersonality.TimmyTeam6;
		case EPersonality.SnappingTurtle:
		case EPersonality.Rat:
			return ESquadPersonality.Rats;
		default:
			return GClass1835.PickRandom<ESquadPersonality>((IReadOnlyList<ESquadPersonality>)EnumValues.GetEnum<ESquadPersonality>());
		}
	}

	private static SquadPersonalitySettings GetSquadSettings(ESquadPersonality squadPersonality)
	{
		return squadPersonality switch
		{
			ESquadPersonality.Elite => CreateSettings(squadPersonality, 2f, 5f, 4f), 
			ESquadPersonality.GigaChads => CreateSettings(squadPersonality, 5f, 4f, 5f), 
			ESquadPersonality.Rats => CreateSettings(squadPersonality, 1f, 2f, 1f), 
			ESquadPersonality.TimmyTeam6 => CreateSettings(squadPersonality, 3f, 1f, 2f), 
			_ => CreateSettings(squadPersonality, 3f, 3f, 3f), 
		};
	}

	private static SquadPersonalitySettings CreateSettings(ESquadPersonality squadPersonality, float vocalization, float coordination, float aggression)
	{
		if (!SquadSettings.ContainsKey(squadPersonality))
		{
			SquadPersonalitySettings value = new SquadPersonalitySettings
			{
				VocalizationLevel = vocalization,
				CoordinationLevel = coordination,
				AggressionLevel = aggression
			};
			SquadSettings.Add(squadPersonality, value);
		}
		return SquadSettings[squadPersonality];
	}
}
