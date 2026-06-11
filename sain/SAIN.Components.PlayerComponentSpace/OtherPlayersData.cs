using System.Collections.Generic;
using EFT;
using EFT.HealthSystem;
using SAIN.SAINComponent;
using UnityEngine;

namespace SAIN.Components.PlayerComponentSpace;

public class OtherPlayersData : PlayerComponentBase
{
	public Dictionary<string, OtherPlayerData> DataDictionary { get; } = new Dictionary<string, OtherPlayerData>();

	public HashSet<OtherPlayerData> DataHashSet { get; } = new HashSet<OtherPlayerData>();

	public List<OtherPlayerData> DataList { get; } = new List<OtherPlayerData>();

	public OtherPlayersData(PlayerComponent playerComponent)
		: base(playerComponent)
	{
		PlayerSpawnTracker playerSpawnTracker = GameWorldComponent.Instance?.PlayerTracker;
		if (playerSpawnTracker == null)
		{
			Logger.LogError("player tracker null");
			return;
		}
		playerSpawnTracker.OnPlayerAdded += PlayerAdded;
		playerSpawnTracker.OnPlayerRemoved += PlayerRemoved;
		foreach (PlayerComponent item in playerSpawnTracker.AlivePlayerArray)
		{
			PlayerAdded(item);
		}
	}

	public override void Dispose()
	{
		PlayerSpawnTracker playerSpawnTracker = GameWorldComponent.Instance?.PlayerTracker;
		if (playerSpawnTracker != null)
		{
			playerSpawnTracker.OnPlayerAdded -= PlayerAdded;
			playerSpawnTracker.OnPlayerRemoved -= PlayerRemoved;
		}
		DataDictionary.Clear();
		DataHashSet.Clear();
		DataList.Clear();
	}

	private void PlayerAdded(PlayerComponent playerComp)
	{
		if ((Object)(object)playerComp == (Object)(object)base.PlayerComponent)
		{
			return;
		}
		if ((Object)(object)playerComp == (Object)null)
		{
			Logger.LogWarning("Player Component is null. Cannot add other player data!");
			return;
		}
		Player player = playerComp.Player;
		if (player != null)
		{
			IHealthController healthController = player.HealthController;
			if (((healthController != null) ? new bool?(healthController.IsAlive) : ((bool?)null)) == true)
			{
				string profileId = playerComp.ProfileId;
				if (profileId == base.PlayerComponent.ProfileId)
				{
					return;
				}
				if (DataDictionary.TryGetValue(profileId, out var value))
				{
					DataHashSet.Remove(value);
					DataDictionary.Remove(profileId);
					string[] obj = new string[7] { "Removed Existing Playerdata for profile ID [", profileId, "] : Old Data Nickname: [", null, null, null, null };
					PlayerComponent playerComponent = value.PlayerComponent;
					object obj2;
					if (playerComponent == null)
					{
						obj2 = null;
					}
					else
					{
						Player player2 = playerComponent.Player;
						obj2 = ((player2 != null) ? player2.Profile.Nickname : null);
					}
					obj[3] = (string)obj2;
					obj[4] = "] : New Data Nickname: [";
					Player player3 = playerComp.Player;
					obj[5] = ((player3 != null) ? player3.Profile.Nickname : null);
					obj[6] = "]";
					Logger.LogWarning(string.Concat(obj));
				}
				value = new OtherPlayerData(profileId, playerComp);
				DataHashSet.Add(value);
				DataDictionary.Add(profileId, value);
				DataList.Add(value);
				return;
			}
		}
		Logger.LogWarning("Player is dead. Cannot add other player data!");
	}

	private void PlayerRemoved(string profileId, PlayerComponent playerComp)
	{
		if (DataDictionary.TryGetValue(profileId, out var value))
		{
			DataHashSet.Remove(value);
			DataDictionary.Remove(profileId);
			DataList.Remove(value);
		}
	}
}
