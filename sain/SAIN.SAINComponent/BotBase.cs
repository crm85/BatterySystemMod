using System;
using EFT;
using SAIN.Components;
using SAIN.Components.PlayerComponentSpace;
using SAIN.Components.PlayerComponentSpace.PersonClasses;
using SAIN.Plugin;
using SAIN.Preset;
using SAIN.Preset.GlobalSettings;
using UnityEngine;

namespace SAIN.SAINComponent;

public abstract class BotBase(BotComponent bot) : IBotClass, IDisposable
{
	public BotComponent Bot { get; } = bot;

	public PersonClass Person => Bot.Person;

	public PlayerComponent PlayerComponent => Bot.PlayerComponent;

	public BotOwner BotOwner => Bot.BotOwner;

	public Player Player => Bot.Player;

	protected static GlobalSettingsClass GlobalSettings => GlobalSettingsClass.Instance;

	public ESAINTickState TickRequirement { get; protected set; } = ESAINTickState.AlwaysUpdate;

	public bool CanEverTick { get; protected set; } = true;

	public float TickInterval { get; protected set; }

	public float LastTickTime { get; protected set; }

	public virtual void Init()
	{
		PresetHandler.OnPresetUpdated += UpdatePresetSettings;
	}

	public virtual bool ShallTick(float CurrentTime)
	{
		if (CanEverTick && LastTickTime + TickInterval < CurrentTime)
		{
			LastTickTime = Time.time;
			return true;
		}
		return false;
	}

	public virtual void ManualUpdate()
	{
	}

	protected virtual void UpdatePresetSettings(SAINPresetClass preset)
	{
	}

	public virtual void Dispose()
	{
		PresetHandler.OnPresetUpdated -= UpdatePresetSettings;
	}
}
