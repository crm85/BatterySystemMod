using System;
using System.Collections.Generic;
using System.Diagnostics;
using EFT;
using SAIN.Components;
using SAIN.Components.PlayerComponentSpace;
using SAIN.Components.PlayerComponentSpace.PersonClasses;
using SAIN.Helpers;
using SAIN.Models.Enums;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.EnemyClasses;

public class EnemyListController : BotSubClass<SAINEnemyController>, IBotClass, IDisposable
{
	private float _nextCompareListsTime;

	private const float COMPARE_ENEMY_LIST_FREQ = 1f;

	public Dictionary<string, Enemy> Enemies { get; } = new Dictionary<string, Enemy>();

	public HashSet<Enemy> EnemiesArray { get; } = new HashSet<Enemy>();

	public EnemyListController(SAINEnemyController controller)
		: base(controller)
	{
	}

	public override void Init()
	{
		GameWorldComponent.Instance.PlayerTracker.AlivePlayersDictionary.OnPlayerComponentRemoved += RemoveEnemy;
		base.BotOwner.Memory.OnAddEnemy += enemyAdded;
		compareEnemyLists();
		base.Init();
	}

	public override void ManualUpdate()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		compareEnemyLists();
		if (SAINPlugin.DebugMode)
		{
			foreach (Enemy enemy in base.Bot.EnemyController.EnemyLists.GetEnemyList(EEnemyListType.Visible))
			{
				DebugGizmos.Line(base.Bot.Transform.HeadPosition, enemy.EnemyPosition, Color.red, 0.1f, 0.02f);
				if (enemy.LastKnownPosition.HasValue)
				{
					DebugGizmos.Line(enemy.LastKnownPosition.Value, enemy.EnemyPosition, Color.red, 0.025f, 0.02f);
				}
			}
			foreach (Enemy enemy2 in base.Bot.EnemyController.EnemyLists.GetEnemyList(EEnemyListType.InLineOfSight))
			{
				DebugGizmos.Line(base.Bot.Transform.HeadPosition, enemy2.EnemyPosition, Color.yellow, 0.075f, 0.02f);
				if (enemy2.LastKnownPosition.HasValue)
				{
					DebugGizmos.Line(enemy2.LastKnownPosition.Value, enemy2.EnemyPosition, Color.yellow, 0.025f, 0.02f);
				}
			}
			foreach (Enemy enemy3 in base.Bot.EnemyController.EnemyLists.GetEnemyList(EEnemyListType.Known))
			{
				DebugGizmos.Line(base.Bot.Transform.HeadPosition, enemy3.EnemyPosition, Color.blue, 0.05f, 0.02f);
				if (enemy3.LastKnownPosition.HasValue)
				{
					DebugGizmos.Line(enemy3.LastKnownPosition.Value, enemy3.EnemyPosition, Color.blue, 0.025f, 0.02f);
				}
			}
		}
		base.ManualUpdate();
	}

	public override void Dispose()
	{
		GameWorldComponent.Instance.PlayerTracker.AlivePlayersDictionary.OnPlayerComponentRemoved -= RemoveEnemy;
		BotMemoryClass val = base.BotOwner?.Memory;
		if (val != null)
		{
			val.OnAddEnemy -= enemyAdded;
		}
		foreach (Enemy item in EnemiesArray)
		{
			destroyEnemy(item);
		}
		EnemiesArray.Clear();
		Enemies.Clear();
		base.Dispose();
	}

	public Enemy GetEnemy(string profileID, bool mustBeActive)
	{
		if (!Enemies.TryGetValue(profileID, out var value))
		{
			return null;
		}
		if (value == null || !value.CheckValid())
		{
			destroyEnemy(value);
			Enemies.Remove(profileID);
			EnemiesArray.Remove(value);
			return null;
		}
		if (mustBeActive && !value.EnemyPerson.Active)
		{
			return null;
		}
		return value;
	}

	private void removeEnemy(PersonClass person)
	{
		RemoveEnemy(person.ProfileId);
	}

	public void RemoveEnemy(string profileId)
	{
		if (Enemies.TryGetValue(profileId, out var value))
		{
			destroyEnemy(value);
			Enemies.Remove(profileId);
			EnemiesArray.Remove(value);
		}
	}

	private void destroyEnemy(Enemy enemy)
	{
		if (enemy != null)
		{
			base.BaseClass.Events.EnemyRemoved(enemy.EnemyProfileId, enemy);
			enemy.Dispose();
			removeEnemyInfo(enemy);
			if ((Object)(object)enemy.EnemyPlayerComponent != (Object)null)
			{
				enemy.EnemyPlayerComponent.OnComponentDestroyed -= RemoveEnemy;
			}
			if (enemy.EnemyPerson != null)
			{
				enemy.EnemyPerson.ActivationClass.OnPersonDeadOrDespawned -= removeEnemy;
			}
		}
	}

	public Enemy CheckAddEnemy(IPlayer IPlayer)
	{
		return tryAddEnemy(IPlayer);
	}

	private void enemyAdded(IPlayer player)
	{
		tryAddEnemy(player);
	}

	public bool IsBotInBotsGroup(BotOwner botOwner)
	{
		int membersCount = base.BotOwner.BotsGroup.MembersCount;
		for (int i = 0; i < membersCount; i++)
		{
			BotOwner val = base.BotOwner.BotsGroup.Member(i);
			if (!((Object)(object)val == (Object)null) && (Object)(object)val == (Object)(object)botOwner)
			{
				return true;
			}
		}
		return false;
	}

	private Enemy tryAddEnemy(IPlayer enemyPlayer)
	{
		if (enemyPlayer == null)
		{
			return null;
		}
		if (!enemyPlayer.HealthController.IsAlive)
		{
			return null;
		}
		if (enemyPlayer.ProfileId == base.Bot.ProfileId)
		{
			string text = "Cannot add enemy that matches this bot: ";
			return null;
		}
		if (enemyPlayer.IsAI)
		{
			IAIData aIData = enemyPlayer.AIData;
			BotOwner val = ((aIData != null) ? aIData.BotOwner : null);
			if ((Object)(object)val == (Object)null)
			{
				return null;
			}
			if (IsBotInBotsGroup(val))
			{
				return null;
			}
		}
		PlayerComponent enemyPlayerComponent = getEnemyPlayerComponent(enemyPlayer);
		if ((Object)(object)enemyPlayerComponent == (Object)null)
		{
			return null;
		}
		if (Enemies.TryGetValue(enemyPlayer.ProfileId, out var value))
		{
			return value;
		}
		EnemyInfo enemyInfo = getEnemyInfo(enemyPlayer);
		if (enemyInfo == null)
		{
			return null;
		}
		return createEnemy(enemyPlayerComponent, enemyInfo);
	}

	private PlayerComponent getEnemyPlayerComponent(IPlayer enemyPlayer)
	{
		PlayerSpawnTracker playerTracker = GameWorldComponent.Instance.PlayerTracker;
		PlayerComponent playerComponent = playerTracker.GetPlayerComponent(enemyPlayer.ProfileId);
		if ((Object)(object)playerComponent == (Object)null)
		{
			if (Enemies.TryGetValue(enemyPlayer.ProfileId, out var value))
			{
				destroyEnemy(value);
				Enemies.Remove(enemyPlayer.ProfileId);
				Logger.LogDebug("Removed Old Enemy.");
			}
			playerComponent = playerTracker.AddPlayerManual(enemyPlayer);
			if (!((Object)(object)playerComponent == (Object)null))
			{
			}
		}
		return playerComponent;
	}

	private EnemyInfo getEnemyInfo(IPlayer enemyPlayer)
	{
		if (!base.BotOwner.EnemiesController.EnemyInfos.TryGetValue(enemyPlayer, out var value) && base.BotOwner.BotsGroup.Enemies.TryGetValue(enemyPlayer, out var value2))
		{
			value = base.BotOwner.EnemiesController.AddNew(base.BotOwner.BotsGroup, enemyPlayer, value2);
			if (value == null)
			{
			}
		}
		return value;
	}

	private Enemy createEnemy(PlayerComponent enemyPlayerComponent, EnemyInfo enemyInfo)
	{
		Enemy enemy = new Enemy(base.Bot, enemyPlayerComponent, enemyInfo);
		enemy.Init();
		enemyPlayerComponent.Person.ActivationClass.OnPersonDeadOrDespawned += removeEnemy;
		enemyPlayerComponent.OnComponentDestroyed += RemoveEnemy;
		Enemies.Add(enemy.EnemyProfileId, enemy);
		EnemiesArray.Add(enemy);
		base.BaseClass.Events.EnemyAdded(enemy);
		return enemy;
	}

	public bool IsPlayerAnEnemy(string profileID)
	{
		return !GClass1437.IsNullOrEmpty(profileID) && Enemies.ContainsKey(profileID);
	}

	public bool IsPlayerFriendly(IPlayer iPlayer)
	{
		if (iPlayer == null)
		{
			return false;
		}
		if (iPlayer.ProfileId == base.Bot.ProfileId)
		{
			return true;
		}
		if (Enemies.ContainsKey(iPlayer.ProfileId))
		{
			return false;
		}
		if (iPlayer.AIData.IsAI && base.BotOwner.BotsGroup.Contains(iPlayer.AIData.BotOwner))
		{
			return true;
		}
		if (!base.BotOwner.BotsGroup.IsPlayerEnemy(iPlayer) && base.BotOwner.BotsGroup.Neutrals.ContainsKey(iPlayer))
		{
			return true;
		}
		if (base.BotOwner.BotsGroup.Allies.Contains(iPlayer))
		{
			return true;
		}
		if (iPlayer.IsAI)
		{
			IAIData aIData = iPlayer.AIData;
			object obj;
			if (aIData == null)
			{
				obj = null;
			}
			else
			{
				BotOwner botOwner = aIData.BotOwner;
				if (botOwner == null)
				{
					obj = null;
				}
				else
				{
					EnemyInfo goalEnemy = botOwner.Memory.GoalEnemy;
					obj = ((goalEnemy != null) ? goalEnemy.ProfileId : null);
				}
			}
			if ((string)obj == base.Bot.ProfileId)
			{
				return false;
			}
		}
		if (!base.BotOwner.BotsGroup.Enemies.ContainsKey(iPlayer))
		{
			return true;
		}
		return false;
	}

	private void removeEnemyInfo(Enemy enemy)
	{
		if (enemy == null)
		{
			return;
		}
		if (enemy.EnemyIPlayer != null && base.BotOwner.EnemiesController.EnemyInfos.ContainsKey(enemy.EnemyIPlayer))
		{
			base.BotOwner.EnemiesController.Remove(enemy.EnemyIPlayer);
			return;
		}
		EnemyInfo val = null;
		foreach (EnemyInfo value in base.BotOwner.EnemiesController.EnemyInfos.Values)
		{
			if (((value != null) ? value.Person : null) != null && value.ProfileId == enemy.EnemyProfileId)
			{
				val = value;
				break;
			}
		}
		if (((val != null) ? val.Person : null) != null)
		{
			base.BotOwner.EnemiesController.Remove(val.Person);
		}
	}

	private void compareEnemyLists()
	{
		if (!(_nextCompareListsTime < Time.time))
		{
			return;
		}
		_nextCompareListsTime = Time.time + 1f;
		int count = Enemies.Count;
		int num = 0;
		Dictionary<IPlayer, BotSettingsClass> enemies = base.BotOwner.BotsGroup.Enemies;
		foreach (IPlayer key in enemies.Keys)
		{
			Enemy enemy = tryAddEnemy(key);
			if (enemy == null)
			{
				num++;
			}
		}
		int num2 = 0;
		Dictionary<IPlayer, EnemyInfo> enemyInfos = base.BotOwner.EnemiesController.EnemyInfos;
		foreach (IPlayer key2 in enemyInfos.Keys)
		{
			Enemy enemy2 = tryAddEnemy(key2);
			if (enemy2 == null)
			{
				num2++;
			}
		}
		if (num2 <= 0 && num <= 0)
		{
		}
	}

	private string getBotInfo(Player player)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		return $" [{player.Profile.Nickname}, {player.Profile.Info.Settings.Role}, {player.ProfileId}] ";
	}

	private string getBotInfo(IPlayer player)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		return $" [{player.Profile.Nickname}, {player.Profile.Info.Settings.Role}, {player.ProfileId}] ";
	}

	private string findSourceDebug(string debugString)
	{
		StackTrace stackTrace = new StackTrace();
		debugString = debugString + " StackTrace: [" + stackTrace.ToString() + "]";
		return debugString;
	}
}
