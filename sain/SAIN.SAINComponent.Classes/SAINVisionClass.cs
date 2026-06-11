using System;
using SAIN.Components;
using SAIN.Preset.GlobalSettings;
using SAIN.SAINComponent.Classes.Sense;
using UnityEngine;

namespace SAIN.SAINComponent.Classes;

public class SAINVisionClass : BotComponentClassBase
{
	public float VISIONDISTANCE_UPDATE_FREQ = 5f;

	public float VISIONDISTANCE_UPDATE_FREQ_FLASHED = 0.5f;

	private float _nextUpdateVisibleDist;

	public float TimeLastCheckedLOS { get; set; }

	public float TimeSinceCheckedLOS => Time.time - TimeLastCheckedLOS;

	public FlashLightDazzleClass FlashLightDazzle { get; private set; }

	public SAINBotLookClass BotLook { get; private set; }

	public SAINVisionClass(BotComponent component)
		: base(component)
	{
		base.TickRequirement = ESAINTickState.OnlyNoSleep;
		FlashLightDazzle = new FlashLightDazzleClass(component);
		BotLook = new SAINBotLookClass(component);
	}

	public override void Init()
	{
		BotLook.Init();
		base.Init();
	}

	public override void ManualUpdate()
	{
		UpdateVisionDistance();
		FlashLightDazzle.CheckIfDazzleApplied(base.Bot.Enemy);
		base.ManualUpdate();
	}

	public override void Dispose()
	{
		BotLook.Dispose();
		base.Dispose();
	}

	private void UpdateVisionDistance()
	{
		if (_nextUpdateVisibleDist < Time.time)
		{
			_nextUpdateVisibleDist = Time.time + (base.BotOwner.FlashGrenade.IsFlashed ? VISIONDISTANCE_UPDATE_FREQ_FLASHED : VISIONDISTANCE_UPDATE_FREQ);
			TimeSettings time = BotBase.GlobalSettings.Look.Time;
			LookSensor lookSensor = base.BotOwner.LookSensor;
			float num = 1f;
			float num2 = 1f;
			BotManagerComponent instance = BotManagerComponent.Instance;
			if ((Object)(object)instance != (Object)null)
			{
				num = instance.TimeVision.TimeVisionDistanceModifier;
				num2 = Mathf.Clamp(instance.WeatherVision.VisionDistanceModifier, time.VISION_WEATHER_MIN_COEF, 1f);
				DateTime? dateTime = instance.TimeVision.DateTime;
				if (dateTime.HasValue)
				{
					lookSensor.HourServer = dateTime.Value.Hour;
				}
			}
			float currentVisibleDistance = base.BotOwner.Settings.Current.CurrentVisibleDistance;
			float num3 = Mathf.Clamp(currentVisibleDistance * num2, time.VISION_WEATHER_MIN_DIST_METERS, currentVisibleDistance);
			float num4 = (lookSensor.ClearVisibleDist = num3 * num);
			num4 = base.BotOwner.NightVision.UpdateVision(num4);
			num4 = base.BotOwner.BotLight.UpdateLightEnable(num4);
			lookSensor.VisibleDist = num4;
		}
		BotLight botLight = base.BotOwner.BotLight;
		if (botLight != null)
		{
			botLight.UpdateStrope();
		}
	}
}
