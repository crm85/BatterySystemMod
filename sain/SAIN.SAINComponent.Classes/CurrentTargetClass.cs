using SAIN.Components;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.SAINComponent.Classes;

public class CurrentTargetClass : BotComponentClassBase
{
	private float _updateGoalTargetTime;

	private Vector3? _currentTarget;

	public Enemy CurrentTargetEnemy { get; private set; }

	public Vector3? CurrentTargetPosition => _currentTarget;

	public Vector3? CurrentTargetDirection => CurrentTargetEnemy?.EnemyDirection;

	public float CurrentTargetDistance => HasTarget ? CurrentTargetEnemy.RealDistance : float.MaxValue;

	public bool HasTarget => CurrentTargetEnemy != null;

	public string TargetProfileId => CurrentTargetEnemy?.EnemyProfileId;

	public CurrentTargetClass(BotComponent bot)
		: base(bot)
	{
		base.TickRequirement = ESAINTickState.OnlyBotActive;
	}

	public override void ManualUpdate()
	{
		updateCurrentTarget();
		updateGoalTarget();
		base.ManualUpdate();
	}

	private void updateCurrentTarget()
	{
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		Enemy targetEnemy;
		Vector3? val = (_currentTarget = getTarget(out targetEnemy));
		if (!val.HasValue)
		{
			if (CurrentTargetEnemy != null)
			{
				base.Bot.Cover.CoverFinder.ClearTarget();
				CurrentTargetEnemy = null;
			}
			return;
		}
		bool flag = CurrentTargetEnemy != null;
		bool flag2 = CurrentTargetEnemy == targetEnemy;
		if (!flag || !flag2)
		{
			if (flag)
			{
				CurrentTargetEnemy.SetIsCurrentEnemy(value: false);
				CurrentTargetEnemy = null;
			}
			base.Bot.Aim.LoseAimTarget();
			targetEnemy.SetIsCurrentEnemy(value: true);
			CurrentTargetEnemy = targetEnemy;
			base.Bot.Cover.CoverFinder.CalcTargetPoint(targetEnemy, val.Value);
		}
	}

	private void updateGoalTarget()
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (!(_updateGoalTargetTime < Time.time))
		{
			return;
		}
		_updateGoalTargetTime = Time.time + 0.5f;
		GoalTargetClass goalTarget = base.BotOwner.Memory.GoalTarget;
		Vector3? val = ((goalTarget != null) ? goalTarget.Position : ((Vector3?)null));
		if (val.HasValue)
		{
			Vector3 val2 = val.Value - base.Bot.Position;
			if (((Vector3)(ref val2)).sqrMagnitude < 1f || goalTarget.CreatedTime > 120.0)
			{
				goalTarget.Clear();
				base.BotOwner.CalcGoal();
			}
		}
	}

	private Vector3? getTarget(out Enemy targetEnemy)
	{
		return getVisibleEnemyPos(out targetEnemy) ?? getLastHitPosition(out targetEnemy) ?? getUnderFirePosition(out targetEnemy) ?? getEnemylastKnownPos(out targetEnemy);
	}

	private Vector3? getVisibleEnemyPos(out Enemy targetEnemy)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		Enemy enemy = base.Bot.Enemy;
		if (enemy != null && enemy.IsVisible)
		{
			targetEnemy = enemy;
			return enemy.EnemyPosition;
		}
		targetEnemy = null;
		return null;
	}

	private Vector3? getLastHitPosition(out Enemy targetEnemy)
	{
		targetEnemy = null;
		if (base.Bot.Medical.TimeSinceShot > 5f)
		{
			return null;
		}
		Enemy enemyWhoLastShotMe = base.Bot.Medical.HitByEnemy.EnemyWhoLastShotMe;
		if (enemyWhoLastShotMe == null || !enemyWhoLastShotMe.CheckValid() || enemyWhoLastShotMe.IsCurrentEnemy)
		{
			return null;
		}
		targetEnemy = enemyWhoLastShotMe;
		return enemyWhoLastShotMe.LastKnownPosition ?? enemyWhoLastShotMe.Status.LastShotPosition;
	}

	private Vector3? getUnderFirePosition(out Enemy targetEnemy)
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		targetEnemy = null;
		if (!base.BotOwner.Memory.IsUnderFire)
		{
			return null;
		}
		Enemy lastUnderFireEnemy = base.Bot.Memory.LastUnderFireEnemy;
		if (lastUnderFireEnemy == null || !lastUnderFireEnemy.CheckValid() || lastUnderFireEnemy.IsCurrentEnemy)
		{
			return null;
		}
		targetEnemy = lastUnderFireEnemy;
		return (Vector3)(((_003F?)lastUnderFireEnemy.LastKnownPosition) ?? base.Bot.Memory.UnderFireFromPosition);
	}

	private Vector3? getEnemylastKnownPos(out Enemy targetEnemy)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		Enemy goalEnemy = base.Bot.EnemyController.GoalEnemy;
		if (goalEnemy != null)
		{
			Vector3? lastKnown = getLastKnown(goalEnemy);
			if (lastKnown.HasValue)
			{
				targetEnemy = goalEnemy;
				return lastKnown.Value;
			}
		}
		targetEnemy = null;
		return null;
	}

	private Vector3? getLastKnown(Enemy enemy)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		EnemyKnownPlaces knownPlaces = enemy.KnownPlaces;
		EnemyPlace lastKnownPlace = knownPlaces.LastKnownPlace;
		Vector3? result;
		if (lastKnownPlace == null)
		{
			EnemyPlace lastSeenPlace = knownPlaces.LastSeenPlace;
			result = ((lastSeenPlace != null) ? new Vector3?(lastSeenPlace.Position) : knownPlaces.LastHeardPlace?.Position);
		}
		else
		{
			result = lastKnownPlace.Position;
		}
		return result;
	}
}
