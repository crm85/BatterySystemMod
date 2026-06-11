using EFT;
using SAIN.Components;
using SAIN.Models.Enums;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Mover;

public class SAINSteeringClass : BotComponentClassBase
{
	private readonly RandomLookClass _randomLook;

	private readonly SteerPriorityClass _steerPriorityClass;

	public ESteerPriority CurrentSteerPriority => _steerPriorityClass.CurrentSteerPriority;

	public ESteerPriority LastSteerPriority => _steerPriorityClass.LastSteerPriority;

	public EEnemySteerDir EnemySteerDir { get; private set; }

	public Vector3 WeaponRootOffset => new Vector3(0f, base.BotOwner.WeaponRoot.position.y - base.Bot.Position.y - 0.1f, 0f);

	public HeardSoundSteeringClass HeardSoundSteering { get; }

	private Vector3 _lookDirection => base.Bot.LookDirection;

	public SAINSteeringClass(BotComponent sain)
		: base(sain)
	{
		base.TickRequirement = ESAINTickState.OnlyNoSleep;
		_randomLook = new RandomLookClass(this);
		_steerPriorityClass = new SteerPriorityClass(this);
		HeardSoundSteering = new HeardSoundSteeringClass(this);
	}

	public bool SteerByPriority(Enemy enemy = null, bool lookRandom = true, bool ignoreRunningPath = false)
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if (enemy == null)
		{
			enemy = base.Bot.CurrentTarget.CurrentTargetEnemy;
		}
		switch (_steerPriorityClass.GetCurrentSteerPriority(lookRandom, ignoreRunningPath))
		{
		case ESteerPriority.RunningPath:
			return true;
		case ESteerPriority.Aiming:
			LookToPoint(base.Bot.Aim.EndTargetPoint());
			return true;
		case ESteerPriority.ManualShooting:
			LookToPoint(base.Bot.ManualShoot.ShootPosition + base.Bot.Info.WeaponInfo.Recoil.CurrentRecoilOffset);
			return true;
		case ESteerPriority.EnemyVisible:
			LookToEnemy(enemy);
			return true;
		case ESteerPriority.UnderFire:
			LookToUnderFirePos();
			return true;
		case ESteerPriority.LastHit:
			LookToLastHitPos();
			return true;
		case ESteerPriority.EnemyLastKnownLong:
		case ESteerPriority.EnemyLastKnown:
			if (!LookToLastKnownEnemyPosition(enemy))
			{
				LookToRandomPosition();
			}
			return true;
		case ESteerPriority.HeardThreat:
			HeardSoundSteering.LookToHeardPosition();
			return true;
		case ESteerPriority.Sprinting:
			return true;
		case ESteerPriority.RandomLook:
			LookToRandomPosition();
			return true;
		default:
			return false;
		}
	}

	public bool LookToLastKnownEnemyPosition(Enemy enemy)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (FindLastKnownTarget(enemy, out var Result))
		{
			LookToPoint(Result);
			return true;
		}
		return false;
	}

	public bool LookToMovingDirection(bool sprint = false)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		BotPathFollowerClass pathFollower = base.Bot.Mover.PathFollower;
		if (pathFollower.Moving)
		{
			LookToPoint(pathFollower.MoveData.CurrentCorner.Position);
			return true;
		}
		BotOwner botOwner = base.BotOwner;
		if (botOwner != null)
		{
			BotMover mover = botOwner.Mover;
			if (((mover != null) ? new bool?(mover.HasPathAndNoComplete) : ((bool?)null)) == true)
			{
				Vector3 currentCornerPoint = base.BotOwner.Mover.CurrentCornerPoint;
				Vector3 position = base.BotOwner.WeaponRoot.position;
				Vector3 val = currentCornerPoint + WeaponRootOffset - position;
				if (((Vector3)(ref val)).sqrMagnitude > 0.0625f)
				{
					BotSteering steering = base.BotOwner.Steering;
					if (steering != null)
					{
						steering.LookToDirection(val);
					}
					return true;
				}
			}
		}
		return false;
	}

	public void LookToPoint(Vector3 point)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		Vector3 direction = point - base.BotOwner.WeaponRoot.position;
		if (((Vector3)(ref direction)).sqrMagnitude < 1f)
		{
			direction = ((Vector3)(ref direction)).normalized;
		}
		LookToDirection(direction);
	}

	public void LookToDirection(Vector3 direction, bool flat = false)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (flat)
		{
			direction.y = 0f;
		}
		base.BotOwner.Steering.LookToDirection(direction, float.MaxValue);
	}

	public void LookToEnemy(Enemy enemy)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (enemy != null)
		{
			LookToPoint(enemy.EnemyPosition + WeaponRootOffset);
		}
	}

	public void LookToRandomPosition()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		Vector3? val = _randomLook.UpdateRandomLook();
		if (val.HasValue)
		{
			LookToPoint(val.Value);
		}
	}

	public float AngleToPointFromLookDir(Vector3 point)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = point - base.BotOwner.WeaponRoot.position;
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		return Vector3.Angle(_lookDirection, normalized);
	}

	public float AngleToDirectionFromLookDir(Vector3 direction)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Angle(_lookDirection, direction);
	}

	public override void Init()
	{
		HeardSoundSteering.Init();
		base.Init();
	}

	public override void ManualUpdate()
	{
		base.ManualUpdate();
		HeardSoundSteering.ManualUpdate();
	}

	public override void Dispose()
	{
		HeardSoundSteering.Dispose();
		base.Dispose();
	}

	public bool FindLastKnownTarget(Enemy enemy, out Vector3 Result)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (enemy == null)
		{
			this.EnemySteerDir = EEnemySteerDir.NullEnemy_ERROR;
			Result = Vector3.zero;
			return false;
		}
		if (enemy.FindLookPoint(out var Position, out var EnemySteerDir))
		{
			this.EnemySteerDir = EnemySteerDir;
			Result = Position;
			return true;
		}
		this.EnemySteerDir = EEnemySteerDir.None;
		Result = Vector3.zero;
		return false;
	}

	private void LookToUnderFirePos()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		LookToPoint(base.Bot.Memory.UnderFireFromPosition + WeaponRootOffset);
	}

	private void LookToLastHitPos()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		Enemy enemyWhoLastShotMe = _steerPriorityClass.EnemyWhoLastShotMe;
		if (enemyWhoLastShotMe != null)
		{
			if (FindLastKnownTarget(enemyWhoLastShotMe, out var Result))
			{
				LookToPoint(Result);
				return;
			}
			Vector3? lastShotPosition = enemyWhoLastShotMe.Status.LastShotPosition;
			if (lastShotPosition.HasValue)
			{
				LookToPoint(lastShotPosition.Value + WeaponRootOffset);
				return;
			}
		}
		LookToRandomPosition();
	}
}
