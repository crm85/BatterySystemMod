using System;
using System.Collections.Generic;
using EFT;
using SAIN.Components.PlayerComponentSpace.PersonClasses;
using UnityEngine;

namespace SAIN.Components.PlayerComponentSpace;

public class PlayerSpawnTracker
{
	public readonly HashSet<PlayerComponent> AlivePlayerArray = new HashSet<PlayerComponent>();

	public readonly PlayerDictionary AlivePlayersDictionary = new PlayerDictionary();

	public readonly List<IPlayer> DeadPlayers = new List<IPlayer>();

	private readonly GameWorldComponent _sainGameWorld;

	public event Action<PlayerComponent> OnPlayerAdded;

	public event Action<string, PlayerComponent> OnPlayerRemoved;

	public PlayerComponent GetPlayerComponent(string profileId)
	{
		return AlivePlayersDictionary.GetPlayerComponent(profileId);
	}

	public PlayerComponent GetPlayerComponent(IPlayer Player)
	{
		return AlivePlayersDictionary.GetPlayerComponent(Player);
	}

	public PlayerComponent FindClosestHumanPlayer(out float closestPlayerSqrMag, Vector3 targetPosition, out Player player)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		PlayerComponent result = null;
		closestPlayerSqrMag = float.MaxValue;
		player = null;
		foreach (PlayerComponent value in AlivePlayersDictionary.Values)
		{
			if ((Object)(object)value != (Object)null && (Object)(object)value.Player != (Object)null && !value.IsAI)
			{
				Vector3 val = value.Position - targetPosition;
				float sqrMagnitude = ((Vector3)(ref val)).sqrMagnitude;
				if (sqrMagnitude < closestPlayerSqrMag)
				{
					player = value.Player;
					result = value;
					closestPlayerSqrMag = sqrMagnitude;
				}
			}
		}
		return result;
	}

	public Player FindClosestHumanPlayer(out float closestPlayerSqrMag, Vector3 targetPosition)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		FindClosestHumanPlayer(out closestPlayerSqrMag, targetPosition, out var player);
		return player;
	}

	public PlayerComponent AddPlayerManual(IPlayer player)
	{
		if (player == null)
		{
			return null;
		}
		AddPlayer(player);
		if (AlivePlayersDictionary.TryGetValue(player.ProfileId, out var value))
		{
			string[] obj = new string[5] { "Successfully created new Player Component for [", null, null, null, null };
			Profile profile = player.Profile;
			obj[1] = ((profile != null) ? profile.Nickname : null);
			obj[2] = " : ";
			obj[3] = player.ProfileId;
			obj[4] = "]";
			Logger.LogDebug(string.Concat(obj));
			return value;
		}
		return null;
	}

	private void AddPlayer(IPlayer iPlayer)
	{
		if (iPlayer == null)
		{
			Logger.LogError("Could not add PlayerComponent for Null IPlayer.");
			return;
		}
		string profileId = iPlayer.ProfileId;
		Player val = (Player)(object)((iPlayer is Player) ? iPlayer : null);
		if ((Object)(object)val == (Object)null)
		{
			Profile profile = iPlayer.Profile;
			Logger.LogError("Could not add PlayerComponent for Null Player. IPlayer: " + ((profile != null) ? profile.Nickname : null) + " : " + profileId);
			return;
		}
		if ((Object)(object)((Component)val).gameObject == (Object)null)
		{
			Profile profile2 = iPlayer.Profile;
			Logger.LogError("Player Has null gameobject? IPlayer: " + ((profile2 != null) ? profile2.Nickname : null) + " : " + profileId);
			return;
		}
		if (AlivePlayersDictionary.TryRemove(profileId, out var destroyedComponent))
		{
			string[] obj = new string[5]
			{
				((Object)val).name,
				" : ",
				null,
				null,
				null
			};
			Profile profile3 = val.Profile;
			obj[2] = ((profile3 != null) ? profile3.Nickname : null);
			obj[3] = " : ";
			obj[4] = profileId;
			string text = string.Concat(obj);
			Logger.LogWarning("PlayerComponent already exists for Player: " + text);
			if (destroyedComponent)
			{
				Logger.LogWarning("Destroyed old Component for: " + text);
			}
		}
		PlayerComponent playerComponent = ((Component)val).gameObject.AddComponent<PlayerComponent>();
		if (playerComponent != null && playerComponent.Init(iPlayer))
		{
			playerComponent.Person.ActivationClass.OnPersonDeadOrDespawned += removePerson;
			AlivePlayersDictionary.Add(profileId, playerComponent);
			AlivePlayerArray.Add(playerComponent);
			this.OnPlayerAdded?.Invoke(playerComponent);
		}
		else
		{
			Logger.LogError("Init PlayerComponent Failed for " + ((Object)val).name + " : " + val.ProfileId);
			Object.Destroy((Object)(object)playerComponent);
		}
	}

	private void removePerson(PersonClass person)
	{
		this.OnPlayerRemoved?.Invoke(person.ProfileId, person.PlayerComponent);
		AlivePlayerArray.Remove(person.PlayerComponent);
		person.ActivationClass.OnPersonDeadOrDespawned -= removePerson;
		AlivePlayersDictionary.TryRemove(person.ProfileId, out var _);
		if (!person.ActivationClass.IsAlive && !((Object)(object)person.Player != (Object)null))
		{
		}
	}

	public PlayerSpawnTracker(GameWorldComponent sainGameWorld)
	{
		_sainGameWorld = sainGameWorld;
		sainGameWorld.GameWorld.OnPersonAdd += AddPlayer;
	}

	public void Dispose()
	{
		GameWorld val = _sainGameWorld?.GameWorld;
		if ((Object)(object)val != (Object)null)
		{
			val.OnPersonAdd -= AddPlayer;
		}
		foreach (KeyValuePair<string, PlayerComponent> item in AlivePlayersDictionary)
		{
			item.Value?.Dispose();
		}
		AlivePlayersDictionary.Clear();
	}
}
