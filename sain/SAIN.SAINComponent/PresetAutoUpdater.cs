using System;
using SAIN.Plugin;
using SAIN.Preset;

namespace SAIN.SAINComponent;

public class PresetAutoUpdater
{
	private Action<SAINPresetClass> _func;

	public bool Subscribed { get; private set; }

	public void Subscribe(Action<SAINPresetClass> func)
	{
		if (func != null)
		{
			Subscribed = true;
			_func = func;
			PresetHandler.OnPresetUpdated += func;
		}
	}

	public void UnSubscribe()
	{
		if (Subscribed && _func != null)
		{
			Subscribed = false;
			PresetHandler.OnPresetUpdated -= _func;
		}
	}
}
