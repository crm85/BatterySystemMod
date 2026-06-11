using SAIN.Components;
using SAIN.Helpers;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;
using UnityEngine.AI;

namespace SAIN.SAINComponent.Classes.Mover;

public class DogFight : BotBase
{
	private float _updateDogFightTimer;

	private float _enemyTimeSinceSeenThreshold = 1f;

	private readonly NavMeshPath dogFightPath = new NavMeshPath();

	public EDogFightStatus Status { get; private set; }

	public DogFight(BotComponent sain)
		: base(sain)
	{
	}//IL_000c: Unknown result type (might be due to invalid IL or missing references)
	//IL_0016: Expected O, but got Unknown


	public void ResetDogFightStatus()
	{
		if (Status != EDogFightStatus.None)
		{
			Status = EDogFightStatus.None;
		}
	}

	public void DogFightMove(bool aggressive, Enemy Enemy)
	{
		bool flag = Enemy != null;
		if (flag && base.BotOwner.WeaponManager.IsMelee)
		{
			base.Bot.Mover.SetTargetPose(1f);
			base.Bot.Mover.SetTargetMoveSpeed(1f);
			base.Bot.Mover.Prone.SetProne(value: false);
			base.BotOwner.WeaponManager.Melee.RunToEnemyUpdate();
			if (base.BotOwner.WeaponManager.Melee.ShallEndRun)
			{
				base.BotOwner.WeaponManager.Selector.TryChangeToMain();
			}
			return;
		}
		if (flag && Enemy.IsVisible && Enemy.CanShoot && base.Player.IsInPronePose)
		{
			base.Bot.Mover.Lean.HoldLean(1f);
			return;
		}
		if (base.Player.IsInPronePose)
		{
			base.Bot.Mover.Prone.SetProne(value: false);
		}
		base.Bot.Mover.SetTargetMoveSpeed(base.Bot.Info.FileSettings.Move.STRAFE_SPEED);
		if (flag && stopMoveToShoot(Enemy))
		{
			Status = EDogFightStatus.Shooting;
			base.Bot.Mover.StopMove(0f);
			float num = 0.5f * Random.Range(0.5f, 1.33f);
			base.Bot.Mover.Lean.HoldLean(num);
			_updateDogFightTimer = Time.time + num;
		}
		else if (!(_updateDogFightTimer > Time.time))
		{
			base.Bot.Suppression.TrySuppressAnyEnemy(Enemy, base.Bot.EnemyController.EnemyLists.KnownEnemies);
			if (flag && BackUpFromEnemy(Enemy))
			{
				Status = EDogFightStatus.BackingUp;
				float num2 = (Enemy.IsVisible ? 0.75f : 1f);
				_updateDogFightTimer = Time.time + num2 * Random.Range(0.66f, 1.33f);
			}
			else if (!aggressive)
			{
				_updateDogFightTimer = Time.time + 0.5f;
			}
			else if (flag && canMoveToEnemy(Enemy) && base.Bot.Mover.GoToEnemy(Enemy, -1f, crawl: false, mustHaveCompletePath: false))
			{
				base.Bot.Mover.SetTargetMoveSpeed(0.9f);
				Status = EDogFightStatus.MovingToEnemy;
				float num3 = Mathf.Clamp(0.25f * Random.Range(0.5f, 1.25f), 0.1f, 0.66f);
				_updateDogFightTimer = Time.time + num3;
			}
			else
			{
				_updateDogFightTimer = Time.time + 0.33f;
			}
		}
	}

	private bool stopMoveToShoot(Enemy Enemy)
	{
		return Status == EDogFightStatus.MovingToEnemy && Enemy.IsVisible && Enemy.CanShoot;
	}

	public bool BackUpFromEnemy(Enemy Enemy)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (Enemy == null)
		{
			return false;
		}
		if (findStrafePoint(out var movePosition, Enemy))
		{
			return true;
		}
		bool calculating;
		if (findStrafePoint2(out movePosition, Enemy))
		{
			return base.Bot.Mover.GoToPoint(movePosition, out calculating, -1f, crawl: false, slowAtEnd: true, mustHaveCompletePath: false);
		}
		return false;
	}

	private Vector3? findBackupTarget(Enemy Enemy)
	{
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		if (Enemy != null)
		{
			if (!Enemy.Bot.BotOwner.WeaponManager.Reload.Reloading && Enemy.Bot.BotOwner.WeaponManager.HaveBullets)
			{
				BotMedecine medecine = Enemy.Bot.BotOwner.Medecine;
				if ((medecine == null || medecine.Using) && (!Enemy.Seen || !(Enemy.TimeSinceSeen < _enemyTimeSinceSeenThreshold)) && (Enemy.Seen || !Enemy.LastKnownPosition.HasValue || !(Enemy.TimeSinceLastKnownUpdated < _enemyTimeSinceSeenThreshold)))
				{
					goto IL_00ea;
				}
			}
			return (Vector3)(((_003F?)Enemy.VisiblePathPoint) ?? ((_003F?)Enemy.LastKnownPosition) ?? Enemy.EnemyTransform.Position);
		}
		goto IL_00ea;
		IL_00ea:
		return null;
	}

	private bool canMoveToEnemy(Enemy Enemy)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Invalid comparison between Unknown and I4
		return Enemy.LastKnownPosition.HasValue && (int)Enemy.Path.PathToEnemy.status != 2;
	}

	private bool findStrafePoint(out Vector3 movePosition, Enemy Enemy)
	{
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		Vector3? val = findBackupTarget(Enemy);
		if (val.HasValue)
		{
			Vector3 position = base.Bot.Position;
			Vector3 v = val.Value - position;
			v.y = 0f;
			Vector3 val2 = -Vector.NormalizeFastSelf(v);
			Vector3 val3 = position + val2 * 3f;
			NavMeshHit val6 = default(NavMeshHit);
			for (int i = 0; i < 5; i++)
			{
				Vector3 val4 = Random.onUnitSphere * 2f;
				val4.y = Mathf.Clamp(val4.y, -0.5f, 0.5f);
				Vector3 val5 = val3 + val4;
				if (!NavMesh.SamplePosition(val5, ref val6, 2f, -1))
				{
					continue;
				}
				Vector3 val7 = ((NavMeshHit)(ref val6)).position - position;
				if (((Vector3)(ref val7)).sqrMagnitude > 1f)
				{
					movePosition = ((NavMeshHit)(ref val6)).position;
					if (base.Bot.Mover.GoToPoint(movePosition, out var _, -1f, crawl: false, slowAtEnd: true, mustHaveCompletePath: false))
					{
						return true;
					}
				}
			}
		}
		movePosition = Vector3.zero;
		return false;
	}

	private bool findStrafePoint2(out Vector3 MovePoint, Enemy Enemy)
	{
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		if (Enemy.Seen && Enemy.TimeSinceSeen < _enemyTimeSinceSeenThreshold * Random.Range(0.66f, 1.33f))
		{
			Vector3? lastKnownPosition = Enemy.LastKnownPosition;
			NavMeshHit val = default(NavMeshHit);
			if (lastKnownPosition.HasValue && NavMesh.SamplePosition(base.Bot.Position, ref val, 0.5f, -1))
			{
				Vector3 position = ((NavMeshHit)(ref val)).position;
				Vector3 val2 = position - lastKnownPosition.Value;
				Vector3 normalized = ((Vector3)(ref val2)).normalized;
				Vector3 val3 = Random.onUnitSphere * Random.Range(1.25f, 2f);
				val3.y = 0f;
				Vector3 val4 = position + normalized * Random.Range(1f, 2f) + val3;
				NavMeshHit val5 = default(NavMeshHit);
				if (NavMesh.Raycast(position, val4, ref val5, -1))
				{
					if (((NavMeshHit)(ref val5)).distance <= 0.5f)
					{
						dogFightPath.ClearCorners();
						if (NavMesh.CalculatePath(base.Bot.Position, val4, -1, dogFightPath))
						{
							MovePoint = dogFightPath.corners[dogFightPath.corners.Length - 1];
							return true;
						}
					}
					MovePoint = ((NavMeshHit)(ref val5)).position;
					return true;
				}
				MovePoint = val4;
				return true;
			}
		}
		MovePoint = base.Bot.Position;
		return false;
	}
}
