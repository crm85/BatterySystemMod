using System;
using System.Collections;
using System.Collections.Generic;
using EFT;
using SAIN.Components.BotController;
using SAIN.SAINComponent.Classes.EnemyClasses;
using SAIN.Types.Jobs;
using Unity.Collections;
using UnityEngine;

namespace SAIN.Components;

public class EnemyPathVisibilityRaycastJob : SainJobTemplate, IDisposable
{
	protected readonly List<NavMeshPathRaycastJob> RaycastJobs = new List<NavMeshPathRaycastJob>();

	protected readonly List<RaycastJob> Jobs = new List<RaycastJob>();

	public EnemyPathVisibilityRaycastJob(MonoBehaviour botcontroller)
		: base("Path Visibility Job", botcontroller, InLooping: true, 0.1f)
	{
	}

	protected override IEnumerator PrimaryFunction()
	{
		CreateJobs();
		int Total = Jobs.Count;
		if (Total > 0)
		{
			yield return null;
			ReadResults(Total);
			Dispose();
		}
	}

	private void ScheduleJobs(int Total)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < Total; i++)
		{
			Jobs[i].Schedule();
		}
	}

	private void ReadResults(int Total)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < Total; i++)
		{
			RaycastJob raycastJob = Jobs[i];
			raycastJob.Complete();
			NativeArray<RaycastHit> hits = raycastJob.Hits;
			NativeArray<RaycastCommand> commands = raycastJob.Commands;
			List<Vector3> points = raycastJob.Points;
			IPlayer owner = raycastJob.Owner;
			object botOwner;
			if (owner == null)
			{
				botOwner = null;
			}
			else
			{
				IAIData aIData = owner.AIData;
				botOwner = ((aIData != null) ? aIData.BotOwner : null);
			}
			if (SAINEnableClass.GetSAIN((BotOwner)botOwner, out var sain))
			{
				SAINEnemyController enemyController = sain.EnemyController;
				IPlayer target = raycastJob.Target;
				Enemy enemy = enemyController.GetEnemy((target != null) ? target.ProfileId : null, mustBeActive: false);
				if (enemy != null)
				{
					bool flag = false;
					for (int num = hits.Length - 1; num >= 0; num--)
					{
						RaycastHit val = hits[num];
						if ((Object)(object)((RaycastHit)(ref val)).collider == (Object)null)
						{
							enemy.SetLastVisiblePathPoint(points[num], num);
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						enemy.ClearVisiblePathPoint();
					}
				}
			}
			points.Clear();
		}
	}

	private void CreateJobs()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Invalid comparison between Unknown and I4
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		LayerMask highPolyWithTerrainMask = LayerMaskClass.HighPolyWithTerrainMask;
		LayerMask val = LayerMask.op_Implicit(LayerMaskClass.DoorLayer);
		LayerMask inMask = LayerMask.op_Implicit(LayerMask.op_Implicit(highPolyWithTerrainMask) & ~(1 << LayerMask.op_Implicit(val)));
		foreach (BotComponent sAINBot in SainJobTemplate.SAINBotController.BotSpawnController.SAINBots)
		{
			if (!((Object)(object)sAINBot != (Object)null) || !sAINBot.BotActive)
			{
				continue;
			}
			foreach (Enemy item2 in sAINBot.EnemyController.EnemiesArray)
			{
				if (item2 == null || !item2.EnemyKnown)
				{
					continue;
				}
				Vector3[] pathCorners = item2.Path.PathCorners;
				if (pathCorners != null)
				{
					int num = pathCorners.Length;
					if (num > 2 && (int)item2.Path.PathToEnemyStatus != 2 && item2.Path.VisionPathPoints.Count > 0)
					{
						item2.Path.VisionPathPoints_Cache.AddRange(item2.Path.VisionPathPoints);
						RaycastJob item = new RaycastJob(item2.Path.VisionPathPoints_Cache, sAINBot.Transform.EyePosition, inMask, (IPlayer)(object)sAINBot.Player, (IPlayer)(object)item2.EnemyPlayer);
						item.Schedule();
						Jobs.Add(item);
						continue;
					}
					if (num == 2)
					{
						item2.SetLastCornerAsVisiblePathPoint(pathCorners[1], 1);
						continue;
					}
				}
				item2.ClearVisiblePathPoint();
			}
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
		foreach (RaycastJob job in Jobs)
		{
			job.Dispose();
		}
		Jobs.Clear();
	}
}
