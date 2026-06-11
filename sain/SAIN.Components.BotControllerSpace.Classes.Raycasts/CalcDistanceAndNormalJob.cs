using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace SAIN.Components.BotControllerSpace.Classes.Raycasts;

public struct CalcDistanceAndNormalJob : IJobFor
{
	[ReadOnly]
	public NativeArray<Vector3> directions;

	[WriteOnly]
	public NativeArray<float> distances;

	[WriteOnly]
	public NativeArray<Vector3> normals;

	public void Execute(int index)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = directions[index];
		distances[index] = ((Vector3)(ref val)).magnitude;
		normals[index] = ((Vector3)(ref val)).normalized;
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
		if (normals.IsCreated)
		{
			normals.Dispose();
		}
	}
}
