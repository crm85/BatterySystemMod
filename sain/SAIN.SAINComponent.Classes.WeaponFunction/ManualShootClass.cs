using EFT;
using EFT.HealthSystem;
using SAIN.Components;
using SAIN.Models.Enums;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.WeaponFunction;

public class ManualShootClass : BotComponentClassBase
{
	private Enemy ManualShootEnemy;

	private float _timeStartManualShoot;

	public bool Shooting => base.BotOwner.ShootData.Shooting;

	public Vector3 ShootPosition { get; private set; }

	public EShootReason Reason { get; private set; }

	public ManualShootClass(BotComponent bot)
		: base(bot)
	{
		base.TickRequirement = ESAINTickState.OnlyNoSleep;
	}

	public override void Init()
	{
		base.Bot.EnemyController.Events.OnEnemyRemoved += CheckClearEnemy;
		base.Init();
	}

	public override void ManualUpdate()
	{
		CheckReset();
		base.ManualUpdate();
	}

	public override void Dispose()
	{
		base.Bot.EnemyController.Events.OnEnemyRemoved -= CheckClearEnemy;
		base.Dispose();
	}

	private void CheckClearEnemy(string ID, Enemy Enemy)
	{
		if (Enemy == ManualShootEnemy)
		{
			Reset();
		}
	}

	public void Reset()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		base.BotOwner.ShootData.EndShoot();
		Reason = EShootReason.None;
		ShootPosition = Vector3.zero;
		ManualShootEnemy = null;
	}

	private void CheckReset()
	{
		if (Reason == EShootReason.None)
		{
			return;
		}
		Enemy manualShootEnemy = ManualShootEnemy;
		if (manualShootEnemy != null)
		{
			Player enemyPlayer = manualShootEnemy.EnemyPlayer;
			bool? obj;
			if (enemyPlayer == null)
			{
				obj = null;
			}
			else
			{
				IHealthController healthController = enemyPlayer.HealthController;
				obj = ((healthController != null) ? new bool?(healthController.IsAlive) : ((bool?)null));
			}
			bool? flag = obj;
			if (flag == true && base.BotOwner.WeaponManager.HaveBullets && !(_timeStartManualShoot + 2f < Time.time))
			{
				return;
			}
		}
		Reset();
	}

	public bool TryShoot(Enemy Enemy, Vector3 targetPos, bool checkFF = true, EShootReason reason = EShootReason.None)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (Enemy != null && CanShoot(checkFF) && base.Bot.Steering.AngleToPointFromLookDir(targetPos) <= 10f && base.Bot.FriendlyFire.UpdateFriendlyFireStatus(targetPos, base.Bot.Transform.WeaponFirePort, base.Bot.Transform.WeaponPointDirection, base.Bot))
		{
			ManualShootEnemy = Enemy;
			ShootPosition = targetPos;
			Reason = reason;
			if (!Shooting && base.BotOwner.ShootData.Shoot())
			{
				_timeStartManualShoot = Time.time;
			}
			return true;
		}
		Reset();
		return false;
	}

	public bool CanShoot(bool checkFF = true)
	{
		if (!checkFF || !base.Bot.FriendlyFire.ClearShot)
		{
		}
		BotWeaponManager weaponManager = base.BotOwner.WeaponManager;
		if (weaponManager.IsMelee)
		{
			return false;
		}
		if (!weaponManager.IsWeaponReady)
		{
			return false;
		}
		if (weaponManager.Reload.Reloading)
		{
			return false;
		}
		if (!base.BotOwner.ShootData.CanShootByState)
		{
			return false;
		}
		if (!weaponManager.HaveBullets)
		{
			return false;
		}
		return true;
	}
}
