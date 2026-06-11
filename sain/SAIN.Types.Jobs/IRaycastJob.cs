using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace SAIN.Types.Jobs;

public interface IRaycastJob : IDisposable
{
	bool IsScheduled { get; }

	bool IsCompleted { get; }

	bool IsCreated { get; }

	JobHandle Handle { get; }

	NativeArray<RaycastHit> Hits { get; }

	NativeArray<RaycastCommand> Commands { get; }

	int TotalRaycasts { get; }

	LayerMask Mask { get; }

	JobHandle Schedule(int MaxCommandsPerJob = 8);

	void Complete();
}
