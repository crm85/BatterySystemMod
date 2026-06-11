using System;
using EFT;
using SAIN.Components;
using SAIN.Components.PlayerComponentSpace;
using SAIN.Helpers;
using SAIN.Preset.Personalities;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.SAINComponent.Classes;

public class SAINHearingSensorClass : BotComponentClassBase
{
	public HearingInputClass SoundInput { get; }

	public HearingAnalysisClass Analysis { get; }

	public HearingDispersionClass Dispersion { get; }

	public event Action<AISoundData, Enemy> OnEnemySoundHeard;

	public SAINHearingSensorClass(BotComponent sain)
		: base(sain)
	{
		base.TickRequirement = ESAINTickState.OnlyNoSleep;
		SoundInput = new HearingInputClass(this);
		Analysis = new HearingAnalysisClass(this);
		Dispersion = new HearingDispersionClass(this);
	}

	public override void Init()
	{
		SoundInput.Init();
		Analysis.Init();
		Dispersion.Init();
		base.Init();
	}

	public override void ManualUpdate()
	{
		SoundInput.ManualUpdate();
		Analysis.ManualUpdate();
		Dispersion.ManualUpdate();
		base.ManualUpdate();
	}

	public override void Dispose()
	{
		SoundInput.Dispose();
		Analysis.Dispose();
		Dispersion.Dispose();
		base.Dispose();
	}

	public void ReactToBulletFlyBy(AISoundData sound, float FlyByDistance)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		bool flag = FlyByDistance <= SAINPlugin.LoadedPreset.GlobalSettings.Mind.MaxUnderFireDistance;
		if (!SoundInput.IgnoreHearing || flag)
		{
			Vector3 estimatedPosition = Dispersion.CalcRandomizedPosition(sound, 1f);
			ReactToBulletFlyBy(sound, FlyByDistance, estimatedPosition, flag);
			this.OnEnemySoundHeard?.Invoke(sound, sound.Enemy);
		}
	}

	public void ReactToHeardSound(AISoundData sound)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (Analysis.CheckIfSoundHeard(sound) && (!sound.IsGunShot || ShallChaseGunshot(sound.PlayerDistance)))
		{
			Vector3 estimatedPosition = Dispersion.CalcRandomizedPosition(sound, 1f);
			base.Bot.Squad.SquadInfo?.AddPointToSearch(sound.Enemy, estimatedPosition, sound, base.Bot);
			CheckCalcGoal();
			this.OnEnemySoundHeard?.Invoke(sound, sound.Enemy);
		}
	}

	private bool ShallChaseGunshot(float Distance)
	{
		PersonalitySearchSettings search = base.Bot.Info.PersonalitySettings.Search;
		if (search.WillChaseDistantGunshots)
		{
			return true;
		}
		if (Distance > search.AudioStraightDistanceToIgnore)
		{
			return false;
		}
		return true;
	}

	private void ReactToBulletFlyBy(AISoundData sound, float ProjectionPointDistance, Vector3 EstimatedPosition, bool UnderFire)
	{
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		Enemy enemy = sound.Enemy;
		if (UnderFire)
		{
			BotOwner botOwner = base.BotOwner;
			if (botOwner != null)
			{
				BotHearingSensor hearingSensor = botOwner.HearingSensor;
				if (hearingSensor != null)
				{
					GDelegate7 onEnemySounHearded = hearingSensor.OnEnemySounHearded;
					if (onEnemySounHearded != null)
					{
						onEnemySounHearded.Invoke(EstimatedPosition, sound.PlayerDistance, sound.SoundType.Convert());
					}
				}
			}
			base.Bot.Memory.SetUnderFire(enemy, EstimatedPosition);
			enemy.SetEnemyAsSniper(enemy.RealDistance > BotBase.GlobalSettings.Mind.ENEMYSNIPER_DISTANCE);
		}
		base.Bot.Suppression.CheckAddSuppression(enemy, ProjectionPointDistance);
		enemy.Status.ShotAtMeRecently = true;
		base.Bot.Squad.SquadInfo?.AddPointToSearch(enemy, EstimatedPosition, sound, base.Bot);
		CheckCalcGoal();
	}

	private void CheckCalcGoal()
	{
		if (base.BotOwner.Memory.GoalEnemy != null)
		{
			Enemy enemy = base.Bot.Enemy;
			if (enemy != null && enemy.IsVisible)
			{
				return;
			}
		}
		try
		{
			base.BotOwner.BotsGroup.CalcGoalForBot(base.BotOwner);
		}
		catch
		{
		}
	}
}
