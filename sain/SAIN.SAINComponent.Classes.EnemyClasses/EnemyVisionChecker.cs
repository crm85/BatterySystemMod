using System;
using EFT;
using SAIN.Components;
using SAIN.Components.PlayerComponentSpace.PersonClasses;
using SAIN.Preset;
using SAIN.Preset.GlobalSettings;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.EnemyClasses;

public class EnemyVisionChecker : EnemyBase, IBotClass, IDisposable
{
	private const float MAX_RANGE_VISION_UNKNOWN = 300f;

	private bool _visionStarted;

	private float _startVisionTime;

	private PersonTransformClass _transform;

	private static float _farDistance;

	private static float _veryFarDistance;

	private static float _narniaDistance;

	public float LastCheckLookTime { get; set; }

	public float LastCheckLOSTime { get; set; }

	public bool LineOfSight => EnemyParts.LineOfSight;

	public EnemyPartsClass EnemyParts { get; }

	public EnemyVisionChecker(Enemy enemy)
		: base(enemy)
	{
		EnemyParts = new EnemyPartsClass(enemy);
		_transform = enemy.Bot.Transform;
		_startVisionTime = Time.time + Random.Range(0f, 0.33f);
	}

	public override void ManualUpdate()
	{
		EnemyParts.Update();
		base.Enemy.Events.OnEnemyLineOfSightChanged.CheckToggle(LineOfSight);
		base.ManualUpdate();
	}

	private bool ShallStart()
	{
		if (_visionStarted)
		{
			return true;
		}
		if (_startVisionTime < Time.time)
		{
			_visionStarted = true;
			return true;
		}
		return false;
	}

	public float AIVisionRangeLimit()
	{
		float num = CheckMaxVisionRangeAI();
		if (!base.Enemy.EnemyKnown && num > 300f)
		{
			return 300f;
		}
		return num;
	}

	private float CheckMaxVisionRangeAI()
	{
		if (!base.Enemy.IsAI)
		{
			return float.MaxValue;
		}
		AILimitSettings aILimit = GlobalSettingsClass.Instance.General.AILimit;
		if (!aILimit.LimitAIvsAIGlobal)
		{
			return float.MaxValue;
		}
		if (!aILimit.LimitAIvsAIVision)
		{
			return float.MaxValue;
		}
		BotComponent botComponent = base.Enemy.EnemyPerson.AIInfo.BotComponent;
		if ((Object)(object)botComponent == (Object)null)
		{
			BotOwner botOwner = base.Enemy.EnemyPerson.AIInfo.BotOwner;
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
				return float.MaxValue;
			}
			return GetMaxVisionRange(base.Bot.CurrentAILimit);
		}
		if (botComponent.Enemy?.EnemyProfileId == base.Bot.ProfileId)
		{
			return float.MaxValue;
		}
		return GetMaxVisionRange(botComponent.CurrentAILimit);
	}

	private static float GetMaxVisionRange(AILimitSetting aiLimit)
	{
		return aiLimit switch
		{
			AILimitSetting.Far => _farDistance, 
			AILimitSetting.VeryFar => _veryFarDistance, 
			AILimitSetting.Narnia => _narniaDistance, 
			_ => float.MaxValue, 
		};
	}

	protected override void UpdatePresetSettings(SAINPresetClass preset)
	{
		AILimitSettings aILimit = preset.GlobalSettings.General.AILimit;
		_farDistance = aILimit.MaxVisionRanges[AILimitSetting.Far];
		_veryFarDistance = aILimit.MaxVisionRanges[AILimitSetting.VeryFar];
		_narniaDistance = aILimit.MaxVisionRanges[AILimitSetting.Narnia];
	}
}
