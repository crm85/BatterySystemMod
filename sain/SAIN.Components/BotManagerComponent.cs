using System;
using System.Collections.Generic;
using Comfort.Common;
using EFT;
using EFT.EnvironmentEffect;
using SAIN.BotController.Classes;
using SAIN.Components.BotController;
using SAIN.Components.BotControllerSpace.Classes;
using UnityEngine;

namespace SAIN.Components;

public class BotManagerComponent : MonoBehaviour
{
	private BotEventHandler _eventHandler;

	private BotSpawner _spawner;

	private readonly List<int> IndexToRemove = new List<int>();

	public List<string> Groups = new List<string>();

	public static BotManagerComponent Instance { get; private set; }

	public BotDictionary Bots => BotSpawnController.Bots;

	public GameWorld GameWorld => SAINGameWorld.GameWorld;

	public IBotGame BotGame => Singleton<IBotGame>.Instance;

	public BotEventHandler BotEventHandler
	{
		get
		{
			if (_eventHandler == null)
			{
				_eventHandler = Singleton<BotEventHandler>.Instance;
				if (_eventHandler != null)
				{
					GrenadeController.Subscribe(_eventHandler);
				}
			}
			return _eventHandler;
		}
	}

	public GameWorldComponent SAINGameWorld { get; private set; }

	public BotsController DefaultController { get; set; }

	public BotSpawner BotSpawner
	{
		get
		{
			return _spawner;
		}
		set
		{
			BotSpawnController.Subscribe(value);
			_spawner = value;
		}
	}

	public GrenadeController GrenadeController { get; private set; }

	public BotJobsClass BotJobs { get; private set; }

	public BotExtractManager BotExtractManager { get; private set; }

	public TimeClass TimeVision { get; private set; }

	public SAINWeatherClass WeatherVision { get; private set; }

	public BotSpawnController BotSpawnController { get; private set; }

	public BotSquads BotSquads { get; private set; }

	public BotHearingClass BotHearing { get; private set; }

	public List<Player> DeadBots { get; private set; } = new List<Player>();

	public List<BotDeathObject> DeathObstacles { get; private set; } = new List<BotDeathObject>();

	public void PlayerEnviromentChanged(string profileID, IndoorTrigger trigger)
	{
		SAINGameWorld.PlayerTracker.GetPlayerComponent(profileID)?.AIData.PlayerLocation.UpdateEnvironment(trigger);
	}

	public void Awake()
	{
		Instance = this;
		SAINGameWorld = ((Component)this).GetComponent<GameWorldComponent>();
		BotSpawnController = new BotSpawnController(this);
		BotExtractManager = new BotExtractManager(this);
		TimeVision = new TimeClass(this);
		WeatherVision = new SAINWeatherClass(this);
		BotSquads = new BotSquads(this);
		BotHearing = new BotHearingClass(this);
		BotJobs = new BotJobsClass(this);
		GrenadeController = new GrenadeController(this);
		GameWorld.OnDispose += Dispose;
	}

	public void ManualUpdate(float currentTime, float deltaTime)
	{
		BotSpawnController.Update(currentTime, deltaTime);
		BotExtractManager.Update(currentTime, deltaTime);
		TimeVision.Update(currentTime, deltaTime);
		WeatherVision.Update(currentTime, deltaTime);
		BotSquads.Update(currentTime, deltaTime);
		HashSet<BotComponent> hashSet = BotSpawnController?.SAINBots;
		if (hashSet == null)
		{
			return;
		}
		foreach (BotComponent item in hashSet)
		{
			item?.ManualUpdate(currentTime, deltaTime);
		}
	}

	public void BotDeath(BotOwner bot)
	{
		if ((Object)(object)((bot != null) ? bot.GetPlayer : null) != (Object)null && bot.IsDead)
		{
			DeadBots.Add(bot.GetPlayer);
		}
	}

	public void AddNavObstacles()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (DeadBots.Count <= 0)
		{
			return;
		}
		Player val3 = default(Player);
		for (int i = 0; i < DeadBots.Count; i++)
		{
			Player val = DeadBots[i];
			if ((Object)(object)val == (Object)null || (Object)(object)val.GetPlayer == (Object)null)
			{
				IndexToRemove.Add(i);
				continue;
			}
			bool flag = true;
			Collider[] array = Physics.OverlapSphere(val.Position, 1.5f, LayerMask.op_Implicit(LayerMaskClass.PlayerMask));
			Collider[] array2 = array;
			foreach (Collider val2 in array2)
			{
				if (!((Object)(object)val2 == (Object)null) && ((Component)val2).TryGetComponent<Player>(ref val3) && val3.IsAI && val3.HealthController.IsAlive)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				if ((Object)(object)val != (Object)null && (Object)(object)val.GetPlayer != (Object)null)
				{
					BotDeathObject botDeathObject = new BotDeathObject(val);
					botDeathObject.Activate(1.5f);
					DeathObstacles.Add(botDeathObject);
				}
				IndexToRemove.Add(i);
			}
		}
		foreach (int item in IndexToRemove)
		{
			DeadBots.RemoveAt(item);
		}
		IndexToRemove.Clear();
	}

	private void UpdateObstacles()
	{
		if (DeathObstacles.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < DeathObstacles.Count; i++)
		{
			BotDeathObject botDeathObject = DeathObstacles[i];
			if (botDeathObject != null && botDeathObject.TimeSinceCreated > 30f)
			{
				botDeathObject?.Dispose();
				IndexToRemove.Add(i);
			}
		}
		foreach (int item in IndexToRemove)
		{
			DeathObstacles.RemoveAt(item);
		}
		IndexToRemove.Clear();
	}

	public void OnDestroy()
	{
	}

	public void Dispose()
	{
		try
		{
			GameWorld.OnDispose -= Dispose;
			((MonoBehaviour)this).StopAllCoroutines();
			BotJobs.Dispose();
			BotSpawnController.UnSubscribe();
			if (BotEventHandler != null)
			{
				GrenadeController.UnSubscribe(BotEventHandler);
			}
			if (Bots != null && Bots.Count > 0)
			{
				foreach (BotComponent value in Bots.Values)
				{
					value?.Dispose();
				}
			}
			Bots?.Clear();
		}
		catch (Exception arg)
		{
			Logger.LogError($"Dispose SAIN BotController Error: {arg}");
		}
		Object.Destroy((Object)(object)this);
	}

	public bool GetSAIN(BotOwner botOwner, out BotComponent bot)
	{
		bot = BotSpawnController.GetSAIN(botOwner);
		return (Object)(object)bot != (Object)null;
	}
}
