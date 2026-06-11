using EFT;
using UnityEngine;
using UnityEngine.AI;

namespace SAIN.Components;

public class BotDeathObject
{
	public NavMeshObstacle NavMeshObstacle { get; private set; }

	public Player Player { get; private set; }

	public Vector3 Position { get; private set; }

	public float TimeCreated { get; private set; }

	public float TimeSinceCreated => Time.time - TimeCreated;

	public bool ObstacleActive => NavMeshObstacle.carving;

	public BotDeathObject(Player player)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		Player = player;
		NavMeshObstacle = ((Component)player).gameObject.AddComponent<NavMeshObstacle>();
		NavMeshObstacle.carving = false;
		((Behaviour)NavMeshObstacle).enabled = false;
		Position = player.Position;
		TimeCreated = Time.time;
	}

	public void Activate(float radius = 2f)
	{
		if ((Object)(object)NavMeshObstacle != (Object)null)
		{
			((Behaviour)NavMeshObstacle).enabled = true;
			NavMeshObstacle.carving = true;
			NavMeshObstacle.radius = radius;
		}
	}

	public void Dispose()
	{
		if ((Object)(object)NavMeshObstacle != (Object)null)
		{
			NavMeshObstacle.carving = false;
			((Behaviour)NavMeshObstacle).enabled = false;
			Object.Destroy((Object)(object)NavMeshObstacle);
		}
	}
}
