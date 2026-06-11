using System.Collections;
using System.Collections.Generic;
using SAIN.SAINComponent.Classes.EnemyClasses;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace SAIN.Components;

public class EnemyPlaceRaycastJob : BotManagerBase
{
	public struct CalcEnemyPlaceJob : IJobFor
	{
		[ReadOnly]
		public NativeArray<Vector3> PlacePositions;

		[ReadOnly]
		public NativeArray<Vector3> BotPositions;

		[ReadOnly]
		public NativeArray<Vector3> EnemyPositions;

		[WriteOnly]
		public NativeArray<float> PlaceDistancesToBot;

		[WriteOnly]
		public NativeArray<float> PlaceDistancesToEnemy;

		public void Execute(int index)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = PlacePositions[index];
			Vector3 val2 = BotPositions[index];
			Vector3 val3 = EnemyPositions[index];
			ref NativeArray<float> placeDistancesToBot = ref PlaceDistancesToBot;
			Vector3 val4 = val2 - val;
			placeDistancesToBot[index] = ((Vector3)(ref val4)).magnitude;
			ref NativeArray<float> placeDistancesToEnemy = ref PlaceDistancesToEnemy;
			val4 = val3 - val;
			placeDistancesToEnemy[index] = ((Vector3)(ref val4)).magnitude;
		}

		public void Dispose()
		{
			if (PlacePositions.IsCreated)
			{
				PlacePositions.Dispose();
			}
			if (BotPositions.IsCreated)
			{
				BotPositions.Dispose();
			}
			if (EnemyPositions.IsCreated)
			{
				EnemyPositions.Dispose();
			}
			if (PlaceDistancesToBot.IsCreated)
			{
				PlaceDistancesToBot.Dispose();
			}
			if (PlaceDistancesToEnemy.IsCreated)
			{
				PlaceDistancesToEnemy.Dispose();
			}
		}
	}

	private JobHandle EnemyPlaceJobHandle;

	private CalcEnemyPlaceJob EnemyPlaceJob;

	private readonly List<EnemyPlace> PlacesToCheck = new List<EnemyPlace>();

	private NativeArray<RaycastHit> _hits;

	private NativeArray<RaycastCommand> _commands;

	private JobHandle RaycastJobHandle;

	private readonly LayerMask Mask = LayerMaskClass.HighPolyWithTerrainMaskAI;

	public EnemyPlaceRaycastJob(BotManagerComponent botcontroller)
		: base(botcontroller)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		((MonoBehaviour)botcontroller).StartCoroutine(EnemyPlaceJobLoop());
	}

	private IEnumerator EnemyPlaceJobLoop()
	{
		yield return null;
		while (true)
		{
			if ((Object)(object)base.BotController == (Object)null)
			{
				yield return null;
				continue;
			}
			HashSet<BotComponent> bots = base.BotController.BotSpawnController?.SAINBots;
			if (bots == null || bots.Count == 0)
			{
				yield return null;
				continue;
			}
			IBotGame botGame = base.BotController.BotGame;
			if (botGame != null && (int)botGame.Status == 5)
			{
				yield return null;
				continue;
			}
			PlacesToCheck.Clear();
			foreach (BotComponent bot in bots)
			{
				if (!(bot?.BotActive ?? false))
				{
					continue;
				}
				foreach (Enemy enemy in bot.EnemyController.EnemiesArray)
				{
					if (enemy?.EnemyKnown ?? false)
					{
						if (enemy.KnownPlaces.LastHeardPlace != null)
						{
							PlacesToCheck.Add(enemy.KnownPlaces.LastHeardPlace);
						}
						if (enemy.KnownPlaces.LastSeenPlace != null)
						{
							PlacesToCheck.Add(enemy.KnownPlaces.LastSeenPlace);
						}
					}
				}
			}
			int Count = PlacesToCheck.Count;
			if (Count == 0)
			{
				yield return null;
				continue;
			}
			NativeArray<Vector3> PlacePositions = new NativeArray<Vector3>(Count, (Allocator)3, (NativeArrayOptions)1);
			NativeArray<Vector3> BotPositions = new NativeArray<Vector3>(Count, (Allocator)3, (NativeArrayOptions)1);
			NativeArray<Vector3> EnemyPositions = new NativeArray<Vector3>(Count, (Allocator)3, (NativeArrayOptions)1);
			for (int i = 0; i < Count; i++)
			{
				EnemyPlace Place = PlacesToCheck[i];
				PlacePositions[i] = Place.Position;
				BotPositions[i] = Place.PlaceData.Owner.Transform.HeadPosition;
				EnemyPositions[i] = Place.PlaceData.Enemy.EnemyTransform.Position;
			}
			EnemyPlaceJob = new CalcEnemyPlaceJob
			{
				PlacePositions = PlacePositions,
				BotPositions = BotPositions,
				EnemyPositions = EnemyPositions,
				PlaceDistancesToBot = new NativeArray<float>(Count, (Allocator)3, (NativeArrayOptions)1),
				PlaceDistancesToEnemy = new NativeArray<float>(Count, (Allocator)3, (NativeArrayOptions)1)
			};
			EnemyPlaceJobHandle = IJobForExtensions.Schedule<CalcEnemyPlaceJob>(EnemyPlaceJob, Count, default(JobHandle));
			_commands = new NativeArray<RaycastCommand>(Count, (Allocator)3, (NativeArrayOptions)1);
			_hits = new NativeArray<RaycastHit>(Count, (Allocator)3, (NativeArrayOptions)1);
			for (int j = 0; j < Count; j++)
			{
				EnemyPlace Place2 = PlacesToCheck[j];
				Vector3 HeadPosition = Place2.PlaceData.Owner.Transform.EyePosition;
				Vector3 PlacePosition = Place2.Position + Vector3.up;
				_commands[j] = new RaycastCommand(HeadPosition, PlacePosition - HeadPosition, new QueryParameters
				{
					layerMask = LayerMask.op_Implicit(Mask)
				}, 1f);
			}
			RaycastJobHandle = RaycastCommand.ScheduleBatch(_commands, _hits, 8, default(JobHandle));
			yield return null;
			((JobHandle)(ref RaycastJobHandle)).Complete();
			((JobHandle)(ref EnemyPlaceJobHandle)).Complete();
			for (int k = 0; k < Count; k++)
			{
				EnemyPlace Place3 = PlacesToCheck[k];
				if (Place3 != null)
				{
					RaycastHit Hit = _hits[k];
					Place3.SetDistances(EnemyPlaceJob.PlaceDistancesToBot[k], EnemyPlaceJob.PlaceDistancesToEnemy[k], Place3.PlaceData.Owner);
					Place3.SetVisibilityOfPlace((Object)(object)((RaycastHit)(ref Hit)).collider == (Object)null, Place3.PlaceData.Owner);
				}
			}
			PlacesToCheck.Clear();
			EnemyPlaceJob.Dispose();
			_commands.Dispose();
			_hits.Dispose();
		}
	}

	public void Dispose()
	{
		if (!((JobHandle)(ref RaycastJobHandle)).IsCompleted)
		{
			((JobHandle)(ref RaycastJobHandle)).Complete();
		}
		if (!((JobHandle)(ref EnemyPlaceJobHandle)).IsCompleted)
		{
			((JobHandle)(ref EnemyPlaceJobHandle)).Complete();
		}
		EnemyPlaceJob.Dispose();
		if (_commands.IsCreated)
		{
			_commands.Dispose();
		}
		if (_hits.IsCreated)
		{
			_hits.Dispose();
		}
	}
}
