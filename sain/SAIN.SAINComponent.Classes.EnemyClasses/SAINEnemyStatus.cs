using System;
using EFT;
using SAIN.Models.Enums;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.EnemyClasses;

public class SAINEnemyStatus : EnemyBase, IBotEnemyClass, IBotClass, IDisposable
{
	private readonly ExpirableBool _heardRecently = new ExpirableBool(2f, 0.85f, 1.15f);

	private readonly ExpirableBool _enemyIsReloading = new ExpirableBool(4f, 0.75f, 1.25f);

	private readonly ExpirableBool _enemyHasGrenade = new ExpirableBool(4f, 0.75f, 1.25f);

	private readonly ExpirableBool _enemyIsHealing = new ExpirableBool(4f, 0.75f, 1.25f);

	private readonly ExpirableBool _enemyShotAtMe = new ExpirableBool(30f, 0.75f, 1.25f);

	private readonly ExpirableBool _enemyIsSuppressed = new ExpirableBool(4f, 0.85f, 1.15f);

	private readonly ExpirableBool _enemyLooting = new ExpirableBool(30f, 0.85f, 1.15f);

	private readonly ExpirableBool _enemySurgery = new ExpirableBool(8f, 0.85f, 1.15f);

	private readonly ExpirableBool _shotByEnemy = new ExpirableBool(2f, 0.75f, 1.25f);

	private bool _enemyLookAtMe;

	private float _nextCheckEnemyLookTime;

	private const float _maxDistFromPosFlareEnabled = 10f;

	private float _nextCheckHealthTime;

	public ETagStatus EnemyHealthStatus { get; private set; }

	public EEnemyAction VulnerableAction { get; private set; }

	public bool PositionalFlareEnabled => base.Enemy.EnemyKnown && base.Enemy.KnownPlaces.EnemyDistanceFromLastKnown < 10f;

	public bool HeardRecently
	{
		get
		{
			return _heardRecently.Value;
		}
		set
		{
			_heardRecently.Value = value;
		}
	}

	public bool EnemyLookingAtMe
	{
		get
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			if (_nextCheckEnemyLookTime < Time.time)
			{
				_nextCheckEnemyLookTime = Time.time + 0.2f;
				Vector3 val = base.Bot.Position - base.EnemyCurrentPosition;
				Vector3 normalized = ((Vector3)(ref val)).normalized;
				val = base.EnemyPerson.Transform.LookDirection;
				Vector3 normalized2 = ((Vector3)(ref val)).normalized;
				float num = Vector3.Dot(normalized, normalized2);
				_enemyLookAtMe = num >= 0.9f;
			}
			return _enemyLookAtMe;
		}
	}

	public bool ShotByEnemyRecently
	{
		get
		{
			return _shotByEnemy.Value;
		}
		set
		{
			if (value)
			{
				UpdateShotStatus();
				UpdateShotPos();
			}
			_shotByEnemy.Value = value;
		}
	}

	public bool EnemyUsingSurgery
	{
		get
		{
			return _enemySurgery.Value;
		}
		set
		{
			_enemySurgery.Value = value;
		}
	}

	public bool EnemyIsLooting
	{
		get
		{
			return _enemyLooting.Value;
		}
		set
		{
			_enemyLooting.Value = value;
		}
	}

	public bool SearchingBecauseLooting { get; set; }

	public bool EnemyIsSuppressed
	{
		get
		{
			return _enemyIsSuppressed.Value;
		}
		set
		{
			_enemyIsSuppressed.Value = value;
		}
	}

	public bool ShotAtMeRecently
	{
		get
		{
			return _enemyShotAtMe.Value;
		}
		set
		{
			_enemyShotAtMe.Value = value;
		}
	}

	public bool EnemyIsReloading
	{
		get
		{
			return _enemyIsReloading.Value;
		}
		set
		{
			_enemyIsReloading.Value = value;
		}
	}

	public bool EnemyHasGrenadeOut
	{
		get
		{
			return _enemyHasGrenade.Value;
		}
		set
		{
			_enemyHasGrenade.Value = value;
		}
	}

	public bool EnemyIsHealing
	{
		get
		{
			return _enemyIsHealing.Value;
		}
		set
		{
			_enemyIsHealing.Value = value;
		}
	}

	public int NumberOfSearchesStarted { get; set; }

	public bool ShotByEnemy { get; private set; }

	public float TimeFirstShot { get; private set; }

	public Vector3? LastShotPosition { get; private set; }

	public override void Init()
	{
		EnemyEvents.EnemyToggleEventTimeTracked onEnemyKnownChanged = base.Enemy.Events.OnEnemyKnownChanged;
		onEnemyKnownChanged.OnToggle = (Action<bool, Enemy>)Delegate.Combine(onEnemyKnownChanged.OnToggle, new Action<bool, Enemy>(OnEnemyKnownChanged));
		base.Init();
	}

	public override void ManualUpdate()
	{
		if (base.Enemy.EnemyKnown)
		{
			UpdateVulnerableState();
			updateHealthStatus();
		}
		base.ManualUpdate();
	}

	public override void Dispose()
	{
		EnemyEvents.EnemyToggleEventTimeTracked onEnemyKnownChanged = base.Enemy.Events.OnEnemyKnownChanged;
		onEnemyKnownChanged.OnToggle = (Action<bool, Enemy>)Delegate.Remove(onEnemyKnownChanged.OnToggle, new Action<bool, Enemy>(OnEnemyKnownChanged));
		base.Dispose();
	}

	private void updateHealthStatus()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (!(_nextCheckHealthTime > Time.time))
		{
			_nextCheckHealthTime = Time.time + 0.5f;
			ETagStatus enemyHealthStatus = EnemyHealthStatus;
			EnemyHealthStatus = base.EnemyPlayer.HealthStatus;
			if (enemyHealthStatus != EnemyHealthStatus)
			{
				base.Enemy.Events.HealthStatusChanged(EnemyHealthStatus);
			}
		}
	}

	public void OnEnemyKnownChanged(bool known, Enemy enemy)
	{
		if (!known)
		{
			EnemyHealthStatus = (ETagStatus)1024;
			SetVulnerableAction(EEnemyAction.None);
		}
	}

	private EEnemyAction CheckVulnerableAction()
	{
		if (EnemyUsingSurgery)
		{
			return EEnemyAction.UsingSurgery;
		}
		if (EnemyIsReloading)
		{
			return EEnemyAction.Reloading;
		}
		if (EnemyHasGrenadeOut)
		{
			return EEnemyAction.HasGrenade;
		}
		if (EnemyIsHealing)
		{
			return EEnemyAction.Healing;
		}
		if (EnemyIsLooting)
		{
			return EEnemyAction.Looting;
		}
		return EEnemyAction.None;
	}

	private void UpdateVulnerableState()
	{
		EEnemyAction vulnerableAction = VulnerableAction;
		VulnerableAction = CheckVulnerableAction();
		if (vulnerableAction != VulnerableAction)
		{
			base.Enemy.Events.EnemyVulnerableChanged(VulnerableAction);
		}
	}

	public void SetVulnerableAction(EEnemyAction action)
	{
		if (action != VulnerableAction)
		{
			VulnerableAction = action;
			switch (action)
			{
			case EEnemyAction.None:
				ResetActions();
				break;
			case EEnemyAction.Reloading:
				EnemyIsReloading = true;
				break;
			case EEnemyAction.HasGrenade:
				EnemyHasGrenadeOut = true;
				break;
			case EEnemyAction.Healing:
				EnemyIsHealing = true;
				break;
			case EEnemyAction.Looting:
				EnemyIsLooting = true;
				break;
			case EEnemyAction.UsingSurgery:
				EnemyUsingSurgery = true;
				break;
			}
			base.Enemy.Events.EnemyVulnerableChanged(action);
		}
	}

	private void ResetActions()
	{
		HeardRecently = false;
		_enemyLookAtMe = false;
		ShotByEnemyRecently = false;
		EnemyUsingSurgery = false;
		EnemyIsLooting = false;
		EnemyHasGrenadeOut = false;
		EnemyIsHealing = false;
		EnemyIsReloading = false;
		ShotByEnemy = false;
		TimeFirstShot = 0f;
		LastShotPosition = null;
	}

	private void UpdateShotStatus()
	{
		if (!ShotByEnemy)
		{
			ShotByEnemy = true;
			TimeFirstShot = Time.time;
		}
	}

	private void UpdateShotPos()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Random.onUnitSphere;
		val.y = 0f;
		val = ((Vector3)(ref val)).normalized;
		val *= Random.Range(0.5f, base.Enemy.RealDistance / 5f);
		LastShotPosition = base.Enemy.EnemyPosition + val;
	}

	public void GetHit(DamageInfoStruct DamageInfoStruct)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		IPlayerOwner player = DamageInfoStruct.Player;
		IPlayer val = ((player != null) ? player.iPlayer : null);
		if (val != null && val.ProfileId == base.Enemy.EnemyProfileId)
		{
			ShotByEnemyRecently = true;
			base.Enemy.Events.ShotByEnemy();
		}
	}

	public SAINEnemyStatus(Enemy enemy)
		: base(enemy)
	{
	}
}
