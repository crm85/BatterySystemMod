using System;
using System.Collections.Generic;
using EFT;
using SAIN.Components;
using SAIN.Components.PlayerComponentSpace;
using SAIN.Components.PlayerComponentSpace.Classes;
using SAIN.Helpers;
using SAIN.Preset;
using SAIN.Preset.GlobalSettings;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.SAINComponent.Classes;

public class HearingAnalysisClass : BotSubClass<SAINHearingSensorClass>, IBotClass, IDisposable
{
	private static int _lastCalcFrame;

	private static float _farDistance;

	private static float _veryFarDistance;

	private static float _narniaDistance;

	private static HearingSettings _settings => GlobalSettingsClass.Instance.Hearing;

	public HearingAnalysisClass(SAINHearingSensorClass hearing)
		: base(hearing)
	{
	}

	public bool CheckIfSoundHeard(AISoundData sound)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (ShallLimitAI(sound))
		{
			return false;
		}
		if (!sound.IsGunShot && !DoIDetectFootsteps(sound))
		{
			return false;
		}
		Vector3 val = sound.Sound.Position - sound.Sound.PlayerComponent.Position;
		if (((Vector3)(ref val)).sqrMagnitude > 25f)
		{
			return false;
		}
		float num = CalcEnvironmentMod(sound);
		float num2 = CalcConditionMod(sound.SoundType);
		float num3 = CalcOcclusionMod(sound.Enemy, sound.SoundType);
		float num4 = Mathf.Clamp(1f * num * num2 * num3 * base.Bot.Info.Difficulty.HearingDistanceModifier, _settings.HEAR_MODIFIER_MIN_CLAMP, _settings.HEAR_MODIFIER_MAX_CLAMP);
		float num5 = sound.Sound.Range * sound.Sound.Volume * num4;
		if (sound.PlayerDistance > num5)
		{
			return false;
		}
		if (!sound.Enemy.Player.IsAI)
		{
		}
		return true;
	}

	private float CalcBunkerVolumeReduction(AISoundData sound)
	{
		PlayerLocationClass playerLocation = base.Bot.PlayerComponent.AIData.PlayerLocation;
		PlayerLocationClass playerLocation2 = sound.HeardPlayerComponent.AIData.PlayerLocation;
		bool inBunker = playerLocation.InBunker;
		bool inBunker2 = playerLocation2.InBunker;
		if (inBunker != inBunker2)
		{
			return _settings.BUNKER_REDUCTION_COEF;
		}
		if (inBunker)
		{
			float num = Mathf.Abs(playerLocation.BunkerDepth - playerLocation2.BunkerDepth);
			if (num > 0f)
			{
				return _settings.BUNKER_ELEV_DIFF_COEF;
			}
		}
		return 1f;
	}

	private bool DoIDetectFootsteps(AISoundData sound)
	{
		bool hasEarPiece = base.Bot.PlayerComponent.Equipment.GearInfo.HasEarPiece;
		float num = (hasEarPiece ? _settings.HEAR_CHANCE_MIN_DIST_HEADPHONES : _settings.HEAR_CHANCE_MIN_DIST);
		float playerDistance = sound.PlayerDistance;
		if (playerDistance <= num)
		{
			return true;
		}
		float num2 = (hasEarPiece ? SAINPlugin.LoadedPreset.GlobalSettings.Hearing.MaxFootstepAudioDistance : SAINPlugin.LoadedPreset.GlobalSettings.Hearing.MaxFootstepAudioDistanceNoHeadphones);
		if (playerDistance > num2)
		{
			return false;
		}
		float num3 = 0f;
		if (hasEarPiece)
		{
			num3 = ((!(playerDistance < num2 * _settings.HEAR_CHANCE_MIDRANGE_COEF)) ? (num3 + _settings.HEAR_CHANCE_LONGRANGE_MINCHANCE_HEADPHONES) : (num3 + _settings.HEAR_CHANCE_MIDRANGE_MINCHANCE_HEADPHONES));
			if (sound.SoundType != SAINSoundType.FootStep)
			{
				num3 += _settings.HEAR_CHANCE_HEADPHONES_OTHERSOUNDS;
			}
		}
		if (base.Bot.PlayerComponent.Transform.VelocityMagnitudeNormal < _settings.HEAR_CHANCE_NOTMOVING_VELOCITY)
		{
			num3 += (hasEarPiece ? _settings.HEAR_CHANCE_NOTMOVING_MINCHANCE_HEADPHONES : _settings.HEAR_CHANCE_NOTMOVING_MINCHANCE);
		}
		if (base.Bot.HasEnemy && base.Bot.Enemy.EnemyProfileId == sound.Sound.PlayerComponent.ProfileId)
		{
			num3 += (hasEarPiece ? _settings.HEAR_CHANCE_CURRENTENEMY_MINCHANCE_HEADPHONES : _settings.HEAR_CHANCE_CURRENTENEMY_MINCHANCE);
		}
		float num4 = num2 - num;
		float num5 = playerDistance - num;
		float num6 = 1f - num5 / num4;
		num6 *= 100f;
		num6 = Mathf.Clamp(num6, num3, 100f);
		if (!sound.Enemy.Player.IsAI)
		{
		}
		return EFTMath.RandomBool(num6);
	}

	private static float CalcOcclusionMod(Enemy Enemy, SAINSoundType SoundType)
	{
		if (Enemy.InLineOfSight)
		{
			return 1f;
		}
		switch (SoundType)
		{
		case SAINSoundType.Shot:
			return _settings.GUNSHOT_OCCLUSION_MOD;
		case SAINSoundType.SuppressedShot:
			return _settings.GUNSHOT_OCCLUSION_MOD_SUPP;
		case SAINSoundType.Sprint:
			return _settings.FOOTSTEP_OCCLUSION_MOD_SPRINT;
		case SAINSoundType.FootStep:
		{
			Player enemyPlayer = Enemy.EnemyPlayer;
			if (enemyPlayer != null && enemyPlayer.IsSprintEnabled)
			{
				return _settings.FOOTSTEP_OCCLUSION_MOD_SPRINT;
			}
			return _settings.FOOTSTEP_OCCLUSION_MOD;
		}
		default:
			return _settings.OTHER_OCCLUSION_MOD;
		}
	}

	private float CalcEnvironmentMod(AISoundData sound)
	{
		if (base.Player.AIData.EnvironmentId == sound.EnvironmentId)
		{
			return 1f;
		}
		float num = (sound.IsGunShot ? _settings.GUNSHOT_ENVIR_MOD : _settings.FOOTSTEP_ENVIR_MOD);
		float num2 = CalcBunkerVolumeReduction(sound);
		float num3 = num * num2;
		return Mathf.Clamp(num3, _settings.MIN_ENVIRONMENT_MOD, 1f);
	}

	private bool ShallLimitAI(AISoundData sound)
	{
		if (!sound.Enemy.IsAI)
		{
			return false;
		}
		AILimitSettings aILimit = GlobalSettingsClass.Instance.General.AILimit;
		if (!aILimit.LimitAIvsAIGlobal)
		{
			return false;
		}
		if (!aILimit.LimitAIvsAIHearing)
		{
			return false;
		}
		PlayerComponent playerComponent = sound.Sound.PlayerComponent;
		if (base.Bot.Enemy?.EnemyProfileId == playerComponent.ProfileId)
		{
			return false;
		}
		BotComponent botComponent = playerComponent.BotComponent;
		float maxRange;
		if ((Object)(object)botComponent == (Object)null)
		{
			BotOwner botOwner = playerComponent.BotOwner;
			object obj;
			if (botOwner == null)
			{
				obj = null;
			}
			else
			{
				EnemyInfo goalEnemy = botOwner.Memory.GoalEnemy;
				obj = ((goalEnemy != null) ? goalEnemy.ProfileId : null);
			}
			if ((string)obj == base.Bot.ProfileId)
			{
				return false;
			}
			maxRange = GetMaxRange(base.Bot.CurrentAILimit);
		}
		else
		{
			if (botComponent.Enemy?.EnemyProfileId == base.Bot.ProfileId)
			{
				return false;
			}
			maxRange = GetMaxRange(botComponent.CurrentAILimit);
		}
		if (sound.PlayerDistance <= maxRange)
		{
			return false;
		}
		return true;
	}

	private static float GetMaxRange(AILimitSetting aiLimit)
	{
		return aiLimit switch
		{
			AILimitSetting.Far => _farDistance, 
			AILimitSetting.VeryFar => _veryFarDistance, 
			AILimitSetting.Narnia => _narniaDistance, 
			_ => float.MaxValue, 
		};
	}

	private float CalcConditionMod(SAINSoundType SoundType)
	{
		float num = 1f;
		BotOwner botOwner = base.BotOwner;
		float? obj;
		if (botOwner == null)
		{
			obj = null;
		}
		else
		{
			BotDifficultySettingsClass settings = botOwner.Settings;
			if (settings == null)
			{
				obj = null;
			}
			else
			{
				GClass596 current = settings.Current;
				obj = ((current != null) ? new float?(current.CurrentHearingSense) : ((float?)null));
			}
		}
		float? num2 = obj;
		if (num2.HasValue)
		{
			num *= num2.Value;
		}
		num *= base.Bot.Info.FileSettings.Core.HearingDistanceMulti;
		if (SoundType != SAINSoundType.Shot)
		{
			if (!base.Bot.PlayerComponent.Equipment.GearInfo.HasEarPiece)
			{
				num *= _settings.HEAR_MODIFIER_NO_EARS;
			}
			if (base.Bot.PlayerComponent.Equipment.GearInfo.HasHeavyHelmet)
			{
				num *= _settings.HEAR_MODIFIER_HEAVY_HELMET;
			}
			if (base.Bot.Memory.Health.Dying && !base.Bot.Memory.Health.OnPainKillers)
			{
				num *= _settings.HEAR_MODIFIER_DYING;
			}
			if (base.Player.IsSprintEnabled)
			{
				num *= _settings.HEAR_MODIFIER_SPRINT;
			}
			if (base.Player.HeavyBreath)
			{
				num *= _settings.HEAR_MODIFIER_HEAVYBREATH;
			}
		}
		return num;
	}

	private void updateSettings(SAINPresetClass preset)
	{
		int frameCount = Time.frameCount;
		if (_lastCalcFrame != frameCount)
		{
			_lastCalcFrame = frameCount;
			Dictionary<AILimitSetting, float> maxHearingRanges = preset.GlobalSettings.General.AILimit.MaxHearingRanges;
			_farDistance = maxHearingRanges[AILimitSetting.Far];
			_veryFarDistance = maxHearingRanges[AILimitSetting.VeryFar];
			_narniaDistance = maxHearingRanges[AILimitSetting.Narnia];
		}
	}
}
