using System;
using System.Collections;
using System.Collections.Generic;
using EFT;
using SAIN.Components.BotController;
using SAIN.Components.PlayerComponentSpace;
using SAIN.Helpers;
using SAIN.Types.Jobs;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace SAIN.Components;

public class RandomVisiblePointGeneratorJob : SainJobTemplate, IDisposable
{
	protected readonly List<RaycastJob> RaycastJobs = new List<RaycastJob>();

	public RandomVisiblePointGeneratorJob(MonoBehaviour botcontroller)
		: base("Random Visible Point Generator", botcontroller)
	{
	}

	protected override IEnumerator PrimaryFunction()
	{
		RandomDir[] LongRandomDirections = GenerateRandomDirections(500, 0.5f, 100f);
		foreach (PlayerComponent player in SainJobTemplate.AlivePlayers.Values)
		{
			if (player?.IsActive ?? false)
			{
				RaycastJobs.Add(new RaycastJob(LongRandomDirections, player.Transform.HeadPosition, LayerMaskClass.HighPolyWithTerrainMask, (IPlayer)(object)player.Player, null));
			}
		}
		int Total = RaycastJobs.Count;
		if (Total <= 0)
		{
			yield break;
		}
		ScheduleJobs(Total);
		yield return AwaitCompletion(Total);
		NavMeshHit NavHit = default(NavMeshHit);
		for (int i = 0; i < Total; i++)
		{
			RaycastJob Job = RaycastJobs[i];
			Job.Complete();
			NativeArray<RaycastHit> Hits = Job.Hits;
			NativeArray<RaycastCommand> Commands = Job.Commands;
			if (GameWorldComponent.TryGetPlayerComponent(Job.Owner, out var Player))
			{
				for (int j = Hits.Length - 1; j >= 0; j--)
				{
					RaycastHit Hit = Hits[j];
					if ((Object)(object)((RaycastHit)(ref Hit)).collider == (Object)null)
					{
						RaycastCommand Command = Commands[j];
						Vector3 Point = ((RaycastCommand)(ref Command)).from + ((RaycastCommand)(ref Command)).direction * ((RaycastCommand)(ref Command)).distance;
						Color RandomColor = DebugGizmos.RandomColor;
						if (Player.Player.IsYourPlayer)
						{
							DebugGizmos.Sphere(Point, 0.025f, RandomColor, 0.05f);
						}
						if (((RaycastCommand)(ref Command)).distance > 3f && NavMesh.SamplePosition(Point, ref NavHit, 1.5f, -1) && Player.Player.IsYourPlayer)
						{
							DebugGizmos.Sphere(((NavMeshHit)(ref NavHit)).position, 0.1f, RandomColor, 0.05f);
							DebugGizmos.Line(((NavMeshHit)(ref NavHit)).position, ((NavMeshHit)(ref NavHit)).position + Vector3.up * 1.5f, RandomColor, 0.025f, 0.05f);
						}
					}
				}
			}
			Player = null;
		}
		Dispose();
	}

	private void ScheduleJobs(int Total)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < Total; i++)
		{
			RaycastJobs[i].Schedule();
		}
	}

	private void ReadResults(int Total)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < Total; i++)
		{
			RaycastJob raycastJob = RaycastJobs[i];
			raycastJob.Complete();
			NativeArray<RaycastHit> hits = raycastJob.Hits;
			NativeArray<RaycastCommand> commands = raycastJob.Commands;
			if (!GameWorldComponent.TryGetPlayerComponent(raycastJob.Owner, out var PlayerComponent))
			{
				continue;
			}
			for (int num = hits.Length - 1; num >= 0; num--)
			{
				RaycastHit val = hits[num];
				if ((Object)(object)((RaycastHit)(ref val)).collider == (Object)null)
				{
					RaycastCommand val2 = commands[num];
					Vector3 val3 = ((RaycastCommand)(ref val2)).from + ((RaycastCommand)(ref val2)).direction * ((RaycastCommand)(ref val2)).distance;
					if (PlayerComponent.Player.IsYourPlayer)
					{
						Color randomColor = DebugGizmos.RandomColor;
						DebugGizmos.Sphere(val3, 0.05f, randomColor, 0.1f);
						DebugGizmos.Line(((RaycastCommand)(ref val2)).from, val3, randomColor, 0.025f, 0.1f);
					}
				}
			}
		}
	}

	protected static RandomDir[] GenerateRandomDirections(int Count, float LengthMin, float LengthMax)
	{
		RandomDir[] array = new RandomDir[Count];
		for (int i = 0; i < Count; i++)
		{
			array[i] = new RandomDir(LengthMin, LengthMax);
		}
		return array;
	}

	private void CreateJobs()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		RandomDir[] directions = GenerateRandomDirections(100, 0.5f, 8f);
		foreach (PlayerComponent value in SainJobTemplate.AlivePlayers.Values)
		{
			if (value != null && value.IsActive)
			{
				RaycastJobs.Add(new RaycastJob(directions, value.Transform.HeadPosition, LayerMaskClass.HighPolyWithTerrainMask, (IPlayer)(object)value.Player, null));
			}
		}
	}

	private IEnumerator AwaitCompletion(int Total)
	{
		int FramesWaited = 0;
		float DeltaTimeWaited = 0f;
		bool JobsComplete = false;
		while (!JobsComplete && FramesWaited < 10)
		{
			for (int i = 0; i < Total; i++)
			{
				if (RaycastJobs[i].IsCompleted)
				{
					JobsComplete = true;
				}
			}
			yield return null;
			FramesWaited++;
			DeltaTimeWaited += Time.deltaTime;
		}
	}

	protected override bool CanProceed()
	{
		BotDictionary botDictionary = SainJobTemplate.SAINBotController?.BotSpawnController?.BotDictionary;
		return botDictionary != null && botDictionary.Count > 0;
	}

	protected override bool LoopCondition()
	{
		return (Object)(object)SainJobTemplate.SAINGameWorld != (Object)null;
	}

	public void Dispose()
	{
		foreach (RaycastJob raycastJob in RaycastJobs)
		{
			raycastJob.Dispose();
		}
		RaycastJobs.Clear();
	}
}
