using SAIN.Components.CoverFinder;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace SAIN.Components.BotControllerSpace.Classes.Raycasts;

public struct CheckCoverJob : IJobFor
{
	[ReadOnly]
	public NativeArray<ColliderCoverData> Input;

	[WriteOnly]
	public NativeArray<ColliderCoverData> Output;

	public void Execute(int index)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		ColliderCoverData colliderCoverData = Input[index];
		colliderCoverData.BotToCoverDirectionData = CalcDirection(colliderCoverData.BotToCoverDirectionData, colliderCoverData.BotPosition, colliderCoverData.ColliderPosition);
		colliderCoverData.TargetToCoverDirectionData = CalcDirection(colliderCoverData.TargetToCoverDirectionData, colliderCoverData.TargetPosition, colliderCoverData.ColliderPosition);
		Output[index] = colliderCoverData;
	}

	private static DirCalcData CalcDirection(DirCalcData Data, Vector3 Point, Vector3 ColliderPosition)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		Data.Dir = ColliderPosition - Point;
		Data.DirNormal = ((Vector3)(ref Data.Dir)).normalized;
		Data.Magnitude = ((Vector3)(ref Data.Dir)).magnitude;
		return Data;
	}

	private static void CreateCoverPosition(ref ColliderCoverData Data)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		Vector3 dirNormal = Data.BotToTargetDirectionData.DirNormal;
		Data.DotFromBotToTargetToCollider = Vector3.Dot(dirNormal, Data.BotToCoverDirectionData.DirNormal);
		Data.DotFromTargetToBotToCollider = Vector3.Dot(-dirNormal, Data.TargetToCoverDirectionData.DirNormal);
	}

	public void Dispose()
	{
		Input.Dispose();
		Output.Dispose();
	}
}
