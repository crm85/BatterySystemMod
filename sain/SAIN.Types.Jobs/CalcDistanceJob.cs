using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace SAIN.Types.Jobs;

public struct CalcDistanceJob : IDisposableJobFor, IJobFor, IDisposable
{
	[ReadOnly]
	public NativeArray<Vector3> Directions;

	[WriteOnly]
	public NativeArray<float> Distances;

	public static CalcDistanceJob Create(Vector3 Origin, Vector3[] Points)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		CalcDistanceJob result = default(CalcDistanceJob);
		int num = Points.Length;
		result.Directions = new NativeArray<Vector3>(num, (Allocator)3, (NativeArrayOptions)1);
		for (int i = 0; i < num; i++)
		{
			result.Directions[i] = Points[i] - Origin;
		}
		result.Distances = new NativeArray<float>(num, (Allocator)3, (NativeArrayOptions)1);
		return result;
	}

	public static CalcDistanceJob Create(Vector3[] Origins, Vector3[] Points)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		CalcDistanceJob result = default(CalcDistanceJob);
		int num = Points.Length;
		result.Directions = new NativeArray<Vector3>(num, (Allocator)3, (NativeArrayOptions)1);
		for (int i = 0; i < num; i++)
		{
			result.Directions[i] = Points[i] - Origins[i];
		}
		result.Distances = new NativeArray<float>(num, (Allocator)3, (NativeArrayOptions)1);
		return result;
	}

	public static CalcDistanceJob Create(Vector3[] Origins, Vector3 Point)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		CalcDistanceJob result = default(CalcDistanceJob);
		int num = Origins.Length;
		result.Directions = new NativeArray<Vector3>(num, (Allocator)3, (NativeArrayOptions)1);
		for (int i = 0; i < num; i++)
		{
			result.Directions[i] = Point - Origins[i];
		}
		result.Distances = new NativeArray<float>(num, (Allocator)3, (NativeArrayOptions)1);
		return result;
	}

	public void Execute(int index)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		ref NativeArray<float> distances = ref Distances;
		Vector3 val = Directions[index];
		distances[index] = ((Vector3)(ref val)).magnitude;
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
	}
}
