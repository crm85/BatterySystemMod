using System.Collections.Generic;
using EFT;
using Newtonsoft.Json;

namespace SAIN.Helpers;

public class EFTBotSettings
{
	[JsonProperty]
	public string Name;

	[JsonProperty]
	public WildSpawnType WildSpawnType;

	[JsonProperty]
	public Dictionary<BotDifficulty, BotSettingsComponents> Settings = new Dictionary<BotDifficulty, BotSettingsComponents>();

	[JsonConstructor]
	public EFTBotSettings()
	{
	}

	public EFTBotSettings(string name, WildSpawnType type, BotDifficulty[] difficulties)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		Name = name;
		WildSpawnType = type;
		foreach (BotDifficulty val in difficulties)
		{
			Settings.Add(val, GClass598.GetSettings(val, type));
		}
	}
}
