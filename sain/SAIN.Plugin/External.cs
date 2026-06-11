using System.Collections.Generic;
using EFT;
using SAIN.Components;
using SAIN.Components.BotController;
using SAIN.SAINComponent.Classes;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;
using UnityEngine.AI;

namespace SAIN.Plugin;

public static class External
{
	public enum ECombatReason
	{
		None,
		EnemyVisible,
		EnemyHeardRecently,
		EnemySeenRecently,
		UnderFireNow,
		UnderFireRecently
	}

	private static bool DebugExternal => SAINPlugin.DebugSettings.Logs.DebugExternal;

	public static bool IgnoreHearing(BotOwner bot, bool value, bool ignoreUnderFire, float duration)
	{
		BotComponent botComponent = GetBotComponent(bot);
		if ((Object)(object)botComponent == (Object)null)
		{
			return false;
		}
		string reason;
		return botComponent.Hearing.SoundInput.SetIgnoreHearingExternal(value, ignoreUnderFire, duration, out reason);
	}

	public static string GetPersonality(BotOwner bot)
	{
		BotComponent botComponent = GetBotComponent(bot);
		if ((Object)(object)botComponent == (Object)null)
		{
			return string.Empty;
		}
		return botComponent.Info.Personality.ToString();
	}

	private static BotComponent GetBotComponent(BotOwner bot)
	{
		BotManagerComponent instance = BotManagerComponent.Instance;
		if (instance != null && instance.GetSAIN(bot, out var bot2))
		{
			return bot2;
		}
		return ((Component)bot).GetComponent<BotComponent>();
	}

	public static bool ExtractBot(BotOwner bot)
	{
		BotComponent botComponent = GetBotComponent(bot);
		if ((Object)(object)botComponent == (Object)null)
		{
			return false;
		}
		botComponent.Info.ForceExtract = true;
		return true;
	}

	public static void GetExtractedBots(List<string> list)
	{
		BotManagerComponent instance = BotManagerComponent.Instance;
		if ((Object)(object)instance == (Object)null)
		{
			Logger.LogWarning("SAIN Bot Controller is Null, cannot retrieve Extracted Bots List.");
			return;
		}
		List<string> list2 = instance.BotExtractManager?.ExtractedBots;
		if (list2 == null)
		{
			Logger.LogWarning("List of extracted bots is null! Cannot copy list.");
			return;
		}
		list.Clear();
		list.AddRange(list2);
	}

	public static void GetExtractionInfos(List<ExtractionInfo> list)
	{
		BotManagerComponent instance = BotManagerComponent.Instance;
		if ((Object)(object)instance == (Object)null)
		{
			Logger.LogWarning("SAIN Bot Controller is Null, cannot retrieve Extracted Bots List.");
			return;
		}
		List<ExtractionInfo> list2 = instance.BotExtractManager?.BotExtractionInfos;
		if (list2 == null)
		{
			Logger.LogWarning("List of extracted bots is null! Cannot copy list.");
			return;
		}
		list.Clear();
		list.AddRange(list2);
	}

	public static bool TrySetExfilForBot(BotOwner bot)
	{
		BotComponent botComponent = GetBotComponent(bot);
		if ((Object)(object)botComponent == (Object)null)
		{
			return false;
		}
		if (!BotExtractManager.IsBotAllowedToExfil(botComponent))
		{
			Logger.LogWarning(((Object)bot).name + " is not allowed to use extracting logic.");
		}
		if (!BotManagerComponent.Instance.BotExtractManager.TryFindExfilForBot(botComponent))
		{
			return false;
		}
		return true;
	}

	public static float TimeSinceSenseEnemy(BotOwner botOwner)
	{
		BotComponent botComponent = GetBotComponent(botOwner);
		if ((Object)(object)botComponent == (Object)null)
		{
			return float.MaxValue;
		}
		return botComponent.Enemy?.TimeSinceLastKnownUpdated ?? float.MaxValue;
	}

	public static bool IsPathTowardEnemy(NavMeshPath path, BotOwner botOwner, float ratioSameOverAll = 0.25f, float sqrDistCheck = 0.05f)
	{
		BotComponent botComponent = GetBotComponent(botOwner);
		if ((Object)(object)botComponent == (Object)null)
		{
			return false;
		}
		Enemy enemy = botComponent.Enemy;
		if (enemy == null)
		{
			return false;
		}
		if (SAINBotSpaceAwareness.ArePathsDifferent(path, enemy.Path.PathToEnemy, ratioSameOverAll, sqrDistCheck))
		{
			return false;
		}
		return true;
	}

	public static bool CanBotQuest(BotOwner botOwner, Vector3 questPosition, float dotProductThresh = 0.33f)
	{
		BotComponent botComponent = GetBotComponent(botOwner);
		if ((Object)(object)botComponent == (Object)null)
		{
			return false;
		}
		if (IsBotInCombat(botComponent, out var reason))
		{
			if (DebugExternal)
			{
				Logger.LogInfo($"{((Object)botOwner).name} is currently engaging an enemy, cannot quest. Reason: [{reason}]");
			}
			return false;
		}
		if (IsBotSearching(botComponent))
		{
			if (DebugExternal)
			{
				Logger.LogInfo(((Object)botOwner).name + " is currently searching and hasn't cleared last known position, cannot quest.");
			}
			return false;
		}
		return true;
	}

	public static bool IsQuestTowardTarget(BotComponent component, Vector3 questPosition, float dotProductThresh)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		Vector3? currentTargetPosition = component.CurrentTargetPosition;
		if (!currentTargetPosition.HasValue)
		{
			return false;
		}
		Vector3 position = component.Position;
		Vector3 val = currentTargetPosition.Value - position;
		Vector3 val2 = questPosition - position;
		return Vector3.Dot(((Vector3)(ref val)).normalized, ((Vector3)(ref val2)).normalized) > dotProductThresh;
	}

	private static bool IsBotSearching(BotComponent component)
	{
		if (component.Decision.CurrentCombatDecision == ECombatDecision.Search || component.Decision.CurrentSquadDecision == ESquadDecision.Search)
		{
			return !component.Search.PathFinder.SearchedTargetPosition;
		}
		return false;
	}

	private static bool IsBotInCombat(BotComponent component, out ECombatReason reason)
	{
		reason = ECombatReason.None;
		Enemy enemy = component?.CurrentTarget.CurrentTargetEnemy;
		if (enemy == null)
		{
			return false;
		}
		if (enemy.IsVisible)
		{
			reason = ECombatReason.EnemyVisible;
			return true;
		}
		if (enemy.TimeSinceSeen < 10f)
		{
			reason = ECombatReason.EnemySeenRecently;
			return true;
		}
		if (enemy.TimeSinceHeard < 5f)
		{
			reason = ECombatReason.EnemyHeardRecently;
			return true;
		}
		BotMemoryClass memory = component.BotOwner.Memory;
		if (memory.IsUnderFire)
		{
			reason = ECombatReason.UnderFireNow;
			return true;
		}
		if (memory.UnderFireTime + 10f < Time.time)
		{
			reason = ECombatReason.UnderFireRecently;
			return true;
		}
		return false;
	}
}
