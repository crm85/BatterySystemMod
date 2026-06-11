using System;
using System.Collections;
using EFT;
using SAIN.Components;
using SAIN.Components.PlayerComponentSpace.PersonClasses;
using SAIN.Helpers;
using SAIN.Preset.GlobalSettings;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;
using UnityEngine.AI;

namespace SAIN.SAINComponent.Classes.Mover;

public class BotPathFollowerClass : BotBase
{
	private Coroutine _moveToPointCoroutine;

	private bool positionMoving;

	private Vector3 lastCheckPos;

	private float nextCheckPosTime;

	private float _timeNotMoving;

	private readonly BotMoveDataClass _moveData = new BotMoveDataClass();

	private static MoveSettings _moveSettings => SAINPlugin.LoadedPreset.GlobalSettings.Move;

	private float timeSinceNotMoving => positionMoving ? 0f : (Time.time - _timeNotMoving);

	public IBotMoveData MoveData => _moveData;

	public bool Moving => _moveToPointCoroutine != null;

	public bool Running => _moveToPointCoroutine != null && MoveData.WantToSprint;

	public BotPathFollowerClass(BotComponent sain)
		: base(sain)
	{
	}

	public void Pause(float duration)
	{
		if (duration > 0f)
		{
			_moveData.CurrentMoveStatus = EBotMoveStatus.Paused;
			_moveData.PauseTime = Time.time + duration;
		}
	}

	public void Unpause()
	{
		_moveData.PauseTime = -1f;
	}

	public void Cancel(float afterTime = -1f)
	{
		if (Moving)
		{
			if (afterTime <= 0f)
			{
				StopMoveCoroutine();
			}
			else if (_moveData.CurrentMoveStatus != EBotMoveStatus.Canceling)
			{
				_moveData.CurrentMoveStatus = EBotMoveStatus.Canceling;
				_moveData.CancelTime = Time.time + afterTime;
			}
		}
	}

	private void StopMoveCoroutine()
	{
		if (_moveToPointCoroutine != null)
		{
			((MonoBehaviour)base.Bot).StopCoroutine(_moveToPointCoroutine);
			_moveData.Dispose();
		}
	}

	public bool RunToPoint(Vector3 point, ESprintUrgency urgency, bool stopSprintEnemyVisible, bool checkSameWay = true, bool mustHaveCompletePath = true, Action callback = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (checkSameWay && TryUpdatePath(point, shallSprint: true))
		{
			_moveData.ShallStopSprintWhenSeeEnemy = stopSprintEnemyVisible;
			return true;
		}
		if (base.Bot.Mover.CanGoToPoint(point, out var path, mustHaveCompletePath))
		{
			TriggerNewMove(path.corners, point, shallSprint: true, urgency, callback);
			_moveData.ShallStopSprintWhenSeeEnemy = stopSprintEnemyVisible;
			return true;
		}
		return false;
	}

	public bool RunToPointByWay(Vector3[] way, ESprintUrgency urgency, bool stopSprintEnemyVisible, bool checkSameWay = true, Action callback = null)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (way == null)
		{
			return false;
		}
		if (way.Length <= 1)
		{
			return false;
		}
		Vector3 point = way[way.Length - 1];
		if (checkSameWay && TryUpdatePath(point, shallSprint: true))
		{
			_moveData.ShallStopSprintWhenSeeEnemy = stopSprintEnemyVisible;
			return true;
		}
		TriggerNewMove(way, point, shallSprint: true, urgency, callback);
		_moveData.ShallStopSprintWhenSeeEnemy = stopSprintEnemyVisible;
		return true;
	}

	public bool RunToPointByWay(NavMeshPath way, ESprintUrgency urgency, bool stopSprintEnemyVisible, bool checkSameWay = true, Action callback = null)
	{
		return RunToPointByWay((way != null) ? way.corners : null, urgency, stopSprintEnemyVisible, checkSameWay, callback);
	}

	public bool WalkToPoint(Vector3 point, bool checkSameWay = true, bool mustHaveCompletePath = true, Action callback = null)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (checkSameWay && TryUpdatePath(point, shallSprint: false))
		{
			return true;
		}
		if (base.Bot.Mover.CanGoToPoint(point, out var path, mustHaveCompletePath))
		{
			TriggerNewMove(path.corners, point, shallSprint: false, ESprintUrgency.None, callback);
			return true;
		}
		return false;
	}

	public bool WalkToPointByWay(Vector3[] way, bool checkSameWay = true, Action callback = null)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (way == null)
		{
			return false;
		}
		if (way.Length <= 1)
		{
			return false;
		}
		Vector3 point = way[way.Length - 1];
		if (checkSameWay && TryUpdatePath(point, shallSprint: false))
		{
			return true;
		}
		TriggerNewMove(way, point, shallSprint: false, ESprintUrgency.None, callback);
		return true;
	}

	public bool WalkToPointByWay(NavMeshPath way, bool checkSameWay = true, Action callback = null)
	{
		return WalkToPointByWay(way.corners, checkSameWay, callback);
	}

	private bool TryUpdatePath(Vector3 point, bool shallSprint)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (Moving && _moveData.TryUpdatePath(point))
		{
			PrepareBot(shallSprint);
			return true;
		}
		return false;
	}

	private void TriggerNewMove(Vector3[] path, Vector3 point, bool shallSprint, ESprintUrgency urgency, Action callback)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		StopMoveCoroutine();
		PrepareBot(shallSprint);
		_moveData.ActivateNewPath(point, shallSprint, urgency, path, 0.25f);
		_moveToPointCoroutine = ((MonoBehaviour)base.Bot).StartCoroutine(GoToPointCoRoutine(_moveData, callback));
	}

	private void PrepareBot(bool sprinting)
	{
		_moveData.WantToSprint = sprinting;
		base.BotOwner.Mover.Stop();
		if (sprinting)
		{
			base.Bot.Aim.LoseAimTarget();
			base.Bot.AimDownSightsController.SetADS(value: false);
			base.Bot.Mover.Prone.SetProne(value: false);
		}
	}

	public bool RecalcPath()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (_moveData.WantToSprint)
		{
			return RunToPoint(_moveData.Destination.Position, _moveData.SprintUrgency, stopSprintEnemyVisible: false);
		}
		return WalkToPoint(_moveData.Destination.Position, checkSameWay: false);
	}

	private IEnumerator GoToPointCoRoutine(BotMoveDataClass MoveData, Action callback = null)
	{
		positionMoving = true;
		yield return ExecutePath(MoveData);
		callback?.Invoke();
		StopMoveCoroutine();
	}

	private IEnumerator ExecutePath(BotMoveDataClass MoveData)
	{
		bool canTryVault = base.Bot.Info.FileSettings.Move.VAULT_TOGGLE && GlobalSettingsClass.Instance.Move.VAULT_TOGGLE;
		for (int i = 0; i <= MoveData.CornerCount; i++)
		{
			MoveData.CurrentIndex = i;
			BotCornerDetails corner;
			if (i == MoveData.CornerCount)
			{
				corner = MoveData.Destination;
				corner.SetStarted(Time.time);
				MoveData.Destination = corner;
			}
			else
			{
				corner = MoveData.PathCornerDetails[i];
				corner.SetStarted(Time.time);
				MoveData.PathCornerDetails[i] = corner;
			}
			MoveData.CurrentCorner = corner;
			Vector3 val = corner.Position - base.Bot.Position;
			MoveData.CurrentCornerDistanceSqr = ((Vector3)(ref val)).sqrMagnitude;
			while ((Object)(object)base.Bot != (Object)null && MoveData.CurrentCornerDistanceSqr > ReachDist(_moveData.WantToSprint))
			{
				if (!base.Bot.SAINLayersActive)
				{
					StopMoveCoroutine();
					yield break;
				}
				BotOwner botOwner = base.Bot.BotOwner;
				Player botPlayer = base.Bot.Player;
				PersonTransformClass botTransform = base.Bot.Transform;
				BotMover botOwnerMover = botOwner.Mover;
				SAINMoverClass sainMover = base.Bot.Mover;
				DoorOpener doorOpener = base.Bot.DoorOpener;
				if (doorOpener.Interacting)
				{
					_timeNotMoving = -1f;
					positionMoving = true;
					sainMover.Prone.SetProne(value: false);
					sainMover.EnableSprintPlayer(value: false);
					if (doorOpener.BreachingDoor)
					{
						yield return null;
						continue;
					}
				}
				if (MoveData.Paused && MoveData.CheckPaused())
				{
					_timeNotMoving = -1f;
					if (MoveData.WantToSprint)
					{
						base.Bot.Steering.SteerByPriority(null, lookRandom: true, ignoreRunningPath: true);
						base.Bot.Mover.EnableSprintPlayer(value: false);
					}
					yield return null;
					continue;
				}
				MoveToCurrentCorner(MoveData, out var recalcPath, base.Bot, botPlayer, botTransform, botOwnerMover, sainMover, doorOpener, canTryVault);
				if (recalcPath)
				{
					RecalcPath();
					yield break;
				}
				if (MoveData.Canceling && MoveData.CancelTime < Time.time)
				{
					StopMoveCoroutine();
					yield break;
				}
				yield return null;
			}
		}
	}

	private static float ReachDist(bool sprinting)
	{
		return sprinting ? SAINPlugin.LoadedPreset.GlobalSettings.Move.BotSprintCornerReachDist : SAINPlugin.LoadedPreset.GlobalSettings.Move.BotWalkCornerReachDist;
	}

	private void MoveToCurrentCorner(BotMoveDataClass MoveData, out bool recalcPath, BotComponent bot, Player botPlayer, PersonTransformClass botTransform, BotMover botOwnerMover, SAINMoverClass sainMover, DoorOpener doorOpener, bool canTryVault)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		BotCornerDetails currentCorner = _moveData.CurrentCorner;
		Enemy currentTargetEnemy = bot.CurrentTarget.CurrentTargetEnemy;
		Vector3 position = botTransform.Position;
		Vector3 val = position;
		NavMeshHit val2 = default(NavMeshHit);
		if (NavMesh.SamplePosition(position, ref val2, 0.5f, -1))
		{
			val.y = ((NavMeshHit)(ref val2)).position.y;
		}
		BotMoveDataClass moveData = _moveData;
		Vector3 val3 = currentCorner.Position - val;
		moveData.CurrentCornerDistanceSqr = ((Vector3)(ref val3)).sqrMagnitude;
		if (botOwnerMover.IsMoving)
		{
			botOwnerMover.Stop();
		}
		sainMover.Prone.SetProne(!MoveData.WantToSprint && sainMover.Crawling);
		if (SAINPlugin.DebugMode)
		{
			DrawMoverDebug(position, currentCorner.Position);
		}
		if (MoveData.WantToSprint)
		{
			HandleSprinting(MoveData, botPlayer.MovementContext, botTransform, currentTargetEnemy, doorOpener, botPlayer.Physical.Stamina.NormalValue);
		}
		else
		{
			MoveData.CurrentSprintStatus = EBotSprintStatus.None;
			MoveData.ShallSprintNow = false;
		}
		bot.Mover.EnableSprintPlayer(MoveData.ShallSprintNow);
		TrackMovement(position);
		float num = timeSinceNotMoving;
		if (num > _moveSettings.BotSprintRecalcTime && Time.time - MoveData.TimeStarted > 2f)
		{
			recalcPath = true;
			return;
		}
		if (canTryVault && num > _moveSettings.BotSprintTryVaultTime)
		{
			bot.Mover.TryVault();
		}
		base.Bot.Mover.MovePlayerCharacterToPoint(currentCorner.Position);
		SetPlayerSteering(MoveData, currentCorner.Position, bot, currentTargetEnemy);
		recalcPath = false;
	}

	private static void DrawMoverDebug(Vector3 BotPos, Vector3 destination)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.up * 0.6f;
		DebugGizmos.Sphere(destination, 0.2f, Color.white, 0.02f);
		DebugGizmos.Line(destination, destination + val, Color.white, 0.075f, 0.02f);
		DebugGizmos.Line(destination + val, BotPos + val, Color.white, 0.075f, 0.02f);
	}

	private static float FindStartSprintStamina(ESprintUrgency urgency)
	{
		if (1 == 0)
		{
		}
		float result;
		switch (urgency)
		{
		case ESprintUrgency.None:
		case ESprintUrgency.Low:
			result = 0.75f;
			break;
		case ESprintUrgency.Middle:
			result = 0.5f;
			break;
		case ESprintUrgency.High:
			result = 0.2f;
			break;
		default:
			result = 0.5f;
			break;
		}
		if (1 == 0)
		{
		}
		return result;
	}

	private static float FindEndSprintStamina(ESprintUrgency urgency)
	{
		if (1 == 0)
		{
		}
		float result;
		switch (urgency)
		{
		case ESprintUrgency.None:
		case ESprintUrgency.Low:
			result = 0.4f;
			break;
		case ESprintUrgency.Middle:
			result = 0.2f;
			break;
		case ESprintUrgency.High:
			result = 0.01f;
			break;
		default:
			result = 0.25f;
			break;
		}
		if (1 == 0)
		{
		}
		return result;
	}

	private static bool SprintCheck1(BotMoveDataClass MoveData, Enemy enemy)
	{
		if (MoveData.Canceling)
		{
			MoveData.CurrentSprintStatus = EBotSprintStatus.Canceling;
			MoveData.ShallSprintNow = false;
			return false;
		}
		if (MoveData.ShallStopSprintWhenSeeEnemy && enemy != null && enemy.IsVisible)
		{
			MoveData.CurrentSprintStatus = EBotSprintStatus.LookAtEnemyNoSprint;
			MoveData.ShallSprintNow = false;
			return false;
		}
		return true;
	}

	private static void HandleSprinting(BotMoveDataClass MoveData, MovementContext movementContext, PersonTransformClass botTransform, Enemy enemy, DoorOpener doorOpener, float staminaNormal)
	{
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		if (!SprintCheck1(MoveData, enemy))
		{
			return;
		}
		if (doorOpener.ShallPauseSprintForOpening())
		{
			MoveData.CurrentSprintStatus = EBotSprintStatus.InteractingWithDoor;
			MoveData.ShallSprintNow = false;
			return;
		}
		if (!movementContext.CanSprint)
		{
			MoveData.CurrentSprintStatus = EBotSprintStatus.CantSprint;
			MoveData.ShallSprintNow = false;
			return;
		}
		if (ShallPauseSprintStamina(staminaNormal, MoveData.SprintUrgency))
		{
			MoveData.CurrentSprintStatus = EBotSprintStatus.NoStamina;
			MoveData.ShallSprintNow = false;
			return;
		}
		bool isSprintEnabled = movementContext.IsSprintEnabled;
		if (CheckArrivingAtDestination(MoveData, isSprintEnabled))
		{
			MoveData.CurrentSprintStatus = EBotSprintStatus.ArrivingAtDestination;
			MoveData.ShallSprintNow = false;
		}
		else if (FindHorizontalAngleFromLookDir(botTransform.WeaponRoot, MoveData.CurrentCorner.Position, botTransform.LookDirection) >= _moveSettings.BotSprintCurrentCornerAngleMax)
		{
			MoveData.CurrentSprintStatus = EBotSprintStatus.Turning;
			MoveData.ShallSprintNow = false;
		}
		else if (MoveData.CurrentCornerDistanceSqr > 0.5f && ShallStartSprintStamina(staminaNormal, MoveData.SprintUrgency))
		{
			MoveData.CurrentSprintStatus = EBotSprintStatus.Running;
			MoveData.ShallSprintNow = true;
		}
	}

	private void HandleDumbShit(ref MovementContext movementContext, bool sprintingNow)
	{
		if (sprintingNow)
		{
			if (base.Bot.IsCheater)
			{
				movementContext.SprintSpeed = 50f;
			}
			else if (_moveSettings.EditSprintSpeed)
			{
				movementContext.SprintSpeed = 1.5f;
			}
		}
	}

	private static bool CheckArrivingAtDestination(BotMoveDataClass MoveData, bool sprintingNow)
	{
		EBotCornerType type = MoveData.CurrentCorner.Type;
		EBotCornerType eBotCornerType = type;
		if ((uint)(eBotCornerType - 5) > 1u)
		{
			return false;
		}
		float num = _moveSettings.BotSprintDistanceToStopSprintDestination.Sqr();
		float num2 = (sprintingNow ? num : (num * 1.1f));
		return MoveData.CurrentCornerDistanceSqr <= num2;
	}

	private static bool ShallPauseSprintStamina(float stamina, ESprintUrgency urgency)
	{
		return stamina <= FindEndSprintStamina(urgency);
	}

	private static bool ShallStartSprintStamina(float stamina, ESprintUrgency urgency)
	{
		return stamina >= FindStartSprintStamina(urgency);
	}

	private static float FindHorizontalAngleFromLookDir(Vector3 start, Vector3 end, Vector3 lookDirection)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = end - start;
		lookDirection.y = 0f;
		val.y = 0f;
		return Vector3.Angle(lookDirection, ((Vector3)(ref val)).normalized);
	}

	private void TrackMovement(Vector3 botPos)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (nextCheckPosTime < Time.time)
		{
			nextCheckPosTime = Time.time + _moveSettings.BotSprintNotMovingCheckFreq;
			Vector3 val = botPos - lastCheckPos;
			positionMoving = ((Vector3)(ref val)).sqrMagnitude > _moveSettings.BotSprintNotMovingThreshold;
			if (positionMoving)
			{
				_timeNotMoving = -1f;
				lastCheckPos = botPos;
			}
			else if (_timeNotMoving < 0f)
			{
				_timeNotMoving = Time.time;
			}
		}
	}

	private static void SetPlayerSteering(BotMoveDataClass MoveData, Vector3 target, BotComponent bot, Enemy enemy)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (MoveData.WantToSprint)
		{
			if (MoveData.ShallStopSprintWhenSeeEnemy && enemy != null && enemy.IsVisible)
			{
				bot.Steering.LookToEnemy(enemy);
			}
			else if (!ShallSteerbyPriority(MoveData) || !bot.Steering.SteerByPriority(enemy, lookRandom: false, ignoreRunningPath: true))
			{
				bot.Steering.LookToPoint(target + bot.Steering.WeaponRootOffset);
			}
		}
	}

	private static bool ShallSteerbyPriority(BotMoveDataClass MoveData)
	{
		if (MoveData.ShallSprintNow)
		{
			return false;
		}
		EBotSprintStatus currentSprintStatus = MoveData.CurrentSprintStatus;
		if (1 == 0)
		{
		}
		bool result = (uint)(currentSprintStatus - 1) > 2u;
		if (1 == 0)
		{
		}
		return result;
	}
}
