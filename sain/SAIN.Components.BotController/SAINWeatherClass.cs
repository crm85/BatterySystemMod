using EFT.Weather;
using SAIN.Preset.GlobalSettings;
using UnityEngine;

namespace SAIN.Components.BotController;

public class SAINWeatherClass : BotManagerBase
{
	public readonly float WEATHER_VISION_UPDATE_FREQ = 5f;

	public readonly float WEATHER_RAINSOUND_UPDATE_FREQ = 1f;

	private float _rainCheckTime;

	private float _weatherCheckTime;

	public float VisionDistanceModifier { get; private set; } = 1f;

	public float GainSightModifier { get; private set; } = 1f;

	public float RainSoundModifierOutdoor { get; private set; } = 1f;

	public float RainSoundModifierIndoor { get; private set; } = 1f;

	public static SAINWeatherClass Instance { get; private set; }

	private TimeSettings _timeSettings => GlobalSettingsClass.Instance.Look.Time;

	public SAINWeatherClass(BotManagerComponent botController)
		: base(botController)
	{
		Instance = this;
	}

	public void Update(float currentTime, float deltaTime)
	{
		if (_weatherCheckTime < currentTime)
		{
			_weatherCheckTime = currentTime + WEATHER_VISION_UPDATE_FREQ;
			VisionDistanceModifier = CalcWeatherVisibility();
			GainSightModifier = 2f - VisionDistanceModifier;
		}
		if (_rainCheckTime < currentTime)
		{
			_rainCheckTime = currentTime + WEATHER_RAINSOUND_UPDATE_FREQ;
			WeatherController instance = WeatherController.Instance;
			IWeatherCurve val = ((instance != null) ? instance.WeatherCurve : null);
			if (val == null)
			{
				RainSoundModifierOutdoor = 1f;
				RainSoundModifierIndoor = 1f;
			}
			else
			{
				HearingSettings hearing = GlobalSettingsClass.Instance.Hearing;
				RainSoundModifierOutdoor = Mathf.Lerp(1f, hearing.RAIN_SOUND_COEF_OUTSIDE, val.Rain);
				RainSoundModifierIndoor = Mathf.Lerp(1f, hearing.RAIN_SOUND_COEF_INSIDE, val.Rain);
			}
		}
	}

	private float CalcWeatherVisibility()
	{
		WeatherController instance = WeatherController.Instance;
		IWeatherCurve val = ((instance != null) ? instance.WeatherCurve : null);
		if (val == null)
		{
			return 1f;
		}
		float num = 1f * (FogModifier(val.Fog) * RainModifier(val.Rain) * CloudsModifier(val.Cloudiness));
		return Mathf.Clamp(num, 0.01f, 1f);
	}

	private float FogModifier(float Fog)
	{
		float num = 0.018f;
		float num2 = Mathf.Clamp(Fog, 0f, num) / num;
		return Mathf.Lerp(1f, _timeSettings.VISION_WEATHER_FOG_MAXCOEF, num2);
	}

	private float RainModifier(float rainValue0to1)
	{
		float num = ((rainValue0to1 <= _timeSettings.VISION_WEATHER_RAIN_SRINKLE_THRESH) ? _timeSettings.VISION_WEATHER_RAIN_SRINKLE_COEF : ((rainValue0to1 < _timeSettings.VISION_WEATHER_RAIN_LIGHT_THRESH) ? _timeSettings.VISION_WEATHER_RAIN_LIGHT_COEF : ((rainValue0to1 < _timeSettings.VISION_WEATHER_RAIN_NORMAL_THRESH) ? _timeSettings.VISION_WEATHER_RAIN_NORMAL_COEF : ((!(rainValue0to1 < _timeSettings.VISION_WEATHER_RAIN_HEAVY_THRESH)) ? _timeSettings.VISION_WEATHER_RAIN_DOWNPOUR_COEF : _timeSettings.VISION_WEATHER_RAIN_HEAVY_COEF))));
		return Mathf.Lerp(1f, num, rainValue0to1);
	}

	private float CloudsModifier(float Clouds)
	{
		float num = (Clouds + 1f) / 2f;
		if (num <= _timeSettings.VISION_WEATHER_NOCLOUDS_THRESH)
		{
			return 1f;
		}
		float num2 = ((!(num <= _timeSettings.VISION_WEATHER_CLOUDY_THRESH)) ? _timeSettings.VISION_WEATHER_OVERCAST_COEF : _timeSettings.VISION_WEATHER_CLOUDY_COEF);
		return Mathf.Lerp(1f, num2, num);
	}
}
