using EFT;
using SAIN.Components.PlayerComponentSpace;
using UnityEngine;

namespace SAIN.Components.RotationController;

public class BotRotationManagerComponent : MonoBehaviour
{
	public static BotRotationManagerComponent Create(GameObject gameObject, BotSpawner botSpawner, PlayerSpawnTracker playerSpawner)
	{
		return gameObject.AddComponent<BotRotationManagerComponent>();
	}

	protected void Update()
	{
	}
}
