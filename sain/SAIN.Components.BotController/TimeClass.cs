using System;
using EFT;
using SAIN.Helpers;
using SAIN.Models.Enums;
using SAIN.Preset.GlobalSettings;
using UnityEngine;

namespace SAIN.Components.BotController;

public class TimeClass : BotManagerBase
{
	public float TIME_CALC_FREQ = 5f;

	private float _visUpdateTime = 0f;

	public float VisibilityRatio { get; private set; }

	public DateTime? DateTime { get; private set; }

	public float TimeVisionDistanceModifier { get; private set; } = 1f;

	public float TimeGainSightModifier { get; private set; } = 1f;

	public ETimeOfDay TimeOfDay { get; private set; }

	public event Action<TimeClass> OnTimeUpdated;

	public TimeClass(BotManagerComponent botController)
		: base(botController)
	{
	}

	public void Update(float currentTime, float deltaTime)
	{
		if (!(_visUpdateTime < currentTime))
		{
			return;
		}
		GameWorld gameWorld = base.GameWorld;
		if (!((Object)(object)gameWorld == (Object)null))
		{
			GameDateTime gameDateTime = gameWorld.GameDateTime;
			if (gameDateTime != null)
			{
				_visUpdateTime = currentTime + TIME_CALC_FREQ;
				DateTime = gameDateTime.Calculate();
				float time = calcTime(DateTime.Value);
				TimeOfDay = getTimeEnum(time);
				TimeVisionDistanceModifier = getModifier(time, TimeOfDay, out var visibilityRatio);
				VisibilityRatio = visibilityRatio;
				TimeGainSightModifier = Mathf.Lerp(1f, GlobalSettingsClass.Instance.Look.Time.TIME_GAIN_SIGHT_SCALE_MAX, 1f - visibilityRatio);
				this.OnTimeUpdated?.Invoke(this);
			}
		}
	}

	private static float calcTime(DateTime dateTime)
	{
		return ((float)dateTime.Hour + (float)dateTime.Minute / 59f).Round100();
	}

	private static float getModifier(float time, ETimeOfDay timeOfDay, out float visibilityRatio)
	{
		TimeSettings time2 = SAINPlugin.LoadedPreset.GlobalSettings.Look.Time;
		float num = 1f;
		float num2 = (GameWorldComponent.Instance.Location.WinterActive ? time2.NightTimeVisionModifierSnow : time2.NightTimeVisionModifier);
		switch (timeOfDay)
		{
		default:
			visibilityRatio = 1f;
			return num;
		case ETimeOfDay.Night:
			visibilityRatio = 0f;
			return num2;
		case ETimeOfDay.Dawn:
		{
			float num3 = time2.HourDawnEnd - time2.HourDawnStart;
			float num4 = time - time2.HourDawnStart;
			visibilityRatio = num4 / num3;
			break;
		}
		case ETimeOfDay.Dusk:
		{
			float num3 = time2.HourDuskEnd - time2.HourDuskStart;
			float num4 = time - time2.HourDuskStart;
			visibilityRatio = 1f - num4 / num3;
			break;
		}
		}
		return Mathf.Lerp(num2, num, visibilityRatio);
	}

	private static ETimeOfDay getTimeEnum(float time)
	{
		TimeSettings time2 = SAINPlugin.LoadedPreset.GlobalSettings.Look.Time;
		if (time <= time2.HourDuskStart && time >= time2.HourDawnEnd)
		{
			return ETimeOfDay.Day;
		}
		if (time >= time2.HourDuskEnd || time <= time2.HourDawnStart)
		{
			return ETimeOfDay.Night;
		}
		if (time < time2.HourDawnEnd)
		{
			return ETimeOfDay.Dawn;
		}
		return ETimeOfDay.Dusk;
	}
}
