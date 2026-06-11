using System.Collections.Generic;
using UnityEngine;

namespace SAIN.Types.Jobs;

public abstract class SainMultiRaycastJobTemplate : SainJobTemplate
{
	protected readonly List<IRaycastJob> Jobs = new List<IRaycastJob>();

	protected SainMultiRaycastJobTemplate(string InName, MonoBehaviour InOwner, bool InLooping = true, float InLoopInterval = 1f / 60f)
		: base(InName, InOwner, InLooping, InLoopInterval)
	{
	}

	protected void StopAndClearJobs()
	{
		foreach (IRaycastJob job in Jobs)
		{
			job.Dispose();
		}
		Jobs.Clear();
	}
}
