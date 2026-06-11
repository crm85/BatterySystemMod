using EFT;
using EFT.InventoryLogic;
using SAIN.Components;
using SAIN.Models.Enums;
using SAIN.Preset.GlobalSettings;
using SAIN.SAINComponent.Classes.Info;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.WeaponFunction;

public class Firemode : BotBase
{
	private float CheckMagTimer;

	private float CheckChamberTimer;

	private float NextCheckTimer;

	private float _nextSwapTime;

	private float _swapFreq = 0.2f;

	private float SemiAutoSwapDist => base.Bot.Info.WeaponInfo.SwapToSemiDist;

	private float FullAutoSwapDist => base.Bot.Info.WeaponInfo.SwapToAutoDist;

	public Firemode(BotComponent sain)
		: base(sain)
	{
	}

	public override void ManualUpdate()
	{
		if (_nextSwapTime < Time.time)
		{
			_nextSwapTime = Time.time + _swapFreq;
			BotOwner botOwner = base.BotOwner;
			BotWeaponManager val = ((botOwner != null) ? botOwner.WeaponManager : null);
			BotWeaponSelector selector = val.Selector;
			if (selector != null && selector.IsWeaponReady)
			{
				checkSwapFiremode();
			}
		}
		base.ManualUpdate();
	}

	private bool checkSwapMachineGun()
	{
		if (base.Bot.ManualShoot.Reason != EShootReason.None && base.Bot.Info.WeaponInfo.EWeaponClass == EWeaponClass.machinegun && CanSetMode((EFireMode)0))
		{
			SetFireMode((EFireMode)0);
			return true;
		}
		return false;
	}

	private void checkSwapFiremode()
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		WeaponInfoClass weaponInfo = base.Bot.Info.WeaponInfo;
		if (weaponInfo == null || checkSwapMachineGun())
		{
			return;
		}
		BotOwner botOwner = base.BotOwner;
		if (botOwner == null)
		{
			return;
		}
		BotWeaponManager weaponManager = botOwner.WeaponManager;
		bool? obj;
		if (weaponManager == null)
		{
			obj = null;
		}
		else
		{
			BotStationaryWeaponData stationary = weaponManager.Stationary;
			obj = ((stationary != null) ? new bool?(stationary.Taken) : ((bool?)null));
		}
		if (obj == false)
		{
			if (getModeToSwap(weaponInfo, out var mode) && CanSetMode(mode))
			{
				SetFireMode(mode);
			}
			else
			{
				tryCheckWeapon();
			}
		}
	}

	private bool getModeToSwap(WeaponInfoClass weaponInfo, out EFireMode mode)
	{
		if (base.Bot.IsCheater)
		{
			if (weaponInfo.HasFireMode((EFireMode)0))
			{
				mode = (EFireMode)0;
				return true;
			}
			if (weaponInfo.HasFireMode((EFireMode)3))
			{
				mode = (EFireMode)3;
				return true;
			}
			mode = (EFireMode)2;
			return false;
		}
		float distanceToAimTarget = base.Bot.DistanceToAimTarget;
		mode = (EFireMode)2;
		if (distanceToAimTarget > SemiAutoSwapDist || GlobalSettingsClass.Instance.Shoot.ONLY_SEMIAUTO_TOGGLE)
		{
			if (weaponInfo.HasFireMode((EFireMode)1))
			{
				mode = (EFireMode)1;
			}
		}
		else if (distanceToAimTarget <= FullAutoSwapDist)
		{
			if (weaponInfo.HasFireMode((EFireMode)0))
			{
				mode = (EFireMode)0;
			}
			else if (weaponInfo.HasFireMode((EFireMode)3))
			{
				mode = (EFireMode)3;
			}
			else if (weaponInfo.HasFireMode((EFireMode)4))
			{
				mode = (EFireMode)4;
			}
		}
		return (int)mode != 2;
	}

	public void SetFireMode(EFireMode fireMode)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		Weapon currentWeapon = base.Bot.Info.WeaponInfo.CurrentWeapon;
		if (currentWeapon != null)
		{
			FireModeComponent fireMode2 = currentWeapon.FireMode;
			if (fireMode2 != null)
			{
				fireMode2.SetFireMode(fireMode);
			}
		}
		Player player = base.Player;
		if (player == null)
		{
			return;
		}
		AbstractHandsController handsController = player.HandsController;
		if (handsController != null)
		{
			FirearmsAnimator firearmsAnimator = handsController.FirearmsAnimator;
			if (firearmsAnimator != null)
			{
				firearmsAnimator.SetFireMode(fireMode, false);
			}
		}
	}

	public bool CanSetMode(EFireMode fireMode)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		WeaponInfoClass weaponInfo = base.Bot.Info.WeaponInfo;
		return weaponInfo?.CurrentWeapon != null && weaponInfo.HasFireMode(fireMode) && !weaponInfo.IsFireModeSet(fireMode);
	}

	private void tryCheckWeapon()
	{
		if (base.Bot.Enemy == null)
		{
			if (CheckMagTimer < Time.time && NextCheckTimer < Time.time)
			{
				NextCheckTimer = Time.time + 30f;
				CheckMagTimer = Time.time + 360f * Random.Range(0.5f, 1.5f);
				base.Player.HandsController.FirearmsAnimator.CheckAmmo();
			}
			else if (CheckChamberTimer < Time.time && NextCheckTimer < Time.time)
			{
				NextCheckTimer = Time.time + 30f;
				CheckChamberTimer = Time.time + 360f * Random.Range(0.5f, 1.5f);
				base.Player.HandsController.FirearmsAnimator.CheckChamber();
			}
		}
	}
}
