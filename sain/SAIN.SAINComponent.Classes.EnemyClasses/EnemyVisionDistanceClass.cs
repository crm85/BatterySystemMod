using SAIN.Components;
using SAIN.Components.PlayerComponentSpace;
using SAIN.SAINComponent.Classes.Info;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.EnemyClasses;

public class EnemyVisionDistanceClass : EnemyBase
{
	private float _nextCalcTime;

	private float _calcFreq = 0.05f;

	private float _visionDist;

	public float Value
	{
		get
		{
			if (_nextCalcTime < Time.time)
			{
				_nextCalcTime = Time.time + _calcFreq;
				_visionDist = CalcVisionDistance();
			}
			return _visionDist;
		}
	}

	private static float _sprintMod => SAINPlugin.LoadedPreset.GlobalSettings.Look.VisionDistance.MovementDistanceModifier;

	public EnemyVisionDistanceClass(Enemy enemy)
		: base(enemy)
	{
	}

	private bool IsEnemyAlwaysInVisibleDistance()
	{
		if (base.Enemy.Vision.Angles.AngleToEnemy < 30f && base.Enemy.KnownPlaces.EnemyDistanceFromLastKnown < 3f && BotManagerComponent.Instance.TimeVision.VisibilityRatio > 0.5f)
		{
			return true;
		}
		return false;
	}

	private float CalcVisionDistance()
	{
		if (IsEnemyAlwaysInVisibleDistance())
		{
			return 1000f;
		}
		float num = CalcAngleMod();
		float num2 = CalcMovementMod();
		float num3 = CalcGearStealthMod();
		float flare = GetFlare();
		SAINEnemyStatus status = base.Enemy.Status;
		bool positionalFlareEnabled = status.PositionalFlareEnabled;
		bool shotAtMeRecently = status.ShotAtMeRecently;
		float num4 = (positionalFlareEnabled ? 1.5f : 1f);
		float num5 = (shotAtMeRecently ? 1.5f : 1f);
		float num6 = num2 * num * flare * num4 * num5 / num3;
		float visibleDist = base.BotOwner.LookSensor.VisibleDist;
		return visibleDist * num6 - visibleDist;
	}

	private float CalcMovementMod()
	{
		float enemyVelocity = base.Enemy.Vision.EnemyVelocity;
		return Mathf.Lerp(0.9f, _sprintMod, enemyVelocity);
	}

	private float CalcAngleMod()
	{
		float angleToEnemy = base.Enemy.Vision.Angles.AngleToEnemy;
		float maxVisionAngle = base.Enemy.Vision.Angles.MaxVisionAngle;
		if (angleToEnemy > maxVisionAngle)
		{
			return 0f;
		}
		float num = 15f;
		if (angleToEnemy <= num)
		{
			WeaponInfo currentWeaponInfo = base.Bot.PlayerComponent.Equipment.CurrentWeaponInfo;
			if (currentWeaponInfo != null && currentWeaponInfo.HasOptic)
			{
				return 3f;
			}
			return 1.5f;
		}
		if (base.Enemy.RealDistance < 10f)
		{
			return 1f;
		}
		float num2 = maxVisionAngle - num;
		float num3 = angleToEnemy - num;
		float num4 = 1f - num3 / num2;
		float num5 = 0.25f;
		float num6 = 1.5f;
		return Mathf.InverseLerp(num5, num6, num4);
	}

	private float CalcGearStealthMod()
	{
		return base.Enemy.EnemyPlayerComponent.AIData.AIGearModifier.StealthModifier(base.Enemy.RealDistance);
	}

	private float GetFlare()
	{
		bool getFlare = base.EnemyPlayer.AIData.GetFlare;
		PlayerComponent enemyPlayerComponent = base.Enemy.EnemyPlayerComponent;
		bool flag = enemyPlayerComponent != null && enemyPlayerComponent.Equipment.CurrentWeaponInfo?.HasSuppressor == true;
		if (getFlare && !flag)
		{
			return 1.25f;
		}
		if (getFlare && flag)
		{
			return 1.1f;
		}
		return 1f;
	}
}
