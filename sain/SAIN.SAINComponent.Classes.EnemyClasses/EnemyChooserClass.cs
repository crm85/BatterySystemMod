using System;
using EFT;
using SAIN.Helpers;
using SAIN.Models.Enums;

namespace SAIN.SAINComponent.Classes.EnemyClasses;

public class EnemyChooserClass : BotSubClass<SAINEnemyController>, IBotClass, IDisposable
{
	private Enemy _activeEnemy;

	public Enemy GoalEnemy
	{
		get
		{
			return _activeEnemy;
		}
		private set
		{
			if (value != _activeEnemy)
			{
				LastGoalEnemy = _activeEnemy;
				_activeEnemy = value;
				base.BaseClass.Events.EnemyChanged(value, LastGoalEnemy);
			}
		}
	}

	public Enemy LastGoalEnemy { get; private set; }

	public EnemyChooserClass(SAINEnemyController controller)
		: base(controller)
	{
	}

	public override void Init()
	{
		base.BaseClass.Events.OnEnemyRemoved += enemyRemoved;
		base.BaseClass.Events.OnEnemyKnownChanged += enemyKnownChanged;
		base.Init();
	}

	public override void ManualUpdate()
	{
		assignActiveEnemy();
		checkDiscrepency();
		base.ManualUpdate();
	}

	public override void Dispose()
	{
		base.BaseClass.Events.OnEnemyRemoved -= enemyRemoved;
		base.BaseClass.Events.OnEnemyKnownChanged -= enemyKnownChanged;
		base.Dispose();
	}

	private void enemyKnownChanged(bool known, Enemy enemy)
	{
		if (!known && _activeEnemy != null && _activeEnemy.EnemyProfileId == enemy.EnemyProfileId)
		{
			setActiveEnemy(null);
		}
	}

	private void enemyRemoved(string profileId, Enemy enemy)
	{
		if (GoalEnemy != null && GoalEnemy.EnemyProfileId == profileId)
		{
			GoalEnemy = null;
			LastGoalEnemy = null;
		}
		else if (LastGoalEnemy != null && LastGoalEnemy.EnemyProfileId == profileId)
		{
			LastGoalEnemy = null;
		}
	}

	public void ClearEnemy()
	{
		setActiveEnemy(null);
	}

	private void assignActiveEnemy()
	{
		Enemy enemy = findActiveEnemy();
		if (enemy != null && (!enemy.CheckValid() || !enemy.EnemyPerson.Active))
		{
			enemy = null;
		}
		if (enemy == null)
		{
			foreach (Enemy item in base.Bot.EnemyController.EnemiesArray)
			{
				if (item != null && item.EnemyKnown && item.WasValid && item.EnemyPerson.Active)
				{
					enemy = item;
					break;
				}
			}
		}
		setActiveEnemy(enemy);
	}

	private Enemy findActiveEnemy()
	{
		Enemy dogFightTarget = base.Bot.Decision.DogFightDecision.DogFightTarget;
		if (dogFightTarget != null && dogFightTarget.CheckValid() && dogFightTarget.EnemyPerson.Active)
		{
			return dogFightTarget;
		}
		Enemy currentTargetEnemy = base.Bot.CurrentTarget.CurrentTargetEnemy;
		if (currentTargetEnemy != null && (GoalEnemy == null || currentTargetEnemy.IsDifferent(GoalEnemy)))
		{
			return currentTargetEnemy;
		}
		checkGoalEnemy(out var enemy);
		if (enemy != null)
		{
			if (!enemy.IsVisible)
			{
				Enemy enemy2 = base.BaseClass.EnemyLists.First(EEnemyListType.Visible);
				if (enemy2 != null && enemy2.CheckValid() && enemy2.EnemyPerson.Active)
				{
					return enemy2;
				}
			}
			return enemy;
		}
		return base.BaseClass.EnemyLists.KnownEnemies.First();
	}

	private void checkGoalEnemy(out Enemy enemy)
	{
		enemy = null;
		EnemyInfo val = base.BotOwner.Memory.GoalEnemy;
		Enemy goalEnemy = GoalEnemy;
		if (((val != null) ? val.Person : null) != null && !val.Person.HealthController.IsAlive)
		{
			try
			{
				base.BotOwner.Memory.GoalEnemy = null;
			}
			catch
			{
			}
			val = null;
		}
		if (val == null)
		{
			if (goalEnemy != null && goalEnemy.CheckValid() && goalEnemy.EnemyPerson.Active && (goalEnemy.Status.ShotAtMeRecently || goalEnemy.IsVisible))
			{
				enemy = goalEnemy;
			}
			return;
		}
		if (goalEnemy != null && goalEnemy.EnemyInfo.ProfileId == val.ProfileId)
		{
			enemy = goalEnemy;
			return;
		}
		goalEnemy = base.BaseClass.CheckAddEnemy((val != null) ? val.Person : null);
		if (goalEnemy == null)
		{
			object obj2;
			if (val == null)
			{
				obj2 = null;
			}
			else
			{
				IPlayer person = val.Person;
				obj2 = ((person != null) ? person.ProfileId : null);
			}
			Logger.LogError((string)obj2 + " not SAIN enemy!");
		}
		else if (goalEnemy.CheckValid() && goalEnemy.EnemyPerson.Active)
		{
			enemy = goalEnemy;
		}
		else
		{
			enemy = null;
		}
	}

	private void setActiveEnemy(Enemy enemy)
	{
		if (enemy == null || (enemy.CheckValid() && enemy.EnemyPerson.Active))
		{
			GoalEnemy = enemy;
			setGoalEnemy(enemy?.EnemyInfo);
		}
	}

	private void setLastEnemy(Enemy activeEnemy)
	{
		bool flag = activeEnemy != null && activeEnemy.EnemyPerson?.Active == true;
		Enemy lastGoalEnemy = LastGoalEnemy;
		bool flag2 = lastGoalEnemy != null && lastGoalEnemy.EnemyPerson?.Active == true;
		if (!(!flag2 && flag))
		{
			if (flag2 && !flag)
			{
				LastGoalEnemy = activeEnemy;
			}
			else if (!AreEnemiesSame(activeEnemy, LastGoalEnemy))
			{
				LastGoalEnemy = activeEnemy;
			}
		}
	}

	private void setGoalEnemy(EnemyInfo enemyInfo)
	{
		if (base.BotOwner.Memory.GoalEnemy != enemyInfo)
		{
			try
			{
				base.BotOwner.Memory.GoalEnemy = enemyInfo;
				base.BotOwner.CalcGoal();
			}
			catch
			{
			}
		}
	}

	public bool AreEnemiesSame(Enemy a, Enemy b)
	{
		return AreEnemiesSame(a?.EnemyIPlayer, b?.EnemyIPlayer);
	}

	public bool AreEnemiesSame(IPlayer a, IPlayer b)
	{
		return a != null && b != null && a.ProfileId == b.ProfileId;
	}

	private void checkDiscrepency()
	{
		EnemyInfo goalEnemy = base.BotOwner.Memory.GoalEnemy;
		if (goalEnemy == null || GoalEnemy != null || goalEnemy.Person == null || goalEnemy.ProfileId == base.Bot.ProfileId || goalEnemy.ProfileId == base.Bot.Player.ProfileId || goalEnemy.ProfileId == base.Bot.BotOwner.ProfileId)
		{
			return;
		}
		Enemy enemy = base.BaseClass.GetEnemy(goalEnemy.ProfileId, mustBeActive: true);
		if (enemy != null)
		{
			setActiveEnemy(enemy);
			return;
		}
		enemy = base.BaseClass.CheckAddEnemy(goalEnemy.Person);
		if (enemy != null)
		{
			setActiveEnemy(enemy);
		}
	}
}
