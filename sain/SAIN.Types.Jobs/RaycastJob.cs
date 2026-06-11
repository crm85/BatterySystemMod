using System;
using System.Collections.Generic;
using EFT;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace SAIN.Types.Jobs;

public struct RaycastJob : IRaycastJob, IDisposable, IBotRaycastJobSingleOwner, IBotRaycastJobSingleTarget
{
	public List<Vector3> Points;

	public int OffsetCount;

	private bool _IsScheduled;

	public readonly bool IsCompleted
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			JobHandle handle = Handle;
			return ((JobHandle)(ref handle)).IsCompleted;
		}
	}

	public IPlayer Owner { get; }

	public IPlayer Target { get; }

	public readonly bool IsCreated => Hits.IsCreated || Commands.IsCreated;

	public int TotalRaycasts { get; }

	public readonly bool IsScheduled => !IsCompleted && _IsScheduled;

	public LayerMask Mask { get; }

	public NativeArray<RaycastHit> Hits { get; }

	public NativeArray<RaycastCommand> Commands { get; }

	public JobHandle Handle { get; private set; }

	public RaycastJob(Vector3[] Points, Vector3 ViewPosition, LayerMask InMask, IPlayer inOwner, IPlayer inTarget)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		this.Points = null;
		Handle = default(JobHandle);
		OffsetCount = 0;
		_IsScheduled = false;
		Owner = inOwner;
		Target = inTarget;
		TotalRaycasts = Points.Length;
		Mask = InMask;
		Hits = new NativeArray<RaycastHit>(Points.Length, (Allocator)3, (NativeArrayOptions)1);
		Commands = CreateCommands(Points.Length, Points, ViewPosition, InMask);
	}

	public RaycastJob(List<Vector3> Points, Vector3 ViewPosition, LayerMask InMask, IPlayer inOwner, IPlayer inTarget)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		Handle = default(JobHandle);
		OffsetCount = 0;
		_IsScheduled = false;
		Owner = inOwner;
		Target = inTarget;
		TotalRaycasts = Points.Count;
		this.Points = Points;
		Mask = InMask;
		Hits = new NativeArray<RaycastHit>(Points.Count, (Allocator)3, (NativeArrayOptions)1);
		Commands = CreateCommands(Points.Count, Points, ViewPosition, InMask);
	}

	public RaycastJob(RandomDir[] Directions, Vector3 OriginPoint, LayerMask InMask, IPlayer inOwner, IPlayer inTarget)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		Points = null;
		Handle = default(JobHandle);
		OffsetCount = 0;
		_IsScheduled = false;
		Owner = inOwner;
		Target = inTarget;
		Mask = InMask;
		TotalRaycasts = Directions.Length;
		Hits = new NativeArray<RaycastHit>(TotalRaycasts, (Allocator)3, (NativeArrayOptions)1);
		Commands = CreateCommands(Directions, OriginPoint, InMask);
	}

	public RaycastJob(List<RandomDir> Directions, Vector3 OriginPoint, LayerMask InMask, IPlayer inOwner, IPlayer inTarget)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		Points = null;
		Handle = default(JobHandle);
		OffsetCount = 0;
		_IsScheduled = false;
		Owner = inOwner;
		Target = inTarget;
		Mask = InMask;
		TotalRaycasts = Directions.Count;
		Hits = new NativeArray<RaycastHit>(TotalRaycasts, (Allocator)3, (NativeArrayOptions)1);
		Commands = CreateCommands(Directions, OriginPoint, InMask);
	}

	public JobHandle Schedule(int MaxCommandsPerJob = 8)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		Handle = RaycastCommand.ScheduleBatch(Commands, Hits, MaxCommandsPerJob, default(JobHandle));
		_IsScheduled = true;
		return Handle;
	}

	public readonly void Complete()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		JobHandle handle = Handle;
		((JobHandle)(ref handle)).Complete();
	}

	public readonly void Dispose()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (!IsCompleted)
		{
			Complete();
		}
		if (Hits.IsCreated)
		{
			Hits.Dispose();
		}
		if (Commands.IsCreated)
		{
			Commands.Dispose();
		}
	}

	private static NativeArray<RaycastCommand> CreateCommands(int Count, Vector3[] Points, Vector3 ViewPosition, LayerMask Mask)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<RaycastCommand> result = default(NativeArray<RaycastCommand>);
		result._002Ector(Count, (Allocator)3, (NativeArrayOptions)1);
		for (int i = 0; i < Count; i++)
		{
			Vector3 val = Points[i] - ViewPosition;
			result[i] = new RaycastCommand(ViewPosition, ((Vector3)(ref val)).normalized, new QueryParameters
			{
				layerMask = LayerMask.op_Implicit(Mask)
			}, ((Vector3)(ref val)).magnitude);
		}
		return result;
	}

	private static NativeArray<RaycastCommand> CreateCommands(int Count, List<Vector3> Points, Vector3 ViewPosition, LayerMask Mask)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<RaycastCommand> result = default(NativeArray<RaycastCommand>);
		result._002Ector(Count, (Allocator)3, (NativeArrayOptions)1);
		for (int i = 0; i < Count; i++)
		{
			Vector3 val = Points[i] - ViewPosition;
			result[i] = new RaycastCommand(ViewPosition, val, new QueryParameters
			{
				layerMask = LayerMask.op_Implicit(Mask)
			}, 1f);
		}
		return result;
	}

	private static NativeArray<RaycastCommand> CreateCommands(RandomDir[] Points, Vector3 ViewPosition, LayerMask Mask)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		int num = Points.Length;
		NativeArray<RaycastCommand> result = default(NativeArray<RaycastCommand>);
		result._002Ector(num, (Allocator)3, (NativeArrayOptions)1);
		for (int i = 0; i < num; i++)
		{
			RandomDir randomDir = Points[i];
			result[i] = new RaycastCommand(ViewPosition, randomDir.DirectionNormal, new QueryParameters
			{
				layerMask = LayerMask.op_Implicit(Mask)
			}, randomDir.Magnitude);
		}
		return result;
	}

	private static NativeArray<RaycastCommand> CreateCommands(List<RandomDir> Points, Vector3 ViewPosition, LayerMask Mask)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		int count = Points.Count;
		NativeArray<RaycastCommand> result = default(NativeArray<RaycastCommand>);
		result._002Ector(count, (Allocator)3, (NativeArrayOptions)1);
		for (int i = 0; i < count; i++)
		{
			RandomDir randomDir = Points[i];
			result[i] = new RaycastCommand(ViewPosition, randomDir.DirectionNormal, new QueryParameters
			{
				layerMask = LayerMask.op_Implicit(Mask)
			}, randomDir.Magnitude);
		}
		return result;
	}
}
