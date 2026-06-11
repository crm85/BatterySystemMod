using System.Collections.Generic;
using EFT;
using SAIN.Components;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Decision;

public class DogFightDecisionClass : BotBase
{
	private float _changeDFTargetTime;

	private readonly List<Enemy> _dogFightTargets = new List<Enemy>();

	private float _dogFightStartDist = 8f;

	private float _dogFightEndDist = 15f;

	public Enemy DogFightTarget { get; set; }

	public DogFightDecisionClass(BotComponent bot)
		: base(bot)
	{
		base.CanEverTick = false;
	}

	public override void Init()
	{
		base.Bot.EnemyController.Events.OnEnemyRemoved += checkClear;
		base.Init();
	}

	public override void Dispose()
	{
		base.Bot.EnemyController.Events.OnEnemyRemoved -= checkClear;
		base.Dispose();
	}

	public bool ShallDogFight(EnemyList KnownEnemies)
	{
		BotOwner botOwner = base.BotOwner;
		BotWeaponManager val = ((botOwner != null) ? botOwner.WeaponManager : null);
		if (val == null || !val.HaveBullets || val.Reload.Reloading)
		{
			DogFightTarget = null;
			return false;
		}
		switch (base.Bot.Decision.CurrentCombatDecision)
		{
		case ECombatDecision.RushEnemy:
			return false;
		case ECombatDecision.Retreat:
		case ECombatDecision.RunToCover:
			if (base.Bot.Decision.SelfActionDecisions.LowOnAmmo(0.2f))
			{
				return false;
			}
			break;
		}
		if (DogFightTarget != null)
		{
			if (shallDogFightEnemy(DogFightTarget))
			{
				return true;
			}
			if (shallClearDogfightTarget(DogFightTarget))
			{
				DogFightTarget = null;
			}
		}
		if (_changeDFTargetTime < Time.time)
		{
			_changeDFTargetTime = Time.time + 0.33f;
			for (int num = _dogFightTargets.Count - 1; num >= 0; num--)
			{
				if (shallClearDogfightTarget(_dogFightTargets[num]))
				{
					_dogFightTargets.RemoveAt(num);
				}
			}
			Enemy enemy = SelectDFTarget();
			if (enemy != null)
			{
				DogFightTarget = enemy;
			}
			else
			{
				foreach (Enemy KnownEnemy in KnownEnemies)
				{
					if (!_dogFightTargets.Contains(KnownEnemy) && shallDogFightEnemy(KnownEnemy))
					{
						_dogFightTargets.Add(KnownEnemy);
					}
				}
				DogFightTarget = SelectDFTarget();
			}
		}
		return DogFightTarget != null;
	}

	private void clearDogFightTarget()
	{
		if (DogFightTarget != null)
		{
			DogFightTarget = null;
		}
	}

	private bool shallClearDogfightTarget(Enemy enemy)
	{
		if (enemy != null && enemy.Seen && enemy.EnemyKnown)
		{
			Player player = enemy.Player;
			if (player == null || player.HealthController.IsAlive)
			{
				if (!base.Bot.EnemyController.Enemies.ContainsValue(enemy) || !enemy.CheckValid())
				{
					return true;
				}
				float pathLength = enemy.Path.PathLength;
				if (pathLength > _dogFightEndDist)
				{
					return true;
				}
				return !enemy.IsVisible && enemy.TimeSinceSeen > 6f;
			}
		}
		return true;
	}

	private void getNewDFTargets()
	{
		_dogFightTargets.Clear();
		Dictionary<string, Enemy> enemies = base.Bot.EnemyController.Enemies;
		foreach (Enemy value in enemies.Values)
		{
			if (shallDogFightEnemy(value))
			{
				_dogFightTargets.Add(value);
			}
		}
	}

	private Enemy SelectDFTarget()
	{
		int count = _dogFightTargets.Count;
		if (count > 0)
		{
			if (count > 1)
			{
				_dogFightTargets.Sort((Enemy x, Enemy y) => x.RealDistance.CompareTo(y.RealDistance));
				foreach (Enemy dogFightTarget in _dogFightTargets)
				{
					if (dogFightTarget.IsVisible)
					{
						return dogFightTarget;
					}
				}
			}
			return _dogFightTargets[0];
		}
		return null;
	}

	private void checkClear(string profileID, Enemy enemy)
	{
		if (DogFightTarget != null && DogFightTarget.EnemyProfileId == profileID)
		{
			DogFightTarget = null;
		}
	}

	private bool shallDogFightEnemy(Enemy enemy)
	{
		return enemy != null && base.Bot.EnemyController.Enemies.ContainsValue(enemy) && enemy != null && enemy.CheckValid() && enemy.IsVisible && enemy.EnemyKnown && enemy.Path.PathLength <= _dogFightStartDist;
	}
}
