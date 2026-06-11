using System;
using SAIN.Components;
using SAIN.Helpers;
using SAIN.Preset;
using SAIN.Preset.GlobalSettings;
using UnityEngine;

namespace SAIN.SAINComponent.Classes;

public class SAINAILimit : BotComponentClassBase
{
	private float _checkDistanceTime;

	private static float _frequency = 3f;

	private static float _farDistance = 40000f;

	private static float _veryFarDistance = 90000f;

	private static float _narniaDistance = 160000f;

	public AILimitSetting CurrentAILimit { get; private set; }

	public float ClosestPlayerDistanceSqr { get; private set; }

	public event Action<AILimitSetting> OnAILimitChanged;

	public SAINAILimit(BotComponent sain)
		: base(sain)
	{
		base.TickRequirement = ESAINTickState.OnlyBotActive;
	}

	public override void ManualUpdate()
	{
		checkAILimit();
		base.ManualUpdate();
	}

	private void checkAILimit()
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		AILimitSetting currentAILimit = CurrentAILimit;
		if (base.Bot.EnemyController.ActiveHumanEnemy)
		{
			CurrentAILimit = AILimitSetting.None;
			ClosestPlayerDistanceSqr = -1f;
		}
		else if (_checkDistanceTime < Time.time)
		{
			_checkDistanceTime = Time.time + _frequency * Random.Range(0.9f, 1.1f);
			GameWorldComponent instance = GameWorldComponent.Instance;
			if ((Object)(object)instance != (Object)null && (Object)(object)instance.PlayerTracker.FindClosestHumanPlayer(out var closestPlayerSqrMag, base.Bot.Position) != (Object)null)
			{
				CurrentAILimit = checkDistances(closestPlayerSqrMag);
				ClosestPlayerDistanceSqr = closestPlayerSqrMag;
			}
		}
		if (currentAILimit != CurrentAILimit)
		{
			this.OnAILimitChanged?.Invoke(CurrentAILimit);
		}
	}

	private AILimitSetting checkDistances(float closestPlayerSqrMag)
	{
		if (closestPlayerSqrMag < _farDistance)
		{
			return AILimitSetting.None;
		}
		if (closestPlayerSqrMag < _veryFarDistance)
		{
			return AILimitSetting.Far;
		}
		if (closestPlayerSqrMag < _narniaDistance)
		{
			return AILimitSetting.VeryFar;
		}
		return AILimitSetting.Narnia;
	}

	protected override void UpdatePresetSettings(SAINPresetClass preset)
	{
		AILimitSettings aILimit = GlobalSettingsClass.Instance.General.AILimit;
		_frequency = aILimit.AILimitUpdateFrequency;
		_farDistance = aILimit.AILimitRanges[AILimitSetting.Far].Sqr();
		_veryFarDistance = aILimit.AILimitRanges[AILimitSetting.VeryFar].Sqr();
		_narniaDistance = aILimit.AILimitRanges[AILimitSetting.Narnia].Sqr();
		if (SAINPlugin.DebugMode)
		{
			Logger.LogDebug($"Updated AI Limit Settings: [{_farDistance.Sqrt()}, {_veryFarDistance.Sqrt()}, {_narniaDistance.Sqrt()}]");
		}
	}
}
