using System;
using EFT;
using SAIN.Helpers.Events;
using SAIN.Models.Enums;
using UnityEngine;
using UnityEngine.AI;

namespace SAIN.SAINComponent.Classes.EnemyClasses;

public class EnemyEvents : EnemyBase, IBotClass, IDisposable
{
	public class EnemyToggleEvent : ToggleEventForObject<Enemy>
	{
		public EnemyToggleEvent(Enemy enemy, bool defaultValue)
			: base(enemy, defaultValue)
		{
		}
	}

	public class EnemyToggleEventTimeTracked : ToggleEventForObjectTimeTracked<Enemy>
	{
		public EnemyToggleEventTimeTracked(Enemy enemy, bool defaultValue)
			: base(enemy, defaultValue)
		{
		}
	}

	public EnemyToggleEventTimeTracked OnEnemyLineOfSightChanged { get; }

	public EnemyToggleEventTimeTracked OnEnemyKnownChanged { get; }

	public EnemyToggleEventTimeTracked OnActiveThreatChanged { get; }

	public EnemyToggleEventTimeTracked OnVisionChange { get; }

	public EnemyToggleEventTimeTracked OnSearch { get; }

	public EnemyToggleEventTimeTracked OnEnemyCanShootChanged { get; }

	public event Action<Enemy> OnEnemyInvalid;

	public event Action<Enemy> OnEnemyLocationsSearched;

	public event Action<Enemy> OnFirstSeen;

	public event Action<Enemy> OnEnemyShot;

	public event Action<Enemy> OnBeingShotByEnemy;

	public event Action<Enemy, EnemyPlace> OnPositionUpdated;

	public event Action<Enemy, SAINSoundType, bool, EnemyPlace> OnEnemyHeard;

	public event Action<Enemy, NavMeshPathStatus> OnPathUpdated;

	public event Action<Enemy, EEnemyAction> OnVulnerableStateChanged;

	public event Action<Enemy, ETagStatus> OnHealthStatusChanged;

	public EnemyEvents(Enemy enemy)
		: base(enemy)
	{
		OnEnemyLineOfSightChanged = new EnemyToggleEventTimeTracked(enemy, defaultValue: false);
		OnEnemyKnownChanged = new EnemyToggleEventTimeTracked(enemy, defaultValue: false);
		OnActiveThreatChanged = new EnemyToggleEventTimeTracked(enemy, defaultValue: false);
		OnVisionChange = new EnemyToggleEventTimeTracked(enemy, defaultValue: false);
		OnSearch = new EnemyToggleEventTimeTracked(enemy, defaultValue: false);
		OnEnemyCanShootChanged = new EnemyToggleEventTimeTracked(enemy, defaultValue: false);
		base.CanEverTick = false;
	}

	public override void Init()
	{
		base.EnemyPlayer.BeingHitAction += enemyHit;
		base.Init();
	}

	public override void Dispose()
	{
		Player enemyPlayer = base.EnemyPlayer;
		if ((Object)(object)enemyPlayer != (Object)null)
		{
			enemyPlayer.BeingHitAction -= enemyHit;
		}
		base.Dispose();
	}

	public void EnemyLocationsSearched()
	{
		this.OnEnemyLocationsSearched?.Invoke(base.Enemy);
	}

	public void LastKnownUpdated(EnemyPlace place)
	{
		OnEnemyKnownChanged.CheckToggle(value: true);
		this.OnPositionUpdated?.Invoke(base.Enemy, place);
	}

	public void SetEnemyAsInvalid()
	{
		this.OnEnemyInvalid?.Invoke(base.Enemy);
	}

	public void ShotByEnemy()
	{
		this.OnBeingShotByEnemy?.Invoke(base.Enemy);
	}

	public void HealthStatusChanged(ETagStatus status)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		this.OnHealthStatusChanged?.Invoke(base.Enemy, status);
	}

	public void EnemyVulnerableChanged(EEnemyAction action)
	{
		this.OnVulnerableStateChanged?.Invoke(base.Enemy, action);
	}

	public void PathUpdated(NavMeshPathStatus status)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		this.OnPathUpdated?.Invoke(base.Enemy, status);
	}

	public void EnemyFirstSeen()
	{
		this.OnFirstSeen?.Invoke(base.Enemy);
	}

	public void EnemyHeard(SAINSoundType type, bool gunFire, EnemyPlace place)
	{
		this.OnEnemyHeard?.Invoke(base.Enemy, type, gunFire, place);
	}

	private void enemyHit(DamageInfoStruct damage, EBodyPart _, float _2)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		IPlayerOwner player = damage.Player;
		IPlayer val = ((player != null) ? player.iPlayer : null);
		if (val != null && val.ProfileId == base.Bot.ProfileId)
		{
			this.OnEnemyShot?.Invoke(base.Enemy);
		}
	}
}
