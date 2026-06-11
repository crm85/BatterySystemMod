using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace SAIN.Components.BotControllerSpace.Classes.Raycasts;

public struct CalcDistanceThreePointsJob : IJobFor
{
	[ReadOnly]
	public NativeArray<DirCalcData> Input;

	[ReadOnly]
	public Vector3 Point1;

	[ReadOnly]
	public Vector3 Point2;

	[WriteOnly]
	public NativeArray<DirCalcData> Point1DataOutput;

	[WriteOnly]
	public NativeArray<DirCalcData> Point2DataOutput;

	public void Execute(int index)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		DirCalcData data = Input[index];
		Calc(ref data, Point1);
		Point1DataOutput[index] = data;
		DirCalcData data2 = Input[index];
		Calc(ref data2, Point2);
		Point2DataOutput[index] = data2;
	}

	private static void Calc(ref DirCalcData data, Vector3 point)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		data.Dir = data.Point - point;
		data.DirNormal = ((Vector3)(ref data.Dir)).normalized;
		data.Magnitude = ((Vector3)(ref data.Dir)).magnitude;
	}

	public void Dispose()
	{
		Input.Dispose();
		Point1DataOutput.Dispose();
		Point2DataOutput.Dispose();
	}
}
