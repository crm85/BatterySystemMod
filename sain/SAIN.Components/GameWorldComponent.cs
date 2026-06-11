using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EFT;
using EFT.Game.Spawning;
using EFT.InventoryLogic;
using SAIN.Components.CoverFinder;
using SAIN.Components.Extract;
using SAIN.Components.PlayerComponentSpace;
using SAIN.Components.RotationController;
using SAIN.Helpers;
using UnityEngine;

namespace SAIN.Components;

public class GameWorldComponent : MonoBehaviour
{
	protected readonly struct BulletData
	{
		public readonly EftBulletClass Bullet;

		public readonly PlayerComponent Owner;

		public readonly List<OtherPlayerData> RelevantPlayers;

		public BulletData(EftBulletClass inBullet, PlayerComponent InOwner, List<OtherPlayerData> InRelevantPlayers)
		{
			Bullet = inBullet;
			Owner = InOwner;
			RelevantPlayers = InRelevantPlayers;
		}
	}

	private readonly List<OtherPlayerData> _tempOtherPlayerCache = new List<OtherPlayerData>();

	private const float _Sounds_PlayerCache_Interval = 1f / 30f;

	private const float _Sounds_BotCache_Interval = 1f / 15f;

	private bool _activated = false;

	private float _Sounds_PlayerCache_Time;

	private float _Sounds_BotCache_Time;

	private List<BulletData> ActiveBullets { get; } = new List<BulletData>();

	public ColliderCoverManager CoverManager { get; private set; }

	public static GameWorldComponent Instance { get; private set; }

	public GameWorld GameWorld { get; private set; }

	public PlayerSpawnTracker PlayerTracker { get; private set; }

	public BotManagerComponent SAINBotController { get; private set; }

	public ExtractFinderComponent ExtractFinder { get; private set; }

	public DoorHandler Doors { get; private set; }

	public LocationClass Location { get; private set; }

	public SpawnPointMarker[] SpawnPointMarkers { get; private set; }

	public JobManager JobManager { get; private set; }

	public static float WorldTickDeltaTime { get; private set; }

	protected BotRotationManagerComponent BotRotationManager { get; set; }

	public static bool TryGetPlayerComponent(IPlayer Player, out PlayerComponent PlayerComponent)
	{
		if (Player == null)
		{
			PlayerComponent = null;
			return false;
		}
		PlayerSpawnTracker playerSpawnTracker = Instance?.PlayerTracker;
		if (playerSpawnTracker == null)
		{
			PlayerComponent = null;
			return false;
		}
		PlayerComponent = playerSpawnTracker.AlivePlayersDictionary.GetPlayerComponent(Player);
		return (Object)(object)PlayerComponent != (Object)null;
	}

	public void RegisterShot(Player Player, EftBulletClass Bullet, Item Weapon)
	{
		if (TryGetPlayerComponent((IPlayer)(object)Player, out var PlayerComponent))
		{
			((MonoBehaviour)this).StartCoroutine(TrackBullet(PlayerComponent, Bullet));
		}
	}

	private void UpdateActiveBullets(List<EftBulletClass> bullets)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Invalid comparison between Unknown and I4
		int count = bullets.Count;
		if (count == 0)
		{
			return;
		}
		for (int i = 0; i < count; i++)
		{
			EftBulletClass val = bullets[i];
			if (val == null || (int)val.BulletState > 0)
			{
				bullets.RemoveAt(i);
			}
		}
	}

	private IEnumerator TrackBullet(PlayerComponent Player, EftBulletClass Bullet)
	{
		Dictionary<string, OtherPlayerData> OtherPlayerData = Player.OtherPlayersData.DataDictionary;
		Vector3 PlayerLookDir = Player.LookDirection;
		List<OtherPlayerData> PlayersToCheck = new List<OtherPlayerData>();
		PlayersToCheck.AddRange(from _003C_003Eh__TransparentIdentifier1 in (from _003C_003Eh__TransparentIdentifier0 in OtherPlayerData.Select(delegate(KeyValuePair<string, OtherPlayerData> keyValuePair2)
				{
					//IL_000f: Unknown result type (might be due to invalid IL or missing references)
					KeyValuePair<string, OtherPlayerData> keyValuePair = keyValuePair2;
					return new
					{
						Data = keyValuePair2,
						OtherPlayerDirNormal = keyValuePair.Value.DistanceData.DirectionNormal
					};
				})
				select new
				{
					_003C_003Eh__TransparentIdentifier0,
					_003C_003Eh__TransparentIdentifier0.Data.Value.PlayerComponent
				}).Where(_003C_003Eh__TransparentIdentifier1 =>
			{
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				PlayerComponent playerComponent = _003C_003Eh__TransparentIdentifier1.PlayerComponent;
				return playerComponent != null && playerComponent.IsAI && _003C_003Eh__TransparentIdentifier1.PlayerComponent.IsActive && Vector3.Dot(_003C_003Eh__TransparentIdentifier1._003C_003Eh__TransparentIdentifier0.OtherPlayerDirNormal, PlayerLookDir) > 0.75f;
			})
			select _003C_003Eh__TransparentIdentifier1._003C_003Eh__TransparentIdentifier0.Data.Value);
		if (PlayersToCheck.Count <= 0)
		{
			yield break;
		}
		while (!Bullet.IsShotFinished && (Player?.IsActive ?? false) && PlayersToCheck.Count > 0)
		{
			Vector3 BulletPosition = Bullet.CurrentPosition;
			for (int i = PlayersToCheck.Count - 1; i >= 0; i--)
			{
				OtherPlayerData Data = PlayersToCheck[i];
				if (Data != null && Data.PlayerComponent?.IsActive == false)
				{
					PlayersToCheck.RemoveAt(i);
				}
				else
				{
					Vector3 PlayerPosition = Data.DistanceData.Position;
					Vector3 val = PlayerPosition - BulletPosition;
					float BulletDistSqr = ((Vector3)(ref val)).sqrMagnitude;
					if (BulletDistSqr < 100f)
					{
						Data.PlayerComponent.RegisterFlyBy(Player, Bullet);
						PlayersToCheck.RemoveAt(i);
					}
				}
			}
			yield return null;
		}
	}

	public void WorldTick(float deltaTime)
	{
		WorldTickDeltaTime = deltaTime;
		float time = Time.time;
		ManualUpdate(time, deltaTime);
		SAINBotController.ManualUpdate(time, deltaTime);
	}

	protected void ManualUpdate(float CurrentTime, float DeltaTime)
	{
		if (!_activated)
		{
			return;
		}
		ExtractFinder.ManualUpdate(CurrentTime, DeltaTime);
		Doors.ManualUpdate(CurrentTime, DeltaTime);
		Location.ManualUpdate(CurrentTime, DeltaTime);
		findSpawnPointMarkers();
		HashSet<PlayerComponent> hashSet = PlayerTracker?.AlivePlayerArray;
		if (hashSet == null || hashSet.Count <= 0)
		{
			return;
		}
		foreach (PlayerComponent item in hashSet)
		{
			item?.ManualUpdate(CurrentTime, DeltaTime);
		}
		TickSoundCaches(hashSet, CurrentTime);
	}

	protected void TickSoundCaches(HashSet<PlayerComponent> PlayerComponents, float CurrentTime)
	{
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Invalid comparison between Unknown and I4
		if (_Sounds_PlayerCache_Time < CurrentTime)
		{
			_Sounds_PlayerCache_Time = CurrentTime + 1f / 30f;
			foreach (PlayerComponent PlayerComponent in PlayerComponents)
			{
				if ((Object)(object)PlayerComponent != (Object)null)
				{
					UpdatePlayerSoundCache(PlayerComponent);
				}
			}
		}
		if (!(_Sounds_BotCache_Time < CurrentTime))
		{
			return;
		}
		_Sounds_BotCache_Time = CurrentTime + 1f / 15f;
		foreach (PlayerComponent PlayerComponent2 in PlayerComponents)
		{
			BotComponent botComponent = PlayerComponent2?.BotComponent;
			if ((Object)(object)botComponent != (Object)null)
			{
				BotOwner botOwner = botComponent.BotOwner;
				if (botOwner != null && (int)botOwner.BotState == 2)
				{
					botComponent.Hearing.SoundInput.ProcessAISoundCache();
				}
			}
		}
	}

	protected static void UpdatePlayerSoundCache(PlayerComponent Player)
	{
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Invalid comparison between Unknown and I4
		List<SoundEvent> aISoundCachedEvents = Player.AISoundCachedEvents;
		if (Player.IsActive)
		{
			foreach (OtherPlayerData item in Player.OtherPlayersData.DataHashSet)
			{
				PlayerComponent playerComponent = item?.PlayerComponent;
				if (!((Object)(object)playerComponent != (Object)null) || !playerComponent.IsActive || !playerComponent.IsSAINBot)
				{
					continue;
				}
				BotComponent botComponent = playerComponent.Person.AIInfo.BotComponent;
				if (!((Object)(object)botComponent != (Object)null))
				{
					continue;
				}
				BotOwner botOwner = botComponent.BotOwner;
				if (botOwner == null || (int)botOwner.BotState != 2)
				{
					continue;
				}
				bool isInHearingRadius_Footsteps = item.IsInHearingRadius_Footsteps;
				bool isInHearingRadius_GunFire = item.IsInHearingRadius_GunFire;
				float distance = item.DistanceData.Distance;
				foreach (SoundEvent item2 in aISoundCachedEvents)
				{
					bool isGunShot = item2.IsGunShot;
					if ((!isGunShot || isInHearingRadius_GunFire) && (isGunShot || isInHearingRadius_Footsteps))
					{
						playerComponent.Person.AIInfo.BotComponent?.Hearing.SoundInput.CheckAddSoundToCache(item2, distance);
					}
				}
			}
		}
		aISoundCachedEvents.Clear();
	}

	private void findSpawnPointMarkers()
	{
		if (SpawnPointMarkers == null && !((Object)(object)Camera.main == (Object)null))
		{
			SpawnPointMarkers = Object.FindObjectsOfType<SpawnPointMarker>();
			if (SAINPlugin.DebugMode)
			{
				Logger.LogInfo($"Found {SpawnPointMarkers.Length} spawn point markers");
			}
		}
	}

	public IEnumerable<Vector3> GetAllSpawnPointPositionsOnNavMesh()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (SpawnPointMarkers == null)
		{
			return Array.Empty<Vector3>();
		}
		List<Vector3> list = new List<Vector3>();
		SpawnPointMarker[] spawnPointMarkers = SpawnPointMarkers;
		foreach (SpawnPointMarker val in spawnPointMarkers)
		{
			Vector3? nearbyNavMeshPoint = NavMeshHelpers.GetNearbyNavMeshPoint(val.Position, 2f);
			if (nearbyNavMeshPoint.HasValue && !list.Contains(nearbyNavMeshPoint.Value))
			{
				list.Add(nearbyNavMeshPoint.Value);
			}
		}
		return list;
	}

	public void Activate(BotsController botsController)
	{
		SAINBotController.DefaultController = botsController;
		SAINBotController.BotSpawner = botsController.BotSpawner;
		BotRotationManager = BotRotationManagerComponent.Create(((Component)this).gameObject, botsController.BotSpawner, PlayerTracker);
		_activated = true;
		JobManager.Start();
	}

	public void Init(GameWorld gameWorld, BotManagerComponent sainBotController)
	{
		Instance = this;
		GameWorld = gameWorld;
		if ((Object)(object)GameWorld == (Object)null)
		{
			Logger.LogWarning("GameWorld Null, cannot Init SAIN Gameworld! Check 2. Disposing Component...");
			DestroyComponent();
			return;
		}
		SAINBotController = sainBotController;
		CoverManager = GClass6.GetOrAddComponent<ColliderCoverManager>((MonoBehaviour)(object)gameWorld);
		PlayerTracker = new PlayerSpawnTracker(this);
		Doors = new DoorHandler(this);
		Location = new LocationClass(this);
		ExtractFinder = GClass6.GetOrAddComponent<ExtractFinderComponent>((MonoBehaviour)(object)this);
		JobManager = new JobManager((MonoBehaviour)(object)this);
		GameWorld.OnDispose += DestroyComponent;
		try
		{
			EFTCoreSettings.UpdateCoreSettings();
		}
		catch (Exception data)
		{
			Logger.LogError(data);
		}
		Doors.Init();
		Location.Init();
	}

	public void DestroyComponent()
	{
		Instance = null;
		try
		{
			PlayerTracker?.Dispose();
			Doors?.Dispose();
			Location?.Dispose();
			JobManager?.Dispose();
		}
		catch (Exception arg)
		{
			Logger.LogError($"Dispose GameWorld Component Class Error: {arg}");
		}
		try
		{
			ComponentHelpers.DestroyComponent<BotManagerComponent>(SAINBotController);
		}
		catch (Exception arg2)
		{
			Logger.LogError($"Dispose GameWorld SubComponent Error: {arg2}");
		}
		((MonoBehaviour)this).StopAllCoroutines();
		Instance = null;
		GameWorld.OnDispose -= DestroyComponent;
		Object.Destroy((Object)(object)this);
	}
}
