using System.Collections.Generic;
using System.ComponentModel;
using EFT;
using Newtonsoft.Json;
using SAIN.Attributes;

namespace SAIN.Preset.BotSettings.SAINSettings;

public class SAINSettingsGroupClass
{
	[JsonProperty]
	public string Name;

	[JsonProperty]
	public WildSpawnType WildSpawnType;

	[JsonProperty]
	[NameAndDescription("Difficulty Modifier", "How much to improve this bot type's recoil handling, fire-rate, and full auto burst length, reaction time, general stats that are used in SAIN.")]
	[DefaultValue(0.5f)]
	[MinMax(0.01f, 1f, 100f)]
	public float DifficultyModifier = 0.5f;

	[JsonProperty]
	public Dictionary<BotDifficulty, SAINSettingsClass> Settings = new Dictionary<BotDifficulty, SAINSettingsClass>();

	[JsonConstructor]
	public SAINSettingsGroupClass()
	{
	}

	public SAINSettingsGroupClass(BotDifficulty[] difficulties)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		foreach (BotDifficulty key in difficulties)
		{
			Settings.Add(key, new SAINSettingsClass());
		}
	}

	public void Init()
	{
		foreach (SAINSettingsClass value in Settings.Values)
		{
			value.Init();
		}
	}

	public void UpdateDefaults()
	{
		foreach (SAINSettingsClass value in Settings.Values)
		{
			value.UpdateDefaults();
		}
	}
}
