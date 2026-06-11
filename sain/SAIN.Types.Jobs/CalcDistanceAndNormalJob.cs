using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace SAIN.Types.Jobs;

public struct CalcDistanceAndNormalJob : IDisposableJobFor, IJobFor, IDisposable
{
	[ReadOnly]
	public NativeArray<Vector3> Directions;

	[WriteOnly]
	public NativeArray<float> Distances;

	[WriteOnly]
	public NativeArray<Vector3> Normals;

	public void Execute(int index)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Directions[index];
		Distances[index] = ((Vector3)(ref val)).magnitude;
		Normals[index] = ((Vector3)(ref val)).normalized;
	}

	public void Dispose()
	{
		if (Directions.IsCreated)
		{
			Directions.Dispose();
		}
		if (Distances.IsCreated)
		{
			Distances.Dispose();
		}
		if (Normals.IsCreated)
		{
			Normals.Dispose();
		}
	}
}
