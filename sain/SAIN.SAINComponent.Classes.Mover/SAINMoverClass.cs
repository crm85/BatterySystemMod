using System.Collections;
using EFT;
using SAIN.Components;
using SAIN.Preset.GlobalSettings;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;
using UnityEngine.AI;

namespace SAIN.SAINComponent.Classes.Mover;

public class SAINMoverClass : BotComponentClassBase
{
	private float _ungroundedTime;

	private float _movingTime;

	private Vector3 _prevLinkPos;

	private readonly float _timeAfterJumpVaultReset = 1.25f;

	private bool _stopping;

	private float _nextJumpTime = 0f;

	private const float CHANGE_STANCE_INTERVAL = 0.33f;

	private float _nextChangeStanceTime;

	public DogFight DogFight { get; private set; }

	public BotPathFollowerClass PathFollower { get; private set; }

	public BlindFireController BlindFire { get; private set; }

	public SideStepClass SideStep { get; private set; }

	public LeanClass Lean { get; private set; }

	public PoseClass Pose { get; private set; }

	public ProneClass Prone { get; private set; }

	public NavMeshObstacle BotBodyObstacle { get; private set; }

	public bool Crawling { get; private set; }

	public Vector3 CurrentMoveDestination { get; private set; }

	public bool Moving
	{
		get
		{
			int result;
			if (!PathFollower.Moving)
			{
				BotMover mover = base.BotOwner.Mover;
				if (mover == null || !mover.IsMoving)
				{
					result = (base.BotOwner.Mover.HasPathAndNoComplete ? 1 : 0);
					goto IL_0039;
				}
			}
			result = 1;
			goto IL_0039;
			IL_0039:
			return (byte)result != 0;
		}
	}

	public NavMeshPathStatus CurrentPathStatus { get; private set; } = (NavMeshPathStatus)2;

	public float TimeLastJumped { get; private set; }

	public float TimeLastVaulted { get; private set; }

	private bool _wantLeftStance => Lean.LeanAngleValue.LastSmoothedValue < 0f;

	public SAINMoverClass(BotComponent sain)
		: base(sain)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		base.TickRequirement = ESAINTickState.OnlyNoSleep;
		BlindFire = new BlindFireController(sain);
		SideStep = new SideStepClass(sain);
		Lean = new LeanClass(sain);
		Prone = new ProneClass(sain);
		Pose = new PoseClass(sain);
		PathFollower = new BotPathFollowerClass(sain);
		DogFight = new DogFight(sain);
	}

	public override void ManualUpdate()
	{
		if (Crawling && !PathFollower.Moving)
		{
			Crawling = false;
		}
		Pose.ManualUpdate();
		Lean.ManualUpdate();
		BlindFire.ManualUpdate();
		if (!base.Player.IsSprintEnabled)
		{
			UpdateStance(Time.time);
		}
		base.ManualUpdate();
	}

	public void MovePlayerCharacterInDirection(Vector3 direction)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		base.PlayerComponent.SmoothController.SetTargetMoveDirection(direction, base.Player);
	}

	public void MovePlayerCharacterToPoint(Vector3 point)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		base.PlayerComponent.SmoothController.SetTargetMovePoint(point, base.Player);
	}

	private bool CanSetPatrol()
	{
		FirearmController firearmController = base.Bot.Transform.WeaponData.FirearmController;
		if ((Object)(object)firearmController != (Object)null && !((AbstractHandsController)firearmController).IsAiming && !firearmController.IsInReloadOperation() && !((AbstractHandsController)firearmController).IsInventoryOpen() && !((AbstractHandsController)firearmController).IsInInteractionStrictCheck() && !firearmController.IsInSpawnOperation() && !((AbstractHandsController)firearmController).IsHandsProcessing())
		{
			return true;
		}
		return false;
	}

	private void CheckSetBotToNavMesh()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		if ((int)base.Player.UpdateQueue > 0)
		{
			return;
		}
		if (PathFollower.Moving || base.BotOwner.Mover.HasPathAndNoComplete)
		{
			_movingTime = Time.time + 1f;
		}
		else if (!(_movingTime > Time.time) && !(Time.time - TimeLastJumped < _timeAfterJumpVaultReset) && !(Time.time - TimeLastVaulted < _timeAfterJumpVaultReset))
		{
			if (!base.Player.MovementContext.IsGrounded)
			{
				_ungroundedTime = Time.time + 1f;
			}
			else if (_ungroundedTime < Time.time)
			{
				ResetToNavMesh();
			}
		}
	}

	public void ResetToNavMesh()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		if (base.BotOwner.Mover == null)
		{
			Logger.LogWarning("Bot Mover Null");
			return;
		}
		Vector3 position = base.Bot.Position;
		Vector3 val = _prevLinkPos - position;
		if (((Vector3)(ref val)).sqrMagnitude > 0.01f)
		{
			Vector3 playerToNavMesh = position + Vector3.up * 0.3f;
			base.BotOwner.Mover.SetPlayerToNavMesh(playerToNavMesh);
			_prevLinkPos = position;
			base.BotOwner.Mover.PositionOnWayInner = ((GClass419)base.BotOwner.Mover).botOwner_0.Position;
			base.BotOwner.Mover.LocalAvoidance.DropOffset();
		}
	}

	public override void Dispose()
	{
		PathFollower?.Dispose();
		base.Dispose();
	}

	public bool GoToPoint(Vector3 point, out bool calculating, float reachDist = -1f, bool crawl = false, bool slowAtEnd = true, bool mustHaveCompletePath = true)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		calculating = false;
		if (PathFollower.WalkToPoint(point, checkSameWay: true, mustHaveCompletePath))
		{
			CurrentPathStatus = (NavMeshPathStatus)0;
			Crawling = crawl && base.Bot.Info.FileSettings.Move.PRONE_TOGGLE && GlobalSettingsClass.Instance.Move.PRONE_TOGGLE;
			Prone.SetProne(Crawling);
			CurrentMoveDestination = point;
			return true;
		}
		return false;
	}

	public bool RunToPoint(Vector3 point, ESprintUrgency urgency, bool stopSprintEnemyVisible, bool checkSameWay = true, bool mustHaveCompletePath = true)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (PathFollower.RunToPoint(point, urgency, stopSprintEnemyVisible, checkSameWay, mustHaveCompletePath))
		{
			CurrentPathStatus = (NavMeshPathStatus)0;
			Crawling = false;
			Prone.SetProne(value: false);
			CurrentMoveDestination = point;
			return true;
		}
		return false;
	}

	public bool GoToEnemy(Enemy enemy, float reachDist = -1f, bool crawl = false, bool mustHaveCompletePath = true)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Invalid comparison between Unknown and I4
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Invalid comparison between Unknown and I4
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		if (enemy == null)
		{
			return false;
		}
		if (reachDist < 0f)
		{
			reachDist = base.BotOwner.Settings.FileSettings.Move.REACH_DIST;
		}
		NavMeshPathStatus pathToEnemyStatus = enemy.Path.PathToEnemyStatus;
		NavMeshPathStatus val = pathToEnemyStatus;
		NavMeshPathStatus val2 = val;
		if ((int)val2 != 1)
		{
			if ((int)val2 == 2)
			{
				return false;
			}
		}
		else if (mustHaveCompletePath)
		{
			return false;
		}
		Vector3[] pathCorners = enemy.Path.PathCorners;
		if (pathCorners.Length >= 2)
		{
			Vector3 val3 = pathCorners[pathCorners.Length - 1] - base.Bot.Position;
			bool calculating;
			if (((Vector3)(ref val3)).sqrMagnitude < reachDist)
			{
				return GoToPoint(enemy.EnemyTransform.Position, out calculating, reachDist, crawl: false, slowAtEnd: true, mustHaveCompletePath: false);
			}
			CurrentPathStatus = pathToEnemyStatus;
			return GoToPointByWay(enemy.Path.PathToEnemy, reachDist, crawl);
		}
		CurrentPathStatus = (NavMeshPathStatus)2;
		return false;
	}

	public bool GoToPointByWay(NavMeshPath Path, float reachDist = -1f, bool crawl = false)
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		if (Path == null || Path.corners.Length < 2)
		{
			return false;
		}
		int num = Path.corners.Length;
		if (crawl && base.Bot.Info.FileSettings.Move.PRONE_TOGGLE && GlobalSettingsClass.Instance.Move.PRONE_TOGGLE)
		{
			Prone.SetProne(value: true);
		}
		if (PathFollower.WalkToPointByWay(Path))
		{
			CurrentMoveDestination = Path.corners[num - 1];
			return true;
		}
		return false;
	}

	public bool CanGoToPoint(Vector3 point, out NavMeshPath path, bool mustHaveCompletePath = true, float navSampleRange = 1f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Invalid comparison between Unknown and I4
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Invalid comparison between Unknown and I4
		NavMeshHit val = default(NavMeshHit);
		NavMeshHit val2 = default(NavMeshHit);
		if (NavMesh.SamplePosition(point, ref val, navSampleRange, -1) && NavMesh.SamplePosition(base.Bot.Transform.Position, ref val2, navSampleRange, -1))
		{
			path = new NavMeshPath();
			if (NavMesh.CalculatePath(((NavMeshHit)(ref val2)).position, ((NavMeshHit)(ref val)).position, -1, path) && path.corners.Length > 1)
			{
				if ((int)path.status == 2)
				{
					return false;
				}
				if (mustHaveCompletePath && (int)path.status > 0)
				{
					return false;
				}
				return true;
			}
		}
		path = null;
		return false;
	}

	public bool SetTargetPose(float pose)
	{
		return Pose.SetTargetPose(pose);
	}

	public void SetTargetMoveSpeed(float speed)
	{
		Pose.SetTargetSpeed(speed);
	}

	public void StopMove(float delay = 0.1f)
	{
		Player player = base.Player;
		if (player != null && player.IsSprintEnabled)
		{
			Sprint(value: false);
		}
		if (delay <= 0f)
		{
			Stop();
		}
		else if (!_stopping && base.Bot.Mover.PathFollower.Moving)
		{
			_stopping = true;
			((MonoBehaviour)base.Bot).StartCoroutine(StopAfterDelay(delay));
		}
	}

	private IEnumerator StopAfterDelay(float delay)
	{
		yield return (object)new WaitForSeconds(delay);
		Stop();
	}

	private void Stop()
	{
		base.Bot?.Mover.PathFollower.Cancel();
		_stopping = false;
	}

	public void PauseMovement(float forDuration)
	{
		if (forDuration > 0f)
		{
			PathFollower.Pause(forDuration);
		}
	}

	public void ResetPath(float delay)
	{
	}

	public void Sprint(bool value)
	{
		if (!((Object)(object)base.BotOwner == (Object)null))
		{
			EnableSprintPlayer(value);
		}
	}

	public void EnableSprintPlayer(bool value)
	{
		BotOwner botOwner = base.BotOwner;
		if (botOwner != null)
		{
			BotDoorOpener doorOpener = botOwner.DoorOpener;
			if (((doorOpener != null) ? new bool?(doorOpener.Interacting) : ((bool?)null)) == true)
			{
				value = false;
			}
		}
		if (value)
		{
			base.Player.MovementContext.SetTilt(0f, false);
		}
		base.Player.EnableSprint(value);
	}

	public bool TryJump()
	{
		if (_nextJumpTime < Time.time)
		{
			MovementContext movementContext = base.Player.MovementContext;
			if (movementContext != null && movementContext.CanJump)
			{
				_nextJumpTime = Time.time + 0.5f;
				MovementContext movementContext2 = base.Player.MovementContext;
				if (movementContext2 != null)
				{
					movementContext2.TryJump();
				}
				TimeLastJumped = Time.time;
				return true;
			}
		}
		return false;
	}

	public bool TryVault()
	{
		Player player = base.Player;
		int num;
		if (player == null)
		{
			num = 0;
		}
		else
		{
			MovementContext movementContext = player.MovementContext;
			num = ((((movementContext != null) ? new bool?(movementContext.TryVaulting()) : ((bool?)null)) == true) ? 1 : 0);
		}
		bool flag = (byte)num != 0;
		if (flag)
		{
			TimeLastVaulted = Time.time;
		}
		return flag;
	}

	private void UpdateStance(float time)
	{
		if (!(_nextChangeStanceTime < time))
		{
			return;
		}
		_nextChangeStanceTime = time + 0.33f;
		MovementContext movementContext = base.Player.MovementContext;
		if (movementContext == null)
		{
			return;
		}
		LeftStanceController leftStanceController = movementContext.LeftStanceController;
		bool flag = !movementContext.IsSprintEnabled && base.Bot.CurrentTarget.CurrentTargetEnemy == null;
		if (flag != movementContext._isInPatrol)
		{
			if (flag && leftStanceController != null && leftStanceController.LeftStance)
			{
				leftStanceController.ToggleLeftStance();
			}
			else
			{
				movementContext.SetPatrol(flag && CanSetPatrol());
			}
		}
		else if (leftStanceController != null && leftStanceController.LeftStance != _wantLeftStance)
		{
			leftStanceController.ToggleLeftStance();
		}
	}
}
