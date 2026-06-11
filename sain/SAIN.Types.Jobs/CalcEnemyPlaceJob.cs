using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace SAIN.Types.Jobs;

public struct CalcEnemyPlaceJob : IDisposableJobFor, IJobFor, IDisposable
{
	[ReadOnly]
	public NativeArray<Vector3> PlacePositions;

	[ReadOnly]
	public NativeArray<Vector3> BotPositions;

	[ReadOnly]
	public NativeArray<Vector3> EnemyPositions;

	[WriteOnly]
	public NativeArray<float> PlaceDistancesToBot;

	[WriteOnly]
	public NativeArray<float> PlaceDistancesToEnemy;

	public void Execute(int index)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = PlacePositions[index];
		ref NativeArray<float> placeDistancesToBot = ref PlaceDistancesToBot;
		Vector3 val2 = BotPositions[index] - val;
		placeDistancesToBot[index] = ((Vector3)(ref val2)).magnitude;
		ref NativeArray<float> placeDistancesToEnemy = ref PlaceDistancesToEnemy;
		val2 = EnemyPositions[index] - val;
		placeDistancesToEnemy[index] = ((Vector3)(ref val2)).magnitude;
	}

	public void Dispose()
	{
		if (PlacePositions.IsCreated)
		{
			PlacePositions.Dispose();
		}
		if (BotPositions.IsCreated)
		{
			BotPositions.Dispose();
		}
		if (EnemyPositions.IsCreated)
		{
			EnemyPositions.Dispose();
		}
		if (PlaceDistancesToBot.IsCreated)
		{
			PlaceDistancesToBot.Dispose();
		}
		if (PlaceDistancesToEnemy.IsCreated)
		{
			PlaceDistancesToEnemy.Dispose();
		}
	}
}
