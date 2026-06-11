using System.Collections;
using System.Collections.Generic;
using SAIN.Components.BotController;
using SAIN.Components.PlayerComponentSpace.PersonClasses;
using SAIN.Models.Enums;
using SAIN.Models.Structs;
using SAIN.SAINComponent.Classes.EnemyClasses;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace SAIN.Components;

public class VisionRaycastJob : BotManagerBase
{
	private NativeArray<RaycastHit> _hits;

	private NativeArray<RaycastCommand> _commands;

	private JobHandle _handle;

	private const int RAYCAST_CHECKS = 3;

	private readonly LayerMask _LOSMask = LayerMaskClass.HighPolyWithTerrainMask;

	private readonly LayerMask _VisionMask = LayerMaskClass.AI;

	private readonly LayerMask _ShootMask = LayerMaskClass.HighPolyWithTerrainMask;

	private int _partCount = -1;

	private readonly List<EBodyPartColliderType> _colliderTypes = new List<EBodyPartColliderType>();

	private readonly List<Vector3> _castPoints = new List<Vector3>();

	private readonly List<Enemy> _enemies = new List<Enemy>();

	public VisionRaycastJob(BotManagerComponent botcontroller)
		: base(botcontroller)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		((MonoBehaviour)botcontroller).StartCoroutine(CheckVisionLoop());
	}

	private IEnumerator CheckVisionLoop()
	{
		yield return null;
		while (true)
		{
			if ((Object)(object)base.BotController == (Object)null)
			{
				yield return null;
				continue;
			}
			BotDictionary bots = base.BotController.BotSpawnController?.BotDictionary;
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
			FindEnemies(bots, _enemies);
			int enemyCount = _enemies.Count;
			if (enemyCount == 0)
			{
				yield return null;
				continue;
			}
			if (_partCount < 0)
			{
				_partCount = _enemies[0].Vision.VisionChecker.EnemyParts.PartsArray.Length;
			}
			int partCount = _partCount;
			int totalRaycasts = enemyCount * partCount * 3;
			_hits = new NativeArray<RaycastHit>(totalRaycasts, (Allocator)3, (NativeArrayOptions)1);
			_commands = new NativeArray<RaycastCommand>(totalRaycasts, (Allocator)3, (NativeArrayOptions)1);
			CreateCommands(_enemies, _commands, enemyCount, partCount);
			_handle = RaycastCommand.ScheduleBatch(_commands, _hits, 24, default(JobHandle));
			yield return null;
			((JobHandle)(ref _handle)).Complete();
			AnalyzeHits(_enemies, _hits, enemyCount, partCount);
			_commands.Dispose();
			_hits.Dispose();
		}
	}

	public void Dispose()
	{
		if (!((JobHandle)(ref _handle)).IsCompleted)
		{
			((JobHandle)(ref _handle)).Complete();
		}
		if (_commands.IsCreated)
		{
			_commands.Dispose();
		}
		if (_hits.IsCreated)
		{
			_hits.Dispose();
		}
	}

	private void CreateCommands(List<Enemy> enemies, NativeArray<RaycastCommand> raycastCommands, int enemyCount, int partCount)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		_colliderTypes.Clear();
		_castPoints.Clear();
		int num = 0;
		for (int i = 0; i < enemyCount; i++)
		{
			Enemy enemy = _enemies[i];
			PersonTransformClass transform = enemy.Bot.Transform;
			Vector3 eyePosition = transform.EyePosition;
			Vector3 weaponFirePort = transform.WeaponFirePort;
			EnemyPartDataClass[] partsArray = enemy.Vision.VisionChecker.EnemyParts.PartsArray;
			for (int j = 0; j < partCount; j++)
			{
				EnemyPartDataClass enemyPartDataClass = partsArray[j];
				SAINBodyPartRaycast raycast = enemyPartDataClass.GetRaycast();
				Vector3 castPoint = raycast.CastPoint;
				_colliderTypes.Add(raycast.ColliderType);
				_castPoints.Add(castPoint);
				Vector3 val = castPoint - weaponFirePort;
				Vector3 val2 = castPoint - eyePosition;
				raycastCommands[num] = new RaycastCommand(eyePosition, val2, new QueryParameters
				{
					layerMask = LayerMask.op_Implicit(_LOSMask)
				}, 1f);
				num++;
				raycastCommands[num] = new RaycastCommand(eyePosition, val2, new QueryParameters
				{
					layerMask = LayerMask.op_Implicit(_VisionMask)
				}, 1f);
				num++;
				raycastCommands[num] = new RaycastCommand(weaponFirePort, val, new QueryParameters
				{
					layerMask = LayerMask.op_Implicit(_ShootMask)
				}, 1f);
				num++;
			}
		}
	}

	private void AnalyzeHits(List<Enemy> enemies, NativeArray<RaycastHit> raycastHits, int enemyCount, int partCount)
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		float time = Time.time;
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < enemyCount; i++)
		{
			Enemy enemy = _enemies[i];
			EnemyVisionChecker visionChecker = enemy.Vision.VisionChecker;
			EnemyPartDataClass[] partsArray = visionChecker.EnemyParts.PartsArray;
			visionChecker.LastCheckLOSTime = time + (enemy.IsAI ? 0.1f : 0.05f);
			enemy.Bot.Vision.TimeLastCheckedLOS = time;
			for (int j = 0; j < partCount; j++)
			{
				EnemyPartDataClass enemyPartDataClass = partsArray[j];
				EBodyPartColliderType colliderType = _colliderTypes[num2];
				Vector3 castPoint = _castPoints[num2];
				num2++;
				enemyPartDataClass.SetLineOfSight(castPoint, colliderType, raycastHits[num], ERaycastCheck.LineofSight, time);
				num++;
				enemyPartDataClass.SetLineOfSight(castPoint, colliderType, raycastHits[num], ERaycastCheck.Vision, time);
				num++;
				enemyPartDataClass.SetLineOfSight(castPoint, colliderType, raycastHits[num], ERaycastCheck.Shoot, time);
				num++;
			}
		}
	}

	private static void FindEnemies(BotDictionary bots, List<Enemy> result)
	{
		result.Clear();
		float time = Time.time;
		foreach (BotComponent value in bots.Values)
		{
			if ((Object)(object)value == (Object)null || !value.BotActive || value.Vision.TimeSinceCheckedLOS < 0.05f)
			{
				continue;
			}
			foreach (Enemy value2 in value.EnemyController.Enemies.Values)
			{
				if (value2.CheckValid())
				{
					EnemyVisionChecker visionChecker = value2.Vision.VisionChecker;
					if (!(value2.RealDistance > visionChecker.AIVisionRangeLimit()) && visionChecker.LastCheckLOSTime < time)
					{
						result.Add(value2);
					}
				}
			}
		}
	}
}
