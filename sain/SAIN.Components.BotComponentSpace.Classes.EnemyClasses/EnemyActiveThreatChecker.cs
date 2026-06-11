using System;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.Components.BotComponentSpace.Classes.EnemyClasses;

public class EnemyActiveThreatChecker : EnemyBase, IBotClass, IDisposable
{
	private float _activeForPeriod = 180f;

	private float _activeForPeriodAI = 90f;

	private float _activeDistanceThreshold = 150f;

	private float _activeDistanceThresholdAI = 75f;

	public bool ActiveThreat { get; private set; }

	public EnemyActiveThreatChecker(Enemy enemy)
		: base(enemy)
	{
	}

	public override void ManualUpdate()
	{
		checkActiveThreat();
		base.ManualUpdate();
	}

	private void checkActiveThreat()
	{
		ActiveThreat = isActiveThreat();
		base.Enemy.Events.OnActiveThreatChanged.CheckToggle(ActiveThreat);
	}

	private bool isActiveThreat()
	{
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		if (!base.Enemy.EnemyKnown)
		{
			return false;
		}
		if (base.Enemy.IsCurrentEnemy)
		{
			return true;
		}
		if (base.Enemy.IsVisible || (base.Enemy.Seen && base.Enemy.TimeSinceSeen < 30f))
		{
			return true;
		}
		if (base.Enemy.Status.HeardRecently || (base.Enemy.Heard && base.Enemy.TimeSinceHeard < 10f))
		{
			return true;
		}
		Vector3? lastKnownPosition = base.Enemy.KnownPlaces.LastKnownPosition;
		if (!lastKnownPosition.HasValue)
		{
			return false;
		}
		float timeSinceCurrentEnemy = base.Enemy.TimeSinceCurrentEnemy;
		Vector3 val = lastKnownPosition.Value - base.EnemyCurrentPosition;
		float sqrMagnitude = ((Vector3)(ref val)).sqrMagnitude;
		if (base.Enemy.IsAI)
		{
			if (sqrMagnitude > _activeDistanceThresholdAI * _activeDistanceThresholdAI)
			{
				return timeSinceCurrentEnemy < _activeForPeriodAI;
			}
			return true;
		}
		if (sqrMagnitude > _activeDistanceThreshold * _activeDistanceThreshold)
		{
			return timeSinceCurrentEnemy < _activeForPeriod;
		}
		return true;
	}
}
