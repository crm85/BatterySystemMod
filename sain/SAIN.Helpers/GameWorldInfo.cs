using System.Collections.Generic;
using Comfort.Common;
using EFT;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.Helpers;

internal class GameWorldInfo
{
	public static GameWorld GameWorld => Singleton<GameWorld>.Instance;

	public static List<Player> AlivePlayers => GameWorld?.AllAlivePlayersList;

	public static Dictionary<string, Player> AlivePlayersDictionary => GameWorld?.allAlivePlayersByID;

	public static bool IsEnemyMainPlayer(Enemy enemy)
	{
		Player val = enemy?.EnemyPlayer;
		Player val2 = GameWorld?.MainPlayer;
		return (Object)(object)val != (Object)null && (Object)(object)val2 != (Object)null && val.ProfileId == val2.ProfileId;
	}

	public static Player GetAlivePlayer(IPlayer person)
	{
		return GetAlivePlayer((person != null) ? person.ProfileId : null);
	}

	public static Player GetAlivePlayer(string profileID)
	{
		GameWorld gameWorld = GameWorld;
		return (gameWorld != null) ? gameWorld.GetAlivePlayerByProfileID(profileID) : null;
	}
}
