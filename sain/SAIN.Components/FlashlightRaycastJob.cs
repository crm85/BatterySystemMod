using System;
using System.Collections;
using System.Collections.Generic;
using EFT;
using SAIN.Components.BotController;
using SAIN.Components.PlayerComponentSpace;
using SAIN.SAINComponent.Classes.EnemyClasses;
using SAIN.Types.Jobs;
using Unity.Collections;
using UnityEngine;

namespace SAIN.Components;

public class FlashlightRaycastJob : SainJobTemplate, IDisposable
{
	private const float LaserTraceDistance = 75f;

	private const float Wide_FlashLightBeamAngle = 16f;

	private const int Wide_FlashlightBeamPointCount = 32;

	private const float Wide_FlashlightTraceDistance = 30f;

	private const float Tight_FlashLightBeamAngle = 8f;

	private const int Tight_FlashlightBeamPointCount = 16;

	private const float Tight_FlashlightTraceDistance = 60f;

	protected readonly List<RaycastJob> RaycastJobs = new List<RaycastJob>();

	protected readonly List<Quaternion> _rotationsList_Wide = new List<Quaternion>();

	protected readonly List<Quaternion> _rotationsList_Tight = new List<Quaternion>();

	private readonly List<RandomDir> _directionsList = new List<RandomDir>();

	public FlashlightRaycastJob(MonoBehaviour gameWorld)
		: base("Flashlight Detection Job", gameWorld, InLooping: true, 0.1f)
	{
		GenerateRandomYawPitchRotationsNonAlloc(_rotationsList_Wide, 32, 16f);
		GenerateRandomYawPitchRotationsNonAlloc(_rotationsList_Tight, 16, 8f);
	}

	protected override IEnumerator PrimaryFunction()
	{
		CreateFlashlightJobs();
		int Total = RaycastJobs.Count;
		if (Total > 0)
		{
			ScheduleJobs(Total);
			yield return null;
			ReadFlashlightJobData(Total);
			Dispose();
			CreateLightDetectionJobs();
			Total = RaycastJobs.Count;
			if (Total > 0)
			{
				ScheduleJobs(Total);
				yield return null;
				ReadLightDetectionJobData(Total);
				Dispose();
			}
		}
	}

	private void CreateFlashlightJobs()
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		List<RandomDir> directionsList = _directionsList;
		HashSet<PlayerComponent> alivePlayerArray = GameWorldComponent.Instance.PlayerTracker.AlivePlayerArray;
		foreach (PlayerComponent item in alivePlayerArray)
		{
			if ((Object)(object)item != (Object)null && item.IsActive && item.Flashlight.DeviceActive)
			{
				Vector3 weaponPointDirection = item.Transform.WeaponPointDirection;
				if (item.Flashlight.Laser || item.Flashlight.IRLaser)
				{
					directionsList.Add(new RandomDir(75f, weaponPointDirection));
				}
				if (item.Flashlight.WhiteLight || item.Flashlight.IRLight)
				{
					CreateFlashlightBeam(directionsList, _rotationsList_Wide, weaponPointDirection, 30f);
					CreateFlashlightBeam(directionsList, _rotationsList_Tight, weaponPointDirection, 60f);
				}
				if (directionsList.Count > 0)
				{
					RaycastJobs.Add(new RaycastJob(directionsList, item.Transform.WeaponFirePort, LayerMaskClass.HighPolyWithTerrainMaskAI, (IPlayer)(object)item.Player, null));
					directionsList.Clear();
				}
			}
		}
	}

	private void ReadFlashlightJobData(int Total)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < Total; i++)
		{
			RaycastJob raycastJob = RaycastJobs[i];
			raycastJob.Complete();
			NativeArray<RaycastHit> hits = raycastJob.Hits;
			if (!GameWorldComponent.TryGetPlayerComponent(raycastJob.Owner, out var PlayerComponent))
			{
				continue;
			}
			List<Vector3> lightPoints = PlayerComponent.Flashlight.LightDetection.LightPoints;
			lightPoints.Clear();
			for (int num = hits.Length - 1; num >= 0; num--)
			{
				RaycastHit val = hits[num];
				if ((Object)(object)((RaycastHit)(ref val)).collider != (Object)null)
				{
					lightPoints.Add(((RaycastHit)(ref val)).point + ((RaycastHit)(ref val)).normal * 0.05f);
				}
			}
		}
	}

	private void CreateLightDetectionJobs()
	{
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		foreach (BotComponent value in SainJobTemplate.AliveBots.Values)
		{
			if (!((Object)(object)value != (Object)null) || !value.BotActive)
			{
				continue;
			}
			foreach (Enemy value2 in value.EnemyController.Enemies.Values)
			{
				if (value2 != null && value2.EnemyPerson.Active)
				{
					FlashLightClass flashlight = value2.EnemyPlayerComponent.Flashlight;
					if (flashlight.DeviceActive && value.PlayerComponent.Flashlight.LightDetection.CheckIsBeamVisible(flashlight) && value2.RealDistance <= 125f)
					{
						RaycastJobs.Add(new RaycastJob(flashlight.LightDetection.LightPoints, value.Transform.HeadPosition, LayerMaskClass.HighPolyWithTerrainMaskAI, (IPlayer)(object)value.Player, (IPlayer)(object)value2.Player));
					}
				}
			}
		}
	}

	private void ReadLightDetectionJobData(int Total)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < Total; i++)
		{
			RaycastJob raycastJob = RaycastJobs[i];
			raycastJob.Complete();
			NativeArray<RaycastHit> hits = raycastJob.Hits;
			if (!GameWorldComponent.TryGetPlayerComponent(raycastJob.Owner, out var PlayerComponent))
			{
				continue;
			}
			bool flag = false;
			Enumerator<RaycastHit> enumerator = hits.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					RaycastHit current = enumerator.Current;
					if ((Object)(object)((RaycastHit)(ref current)).collider == (Object)null)
					{
						flag = true;
						break;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
			if (flag && raycastJob.Target != null)
			{
				PlayerComponent.Flashlight.LightDetection.TryToInvestigate(raycastJob.Target);
			}
		}
	}

	private void ScheduleJobs(int Total)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < Total; i++)
		{
			RaycastJobs[i].Schedule();
		}
	}

	public static void GenerateRandomYawPitchRotationsNonAlloc(List<Quaternion> nonAllocList, int count, float coneAngle)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < count; i++)
		{
			float num = Random.Range(0f - coneAngle, coneAngle);
			float num2 = Random.Range(0f - coneAngle, coneAngle);
			float num3 = Random.Range(0f - coneAngle, coneAngle);
			nonAllocList.Add(Quaternion.Euler(num2, num, num3));
		}
	}

	private static void CreateFlashlightBeam(List<RandomDir> beamDirections, List<Quaternion> rotationsList, Vector3 weaponPointDir, float distance)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < rotationsList.Count; i++)
		{
			Vector3 val = rotationsList[i] * weaponPointDir;
			beamDirections.Add(new RandomDir(distance, ((Vector3)(ref val)).normalized));
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
