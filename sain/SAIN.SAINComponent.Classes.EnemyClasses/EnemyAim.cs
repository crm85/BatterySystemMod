using System.Collections.Generic;
using SAIN.Preset.GlobalSettings;
using SAIN.SAINComponent.Classes.Info;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.EnemyClasses;

public class EnemyAim : EnemyBase
{
	private const float CALC_SCATTER_FREQ = 0.025f;

	private const float CALC_SCATTER_FREQ_AI = 0.1f;

	private float _modifier;

	private float _getModTime;

	private float _visFactor;

	private float _checkVisTime;

	private float _checkVisFreq = 0.1f;

	public float AimAndScatterMultiplier
	{
		get
		{
			if (_getModTime < Time.time)
			{
				_getModTime = Time.time + (base.Enemy.IsAI ? 0.1f : 0.025f);
				_modifier = PoseFactor * VisibilityFactor * OpticFactor * InjuryFactor * VelocityFactor;
			}
			return _modifier;
		}
	}

	private float InjuryFactor => base.Bot.Info.WeaponInfo.Recoil.ArmInjuryModifier;

	private static AimSettings AimSettings => SAINPlugin.LoadedPreset.GlobalSettings.Aiming;

	private float OpticFactor
	{
		get
		{
			WeaponInfo currentWeaponInfo = base.Enemy.Bot.PlayerComponent.Equipment.CurrentWeaponInfo;
			if (currentWeaponInfo == null)
			{
				return 1f;
			}
			float realDistance = base.Enemy.RealDistance;
			if (currentWeaponInfo.HasOptic)
			{
				if (realDistance >= AimSettings.OpticFarDistance)
				{
					return AimSettings.OpticFarMulti;
				}
				if (realDistance <= AimSettings.OpticCloseDistance)
				{
					return AimSettings.OpticCloseMulti;
				}
			}
			if (currentWeaponInfo.HasRedDot)
			{
				if (realDistance <= AimSettings.RedDotCloseDistance)
				{
					return AimSettings.RedDotCloseMulti;
				}
				if (realDistance >= AimSettings.RedDotFarDistance)
				{
					return AimSettings.RedDotFarMulti;
				}
			}
			if (!currentWeaponInfo.HasRedDot && !currentWeaponInfo.HasOptic)
			{
				float ironSightScaleDistanceStart = AimSettings.IronSightScaleDistanceStart;
				if (realDistance < ironSightScaleDistanceStart)
				{
					return 1f;
				}
				float ironSightFarMulti = AimSettings.IronSightFarMulti;
				float ironSightScaleDistanceEnd = AimSettings.IronSightScaleDistanceEnd;
				if (realDistance > ironSightScaleDistanceEnd)
				{
					return ironSightFarMulti;
				}
				float num = ironSightScaleDistanceEnd - ironSightScaleDistanceStart;
				float num2 = realDistance - ironSightScaleDistanceStart;
				float num3 = 1f - num2 / num;
				return Mathf.Lerp(ironSightFarMulti, 1f, num3);
			}
			return 1f;
		}
	}

	private float PoseLevel => base.EnemyPlayer.PoseLevel;

	private float PoseFactor
	{
		get
		{
			if (base.EnemyPlayer.IsInPronePose)
			{
				return AimSettings.ScatterMulti_Prone;
			}
			float scatterMulti_PoseLevel = AimSettings.ScatterMulti_PoseLevel;
			float num = 1f;
			return Mathf.Lerp(scatterMulti_PoseLevel, num, PoseLevel);
		}
	}

	private float VisibilityFactor
	{
		get
		{
			if (_checkVisTime < Time.time)
			{
				_checkVisTime = Time.time + _checkVisFreq;
				_visFactor = CalcVisFactor();
			}
			return _visFactor;
		}
	}

	private float VelocityFactor
	{
		get
		{
			if (base.Enemy.Player.IsSprintEnabled)
			{
				return AimSettings.EnemySprintingScatterMulti;
			}
			return Mathf.Lerp(AimSettings.EnemyVelocityMaxDebuff, AimSettings.EnemyVelocityMaxBuff, 1f - base.Enemy.EnemyTransform.VelocityMagnitudeNormal);
		}
	}

	public EnemyAim(Enemy enemy)
		: base(enemy)
	{
	}

	private float CalcVisFactor()
	{
		Dictionary<EnemyPart, EnemyPartData> allActiveParts = base.Enemy.EnemyInfo.AllActiveParts;
		if (allActiveParts == null || allActiveParts.Count < 1)
		{
			return 1f;
		}
		int num = 0;
		int num2 = 0;
		foreach (KeyValuePair<EnemyPart, EnemyPartData> item in allActiveParts)
		{
			num2++;
			if (item.Value.IsVisible)
			{
				num++;
			}
		}
		num2++;
		EnemyPartData value = base.Enemy.EnemyInfo.BodyData().Value;
		if (value.IsVisible)
		{
			num++;
		}
		float num3 = (float)num / (float)num2;
		float scatterMulti_PartVis = AimSettings.ScatterMulti_PartVis;
		float num4 = 1f;
		return Mathf.Lerp(scatterMulti_PartVis, num4, num3);
	}
}
