using System.Collections.Generic;
using EFT;
using SAIN.Components;
using SAIN.Helpers;
using SAIN.Preset.BotSettings.SAINSettings;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Info;

public class BotProfile : BotBase
{
	public readonly string Name;

	public readonly string NickName;

	public readonly bool IsBoss;

	public readonly bool IsFollower;

	public readonly bool IsScav;

	public readonly bool IsPMC;

	public readonly bool IsPlayerScav;

	public readonly BotDifficulty BotDifficulty;

	public readonly WildSpawnType WildSpawnType;

	public readonly EPlayerSide Side;

	public readonly int PlayerLevel;

	public float DifficultyModifier { get; private set; }

	public float DifficultyModifierSqrt { get; private set; }

	public float PowerLevel => base.BotOwner.AIData.PowerOfEquipment;

	public BotProfile(BotComponent sain)
		: base(sain)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		Name = ((Object)sain.BotOwner).name;
		Profile profile = sain.BotOwner.Profile;
		Side = profile.Side;
		WildSpawnType = profile.Info.Settings.Role;
		BotDifficulty = profile.Info.Settings.BotDifficulty;
		PlayerLevel = profile.Info.Level;
		IsBoss = EnumValues.WildSpawn.IsBoss(WildSpawnType);
		IsFollower = EnumValues.WildSpawn.IsFollower(WildSpawnType);
		IsScav = EnumValues.WildSpawn.IsScav(WildSpawnType);
		IsPMC = EnumValues.WildSpawn.IsPMC(WildSpawnType);
		IsPlayerScav = IsScav && SAINEnableClass.IsPlayerScav(profile);
		SetDiffModifier(BotDifficulty);
	}

	private void SetDiffModifier(BotDifficulty difficulty)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected I4, but got Unknown
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		float num = 1f;
		Dictionary<WildSpawnType, SAINSettingsGroupClass> sAINSettings = SAINPlugin.LoadedPreset.BotSettings.SAINSettings;
		if (sAINSettings.ContainsKey(WildSpawnType))
		{
			num = sAINSettings[WildSpawnType].DifficultyModifier;
		}
		switch ((int)difficulty)
		{
		case 0:
			num *= 0.5f;
			break;
		case 1:
			num *= 1f;
			break;
		case 2:
			num *= 1.5f;
			break;
		case 3:
			num *= 1.75f;
			break;
		}
		DifficultyModifier = num.Round100();
		DifficultyModifierSqrt = Mathf.Sqrt(num).Round100();
	}
}
