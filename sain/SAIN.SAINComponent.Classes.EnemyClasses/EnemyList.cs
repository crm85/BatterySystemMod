using System;
using System.Collections.Generic;

namespace SAIN.SAINComponent.Classes.EnemyClasses;

public class EnemyList : List<Enemy>
{
	public enum EBotListSortType
	{
		None,
		ByRealDistance,
		ByTimeSinceSensed,
		ByLastKnownDistance,
		ByPathLength,
		VisiblePathPointDistanceToBot,
		VisiblePathPointDistanceToEnemy
	}

	public string Name { get; }

	public int Humans { get; private set; }

	public int Bots { get; private set; }

	public event Action<bool> OnListEmptyOrGetFirst;

	public event Action<bool> OnListEmptyOrGetFirstHuman;

	public EnemyList(string name)
	{
		Name = name;
	}

	public void SubOrUnSub(bool value, ref Action<bool, Enemy> action, Enemy enemy)
	{
		if (value)
		{
			action = (Action<bool, Enemy>)Delegate.Combine(action, new Action<bool, Enemy>(AddOrRemoveEnemy));
			return;
		}
		action = (Action<bool, Enemy>)Delegate.Remove(action, new Action<bool, Enemy>(AddOrRemoveEnemy));
		RemoveEnemy(enemy);
	}

	public void SortBy(EBotListSortType sortingType)
	{
		if (base.Count <= 1)
		{
			return;
		}
		switch (sortingType)
		{
		case EBotListSortType.ByRealDistance:
			Sort((Enemy x, Enemy y) => x.RealDistance.CompareTo(y.RealDistance));
			break;
		case EBotListSortType.ByTimeSinceSensed:
			Sort((Enemy x, Enemy y) => x.TimeSinceLastKnownUpdated.CompareTo(y.TimeSinceLastKnownUpdated));
			break;
		case EBotListSortType.ByLastKnownDistance:
			Sort((Enemy x, Enemy y) => x.KnownPlaces.BotDistanceFromLastKnown.CompareTo(y.KnownPlaces.BotDistanceFromLastKnown));
			break;
		case EBotListSortType.ByPathLength:
			Sort((Enemy x, Enemy y) => x.Path.PathLength.CompareTo(y.Path.PathLength));
			break;
		case EBotListSortType.VisiblePathPointDistanceToBot:
			Sort((Enemy x, Enemy y) => x.VisiblePathPointDistanceToBot.CompareTo(y.VisiblePathPointDistanceToBot));
			break;
		case EBotListSortType.VisiblePathPointDistanceToEnemy:
			Sort((Enemy x, Enemy y) => x.VisiblePathPointDistanceToEnemyLastKnown.CompareTo(y.VisiblePathPointDistanceToEnemyLastKnown));
			break;
		}
	}

	public void AddOrRemoveEnemy(bool value, Enemy enemy)
	{
		if (value)
		{
			AddEnemy(enemy);
		}
		else
		{
			RemoveEnemy(enemy);
		}
	}

	private void sortByLastUpdated()
	{
		Sort((Enemy x, Enemy y) => x.KnownPlaces.TimeSinceLastKnownUpdated.CompareTo(y.KnownPlaces.TimeSinceLastKnownUpdated));
	}

	public Enemy First()
	{
		switch (base.Count)
		{
		case 0:
			return null;
		default:
			sortByLastUpdated();
			break;
		case 1:
			break;
		}
		return base[0];
	}

	public void AddEnemy(Enemy enemy)
	{
		Add(enemy);
		if (base.Count == 1)
		{
			this.OnListEmptyOrGetFirst?.Invoke(obj: true);
		}
		if (!enemy.IsAI)
		{
			Humans++;
			if (Humans == 1)
			{
				this.OnListEmptyOrGetFirstHuman?.Invoke(obj: true);
			}
		}
		else
		{
			Bots++;
		}
	}

	public void RemoveEnemy(Enemy enemy)
	{
		if (enemy == null)
		{
			return;
		}
		Remove(enemy);
		if (!enemy.IsAI)
		{
			Humans--;
			if (Humans == 0)
			{
				this.OnListEmptyOrGetFirstHuman?.Invoke(obj: false);
			}
		}
		else
		{
			Bots--;
		}
		if (Bots < 0)
		{
			Bots = 0;
		}
		if (Humans < 0)
		{
			Humans = 0;
		}
		if (base.Count == 0)
		{
			this.OnListEmptyOrGetFirst?.Invoke(obj: false);
		}
	}
}
