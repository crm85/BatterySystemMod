using EFT;
using SAIN.Components;
using UnityEngine;

namespace SAIN;

public class GameWorldHandler
{
	public static GameWorldComponent SAINGameWorld { get; private set; }

	public static void Create(GameWorld gameWorld)
	{
		if ((Object)(object)SAINGameWorld != (Object)null)
		{
			Logger.LogWarning("Old SAIN Gameworld is not null! Destroying...");
			SAINGameWorld.DestroyComponent();
			Object.Destroy((Object)(object)SAINGameWorld);
		}
		SAINGameWorld = ((Component)gameWorld).gameObject.AddComponent<GameWorldComponent>();
		BotManagerComponent sainBotController = ((Component)gameWorld).gameObject.AddComponent<BotManagerComponent>();
		SAINGameWorld.Init(gameWorld, sainBotController);
	}
}
