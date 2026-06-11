using EFT;
using EFT.InventoryLogic;
using SAIN.Components;
using SAIN.Helpers;
using SAIN.Preset.GlobalSettings;
using SAIN.SAINComponent.Classes.Info;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.WeaponFunction;

public class Recoil : BotBase
{
	private float _barrelRecoveryTime;

	private bool _recoilFinished;

	public Vector3 CurrentRecoilOffset { get; private set; } = Vector3.zero;

	public float ArmInjuryModifier => calcModFromInjury(base.Bot.Medical.HitReaction.LeftArmInjury) * calcModFromInjury(base.Bot.Medical.HitReaction.RightArmInjury);

	private static bool _debugRecoilLogs => SAINPlugin.DebugSettings.Logs.DebugRecoilCalculations;

	private static float _recoilDecayCoef => SAINPlugin.LoadedPreset.GlobalSettings.Shoot.RECOIL_DECAY_COEF;

	private bool _armsInjured => base.Bot.Medical.HitReaction.ArmsInjured;

	private float RecoilMultiplier => Mathf.Round(base.Bot.Info.FileSettings.Shoot.RecoilMultiplier * BotBase.GlobalSettings.Shoot.RecoilMultiplier * 100f) / 100f;

	private float _shootModifier => base.Bot.Info.WeaponInfo.FinalModifier;

	private ShootSettings _shootSettings => GlobalSettingsClass.Instance.Shoot;

	public Recoil(BotComponent sain)
		: base(sain)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_0006: Unknown result type (might be due to invalid IL or missing references)


	public override void Init()
	{
		base.PlayerComponent.OnShoot += WeaponShot;
		base.Init();
	}

	public override void ManualUpdate()
	{
		calcDecay();
		base.ManualUpdate();
	}

	public override void Dispose()
	{
		base.PlayerComponent.OnShoot -= WeaponShot;
		base.Dispose();
	}

	private void calcDecay()
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (!_recoilFinished)
		{
			float num = GameWorldComponent.WorldTickDeltaTime * _recoilDecayCoef;
			_barrelRecoveryTime += num;
			if (_barrelRecoveryTime >= 1f)
			{
				_barrelRecoveryTime = 0f;
				_recoilFinished = true;
				CurrentRecoilOffset = Vector3.zero;
			}
			else
			{
				CurrentRecoilOffset = Vector3.Lerp(CurrentRecoilOffset, Vector3.zero, _barrelRecoveryTime);
			}
		}
	}

	public void WeaponShot(WeaponInfo WeaponInfo, Vector3 force)
	{
		if (base.Bot.IsCheater)
		{
			Logger.LogDebug("cheato");
		}
		else
		{
			calculateRecoil(WeaponInfo.Weapon);
		}
	}

	private void calculateRecoil(Weapon weapon)
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		if (weapon == null)
		{
			Logger.LogError("Weapon Null!");
			return;
		}
		_recoilFinished = false;
		float addRecoil = SAINPlugin.LoadedPreset.GlobalSettings.Shoot.AddRecoil;
		float num = calcRecoilMod();
		float recoilTotal = weapon.RecoilTotal;
		float num2 = calcRecoilNum(recoilTotal) + addRecoil;
		float num3 = num2 * num;
		float num4 = Random.Range(num3 / 2f, num3) * randomSign();
		float num5 = Random.Range(num3 / 2f, num3) * randomSign();
		Vector3 weaponPointDirection = base.Bot.Transform.WeaponPointDirection;
		Vector3 val = Vector.Rotate(weaponPointDirection, num5, num4, 0f);
		val -= weaponPointDirection;
		CurrentRecoilOffset += val;
		if (SAINPlugin.DebugSettings.Gizmos.DebugDrawRecoilGizmos)
		{
			DebugGizmos.Ray(base.Bot.Transform.WeaponFirePort, weaponPointDirection * base.BotOwner.AimingManager.CurrentAiming.LastDist2Target, Color.red, base.BotOwner.AimingManager.CurrentAiming.LastDist2Target, 0.02f, temporary: true, 10f);
		}
		if (_debugRecoilLogs)
		{
			string[] obj = new string[5]
			{
				$"Recoil! New Recoil: [{((Vector3)(ref val)).magnitude}] ",
				null,
				null,
				null,
				null
			};
			Vector3 currentRecoilOffset = CurrentRecoilOffset;
			obj[1] = $"Current Total Recoil Magnitude: [{((Vector3)(ref currentRecoilOffset)).magnitude}] ";
			obj[2] = $"recoilNum: [{num2}] calcdRecoil: [{num3}] : ";
			obj[3] = $"Randomized Vert [{num4}] : Randomized Horiz [{num5}] ";
			obj[4] = $"Modifiers [ Add: [{addRecoil}] Multi: [{num}] Weapon RecoilTotal [{recoilTotal}]] Shoot Modifier: [{base.Bot.Info.WeaponInfo.FinalModifier}]";
			Logger.LogDebug(string.Concat(obj));
		}
	}

	private float randomSign()
	{
		return (!EFTMath.RandomBool()) ? 1 : (-1);
	}

	private float calcModFromInjury(EInjurySeverity severity)
	{
		return severity switch
		{
			EInjurySeverity.Injury => 1.15f, 
			EInjurySeverity.HeavyInjury => 1.35f, 
			EInjurySeverity.Destroyed => 1.65f, 
			_ => 1f, 
		};
	}

	private float calcRecoilMod()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Invalid comparison between Unknown and I4
		float num = 1f * RecoilMultiplier;
		if (base.Player.IsInPronePose)
		{
			num *= 0.7f;
		}
		else if ((int)base.Player.Pose == 1)
		{
			num *= 0.9f;
		}
		BotWeaponManager weaponManager = base.BotOwner.WeaponManager;
		if (weaponManager != null)
		{
			IFirearmHandsController shootController = weaponManager.ShootController;
			if (((shootController != null) ? new bool?(((IHandsController)shootController).IsAiming) : ((bool?)null)) == true)
			{
				num *= 0.9f;
			}
		}
		if (base.Bot.Transform.VelocityMagnitudeNormal < 0.1f)
		{
			num *= 0.85f;
		}
		if (_armsInjured)
		{
			num *= Mathf.Sqrt(ArmInjuryModifier);
		}
		return num;
	}

	private float calcRecoilNum(float recoilVal)
	{
		float num = recoilVal / _shootSettings.RECOIL_BASELINE;
		if (ModDetection.RealismLoaded)
		{
			num = recoilVal / _shootSettings.RECOIL_BASELINE_REALISM;
		}
		num *= shootModClamped();
		return num * Random.Range(0.8f, 1.2f);
	}

	private float shootModClamped()
	{
		return Mathf.Clamp(_shootModifier, 0.5f, 2f);
	}
}
