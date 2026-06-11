using System;
using System.Collections.Generic;
using SAIN.Helpers;
using SAIN.Models.Enums;

namespace SAIN.SAINComponent.Classes.EnemyClasses;

public class EnemyListsClass : BotSubClass<SAINEnemyController>, IBotClass, IDisposable
{
	public readonly Dictionary<EEnemyListType, EnemyList> EnemyLists = new Dictionary<EEnemyListType, EnemyList>();

	private readonly List<Enemy> _enemiesToRemove = new List<Enemy>();

	private static readonly EEnemyListType[] _types = EnumValues.GetEnum<EEnemyListType>();

	public EnemyList KnownEnemies { get; private set; }

	public EnemyListsClass(SAINEnemyController controller)
		: base(controller)
	{
		createLists();
		base.CanEverTick = false;
	}

	private void createLists()
	{
		EEnemyListType[] types = _types;
		for (int i = 0; i < types.Length; i++)
		{
			EEnemyListType key = types[i];
			EnemyLists.Add(key, new EnemyList(key.ToString()));
		}
		KnownEnemies = GetEnemyList(EEnemyListType.Known);
	}

	public EnemyList GetEnemyList(EEnemyListType type)
	{
		EnemyLists.TryGetValue(type, out var value);
		return value;
	}

	public Enemy First(EEnemyListType type)
	{
		return GetEnemyList(type).First();
	}

	public int HumanCount(EEnemyListType type)
	{
		return GetEnemyList(type).Humans;
	}

	public int TotalCount(EEnemyListType type)
	{
		return GetEnemyList(type).Count;
	}

	public int BotCount(EEnemyListType type)
	{
		return GetEnemyList(type).Bots;
	}

	public override void Init()
	{
		base.Bot.EnemyController.Events.OnEnemyAdded += enemyAdded;
		base.Bot.EnemyController.Events.OnEnemyRemoved += enemyRemoved;
		base.Init();
	}

	private void enemyAdded(Enemy enemy)
	{
		subOrUnSub(value: true, enemy);
	}

	private void enemyRemoved(string profileID, Enemy enemy)
	{
		subOrUnSub(value: false, enemy);
		foreach (EnemyList value in EnemyLists.Values)
		{
			value.RemoveEnemy(enemy);
		}
	}

	public override void Dispose()
	{
		SAINEnemyController enemyController = base.Bot.EnemyController;
		if (enemyController != null)
		{
			enemyController.Events.OnEnemyAdded -= enemyAdded;
			enemyController.Events.OnEnemyRemoved -= enemyRemoved;
		}
		clearLists();
		base.Dispose();
	}

	private void clearLists()
	{
		foreach (EnemyList value in EnemyLists.Values)
		{
			if (value.Count <= 0)
			{
				continue;
			}
			Logger.LogWarning($"List [{value.Name}] still has [{value.Count}] enemies contained! This shouldn't be the case Solarint, you fuck!");
			foreach (Enemy item in value)
			{
				if (item != null && !_enemiesToRemove.Contains(item))
				{
					_enemiesToRemove.Add(item);
				}
			}
		}
		if (_enemiesToRemove.Count > 0)
		{
			Logger.LogWarning($"Had to manually remove [{_enemiesToRemove.Count}] enemies...");
			foreach (Enemy item2 in _enemiesToRemove)
			{
				enemyRemoved(item2.EnemyProfileId, item2);
			}
		}
		foreach (EnemyList value2 in EnemyLists.Values)
		{
			value2.Clear();
		}
		EnemyLists.Clear();
	}

	private void subOrUnSub(bool value, Enemy enemy)
	{
		EnemyEvents events = enemy.Events;
		GetEnemyList(EEnemyListType.Known).SubOrUnSub(value, ref events.OnEnemyKnownChanged.OnToggle, enemy);
		GetEnemyList(EEnemyListType.ActiveThreats).SubOrUnSub(value, ref events.OnActiveThreatChanged.OnToggle, enemy);
		GetEnemyList(EEnemyListType.Visible).SubOrUnSub(value, ref events.OnVisionChange.OnToggle, enemy);
		GetEnemyList(EEnemyListType.InLineOfSight).SubOrUnSub(value, ref events.OnEnemyLineOfSightChanged.OnToggle, enemy);
	}
}
