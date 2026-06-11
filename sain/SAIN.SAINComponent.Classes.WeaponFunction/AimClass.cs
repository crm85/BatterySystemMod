using System;
using EFT;
using SAIN.Components;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.WeaponFunction;

public class AimClass : BotComponentClassBase, IBotClass, IDisposable
{
	private bool TurningWeaponToAimPoint;

	private Enemy _lastAimEnemy;

	public bool CanAim { get; private set; }

	public float LastAimTime { get; set; }

	public AimStatus AimStatus
	{
		get
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			BotOwner botOwner = base.BotOwner;
			object obj;
			if (botOwner == null)
			{
				obj = null;
			}
			else
			{
				AimingManager aimingManager = botOwner.AimingManager;
				obj = ((aimingManager != null) ? aimingManager.CurrentAiming : null);
			}
			return (AimStatus)(((_003F?)((BotAimingClass)(((obj is BotAimingClass) ? obj : null)?)).aimStatus_0) ?? 1);
		}
		set
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			BotOwner botOwner = base.BotOwner;
			object obj;
			if (botOwner == null)
			{
				obj = null;
			}
			else
			{
				AimingManager aimingManager = botOwner.AimingManager;
				obj = ((aimingManager != null) ? aimingManager.CurrentAiming : null);
			}
			BotAimingClass val = (BotAimingClass)((obj is BotAimingClass) ? obj : null);
			if (val != null)
			{
				val.aimStatus_0 = value;
			}
		}
	}

	public event Action<bool> OnAimAllowedOrBlocked;

	public AimClass(BotComponent sain)
		: base(sain)
	{
		base.TickRequirement = ESAINTickState.OnlyNoSleep;
	}

	public Vector3 EndTargetPoint()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		IBotAiming currentAiming = base.BotOwner.AimingManager.CurrentAiming;
		BotAimingClass val = (BotAimingClass)(object)((currentAiming is BotAimingClass) ? currentAiming : null);
		if (val != null)
		{
			return val.EndTargetPoint;
		}
		return Vector3.zero;
	}

	public Vector3 RealTargetPoint()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		IBotAiming currentAiming = base.BotOwner.AimingManager.CurrentAiming;
		BotAimingClass val = (BotAimingClass)(object)((currentAiming is BotAimingClass) ? currentAiming : null);
		if (val != null)
		{
			return val.RealTargetPoint;
		}
		return Vector3.zero;
	}

	public override void ManualUpdate()
	{
		checkCanAim();
		CheckLoseTarget();
		base.ManualUpdate();
	}

	public bool AimAtTarget(Vector3 shootPoint, Enemy enemy, out bool AimComplete, IBotAiming currentAiming, BotComponent bot)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		BotOwner botOwner = bot.BotOwner;
		BotWeaponManager weaponManager = botOwner.WeaponManager;
		if (!weaponManager.HaveBullets || weaponManager.Reload.Reloading)
		{
			botOwner.ShootData.EndShoot();
			AimComplete = false;
			base.Bot.Aim.LoseAimTarget();
			return false;
		}
		currentAiming.SetTarget(shootPoint);
		if (!bot.FriendlyFire.UpdateFriendlyFireStatus(currentAiming.LastDist2Target, bot.Transform.WeaponFirePort, bot.Transform.WeaponPointDirection, bot))
		{
			botOwner.ShootData.EndShoot();
			AimComplete = false;
			base.Bot.Aim.LoseAimTarget();
			return false;
		}
		CheckAimToEnemy(enemy);
		if (TurningWeaponToAimPoint)
		{
			AimComplete = false;
			return true;
		}
		currentAiming.NodeUpdate();
		AimComplete = currentAiming.IsReady;
		return true;
	}

	private void CheckAimToEnemy(Enemy enemy)
	{
		if (enemy != _lastAimEnemy)
		{
			TurningWeaponToAimPoint = true;
			_lastAimEnemy = enemy;
		}
		if (TurningWeaponToAimPoint && enemy.Vision.Angles.AngleToEnemyHorizontal <= 20f)
		{
			TurningWeaponToAimPoint = false;
		}
	}

	private void checkCanAim()
	{
		bool flag = CanAim;
		CanAim = canAim();
		if (flag != CanAim)
		{
			this.OnAimAllowedOrBlocked?.Invoke(CanAim);
		}
	}

	private bool canAim()
	{
		if (base.Player.IsSprintEnabled)
		{
			return false;
		}
		if (base.BotOwner.WeaponManager.Reload.Reloading)
		{
		}
		if (!base.Bot.HasEnemy)
		{
		}
		return true;
	}

	public void LoseAimTarget()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Invalid comparison between Unknown and I4
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		IBotAiming currentAiming = base.BotOwner.AimingManager.CurrentAiming;
		BotAimingClass val = (BotAimingClass)(object)((currentAiming is BotAimingClass) ? currentAiming : null);
		if (val != null && (int)val.aimStatus_0 != 1)
		{
			base.Bot.Steering.LookToDirection(base.Bot.LookDirection);
			val.aimStatus_0 = (AimStatus)1;
			TurningWeaponToAimPoint = false;
			_lastAimEnemy = null;
		}
	}

	private void CheckLoseTarget()
	{
		BotWeaponManager weaponManager = base.BotOwner.WeaponManager;
		if (!CanAim || !weaponManager.HaveBullets || weaponManager.Reload.Reloading)
		{
			LoseAimTarget();
			ShootData shootData = base.BotOwner.ShootData;
			if (shootData != null)
			{
				shootData.EndShoot();
			}
		}
	}
}
