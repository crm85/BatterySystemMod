using EFT;
using UnityEngine;

namespace SAIN.Types.Jobs;

public struct NavMeshPathRaycastJob : IBotRaycastJobSingleOwner, IBotRaycastJobSingleTarget
{
	public Vector3 ViewPosition;

	public int CornerCount;

	public int OffsetCount;

	public RaycastJob RaycastJob;

	public readonly IPlayer Owner => RaycastJob.Owner;

	public readonly IPlayer Target => RaycastJob.Target;

	public Vector3[] Corners { get; }

	public Vector3[] Offsets { get; }

	public Vector3[] RaycastPoints { get; }

	public NavMeshPathRaycastJob(Vector3[] PathCorners, Vector3[] inOffsets, Vector3[] inRaycastPoints, Vector3 InViewPosition, LayerMask Mask, IPlayer inOwner, IPlayer inTarget)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		ViewPosition = InViewPosition;
		CornerCount = PathCorners.Length;
		OffsetCount = inOffsets.Length;
		RaycastJob = new RaycastJob(inRaycastPoints, InViewPosition, Mask, inOwner, inTarget);
		Corners = PathCorners;
		Offsets = inOffsets;
		RaycastPoints = inRaycastPoints;
	}

	public int GetCornerFromHitIndex(int hitIndex)
	{
		int num = RaycastPoints.Length;
		if (hitIndex >= num)
		{
			Logger.LogError("dumbass 1");
			return -1;
		}
		int num2 = hitIndex / OffsetCount;
		if (num2 >= CornerCount)
		{
			Logger.LogError("dumbass 2");
			return -1;
		}
		return num2;
	}

	public static NavMeshPathRaycastJob Create(Vector3[] PathCorners, int InOffsetCount, LayerMask Mask, IPlayer inOwner, IPlayer inTarget)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = inOwner.MainParts[(BodyPartType)0].Position;
		Vector3[] array = CreateOffsets(InOffsetCount, 1.65f);
		Vector3[] inRaycastPoints = CreateVectorArray(PathCorners, array);
		return new NavMeshPathRaycastJob(PathCorners, array, inRaycastPoints, position, Mask, inOwner, inTarget);
	}

	public static Vector3[] CreateVectorArray(Vector3[] PathCorners, Vector3[] Offsets)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] array = (Vector3[])(object)new Vector3[PathCorners.Length * Offsets.Length];
		int num = 0;
		for (int i = 0; i < PathCorners.Length; i++)
		{
			for (int j = 0; j < Offsets.Length; j++)
			{
				array[num] = PathCorners[i] + Offsets[j];
				num++;
			}
		}
		return array;
	}

	public static Vector3[] CreateOffsets(int Count, float Extent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.up * (Extent / (float)Count);
		Vector3[] array = (Vector3[])(object)new Vector3[Count];
		Vector3 val2 = Vector3.zero;
		for (int i = 0; i < Count; i++)
		{
			val2 = (array[i] = val2 + val);
		}
		return array;
	}
}
