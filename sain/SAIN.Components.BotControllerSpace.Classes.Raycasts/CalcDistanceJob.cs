using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace SAIN.Components.BotControllerSpace.Classes.Raycasts;

public struct CalcDistanceJob : IJobFor
{
	[ReadOnly]
	public NativeArray<Vector3> directions;

	[WriteOnly]
	public NativeArray<float> distances;

	public void Execute(int index)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = directions[index];
		distances[index] = ((Vector3)(ref val)).magnitude;
	}

	public void Dispose()
	{
		if (directions.IsCreated)
		{
			directions.Dispose();
		}
		if (distances.IsCreated)
		{
			distances.Dispose();
		}
	}
}
