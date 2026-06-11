using System.Collections.Generic;
using Newtonsoft.Json;
using SAIN.Attributes;
using SAIN.Preset.GlobalSettings;

namespace SAIN.Preset.Personalities;

public abstract class SettingsGroupBase<T> : ISettingsGroup
{
	protected bool initialized;

	[JsonIgnore]
	[Hidden]
	public List<ISAINSettings> SettingsList { get; } = new List<ISAINSettings>();

	public virtual void InitList()
	{
		if (!initialized)
		{
			initialized = true;
		}
	}

	public virtual void Init()
	{
		InitList();
		CreateDefaults();
		Update();
	}

	public void Update()
	{
		foreach (ISAINSettings settings in SettingsList)
		{
			settings.Update();
		}
	}

	public void CreateDefaults()
	{
		foreach (ISAINSettings settings in SettingsList)
		{
			settings.CreateDefault();
		}
	}

	public void UpdateDefaults(ISettingsGroup replacementGroup = null)
	{
		if (replacementGroup == null)
		{
			foreach (ISAINSettings settings in SettingsList)
			{
				settings.UpdateDefaults(settings);
			}
			return;
		}
		replacementGroup.InitList();
		for (int i = 0; i < SettingsList.Count; i++)
		{
			ISAINSettings iSAINSettings = SettingsList[i];
			ISAINSettings values = replacementGroup.SettingsList[i];
			iSAINSettings.UpdateDefaults(values);
		}
	}
}
