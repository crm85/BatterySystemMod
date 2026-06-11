using System;
using System.Collections.Generic;
using SAIN.Types.Jobs;
using UnityEngine;

namespace SAIN.Components;

public class JobManager : IDisposable
{
	public readonly List<ISainJob> Jobs = new List<ISainJob>();

	public JobManager(MonoBehaviour Owner)
	{
		Jobs.Add(new FlashlightRaycastJob(Owner));
		Jobs.Add(new EnemyPathVisibilityRaycastJob(Owner));
	}

	public void Start()
	{
		foreach (ISainJob job in Jobs)
		{
			job?.Start();
		}
	}

	public void Stop()
	{
		Dispose();
	}

	public void Dispose()
	{
		foreach (ISainJob job in Jobs)
		{
			job?.Stop();
		}
	}
}
