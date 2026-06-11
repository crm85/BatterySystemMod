using System.Linq;
using EFT;
using EFT.InventoryLogic;
using SAIN.Components;
using SAIN.Components.BotComponentSpace.Classes;
using SAIN.Helpers;
using SAIN.Preset;
using SAIN.Preset.GlobalSettings;
using SAIN.SAINComponent.Classes.WeaponFunction;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Info;

public class WeaponInfoClass : BotBase
{
	private const float MACHINEGUN_SWAPDIST_MULTI = 1.5f;

	private Weapon _lastCheckedWeapon;

	private float _nextRecalcTime;

	private const float _recalcFreq = 60f;

	private float _nextCheckWeapTime;

	private const float _checkWeapFreq = 1f;

	private bool _forceNewCheck = false;

	public float FinalModifier { get; private set; }

	public EWeaponClass EWeaponClass { get; private set; }

	public ECaliber ECaliber { get; private set; }

	public float SwapToSemiDist { get; private set; } = 50f;

	public float SwapToAutoDist { get; private set; } = 45f;

	public Recoil Recoil { get; private set; }

	public Firerate Firerate { get; private set; }

	public Firemode Firemode { get; private set; }

	public ReloadClass Reload { get; private set; }

	public float EffectiveWeaponDistance
	{
		get
		{
			if (ECaliber == ECaliber.Caliber9x39)
			{
				return 125f;
			}
			if (BotBase.GlobalSettings.Shoot.EngagementDistance.TryGetValue(EWeaponClass, out var value))
			{
				return value;
			}
			return 125f;
		}
	}

	public float PreferedShootDistance => EffectiveWeaponDistance * 0.66f;

	public bool HasFullAuto => HasFireMode((EFireMode)0);

	public bool HasBurst => HasFireMode((EFireMode)3);

	public bool HasSemi => HasFireMode((EFireMode)1);

	public bool HasDoubleAction => HasFireMode((EFireMode)4);

	public EFireMode SelectedFireMode
	{
		get
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			if (CurrentWeapon != null)
			{
				return CurrentWeapon.SelectedFireMode;
			}
			return (EFireMode)0;
		}
	}

	public Weapon CurrentWeapon
	{
		get
		{
			BotOwner botOwner = base.BotOwner;
			object result;
			if (botOwner == null)
			{
				result = null;
			}
			else
			{
				BotWeaponManager weaponManager = botOwner.WeaponManager;
				result = ((weaponManager != null) ? weaponManager.CurrentWeapon : null);
			}
			return (Weapon)result;
		}
	}

	private static ShootSettings _shootSettings => SAINPlugin.LoadedPreset.GlobalSettings.Shoot;

	public WeaponInfoClass(BotComponent bot)
		: base(bot)
	{
		Recoil = new Recoil(bot);
		Firerate = new Firerate(bot);
		Firemode = new Firemode(bot);
		Reload = new ReloadClass(bot);
	}

	public override void Init()
	{
		Recoil.Init();
		Firerate.Init();
		Firemode.Init();
		Reload.Init();
		base.Init();
	}

	protected override void UpdatePresetSettings(SAINPresetClass preset)
	{
		_forceNewCheck = true;
	}

	public override void ManualUpdate()
	{
		checkCalcWeaponInfo();
		Recoil.ManualUpdate();
		Firerate.ManualUpdate();
		Firemode.ManualUpdate();
		Reload.ManualUpdate();
		base.ManualUpdate();
	}

	public void checkCalcWeaponInfo()
	{
		if (!(_nextCheckWeapTime < Time.time) && !_forceNewCheck)
		{
			return;
		}
		Weapon currentWeapon = CurrentWeapon;
		if (currentWeapon == null)
		{
			return;
		}
		_nextCheckWeapTime = Time.time + 1f;
		if (_forceNewCheck || _nextRecalcTime < Time.time || _lastCheckedWeapon == null || _lastCheckedWeapon != currentWeapon)
		{
			if (_forceNewCheck)
			{
				_forceNewCheck = false;
			}
			_nextRecalcTime = Time.time + 60f;
			_lastCheckedWeapon = currentWeapon;
			calculateCurrentWeapon(currentWeapon);
		}
	}

	private void calculateCurrentWeapon(Weapon weapon)
	{
		EWeaponClass = EnumValues.ParseWeaponClass(weapon.Template.weapClass);
		ECaliber = EnumValues.ParseCaliber(weapon.CurrentAmmoTemplate.Caliber);
		calculateShootModifier();
		SwapToSemiDist = getWeaponSwapToSemiDist(ECaliber, EWeaponClass);
		SwapToAutoDist = getWeaponSwapToFullAutoDist(ECaliber, EWeaponClass);
	}

	private static float getAmmoShootability(ECaliber caliber)
	{
		if (_shootSettings.AmmoCaliberShootability.TryGetValue(caliber, out var value))
		{
			return value;
		}
		return 0.5f;
	}

	private static float getWeaponShootability(EWeaponClass weaponClass)
	{
		if (_shootSettings.WeaponClassShootability.TryGetValue(weaponClass, out var value))
		{
			return value;
		}
		return 0.5f;
	}

	private static float getWeaponSwapToSemiDist(ECaliber caliber, EWeaponClass weaponClass)
	{
		if (_shootSettings.AmmoCaliberFullAutoMaxDistances.TryGetValue(caliber, out var value))
		{
			if (weaponClass == EWeaponClass.machinegun)
			{
				return value * 1.5f;
			}
			return value;
		}
		return 55f;
	}

	private static float getWeaponSwapToFullAutoDist(ECaliber caliber, EWeaponClass weaponClass)
	{
		return getWeaponSwapToSemiDist(caliber, weaponClass) * 0.85f;
	}

	private void calculateShootModifier()
	{
		WeaponInfoClass weaponInfo = base.Bot.Info.WeaponInfo;
		float num = getAmmoShootability(ECaliber).Scale0to1(_shootSettings.AmmoCaliberScaling).Round100();
		float num2 = getWeaponShootability(EWeaponClass).Scale0to1(_shootSettings.WeaponClassScaling).Round100();
		float num3 = base.Bot.Info.FileSettings.Mind.WeaponProficiency.Scale0to1(_shootSettings.WeaponProficiencyScaling).Round100();
		Weapon currentWeapon = weaponInfo.CurrentWeapon;
		float num4 = Mathf.Clamp(1f - currentWeapon.ErgonomicsTotal / 100f, 0.01f, 1f).Scale0to1(_shootSettings.ErgoScaling).Round100();
		float num5 = (currentWeapon.RecoilTotal / currentWeapon.RecoilBase + (float)currentWeapon.CurrentAmmoTemplate.ammoRec / 200f).Scale0to1(_shootSettings.RecoilScaling).Round100();
		float num6 = base.Bot.Info.Profile.DifficultyModifier.Scale0to1(_shootSettings.DifficultyScaling).Round100();
		FinalModifier = (num2 * num5 * num4 * num * num3 * num6).Round100();
	}

	public override void Dispose()
	{
		Recoil.Dispose();
		Firerate.Dispose();
		Firemode.Dispose();
		Reload.Dispose();
		base.Dispose();
	}

	public bool IsFireModeSet(EFireMode mode)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return SelectedFireMode == mode;
	}

	public bool HasFireMode(EFireMode fireMode)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Weapon currentWeapon = CurrentWeapon;
		return ((currentWeapon != null) ? currentWeapon.WeapFireType : null)?.Contains(fireMode) ?? false;
	}
}
