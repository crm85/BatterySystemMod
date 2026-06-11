using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

namespace SAIN.Types.Jobs;

public abstract class SainMultiJobTemplate : SainJobTemplate
{
	protected readonly List<JobHandle> JobHandles = new List<JobHandle>();

	protected SainMultiJobTemplate(string InName, MonoBehaviour InOwner, bool InLooping = true, float InLoopInterval = 1f / 60f)
		: base(InName, InOwner, InLooping, InLoopInterval)
	{
	}

	protected virtual void ClearJobHandles()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		foreach (JobHandle jobHandle in JobHandles)
		{
			JobHandle current = jobHandle;
			if (((JobHandle)(ref current)).IsCompleted)
			{
				((JobHandle)(ref current)).Complete();
			}
		}
		JobHandles.Clear();
	}
}
