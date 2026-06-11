using System;
using System.Collections.Generic;
using EFT;
using SAIN.Components.PlayerComponentSpace;
using SAIN.Components.PlayerComponentSpace.PersonClasses;
using UnityEngine;

namespace SAIN.Components.BotController;

public class BotSpawnController : BotManagerBase
{
	public static BotSpawnController Instance;

	public BotDictionary BotDictionary = new BotDictionary();

	public static readonly List<WildSpawnType> StrictExclusionList = new List<WildSpawnType>
	{
		(WildSpawnType)29,
		(WildSpawnType)30,
		(WildSpawnType)38,
		(WildSpawnType)40,
		(WildSpawnType)46,
		(WildSpawnType)0,
		(WildSpawnType)60,
		(WildSpawnType)62,
		(WildSpawnType)63,
		(WildSpawnType)61,
		(WildSpawnType)64
	};

	private bool Subscribed = false;

	public bool GameEnding
	{
		get
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Invalid comparison between Unknown and I4
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Invalid comparison between Unknown and I4
			GameStatus gameStatus = GameStatus;
			return (int)gameStatus == 5 || (int)gameStatus == 0 || (int)gameStatus == 6;
		}
	}

	private GameStatus GameStatus
	{
		get
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			IBotGame val = base.BotController?.BotGame;
			if (val != null)
			{
				return val.Status;
			}
			return (GameStatus)3;
		}
	}

	public HashSet<BotOwner> VanillaBots { get; } = new HashSet<BotOwner>();

	public HashSet<BotComponent> SAINBots { get; } = new HashSet<BotComponent>();

	public event Action<BotComponent> OnBotAdded;

	public event Action<BotComponent> OnBotRemoved;

	public BotSpawnController(BotManagerComponent botController)
		: base(botController)
	{
		Instance = this;
	}

	public void Update(float currentTime, float deltaTime)
	{
		if (Subscribed && GameEnding)
		{
			UnSubscribe();
		}
	}

	public void AddBot(BotOwner botOwner)
	{
		PlayerComponent playerComponent = null;
		BotComponent botComponent = null;
		try
		{
			playerComponent = getPlayerComp(botOwner);
			checkExisting(botOwner);
			if (SAINPlugin.IsBotExluded(botOwner))
			{
				((Component)botOwner).gameObject.AddComponent<SAINNoBushESP>().Init(botOwner);
				return;
			}
			botComponent = ((Component)botOwner).gameObject.AddComponent<BotComponent>();
		}
		catch (Exception data)
		{
			Logger.LogError(data);
			return;
		}
		if ((Object)(object)botComponent == (Object)null)
		{
			Logger.LogError("Bot Component Null!");
		}
		else if ((Object)(object)playerComponent == (Object)null)
		{
			botComponent.Dispose();
			Logger.LogError("Player Component Null!");
		}
		else
		{
			botComponent.OnBotActivated += OnBotActivated;
			botComponent.ActivateIfBotActive(botOwner, playerComponent.Person);
		}
	}

	private void OnBotActivated(BotComponent bot)
	{
		bot.OnBotActivated -= OnBotActivated;
		SAINBots.Add(bot);
		BotDictionary.Add(bot.ProfileId, bot);
		bot.PlayerComponent.InitBotComponent(bot);
		bot.BotOwner.LeaveData.OnLeave += removeBot;
		bot.PlayerComponent.Person.ActivationClass.OnPersonDeadOrDespawned += removePerson;
		this.OnBotAdded?.Invoke(bot);
	}

	private void addBot(BotOwner botOwner)
	{
		PlayerComponent playerComponent = null;
		BotComponent botComponent = null;
		try
		{
			playerComponent = getPlayerComp(botOwner);
			checkExisting(botOwner);
			if (SAINPlugin.IsBotExluded(botOwner))
			{
				((Component)botOwner).gameObject.AddComponent<SAINNoBushESP>().Init(botOwner);
				VanillaBots.Add(botOwner);
				return;
			}
			botComponent = ((Component)botOwner).gameObject.AddComponent<BotComponent>();
		}
		catch (Exception data)
		{
			Logger.LogError(data);
			return;
		}
		if ((Object)(object)botComponent == (Object)null)
		{
			Logger.LogError("Bot Component Null!");
		}
		else if ((Object)(object)playerComponent == (Object)null)
		{
			botComponent.Dispose();
			Logger.LogError("Player Component Null!");
		}
		else if (botComponent.InitializeBot(playerComponent.Person))
		{
			BotDictionary.Add(botOwner.ProfileId, botComponent);
			playerComponent.InitBotComponent(botComponent);
			botOwner.LeaveData.OnLeave += removeBot;
			playerComponent.Person.ActivationClass.OnPersonDeadOrDespawned += removePerson;
			this.OnBotAdded?.Invoke(botComponent);
		}
		else
		{
			Logger.LogDebug("Failed to Init Bot [" + ((Object)botOwner).name + "]");
			botComponent.Dispose();
		}
	}

	public void Subscribe(BotSpawner botSpawner)
	{
		if (!Subscribed)
		{
			botSpawner.OnBotRemoved += removeBot;
			Subscribed = true;
		}
	}

	public void UnSubscribe()
	{
		if (Subscribed && base.BotController?.BotSpawner != null)
		{
			base.BotController.BotSpawner.OnBotRemoved -= removeBot;
			Subscribed = false;
		}
	}

	public BotComponent GetSAIN(BotOwner botOwner)
	{
		return GetSAIN((botOwner != null) ? botOwner.ProfileId : null);
	}

	public BotComponent GetSAIN(string profileId)
	{
		if (!GClass1437.IsNullOrEmpty(profileId) && BotDictionary.TryGetValue(profileId, out var value))
		{
			return value;
		}
		return null;
	}

	private PlayerComponent getPlayerComp(BotOwner botOwner)
	{
		PlayerComponent component = ((Component)botOwner).gameObject.GetComponent<PlayerComponent>();
		component.InitBotOwner(botOwner);
		return component;
	}

	private void removePerson(PersonClass person)
	{
		person.ActivationClass.OnPersonDeadOrDespawned -= removePerson;
		removeBot(person.AIInfo.BotOwner);
	}

	private void checkExisting(BotOwner botOwner)
	{
		string profileId = botOwner.ProfileId;
		if (BotDictionary.ContainsKey(profileId))
		{
			Logger.LogDebug(profileId + " was already present in Bot Dictionary. Removing...");
			BotDictionary.Remove(profileId);
		}
		GameObject gameObject = ((Component)botOwner).gameObject;
		BotComponent botComponent = default(BotComponent);
		if (gameObject.TryGetComponent<BotComponent>(ref botComponent))
		{
			Logger.LogDebug(profileId + " already had a BotComponent attached. Destroying...");
			botComponent.Dispose();
		}
		SAINNoBushESP sAINNoBushESP = default(SAINNoBushESP);
		if (gameObject.TryGetComponent<SAINNoBushESP>(ref sAINNoBushESP))
		{
			Logger.LogDebug(profileId + " already had No Bush ESP attached. Destroying...");
			Object.Destroy((Object)(object)sAINNoBushESP);
		}
	}

	private void initBotComp(BotOwner botOwner, PlayerComponent playerComponent)
	{
		BotComponent botComponent = ((Component)botOwner).gameObject.AddComponent<BotComponent>();
		if (botComponent.Init(playerComponent.Person))
		{
			BotDictionary.Add(botOwner.ProfileId, botComponent);
			playerComponent.InitBotComponent(botComponent);
			botOwner.LeaveData.OnLeave += removeBot;
			playerComponent.Person.ActivationClass.OnPersonDeadOrDespawned += removePerson;
		}
		else
		{
			botComponent?.Dispose();
		}
	}

	public void removeBot(BotOwner botOwner)
	{
		try
		{
			if ((Object)(object)botOwner != (Object)null)
			{
				if (BotDictionary.TryGetValue(botOwner.ProfileId, out var value))
				{
					this.OnBotRemoved?.Invoke(value);
					value.Dispose();
				}
				BotDictionary.Remove(botOwner.ProfileId);
				BotComponent botComponent = default(BotComponent);
				if (((Component)botOwner).TryGetComponent<BotComponent>(ref botComponent))
				{
					this.OnBotRemoved?.Invoke(value);
					botComponent.Dispose();
				}
				SAINNoBushESP sAINNoBushESP = default(SAINNoBushESP);
				if (((Component)botOwner).TryGetComponent<SAINNoBushESP>(ref sAINNoBushESP))
				{
					Object.Destroy((Object)(object)sAINNoBushESP);
				}
			}
			else
			{
				Logger.LogError("Bot is null, cannot dispose!");
			}
		}
		catch (Exception arg)
		{
			Logger.LogError($"Dispose Component Error: {arg}");
		}
	}
}
