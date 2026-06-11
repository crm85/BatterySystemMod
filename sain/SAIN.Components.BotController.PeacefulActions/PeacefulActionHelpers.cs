using System.Collections.Generic;
using UnityEngine;

namespace SAIN.Components.BotController.PeacefulActions;

public static class PeacefulActionHelpers
{
	public static bool findBotsForPeacefulAction(BotZoneData data, List<BotComponent> localList, List<BotComponent> selectedList, float maxRangeSqr = -1f)
	{
		List<BotComponent> allContainedBots = data.AllContainedBots;
		int count = allContainedBots.Count;
		if (count < 2)
		{
			return false;
		}
		Logger.LogDebug($"Currently [{count}] bots in BotZone [{data.Name}]");
		List<BotComponent> allPeacefulBots = data.AllPeacefulBots;
		int count2 = allPeacefulBots.Count;
		if (count2 < 2)
		{
			return false;
		}
		Logger.LogDebug($"Currently [{count2}] peaceful bots in BotZone [{data.Name}]");
		localList.Clear();
		localList.AddRange(allPeacefulBots);
		for (int num = count - 1; num >= 0; num--)
		{
			BotComponent bot = localList[num];
			if (selectedList.findFriendlyBotsandFilter(localList, bot, maxRangeSqr))
			{
				return true;
			}
		}
		return false;
	}

	public static bool findFriendlyBotsandFilter(this List<BotComponent> result, List<BotComponent> localList, BotComponent bot, float minSqrMag = -1f)
	{
		if ((Object)(object)bot == (Object)null)
		{
			return false;
		}
		result.Clear();
		result.Add(bot);
		result.FindBotsOfSameSide(bot, localList);
		if (result.Count < 2)
		{
			localList.Remove(bot);
			return false;
		}
		if (minSqrMag > 0f)
		{
			result.FilterBotsBySquareMagnitude(bot, minSqrMag);
			if (result.Count < 2)
			{
				localList.Remove(bot);
				return false;
			}
		}
		return true;
	}

	public static void FindBotsOfSameSide(this List<BotComponent> result, BotComponent bot, List<BotComponent> listToCheck)
	{
		int count = listToCheck.Count;
		for (int i = 0; i < count; i++)
		{
			BotComponent botComponent = listToCheck[i];
			if (goodForSelection(botComponent) && !isBotSame(bot, botComponent) && areBotsFriendly(bot, botComponent))
			{
				result.Add(bot);
			}
		}
	}

	public static void FilterBotsBySquareMagnitude(this List<BotComponent> result, BotComponent bot, float minSqrMag)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		int count = result.Count;
		for (int num = count - 1; num >= 0; num--)
		{
			BotComponent botComponent = result[num];
			if (!isBotSame(bot, botComponent))
			{
				Vector3 val = bot.Position - botComponent.Position;
				if (!(((Vector3)(ref val)).sqrMagnitude <= minSqrMag))
				{
					result.Remove(botComponent);
				}
			}
		}
	}

	public static bool isBotSame(BotComponent a, BotComponent b)
	{
		return a.ProfileId == b.ProfileId;
	}

	public static bool areBotsFriendly(BotComponent a, BotComponent b)
	{
		if (isEnemy(a, b))
		{
			return false;
		}
		if (isEnemy(b, a))
		{
			return false;
		}
		return true;
	}

	public static bool isEnemy(BotComponent a, BotComponent b)
	{
		return a.EnemyController.GetEnemy(b.ProfileId, mustBeActive: false) != null;
	}

	public static bool goodForSelection(BotComponent bot)
	{
		return (Object)(object)bot != (Object)null && bot.BotActive;
	}
}
