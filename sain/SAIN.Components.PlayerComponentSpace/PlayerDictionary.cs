using System;
using System.Collections.Generic;
using EFT;
using UnityEngine;

namespace SAIN.Components.PlayerComponentSpace;

public class PlayerDictionary : Dictionary<string, PlayerComponent>
{
	private readonly List<string> _ids = new List<string>();

	public event Action<string> OnPlayerComponentRemoved;

	public PlayerComponent GetPlayerComponent(IPlayer Player)
	{
		if (Player != null && TryGetValue(Player.ProfileId, out var value))
		{
			return value;
		}
		return null;
	}

	public PlayerComponent GetPlayerComponent(string profileId)
	{
		if (!GClass1437.IsNullOrEmpty(profileId) && TryGetValue(profileId, out var value))
		{
			return value;
		}
		return null;
	}

	public bool TryRemove(string profileId, out bool destroyedComponent)
	{
		destroyedComponent = false;
		if (GClass1437.IsNullOrEmpty(profileId))
		{
			ClearNullPlayers();
			return false;
		}
		if (TryGetValue(profileId, out var value))
		{
			this.OnPlayerComponentRemoved?.Invoke(profileId);
			if ((Object)(object)value != (Object)null)
			{
				destroyedComponent = true;
				value.Dispose();
			}
			Remove(profileId);
			return true;
		}
		return false;
	}

	public void ClearNullPlayers()
	{
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, PlayerComponent> current = enumerator.Current;
				PlayerComponent value = current.Value;
				if ((Object)(object)value == (Object)null || value.IPlayer == null || (Object)(object)value.Player == (Object)null)
				{
					_ids.Add(current.Key);
					if (value.IPlayer != null)
					{
						Profile profile = value.IPlayer.Profile;
						Logger.LogDebug("Removing " + ((profile != null) ? profile.Nickname : null) + " from player dictionary");
					}
				}
			}
		}
		if (_ids.Count <= 0)
		{
			return;
		}
		Logger.LogDebug($"Removing {_ids.Count} null players");
		foreach (string id in _ids)
		{
			TryRemove(id, out var _);
		}
		_ids.Clear();
	}
}
