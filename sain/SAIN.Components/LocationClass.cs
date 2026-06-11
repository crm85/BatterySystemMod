using EFT;
using UnityEngine;

namespace SAIN.Components;

public class LocationClass : GameWorldBase, IGameWorldClass
{
	private const string WEATHER_INTERFACE = "ginterface29_0";

	private bool _weatherFound;

	private float _nextCheckWeatherTime;

	private bool _foundLocation;

	public bool WinterActive => (int)Season == 2;

	public ESeason Season { get; private set; }

	public ELocation Location { get; private set; }

	public LocationClass(GameWorldComponent component)
		: base(component)
	{
	}

	public void Init()
	{
	}

	public void ManualUpdate(float currentTime, float deltaTime)
	{
		findLocation();
		findWeather(currentTime, deltaTime);
	}

	public void Dispose()
	{
	}

	private void findWeather(float currentTime, float deltaTime)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (!_weatherFound && !(_nextCheckWeatherTime > Time.time))
		{
			_nextCheckWeatherTime = Time.time + 0.5f;
			if (Class436.Controller != null)
			{
				Season = Class436.Controller.Season;
				Logger.LogDebug($"Got Season {Season}");
				_weatherFound = true;
			}
		}
	}

	private void findLocation()
	{
		if (!_foundLocation)
		{
			Location = parseLocation();
		}
	}

	private ELocation parseLocation()
	{
		ELocation result = ELocation.None;
		GameWorld gameWorld = base.GameWorld.GameWorld;
		string text = ((gameWorld != null) ? gameWorld.LocationId : null);
		if (GClass1437.IsNullOrEmpty(text))
		{
			return result;
		}
		switch (text.ToLower())
		{
		case "bigmap":
			result = ELocation.Customs;
			break;
		case "factory4_day":
			result = ELocation.Factory;
			break;
		case "factory4_night":
			result = ELocation.FactoryNight;
			break;
		case "interchange":
			result = ELocation.Interchange;
			break;
		case "laboratory":
			result = ELocation.Labs;
			break;
		case "lighthouse":
			result = ELocation.Lighthouse;
			break;
		case "rezervbase":
			result = ELocation.Reserve;
			break;
		case "sandbox":
			result = ELocation.GroundZero;
			break;
		case "sandbox_high":
			result = ELocation.GroundZero;
			break;
		case "shoreline":
			result = ELocation.Shoreline;
			break;
		case "tarkovstreets":
			result = ELocation.Streets;
			break;
		case "woods":
			result = ELocation.Streets;
			break;
		case "terminal":
			result = ELocation.Terminal;
			break;
		case "town":
			result = ELocation.Town;
			break;
		default:
			Logger.LogError(text ?? "");
			result = ELocation.None;
			break;
		}
		_foundLocation = result != ELocation.None;
		return result;
	}
}
