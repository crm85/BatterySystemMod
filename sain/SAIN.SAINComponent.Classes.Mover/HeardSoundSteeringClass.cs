using System;
using SAIN.Helpers;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Mover;

public class HeardSoundSteeringClass : BotSubClass<SAINSteeringClass>, IBotClass, IDisposable
{
	private float _nextCheckClearTime;

	private const float CHECK_CLEAR_FREQ = 1f;

	public bool HasDangerToLookAt
	{
		get
		{
			SoundStruct? lastHeardVisibleDanger = LastHeardVisibleDanger;
			return (lastHeardVisibleDanger.HasValue && lastHeardVisibleDanger.GetValueOrDefault().ShallLook) || (LastHeardDanger?.ShallLook ?? false);
		}
	}

	public SoundStruct? LastHeardDanger { get; private set; }

	public SoundStruct? LastHeardVisibleDanger { get; private set; }

	public HeardSoundSteeringClass(SAINSteeringClass steering)
		: base(steering)
	{
	}

	public override void Init()
	{
		base.Bot.EnemyController.Events.OnEnemyHeard += enemyHeard;
		base.Init();
	}

	public override void ManualUpdate()
	{
		checkClearPlaces();
		base.ManualUpdate();
	}

	private void checkClearPlaces()
	{
		if (_nextCheckClearTime < Time.time)
		{
			_nextCheckClearTime = Time.time + 1f;
			SoundStruct? lastHeardDanger = LastHeardDanger;
			if (lastHeardDanger.HasValue && lastHeardDanger.GetValueOrDefault().ShallClear)
			{
				clearPlace(LastHeardDanger.Value.Place);
			}
			lastHeardDanger = LastHeardVisibleDanger;
			if (lastHeardDanger.HasValue && lastHeardDanger.GetValueOrDefault().ShallClear)
			{
				clearPlace(LastHeardVisibleDanger.Value.Place);
			}
		}
	}

	private void clearPlace(EnemyPlace place)
	{
		if (place != null)
		{
			if (LastHeardDanger?.Place == place)
			{
				LastHeardDanger.Value.Place.OnDispose -= clearPlace;
				LastHeardDanger = null;
			}
			if (LastHeardVisibleDanger?.Place == place)
			{
				LastHeardVisibleDanger.Value.Place.OnDispose -= clearPlace;
				LastHeardVisibleDanger = null;
			}
		}
	}

	public override void Dispose()
	{
		base.Bot.EnemyController.Events.OnEnemyHeard -= enemyHeard;
		base.Dispose();
	}

	public void LookToHeardPosition()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		SoundStruct? lastHeardVisibleDanger = LastHeardVisibleDanger;
		if (lastHeardVisibleDanger.HasValue && lastHeardVisibleDanger.GetValueOrDefault().ShallLook)
		{
			base.BaseClass.LookToPoint(lastHeardVisibleDanger.Value.Position + base.BaseClass.WeaponRootOffset);
			return;
		}
		SoundStruct? lastHeardDanger = LastHeardDanger;
		if (lastHeardDanger.HasValue && lastHeardDanger.GetValueOrDefault().ShallLook)
		{
			Vector3 pathPoint;
			if (lastHeardDanger.Value.Enemy.InLineOfSight)
			{
				base.BaseClass.LookToPoint(lastHeardDanger.Value.Position + base.BaseClass.WeaponRootOffset);
			}
			else if (lastHeardDanger.Value.Enemy.GetVisibilePathPoint(out pathPoint))
			{
				base.BaseClass.LookToPoint(pathPoint);
			}
			else
			{
				base.BaseClass.LookToPoint(lastHeardDanger.Value.Position + base.BaseClass.WeaponRootOffset);
			}
		}
		else
		{
			base.BaseClass.LookToRandomPosition();
		}
	}

	private void enemyHeard(Enemy enemy, SAINSoundType soundType, bool isDanger, EnemyPlace place)
	{
		if (place == null || !place.Visible || !isDanger)
		{
			return;
		}
		if (place.PlaceData.OwnerID == base.Bot.ProfileId)
		{
			setLastVisSound(place, enemy);
			return;
		}
		Enemy enemy2 = base.Bot.EnemyController.GetEnemy(enemy.EnemyProfileId, mustBeActive: true);
		if (enemy2 != null && enemy2.InLineOfSight)
		{
			setLastVisSound(place, enemy2);
		}
	}

	private void setLastVisSound(EnemyPlace place, Enemy enemy)
	{
		if (place.Visible && place.PlaceData.OwnerID == base.Bot.ProfileId)
		{
			LastHeardVisibleDanger = new SoundStruct(enemy, place);
			return;
		}
		Enemy enemy2 = base.Bot.Enemy;
		if (enemy2 == null || enemy.IsDifferent(enemy2))
		{
			LastHeardDanger = new SoundStruct(enemy, place);
		}
	}
}
