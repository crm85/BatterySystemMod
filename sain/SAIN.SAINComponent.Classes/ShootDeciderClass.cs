using System;
using Comfort.Common;
using EFT;
using EFT.HealthSystem;
using EFT.InventoryLogic;
using SAIN.Components;
using SAIN.Components.PlayerComponentSpace.Classes.Equipment;
using SAIN.Models.Enums;
using SAIN.Preset.GlobalSettings;
using SAIN.SAINComponent.Classes.EnemyClasses;
using SAIN.SAINComponent.Classes.Info;
using UnityEngine;

namespace SAIN.SAINComponent.Classes;

public class ShootDeciderClass : BotComponentClassBase
{
	private bool _shooting;

	private EquipmentSlot optimalSlot;

	private float _nextCheckOptimalTime;

	private float _nextChangeWeaponTime;

	private float _changeAimTimer;

	public Enemy LastShotEnemy { get; private set; }

	private EquipmentSlot CurrentSlot => base.BotOwner.WeaponManager.Selector.EquipmentSlot;

	public ShootDeciderClass(BotComponent bot)
		: base(bot)
	{
		base.TickRequirement = ESAINTickState.OnlyBotInCombat;
	}

	public override void Init()
	{
		base.Bot.EnemyController.Events.OnEnemyRemoved += CheckClearEnemy;
		base.Init();
	}

	public override void ManualUpdate()
	{
		CheckEndShoot();
		base.ManualUpdate();
	}

	public override void Dispose()
	{
		base.Bot.EnemyController.Events.OnEnemyRemoved -= CheckClearEnemy;
		base.Dispose();
	}

	private void CheckEndShoot()
	{
		if (!_shooting)
		{
			return;
		}
		BotWeaponManager weaponManager = base.BotOwner.WeaponManager;
		if (weaponManager == null || !weaponManager.HaveBullets || weaponManager.Reload.Reloading)
		{
			EndShoot();
			return;
		}
		Enemy lastShotEnemy = LastShotEnemy;
		if (lastShotEnemy != null)
		{
			Player enemyPlayer = lastShotEnemy.EnemyPlayer;
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
			if (flag == true)
			{
				if (!base.BotOwner.ShootData.Shooting)
				{
					EndShoot();
				}
				return;
			}
		}
		EndShoot();
		LastShotEnemy = null;
	}

	private void CheckClearEnemy(string profileId, Enemy enemy)
	{
		if (LastShotEnemy != null && LastShotEnemy.EnemyProfileId == profileId)
		{
			LastShotEnemy = null;
			if (_shooting)
			{
				EndShoot();
			}
		}
	}

	public void EndShoot()
	{
		_shooting = false;
		ShootData shootData = base.BotOwner.ShootData;
		if (shootData != null)
		{
			shootData.EndShoot();
		}
	}

	public Enemy GetEnemyToShoot(Enemy priorityEnemy = null)
	{
		if (priorityEnemy == null)
		{
			base.Bot.Aim.LoseAimTarget();
			return null;
		}
		if (AimAndShootAtEnemy(priorityEnemy, base.Bot))
		{
			UpdateADS(priorityEnemy);
			return priorityEnemy;
		}
		Enemy enemy = CheckEnemiesForShootableTargets(priorityEnemy.Bot.EnemyController.EnemyLists.GetEnemyList(EEnemyListType.Visible));
		if (enemy != null)
		{
			UpdateADS(enemy);
			return enemy;
		}
		UpdateADS(priorityEnemy);
		base.Bot.Aim.LoseAimTarget();
		return null;
	}

	public bool ShootAnyVisibleEnemies(Enemy priorityEnemy = null)
	{
		return GetEnemyToShoot(priorityEnemy) != null;
	}

	private void UpdateADS(Enemy enemy)
	{
		if (_changeAimTimer < Time.time)
		{
			_changeAimTimer = Time.time + 0.25f;
			base.Bot.AimDownSightsController.UpdateADSstatus(enemy);
		}
	}

	public Enemy CheckEnemiesForShootableTargets(EnemyList VisibleEnemies)
	{
		foreach (Enemy VisibleEnemy in VisibleEnemies)
		{
			if (AimAndShootAtEnemy(VisibleEnemy, base.Bot))
			{
				return VisibleEnemy;
			}
		}
		return null;
	}

	private bool AimAndShootAtEnemy(Enemy Enemy, BotComponent bot)
	{
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Invalid comparison between Unknown and I4
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		if (Enemy == null)
		{
			return false;
		}
		Player player = Enemy.Player;
		if (player != null)
		{
			IHealthController healthController = player.HealthController;
			if (((healthController != null) ? new bool?(healthController.IsAlive) : ((bool?)null)) == false)
			{
				return false;
			}
		}
		BotWeaponManager weaponManager = bot.BotOwner.WeaponManager;
		if (weaponManager == null)
		{
			return false;
		}
		bool reloading = weaponManager.Reload.Reloading;
		if (reloading || !weaponManager.HaveBullets)
		{
			if (!reloading && (int)weaponManager.Selector.EquipmentSlot == 2 && !weaponManager.Selector.TryChangeToMain())
			{
				SelectWeapon(Enemy);
			}
			return false;
		}
		if (!bot.Aim.CanAim)
		{
			return false;
		}
		Vector3? shootTargetPosition = GetShootTargetPosition(Enemy, bot);
		if (shootTargetPosition.HasValue && Enemy != null)
		{
			bot.BotLight.HandleLightForEnemy(Enemy);
			if (bot.Aim.AimAtTarget(shootTargetPosition.Value, Enemy, out var AimComplete, bot.BotOwner.AimingManager.CurrentAiming, bot))
			{
				ShootWhenAimComplete(Enemy, bot, AimComplete);
				return true;
			}
		}
		return false;
	}

	private void ShootWhenAimComplete(Enemy Enemy, BotComponent bot, bool AimComplete)
	{
		if (AimComplete && bot.BotOwner.ShootData.Shoot())
		{
			LastShotEnemy = Enemy;
			EnemyInfo enemyInfo = Enemy.EnemyInfo;
			if (enemyInfo != null)
			{
				enemyInfo.SetLastShootTime();
			}
			_shooting = true;
		}
	}

	private void SelectWeapon(Enemy Enemy)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		FindOptimalWeaponForDistance(Enemy.RealDistance);
		if (CurrentSlot != optimalSlot)
		{
			TryChangeWeapon(optimalSlot);
		}
	}

	private void TryChangeWeapon(EquipmentSlot slot)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected I4, but got Unknown
		if (!(_nextChangeWeaponTime < Time.time))
		{
			return;
		}
		BotOwner botOwner = base.BotOwner;
		object obj;
		if (botOwner == null)
		{
			obj = null;
		}
		else
		{
			BotWeaponManager weaponManager = botOwner.WeaponManager;
			obj = ((weaponManager != null) ? weaponManager.Selector : null);
		}
		BotWeaponSelector val = (BotWeaponSelector)obj;
		if (val != null)
		{
			_nextChangeWeaponTime = Time.time + 1f;
			switch ((int)slot)
			{
			case 0:
				val.TryChangeToMain();
				break;
			case 1:
				val.ChangeToSecond((Action<Result<IHandsController>>)null);
				break;
			case 2:
				val.TryChangeWeapon(true);
				break;
			}
		}
	}

	private void FindOptimalWeaponForDistance(float distance)
	{
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		if (!(_nextCheckOptimalTime < Time.time))
		{
			return;
		}
		_nextCheckOptimalTime = Time.time + 0.5f;
		SAINEquipmentClass equipment = base.Bot.PlayerComponent.Equipment;
		float? num = null;
		WeaponInfo primaryWeapon = equipment.PrimaryWeapon;
		if (IsWeaponDurableEnough(primaryWeapon))
		{
			num = primaryWeapon.EngagementDistance;
		}
		float? num2 = null;
		WeaponInfo secondaryWeapon = equipment.SecondaryWeapon;
		if (IsWeaponDurableEnough(secondaryWeapon))
		{
			num2 = secondaryWeapon.EngagementDistance;
		}
		float? num3 = null;
		WeaponInfo holsterWeapon = equipment.HolsterWeapon;
		if (IsWeaponDurableEnough(holsterWeapon))
		{
			num3 = holsterWeapon.EngagementDistance;
		}
		float num4 = Mathf.Abs((distance - num).GetValueOrDefault());
		optimalSlot = (EquipmentSlot)0;
		float num5 = Mathf.Abs((distance - num2).GetValueOrDefault());
		if (num5 < num4)
		{
			num4 = num5;
			optimalSlot = (EquipmentSlot)1;
		}
		if (!base.BotOwner.WeaponManager.HaveBullets)
		{
			num5 = Mathf.Abs((distance - num3).GetValueOrDefault());
			if (num5 < num4)
			{
				num4 = num5;
				optimalSlot = (EquipmentSlot)2;
			}
		}
	}

	private static bool IsWeaponDurableEnough(WeaponInfo info, float min = 0.5f)
	{
		return info != null && info.Durability > min && info.Weapon.ChamberAmmoCount > 0;
	}

	private static Vector3? GetShootTargetPosition(Enemy enemy, BotComponent bot)
	{
		return GetAimTarget(enemy, bot) ?? GetAimTarget(bot.LastEnemy, bot);
	}

	private static Vector3? GetAimTarget(Enemy enemy, BotComponent bot)
	{
		if (enemy != null && enemy.IsVisible && enemy.CanShoot)
		{
			Vector3? val = FindCenterMassPoint(enemy, bot);
			Vector3? enemyPartToShoot = GetEnemyPartToShoot(enemy.EnemyInfo);
			return CheckYValue(val, enemyPartToShoot) ?? enemyPartToShoot ?? val;
		}
		return null;
	}

	private static Vector3? CheckYValue(Vector3? centerMass, Vector3? partTarget)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (centerMass.HasValue && partTarget.HasValue && centerMass.Value.y < partTarget.Value.y)
		{
			Vector3 value = partTarget.Value;
			value.y = centerMass.Value.y;
			return value;
		}
		return null;
	}

	private static Vector3? FindCenterMassPoint(Enemy enemy, BotComponent bot)
	{
		if (enemy.IsAI)
		{
			return null;
		}
		if (!SAINPlugin.LoadedPreset.GlobalSettings.Aiming.AimCenterMassGlobal)
		{
			return null;
		}
		if (!bot.Info.FileSettings.Aiming.AimCenterMass)
		{
			return null;
		}
		if (bot.Info.Profile.IsPMC && GlobalSettingsClass.Instance.Aiming.PMCSAimForHead)
		{
			return null;
		}
		return enemy.CenterMass;
	}

	private static Vector3? GetEnemyPartToShoot(EnemyInfo enemy)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (enemy != null)
		{
			return (!(enemy.Distance < 6f)) ? enemy.GetPartToShoot() : enemy.GetCenterPart();
		}
		return null;
	}
}
