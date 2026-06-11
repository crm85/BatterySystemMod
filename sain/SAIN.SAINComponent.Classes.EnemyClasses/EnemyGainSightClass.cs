using System.Collections.Generic;
using SAIN.Components;
using SAIN.Components.PlayerComponentSpace;
using SAIN.Preset.BotSettings.SAINSettings.Categories;
using SAIN.Preset.GlobalSettings;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.EnemyClasses;

public class EnemyGainSightClass : EnemyBase
{
	private enum SeenSpeedCheck
	{
		None,
		Vision,
		Audio
	}

	private const float UNDER_FIRE_FROM_ME_COEF = 0.4f;

	private const float DIST_SEEN_MIN_COEF = 0.01f;

	private const float DIST_SEEN_MIN_DIST = 1f;

	private const float DIST_SEEN_MAX_DIST = 25f;

	private const float DIST_HEARD_MIN_COEF = 0.2f;

	private const float DIST_HEARD_MIN_DIST = 1f;

	private const float DIST_HEARD_MAX_DIST = 20f;

	private const float TIME_MAX_DIST_CLAMP = 200f;

	private const float TIME_MAX_DIST_CLAMP_NVGS = 250f;

	private const float TIME_MIN_DIST_CLAMP = 10f;

	private const float TIME_MIN_DIST_CLAMP_NVGS = 65f;

	private const float ENEMYLIGHT_WHITELIGHT_MOD = 0.75f;

	private const float ENEMYLIGHT_LASER_MOD = 0.95f;

	private const float ENEMYLIGHT_NVGS_IR_LASER_MOD = 0.7f;

	private const float ENEMYLIGHT_NVGS_IR_LIGHT_MOD = 0.85f;

	private const float PARTS_VISIBLE_MIN_DIST = 12.5f;

	private const float PARTS_VISIBLE_MAX_PARTS = 6f;

	private const float PARTS_VISIBLE_MIN_PARTS = 2f;

	private const float PARTS_VISIBLE_MAX_TIME_SINCE_CHECKED = 2f;

	private const float PARTS_VISIBLE_MAX_TIME_SINCE_VISIBLE = 1f;

	private const float ELEVATION_LASTKNOWN_MAX_DIST = 1.5f;

	private const float ELEVATION_MIN_ANGLE = 5f;

	private const float THIRDPARTY_VISION_MAX_DIST_LASTKNOWN = 50f;

	private const float PERIPHERAL_VISION_SPEED_DIRECT_FRONT_ANGLE = 3f;

	private const float PERIPHERAL_VISION_SPEED_DIRECT_FRONT_MOD = 0.66f;

	private const float PERIPHERAL_VISION_SPEED_CLOSE_FRONT_ANGLE = 6f;

	private const float PERIPHERAL_VISION_SPEED_CLOSE_FRONT_MOD = 0.8f;

	private const float PERIPHERAL_VISION_SPEED_ENEMY_CLOSE_DIST = 10f;

	private const float PERIPHERAL_VISION_SPEED_ENEMY_CLOSE_MOD = 0.9f;

	private const float PERIPHERAL_VISION_SPEED_ENEMY_VERYCLOSE_DIST = 5f;

	private const float PERIPHERAL_VISION_SPEED_ENEMY_VERYCLOSE_MOD = 0.8f;

	private const float UNKNOWN_ENEMY_HAS_ENEMY_COEF = 1.5f;

	private float _gainSightModifier;

	private float _nextCheckVisTime;

	public float GainSightModifier
	{
		get
		{
			if (_nextCheckVisTime < Time.time)
			{
				_nextCheckVisTime = Time.time + 0.05f;
				_gainSightModifier = CalcModifier() * CalcRepeatSeenCoef();
			}
			return _gainSightModifier;
		}
	}

	private float PARTS_VISIBLE_MAX_COEF => Settings.PartsVisibility.PARTS_VISIBLE_MAX_COEF;

	private float PARTS_VISIBLE_MIN_COEF => Settings.PartsVisibility.PARTS_VISIBLE_MIN_COEF;

	private float THIRDPARTY_VISION_START_ANGLE => Settings.ThirdParty.THIRDPARTY_VISION_START_ANGLE;

	private float THIRDPARTY_VISION_MAX_COEF => Settings.ThirdParty.THIRDPARTY_VISION_MAX_COEF;

	private float PERIPHERAL_VISION_START_ANGLE => Settings.Peripheral.PERIPHERAL_VISION_START_ANGLE;

	private float PERIPHERAL_VISION_MAX_REDUCTION_COEF => Settings.Peripheral.PERIPHERAL_VISION_MAX_REDUCTION_COEF;

	private float PRONE_VISION_SPEED_COEF => Settings.Pose.PRONE_VISION_SPEED_COEF;

	private float DUCK_VISION_SPEED_COEF => Settings.Pose.DUCK_VISION_SPEED_COEF;

	private static VisionSpeedSettings Settings => GlobalSettingsClass.Instance.Look.VisionSpeed;

	private float CalcUnknownMod()
	{
		if (base.Enemy.EnemyKnown)
		{
			return 1f;
		}
		if (base.Enemy.Bot.HasEnemy)
		{
			return 1.5f;
		}
		return 1f;
	}

	private float CalcModifier()
	{
		float num = CalcPartsMod();
		float num2 = CalcGearMod();
		IAIData aIData = base.EnemyPlayer.AIData;
		int num3;
		if (aIData != null && aIData.GetFlare)
		{
			PlayerComponent enemyPlayerComponent = base.Enemy.EnemyPlayerComponent;
			num3 = ((enemyPlayerComponent != null && enemyPlayerComponent.Equipment.CurrentWeaponInfo?.HasSuppressor == false) ? 1 : 0);
		}
		else
		{
			num3 = 0;
		}
		bool flareEnabled = (byte)num3 != 0;
		float num4 = ((base.Bot.BotOwner.Memory.IsUnderFire && base.Bot.Memory.LastUnderFireEnemy == base.Enemy) ? 0.4f : 1f);
		float num5 = CalcWeatherMod(flareEnabled);
		float num6 = CalcTimeModifier(flareEnabled);
		float num7 = CalcMoveModifier();
		float num8 = CalcElevationModifier();
		float num9 = CalcThirdPartyMod();
		float num10 = CalcAngleMod();
		float num11 = PoseModifier();
		float num12 = CalcUnknownMod();
		float num13 = 1f;
		if (!base.Enemy.IsAI)
		{
			num13 = SAINNotLooking.GetVisionSpeedDecrease(base.Enemy.EnemyInfo);
		}
		return 1f * num4 * num * num2 * num5 * num6 * num7 * num8 * num9 * num10 * num13 * num12 * num11;
	}

	private float PoseModifier()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Invalid comparison between Unknown and I4
		if (!Settings.Pose.Enabled)
		{
			return 1f;
		}
		float num = 1f;
		if (base.EnemyPlayer.IsInPronePose)
		{
			num *= PRONE_VISION_SPEED_COEF;
		}
		else if ((int)base.EnemyPlayer.Pose == 1)
		{
			num *= DUCK_VISION_SPEED_COEF;
		}
		return num;
	}

	public EnemyGainSightClass(Enemy enemy)
		: base(enemy)
	{
	}

	private float CalcRepeatSeenCoef()
	{
		EnemyPlace lastSeenPlace = base.Enemy.KnownPlaces.LastSeenPlace;
		float num = 1f;
		if (lastSeenPlace != null)
		{
			num *= CalcVisionSpeedPositional(lastSeenPlace.DistanceToEnemyRealPosition, 0.01f, 1f, 25f, SeenSpeedCheck.Vision);
		}
		EnemyPlace lastHeardPlace = base.Enemy.KnownPlaces.LastHeardPlace;
		if (lastHeardPlace != null)
		{
			num *= CalcVisionSpeedPositional(lastHeardPlace.DistanceToEnemyRealPosition, 0.2f, 1f, 20f, SeenSpeedCheck.Audio);
		}
		return num;
	}

	private float CalcVisionSpeedPositional(float distance, float minSpeedCoef, float minDist, float maxDist, SeenSpeedCheck check)
	{
		if (distance <= minDist)
		{
			return minSpeedCoef;
		}
		if (distance >= maxDist)
		{
			return 1f;
		}
		float num = maxDist - minDist;
		float num2 = distance - minDist;
		float num3 = num2 / num;
		return Mathf.Lerp(minSpeedCoef, 1f, num3);
	}

	private float CalcGearMod()
	{
		return base.Enemy.EnemyPlayerComponent.AIData.AIGearModifier.StealthModifier(base.Enemy.RealDistance);
	}

	private float CalcTimeModifier(bool flareEnabled)
	{
		float num = BaseTimeModifier(flareEnabled);
		if (num <= 1f)
		{
			return 1f;
		}
		if (EnemyUsingLight(out var modifier))
		{
			return modifier;
		}
		bool usingNow = base.BotOwner.NightVision.UsingNow;
		float realDistance = base.Enemy.RealDistance;
		if (EnemyInRangeOfLight(realDistance, usingNow))
		{
			return 1f;
		}
		float num2 = 1f + num;
		float num3 = 1f;
		float num4 = (usingNow ? 250f : 200f);
		float num5 = (usingNow ? 65f : 10f);
		if (realDistance >= num4)
		{
			return num2;
		}
		if (realDistance < num5)
		{
			return num3;
		}
		float enemyVelocity = base.Enemy.Vision.EnemyVelocity;
		if (!(enemyVelocity > 0.1f))
		{
			num2 += 1f;
		}
		float num6 = num4 - num5;
		float num7 = realDistance - num5;
		float num8 = num7 / num6;
		return Mathf.Lerp(num3, num2, num8);
	}

	private bool EnemyUsingLight(out float modifier)
	{
		FlashLightClass flashlight = base.Enemy.EnemyPlayerComponent.Flashlight;
		if (flashlight.WhiteLight)
		{
			modifier = 0.75f;
			return true;
		}
		if (flashlight.Laser)
		{
			modifier = 0.95f;
			return true;
		}
		if (base.BotOwner.NightVision.UsingNow)
		{
			if (flashlight.IRLaser)
			{
				modifier = 0.7f;
				return true;
			}
			if (flashlight.IRLight)
			{
				modifier = 0.85f;
				return true;
			}
		}
		modifier = 1f;
		return false;
	}

	private bool EnemyInRangeOfLight(float enemyDist, bool usingNVGS)
	{
		SAINLookSettings look = base.Bot.Info.FileSettings.Look;
		if (base.Bot.PlayerComponent.Flashlight.WhiteLight && enemyDist <= look.VISIBLE_DISNACE_WITH_LIGHT)
		{
			return true;
		}
		if (usingNVGS && base.Bot.PlayerComponent.Flashlight.IRLight && enemyDist <= look.VISIBLE_DISNACE_WITH_IR_LIGHT)
		{
			return true;
		}
		return false;
	}

	private float CalcWeatherMod(bool flareEnabled)
	{
		float num = BaseWeatherMod(flareEnabled);
		if (num <= 1f)
		{
			return 1f;
		}
		float num2 = 1f + num;
		float num3 = 1f;
		float num4 = 200f;
		float num5 = 30f;
		float realDistance = base.Enemy.RealDistance;
		if (realDistance >= num4)
		{
			return num2;
		}
		if (realDistance < num5)
		{
			return num3;
		}
		if (EnemyUsingLight(out var _))
		{
			return num3;
		}
		if (!(base.Enemy.Vision.EnemyVelocity > 0.1f))
		{
			num2 += 1f;
		}
		float num6 = num4 - num5;
		float num7 = realDistance - num5;
		float num8 = num7 / num6;
		return Mathf.Lerp(num3, num2, num8);
	}

	private float BaseWeatherMod(bool flareEnabled)
	{
		if (flareEnabled && base.Enemy.RealDistance < 100f)
		{
			return 1f;
		}
		return BotManagerComponent.Instance.WeatherVision.GainSightModifier;
	}

	private float BaseTimeModifier(bool flareEnabled)
	{
		if (flareEnabled)
		{
			return 1f;
		}
		return BotManagerComponent.Instance.TimeVision.TimeGainSightModifier;
	}

	private float CalcPartsMod()
	{
		if (!Settings.PartsVisibility.Enabled)
		{
			return 1f;
		}
		if (base.Enemy.IsAI)
		{
			return 1f;
		}
		if (base.Enemy.RealDistance < 12.5f)
		{
			return 1f;
		}
		float pARTS_VISIBLE_MAX_COEF = PARTS_VISIBLE_MAX_COEF;
		float pARTS_VISIBLE_MIN_COEF = PARTS_VISIBLE_MIN_COEF;
		int visibleCount;
		float ratioPartsVisible = GetRatioPartsVisible(out visibleCount);
		if ((float)visibleCount <= 2f)
		{
			return pARTS_VISIBLE_MAX_COEF;
		}
		if ((float)visibleCount >= 6f)
		{
			return pARTS_VISIBLE_MIN_COEF;
		}
		if (ratioPartsVisible >= 1f)
		{
			return pARTS_VISIBLE_MIN_COEF;
		}
		return Mathf.Lerp(pARTS_VISIBLE_MAX_COEF, pARTS_VISIBLE_MIN_COEF, ratioPartsVisible);
	}

	private float GetRatioPartsVisible(out int visibleCount)
	{
		int num = 0;
		visibleCount = 0;
		Dictionary<EBodyPart, EnemyPartDataClass>.ValueCollection values = base.Enemy.Vision.VisionChecker.EnemyParts.Parts.Values;
		foreach (EnemyPartDataClass item in values)
		{
			if (!(item.TimeSinceLastVisionCheck > 2f))
			{
				num++;
				if (item.TimeSinceLastVisionSuccess < 1f)
				{
					visibleCount++;
				}
			}
		}
		return (float)visibleCount / (float)num;
	}

	private float CalcMoveModifier()
	{
		if (!Settings.Movement.Enabled)
		{
			return 1f;
		}
		VisionSpeedSettings visionSpeed = SAINPlugin.LoadedPreset.GlobalSettings.Look.VisionSpeed;
		return Mathf.Lerp(1f, Settings.Movement.MOVEMENT_VISION_MULTIPLIER, base.Enemy.Vision.EnemyVelocity);
	}

	private bool IsLastKnownAtSameElev()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		Vector3? lastKnownPosition = base.Enemy.LastKnownPosition;
		if (lastKnownPosition.HasValue)
		{
			Vector3 enemyCurrentPosition = base.EnemyCurrentPosition;
			if (Mathf.Abs(enemyCurrentPosition.y - lastKnownPosition.Value.y) < 1.5f)
			{
				return true;
			}
		}
		return false;
	}

	private float CalcElevationModifier()
	{
		if (!Settings.Elevation.Enabled)
		{
			return 1f;
		}
		if (IsLastKnownAtSameElev())
		{
			return 1f;
		}
		ElevationVisionSettings elevation = SAINPlugin.LoadedPreset.GlobalSettings.Look.VisionSpeed.Elevation;
		EnemyAnglesClass angles = base.Enemy.Vision.Angles;
		float num = 5f;
		float angleToEnemyVertical = angles.AngleToEnemyVertical;
		if (angleToEnemyVertical < num)
		{
			return 1f;
		}
		bool flag = angles.AngleToEnemyVerticalSigned > 0f;
		float num2 = (flag ? elevation.HighElevationMaxAngle : elevation.LowElevationMaxAngle);
		float num3 = (flag ? elevation.HighElevationVisionModifier : elevation.LowElevationVisionModifier);
		if (angleToEnemyVertical > num2)
		{
			return num3;
		}
		float num4 = num2 - num;
		float num5 = angleToEnemyVertical - num;
		float num6 = num5 / num4;
		return Mathf.Lerp(1f, num3, num6);
	}

	private float CalcThirdPartyMod()
	{
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		if (!Settings.ThirdParty.Enabled)
		{
			return 1f;
		}
		if (base.Enemy.IsCurrentEnemy)
		{
			return 1f;
		}
		if (base.Enemy.EnemyKnown && base.Enemy.KnownPlaces.EnemyDistanceFromLastKnown > 50f)
		{
			return 1f;
		}
		Enemy enemy = base.Enemy.Bot.Enemy;
		if (enemy == null)
		{
			return 1f;
		}
		Vector3? lastKnownPosition = enemy.LastKnownPosition;
		if (!lastKnownPosition.HasValue)
		{
			return 1f;
		}
		Vector3 val = lastKnownPosition.Value - base.Enemy.Bot.Position;
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		normalized.y = 0f;
		Vector3 enemyDirectionNormal = base.Enemy.EnemyDirectionNormal;
		enemyDirectionNormal.y = 0f;
		float num = Vector3.Angle(normalized, enemyDirectionNormal);
		float tHIRDPARTY_VISION_START_ANGLE = THIRDPARTY_VISION_START_ANGLE;
		float tHIRDPARTY_VISION_MAX_COEF = THIRDPARTY_VISION_MAX_COEF;
		if (num <= tHIRDPARTY_VISION_START_ANGLE)
		{
			return 1f;
		}
		float maxVisionAngle = base.Enemy.Vision.Angles.MaxVisionAngle;
		if (num >= maxVisionAngle)
		{
			return tHIRDPARTY_VISION_MAX_COEF;
		}
		float num2 = maxVisionAngle - tHIRDPARTY_VISION_START_ANGLE;
		float num3 = num - tHIRDPARTY_VISION_START_ANGLE;
		float num4 = num3 / num2;
		return Mathf.Lerp(1f, tHIRDPARTY_VISION_MAX_COEF, num4);
	}

	private float CalcAngleMod()
	{
		if (!Settings.Peripheral.Enabled)
		{
			return 1f;
		}
		float angleToEnemyHorizontal = base.Enemy.Vision.Angles.AngleToEnemyHorizontal;
		if (angleToEnemyHorizontal < 3f)
		{
			return 0.66f;
		}
		if (angleToEnemyHorizontal < 6f)
		{
			return 0.8f;
		}
		if (base.Enemy.RealDistance < 5f)
		{
			return 0.8f;
		}
		if (base.Enemy.RealDistance < 10f)
		{
			return 0.9f;
		}
		float pERIPHERAL_VISION_START_ANGLE = PERIPHERAL_VISION_START_ANGLE;
		if (angleToEnemyHorizontal < pERIPHERAL_VISION_START_ANGLE)
		{
			return 1f;
		}
		float maxVisionAngle = base.Enemy.Vision.Angles.MaxVisionAngle;
		float pERIPHERAL_VISION_MAX_REDUCTION_COEF = PERIPHERAL_VISION_MAX_REDUCTION_COEF;
		if (angleToEnemyHorizontal > maxVisionAngle)
		{
			return pERIPHERAL_VISION_MAX_REDUCTION_COEF;
		}
		float num = maxVisionAngle - pERIPHERAL_VISION_START_ANGLE;
		float num2 = angleToEnemyHorizontal - pERIPHERAL_VISION_START_ANGLE;
		float num3 = num2 / num;
		return Mathf.Lerp(1f, pERIPHERAL_VISION_MAX_REDUCTION_COEF, num3);
	}
}
