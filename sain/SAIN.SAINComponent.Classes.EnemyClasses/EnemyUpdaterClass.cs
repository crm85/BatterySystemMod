using System.Collections.Generic;
using EFT;
using SAIN.Components;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.EnemyClasses;

public class EnemyUpdaterClass : BotBase
{
	private readonly List<string> _allyIdsToRemove = new List<string>();

	private readonly List<string> _invalidIdsToRemove = new List<string>();

	public EnemyUpdaterClass(BotComponent bot)
		: base(bot)
	{
	}

	public override void ManualUpdate()
	{
		base.ManualUpdate();
		UpdateEnemies(base.Bot);
	}

	public void UpdateEnemies(BotComponent bot)
	{
		if ((Object)(object)bot == (Object)null)
		{
			return;
		}
		SAINEnemyController enemyController = bot.EnemyController;
		if (enemyController == null)
		{
			return;
		}
		HashSet<Enemy> enemiesArray = enemyController.EnemiesArray;
		if (enemiesArray == null)
		{
			return;
		}
		List<IPlayer> allies = bot.BotOwner.BotsGroup.Allies;
		foreach (Enemy item in enemiesArray)
		{
			if (!item.CheckValid())
			{
				_invalidIdsToRemove.Add(item.EnemyProfileId);
			}
			else if (allies.Contains((IPlayer)(object)item.EnemyPlayer))
			{
				if (SAINPlugin.DebugMode)
				{
					Logger.LogWarning(((Object)item.EnemyPlayer).name + " is an ally of " + ((Object)base.Bot.Player).name + " and will be removed from its enemies collection");
				}
				_allyIdsToRemove.Add(item.EnemyProfileId);
			}
			else
			{
				item.ManualUpdate();
			}
		}
		if (_invalidIdsToRemove.Count > 0)
		{
			foreach (string item2 in _invalidIdsToRemove)
			{
				enemyController.RemoveEnemy(item2);
			}
			Logger.LogWarning($"Removed {_invalidIdsToRemove.Count} Invalid Enemies");
			_invalidIdsToRemove.Clear();
		}
		if (_allyIdsToRemove.Count <= 0)
		{
			return;
		}
		foreach (string item3 in _allyIdsToRemove)
		{
			enemyController.RemoveEnemy(item3);
		}
		if (SAINPlugin.DebugMode)
		{
			Logger.LogWarning($"Removed {_allyIdsToRemove.Count} allies");
		}
		_allyIdsToRemove.Clear();
	}

	public void LateUpdate()
	{
	}
}
