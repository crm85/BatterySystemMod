using System.Collections.Generic;
using Newtonsoft.Json;
using SAIN.Attributes;
using SAIN.Components;
using SAIN.Helpers;
using UnityEngine;

namespace SAIN.Preset.GlobalSettings;

public class LocationSettingsClass : SAINSettingsBase<LocationSettingsClass>, ISAINSettings
{
	[Name("Location Specific Modifiers")]
	[Description("These modifiers only apply to bots on the location they are assigned to. Applies to all bots equally.")]
	[MinMax(0.01f, 5f, 100f)]
	public Dictionary<ELocation, DifficultySettings> LocationSettings = new Dictionary<ELocation, DifficultySettings>();

	[JsonConstructor]
	public LocationSettingsClass()
	{
		addNewLocations();
	}

	private void addNewLocations()
	{
		ELocation[] array = EnumValues.GetEnum<ELocation>();
		foreach (ELocation eLocation in array)
		{
			if (!LocationSettings.ContainsKey(eLocation) && eLocation != ELocation.None && eLocation != ELocation.Terminal && eLocation != ELocation.Town)
			{
				LocationSettings.Add(eLocation, new DifficultySettings());
			}
		}
	}

	public DifficultySettings Current()
	{
		GameWorldComponent instance = GameWorldComponent.Instance;
		if ((Object)(object)instance == (Object)null || instance.Location == null)
		{
			Logger.LogError("gameworld or location class null");
			return null;
		}
		if (LocationSettings.TryGetValue(instance.Location.Location, out var value))
		{
			return value;
		}
		Logger.LogError($"no settings for {instance.Location.Location}");
		return null;
	}

	public override void Init(List<ISAINSettings> list)
	{
		list.Add(this);
	}
}
