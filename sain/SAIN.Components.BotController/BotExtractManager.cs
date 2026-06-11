using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Comfort.Common;
using EFT;
using EFT.Interactive;
using SAIN.Plugin;
using SAIN.SAINComponent.Classes.Info;
using SPT.SinglePlayer.Utils.InRaid;
using UnityEngine;
using UnityEngine.AI;

namespace SAIN.Components.BotController;

public class BotExtractManager : BotManagerBase
{
	public readonly List<string> ExtractedBots = new List<string>();

	public readonly List<ExtractionInfo> BotExtractionInfos = new List<ExtractionInfo>();

	private Dictionary<ExfiltrationPoint, float> exfilActivationTimes = new Dictionary<ExfiltrationPoint, float>();

	private float exfilSearchRetryDelay = 10f;

	private Dictionary<BotComponent, float> botExfilSearchRetryTime = new Dictionary<BotComponent, float>();

	private float CheckRaidProgressTimer = 0f;

	public float TotalRaidTime { get; private set; }

	public static float MinDistanceToExtract { get; private set; } = 10f;

	public float TimeRemaining { get; private set; } = 999f;

	public float PercentageRemaining { get; private set; } = 100f;

	public BotExtractManager(BotManagerComponent botController)
		: base(botController)
	{
	}

	public void Update(float currentTime, float deltaTime)
	{
		AbstractGame instance = Singleton<AbstractGame>.Instance;
		if (((instance != null) ? instance.GameTimer : null) != null && !(CheckRaidProgressTimer > currentTime))
		{
			CheckTimeRemaining();
			CheckRaidProgressTimer = currentTime + 5f;
		}
	}

	public void LogExtractionOfBot(BotOwner bot, Vector3 point, string reason, ExfiltrationPoint exfil)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		Logger.LogInfo($"{((Object)bot).name} Extracted because {reason} at {point} for extract {exfil.Settings.Name} at {DateTime.UtcNow}");
		BotExtractionInfos.Add(new ExtractionInfo(bot, reason, exfil));
		ExtractedBots.Add(bot.GetPlayer.ProfileId);
	}

	public bool HasExfilBeenActivated(ExfiltrationPoint exfil)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Invalid comparison between Unknown and I4
		if ((int)exfil.Settings.ExfiltrationType == 1 && (int)exfil.Status == 2 && exfilActivationTimes.ContainsKey(exfil))
		{
			exfilActivationTimes.Remove(exfil);
		}
		return exfilActivationTimes.ContainsKey(exfil);
	}

	public float GetTimeRemainingForExfil(ExfiltrationPoint exfil)
	{
		if (!HasExfilBeenActivated(exfil))
		{
			return float.MaxValue;
		}
		return Math.Max(0f, exfilActivationTimes[exfil] - Time.time);
	}

	public float GetExfilTime(ExfiltrationPoint exfil)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Invalid comparison between Unknown and I4
		if (HasExfilBeenActivated(exfil))
		{
			return exfilActivationTimes[exfil];
		}
		float num = Time.time + exfil.Settings.ExfiltrationTime;
		if (exfil.Requirements.Any((ExfiltrationRequirement x) => (int)x.Requirement == 11))
		{
			return num;
		}
		if ((int)exfil.Settings.ExfiltrationType == 1)
		{
			num += 0.2f;
			exfilActivationTimes.Add(exfil, num);
		}
		return num;
	}

	public void ResetExfilSearchTime(BotComponent bot)
	{
		if (botExfilSearchRetryTime.ContainsKey(bot))
		{
			botExfilSearchRetryTime[bot] = Time.time + exfilSearchRetryDelay;
		}
		else
		{
			botExfilSearchRetryTime.Add(bot, Time.time + exfilSearchRetryDelay);
		}
	}

	public bool TryFindExfilForBot(BotComponent bot)
	{
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)bot == (Object)null)
		{
			if (SAINPlugin.DebugMode)
			{
				Logger.LogInfo("Skipped searching for Exfil for unknown bot because they are null");
			}
			return false;
		}
		if (botExfilSearchRetryTime.ContainsKey(bot) && Time.time < botExfilSearchRetryTime[bot])
		{
			return false;
		}
		if (!IsBotAllowedToExfil(bot))
		{
			return false;
		}
		if (bot.Memory.Extract.ExfilPosition.HasValue && (Object)(object)bot.Memory.Extract.ExfilPoint != (Object)null)
		{
			return true;
		}
		if (SAINPlugin.DebugMode)
		{
			Logger.LogInfo("Looking for Exfil for " + ((Object)bot).name + "...");
		}
		if (GameWorldHandler.SAINGameWorld.ExtractFinder.CountValidExfilsForBot(bot) == 0)
		{
			if (SAINPlugin.DebugMode)
			{
				Logger.LogInfo("Could not select exfil for " + ((Object)bot).name + "; no valid ones found");
			}
			ResetExfilSearchTime(bot);
			return false;
		}
		if (!(bot.Squad.BotInGroup ? TryAssignSquadExfil(bot) : TryAssignExfilForBot(bot)))
		{
			if (SAINPlugin.DebugMode)
			{
				Logger.LogInfo($"{((Object)bot).name} could not find exfil. Bot spawn type: {bot.Info.Profile.WildSpawnType}");
			}
			ResetExfilSearchTime(bot);
			return false;
		}
		Logger.LogInfo(((Object)bot).name + " has selected " + bot.Memory.Extract.ExfilPoint.Settings.Name + " for extraction");
		return true;
	}

	public static bool IsBotAllowedToExfil(BotComponent bot)
	{
		if (!bot.Info.Profile.IsPMC && !bot.Info.Profile.IsScav)
		{
			return false;
		}
		return true;
	}

	private bool TryAssignExfilForBot(BotComponent bot)
	{
		IDictionary<ExfiltrationPoint, Vector3> validExfilsForBot = GameWorldHandler.SAINGameWorld.ExtractFinder.GetValidExfilsForBot(bot);
		bot.Memory.Extract.ExfilPoint = SelectExfilForBot(bot, validExfilsForBot);
		return (Object)(object)bot.Memory.Extract.ExfilPoint != (Object)null;
	}

	private ExfiltrationPoint SelectExfilForBot(BotComponent bot, IDictionary<ExfiltrationPoint, Vector3> validExfils)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Invalid comparison between Unknown and I4
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		NavMeshPath val = new NavMeshPath();
		Dictionary<ExfiltrationPoint, Vector3> dictionary = new Dictionary<ExfiltrationPoint, Vector3>();
		foreach (KeyValuePair<ExfiltrationPoint, Vector3> validExfil in validExfils)
		{
			ExfiltrationPoint key = validExfil.Key;
			Vector3 value = validExfil.Value;
			if (CanBotsUseExtract(key) && !(Vector3.Distance(bot.Position, value) <= MinDistanceToExtract) && NavMesh.CalculatePath(bot.Position, value, -1, val) && (int)val.status <= 0)
			{
				dictionary.Add(key, value);
			}
		}
		if (dictionary.Count == 0)
		{
			if (SAINPlugin.DebugMode)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append($"Could not assign bot {((Object)bot).name} to any of {validExfils.Count} valid exfils: ");
				bool flag = true;
				foreach (KeyValuePair<ExfiltrationPoint, Vector3> validExfil2 in validExfils)
				{
					if (!flag)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(validExfil2.Key.Settings.Name);
					flag = false;
				}
				Logger.LogInfo(stringBuilder.ToString());
			}
			return null;
		}
		KeyValuePair<ExfiltrationPoint, Vector3> keyValuePair = GClass3760.Random<KeyValuePair<ExfiltrationPoint, Vector3>>((IEnumerable<KeyValuePair<ExfiltrationPoint, Vector3>>)dictionary);
		bot.Memory.Extract.ExfilPosition = keyValuePair.Value;
		if (SAINPlugin.DebugMode)
		{
			Logger.LogInfo("bot " + ((Object)bot).name + " will extract at " + keyValuePair.Key.Settings.Name);
		}
		return keyValuePair.Key;
	}

	public bool CanBotsUseExtract(ExfiltrationPoint exfil)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Invalid comparison between Unknown and I4
		if ((int)exfil.Status == 1)
		{
			return false;
		}
		if (exfil.Requirements.Any((ExfiltrationRequirement x) => (int)x.Requirement == 10))
		{
			return false;
		}
		if (exfil.Requirements.Any((ExfiltrationRequirement x) => (int)x.Requirement == 11))
		{
			return false;
		}
		if ((int)exfil.Status == 2 && exfil.Requirements.Any((ExfiltrationRequirement x) => (int)x.Requirement == 3))
		{
			return false;
		}
		if (GetTimeRemainingForExfil(exfil) < 1f)
		{
			return false;
		}
		return true;
	}

	private bool TryAssignSquadExfil(BotComponent bot)
	{
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		SAINSquadClass squad = bot.Squad;
		if (squad.IAmLeader)
		{
			if (!bot.Memory.Extract.ExfilPosition.HasValue && !TryAssignExfilForBot(bot))
			{
				return false;
			}
			if (bot.Memory.Extract.ExfilPosition.HasValue && squad.Members != null && squad.Members.Count > 0)
			{
				NavMeshHit val3 = default(NavMeshHit);
				foreach (KeyValuePair<string, BotComponent> member in squad.Members)
				{
					if (!member.Value.Memory.Extract.ExfilPosition.HasValue && member.Value.ProfileId != bot.ProfileId)
					{
						Vector3 val = Random.onUnitSphere * 2f;
						val.y = 0f;
						Vector3 val2 = bot.Memory.Extract.ExfilPosition.Value + val;
						if (NavMesh.SamplePosition(val2, ref val3, 1f, -1))
						{
							member.Value.Memory.Extract.ExfilPosition = ((NavMeshHit)(ref val3)).position;
						}
						else
						{
							member.Value.Memory.Extract.ExfilPosition = bot.Memory.Extract.ExfilPosition;
						}
						member.Value.Memory.Extract.ExfilPoint = bot.Memory.Extract.ExfilPoint;
					}
				}
			}
		}
		else
		{
			bot.Memory.Extract.ExfilPoint = squad.LeaderComponent?.Memory.Extract.ExfilPoint;
			bot.Memory.Extract.ExfilPosition = squad.LeaderComponent?.Memory.Extract.ExfilPosition;
		}
		return bot.Memory.Extract.ExfilPosition.HasValue && (Object)(object)bot.Memory.Extract.ExfilPoint != (Object)null;
	}

	private void CheckTimeRemaining()
	{
		TotalRaidTime = RaidChangesUtil.OriginalEscapeTimeSeconds;
		if (GClass1664.Started(Singleton<AbstractGame>.Instance.GameTimer))
		{
			TimeRemaining = RaidTimeUtil.GetRemainingRaidSeconds();
			PercentageRemaining = RaidTimeUtil.GetRaidTimeRemainingFraction() * 100f;
		}
		else
		{
			TimeRemaining = RaidChangesUtil.NewEscapeTimeSeconds;
			PercentageRemaining = 100f * TimeRemaining / TotalRaidTime;
		}
	}
}
