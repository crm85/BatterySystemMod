using EFT;
using SAIN.Components;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.WeaponFunction;

public class BotLightController : BotComponentClassBase
{
	private float _nextLightChangeTime;

	private float _changelightFreq = 1f;

	private bool wantLightOn;

	private float _timeWithinDistanceSearch;

	private float _nextRandomTime;

	private float _randomFreq = 2f;

	private float _randomTime;

	private float _minRandom = 1.5f;

	private float _maxRandom = 6f;

	public bool IsLightEnabled
	{
		get
		{
			BotOwner botOwner = base.BotOwner;
			int result;
			if (botOwner == null)
			{
				result = 0;
			}
			else
			{
				BotLight botLight = botOwner.BotLight;
				result = ((((botLight != null) ? new bool?(botLight.IsEnable) : ((bool?)null)) == true) ? 1 : 0);
			}
			return (byte)result != 0;
		}
	}

	private float randomizedTurnOffTime
	{
		get
		{
			if (_nextRandomTime < Time.time)
			{
				_nextRandomTime = Time.time + _randomFreq * Random.Range(0.66f, 1.33f);
				_randomTime = Random.Range(_minRandom, _maxRandom);
			}
			return _randomTime;
		}
	}

	public BotLightController(BotComponent sain)
		: base(sain)
	{
		base.TickRequirement = ESAINTickState.OnlyNoSleep;
	}

	public override void ManualUpdate()
	{
		base.ManualUpdate();
		BotOwner botOwner = base.BotOwner;
		if (((botOwner != null) ? botOwner.BotLight : null) != null)
		{
			updateLightToggle();
		}
	}

	private void updateLightToggle()
	{
		if (base.Bot.SAINLayersActive && IsLightEnabled != wantLightOn && _nextLightChangeTime < Time.time)
		{
			_nextLightChangeTime = Time.time + _changelightFreq * Random.Range(0.66f, 1.33f);
			setLight(wantLightOn);
		}
	}

	private void setLight(bool value)
	{
		if (value)
		{
			base.BotOwner.BotLight.TurnOn(true);
		}
		else
		{
			base.BotOwner.BotLight.TurnOff(false, true);
		}
	}

	public void ToggleLight(bool value)
	{
		wantLightOn = value;
	}

	public void ToggleLaser(bool value)
	{
	}

	public void HandleLightForSearch(float distanceToCurrentCornerSqr)
	{
		if (distanceToCurrentCornerSqr < 900f)
		{
			_timeWithinDistanceSearch = Time.time;
			ToggleLight(value: true);
		}
		else if (_timeWithinDistanceSearch + 0.66f < Time.time)
		{
			ToggleLight(value: false);
		}
	}

	public void HandleLightForEnemy(Enemy enemy)
	{
		if (base.Bot.Decision.CurrentCombatDecision == ECombatDecision.Search || base.BotOwner.ShootData.Shooting || enemy == null)
		{
			return;
		}
		if (!enemy.Seen)
		{
			ToggleLight(value: false);
			return;
		}
		float num = 50f;
		ECombatDecision currentCombatDecision = base.Bot.Decision.CurrentCombatDecision;
		if (enemy.EnemyNotLooking && enemy.RealDistance <= num * 0.9f)
		{
			ToggleLight(value: true);
		}
		else if (enemy.IsVisible && Time.time - enemy.Vision.VisibleStartTime > 0.75f)
		{
			if (enemy.RealDistance <= num * 0.9f)
			{
				ToggleLight(value: true);
			}
			else if (enemy.RealDistance > num)
			{
				ToggleLight(value: false);
			}
		}
		else if (enemy.Seen)
		{
			BotLight botLight = base.BotOwner.BotLight;
			if (botLight != null && botLight.IsEnable && enemy.TimeSinceSeen > randomizedTurnOffTime)
			{
				ToggleLight(value: false);
			}
		}
	}
}
