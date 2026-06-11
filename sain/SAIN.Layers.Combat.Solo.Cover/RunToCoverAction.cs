using System.Collections.Generic;
using System.Text;
using DrakiaXYZ.BigBrain.Brains;
using EFT;
using SAIN.Classes.Coverfinder;
using SAIN.Helpers;
using SAIN.Preset.GlobalSettings;
using SAIN.SAINComponent.Classes;
using SAIN.SAINComponent.Classes.EnemyClasses;
using SAIN.SAINComponent.Classes.Mover;
using UnityEngine;

namespace SAIN.Layers.Combat.Solo.Cover;

internal class RunToCoverAction : CombatAction, ISAINAction
{
	private bool _runFailed;

	private bool _wasCrawling;

	private bool _moveSuccess;

	private float _recalcMoveTimer;

	private float _jumpTimer;

	private bool _shallJumpToCover;

	private bool _sprinting;

	private Vector3 _runDestination;

	private bool isRunning => base.Bot.Mover.PathFollower.Running;

	public RunToCoverAction(BotOwner bot)
		: base(bot, "Run To Cover")
	{
	}

	public void Toggle(bool value)
	{
		ToggleAction(value);
	}

	public override void Update(ActionData data)
	{
		base.Bot.Mover.SetTargetMoveSpeed(1f);
		base.Bot.Mover.SetTargetPose(1f);
		checkJumpToCover();
		Enemy enemy = base.Bot.Enemy;
		UpdateCoverMove(enemy);
		if (!_moveSuccess)
		{
			base.Bot.Mover.DogFight.DogFightMove(aggressive: true, enemy);
		}
		if ((!_moveSuccess || !base.Bot.Mover.PathFollower.Running) && !base.Shoot.ShootAnyVisibleEnemies(enemy) && !base.Bot.Steering.SteerByPriority(enemy, lookRandom: false))
		{
			Enemy currentTargetEnemy = base.Bot.CurrentTarget.CurrentTargetEnemy;
			if (!base.Shoot.ShootAnyVisibleEnemies(currentTargetEnemy) && !base.Bot.Steering.SteerByPriority(currentTargetEnemy, lookRandom: false))
			{
				base.Bot.Steering.LookToMovingDirection();
			}
		}
	}

	private void UpdateCoverMove(Enemy enemy)
	{
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		if (_recalcMoveTimer < Time.time)
		{
			if (FindCoverToMoveTo(out var sprinting, out var coverDestination, tryWalk: false, enemy))
			{
				_runFailed = false;
				_moveSuccess = true;
			}
			else if (FindCoverToMoveTo(out sprinting, out coverDestination, tryWalk: true, enemy))
			{
				_moveSuccess = true;
				_runFailed = true;
			}
			else
			{
				base.Bot.Mover.EnableSprintPlayer(value: false);
				_moveSuccess = false;
				_runFailed = true;
			}
			_sprinting = sprinting;
			if (_moveSuccess)
			{
				_recalcMoveTimer = Time.time + 2f;
				_shallJumpToCover = EFTMath.RandomBool(10f) && _sprinting && ((CustomLogic)this).BotOwner.Memory.IsUnderFire && base.Bot.Info.Profile.IsPMC;
				base.Bot.Cover.CoverInUse = coverDestination;
				_runDestination = coverDestination.Position;
			}
			else
			{
				_recalcMoveTimer = Time.time + 0.25f;
				base.Bot.Cover.CoverInUse = null;
			}
		}
	}

	private void checkJumpToCover()
	{
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		if (!base.Bot.Info.FileSettings.Move.JUMP_TOGGLE || !GlobalSettingsClass.Instance.Move.JUMP_TOGGLE || !_shallJumpToCover || !_moveSuccess || !_sprinting || !base.Bot.Player.IsSprintEnabled || !(_jumpTimer < Time.time))
		{
			return;
		}
		CoverPoint coverInUse = base.Bot.Cover.CoverInUse;
		if (coverInUse != null)
		{
			Vector3 val = coverInUse.Position - base.Bot.Position;
			float sqrMagnitude = ((Vector3)(ref val)).sqrMagnitude;
			if (sqrMagnitude < 9f && sqrMagnitude > 2.25f)
			{
				_jumpTimer = Time.time + 5f;
				base.Bot.Mover.TryJump();
			}
		}
	}

	private bool FindCoverToMoveTo(out bool sprinting, out CoverPoint coverDestination, bool tryWalk, Enemy enemy)
	{
		CoverPoint coverInUse = base.Bot.Cover.CoverInUse;
		if (TryRunToCoverPoint(coverInUse, out sprinting, tryWalk, enemy))
		{
			coverDestination = coverInUse;
			return true;
		}
		ECombatDecision currentCombatDecision = base.Bot.Decision.CurrentCombatDecision;
		base.Bot.Cover.SortPointsByPathDist();
		sprinting = false;
		List<CoverPoint> coverPoints = base.Bot.Cover.CoverPoints;
		for (int i = 0; i < coverPoints.Count; i++)
		{
			CoverPoint coverPoint = coverPoints[i];
			if (TryRunToCoverPoint(coverPoint, out sprinting, tryWalk, enemy))
			{
				coverDestination = coverPoint;
				return true;
			}
		}
		coverDestination = null;
		return false;
	}

	private bool checkIfPointGoodEnough(CoverPoint coverPoint, float minDot = 0.1f)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if (coverPoint == null)
		{
			return false;
		}
		if (!coverPoint.CoverData.IsBad)
		{
			return true;
		}
		CoverStatus pathDistanceStatus = coverPoint.PathDistanceStatus;
		CoverStatus coverStatus = pathDistanceStatus;
		if ((uint)(coverStatus - 3) <= 1u)
		{
			return true;
		}
		Vector3 val = findTarget();
		if (val == Vector3.zero)
		{
			return true;
		}
		Vector3 protectionDirection = coverPoint.CoverData.ProtectionDirection;
		Vector3 val2 = val - coverPoint.Position;
		float num = Vector3.Dot(protectionDirection, ((Vector3)(ref val2)).normalized);
		return num > minDot;
	}

	private Vector3 findTarget()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		Vector3? grenadeDangerPoint = base.Bot.Grenade.GrenadeDangerPoint;
		if (grenadeDangerPoint.HasValue)
		{
			return grenadeDangerPoint.Value;
		}
		if (base.Bot.CurrentTargetPosition.HasValue)
		{
			return base.Bot.CurrentTargetPosition.Value;
		}
		return Vector3.zero;
	}

	private bool tooCloseToGrenade(Vector3 pos)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		Vector3? grenadeDangerPoint = base.Bot.Grenade.GrenadeDangerPoint;
		if (grenadeDangerPoint.HasValue)
		{
			Vector3 val = grenadeDangerPoint.Value - pos;
			if (((Vector3)(ref val)).sqrMagnitude < 9f)
			{
				return true;
			}
		}
		return false;
	}

	private bool TryRunToCoverPoint(CoverPoint coverPoint, out bool sprinting, bool tryWalk, Enemy enemy)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		sprinting = false;
		if (!checkIfPointGoodEnough(coverPoint))
		{
			return false;
		}
		Vector3 position = coverPoint.Position;
		if (!tryWalk && coverPoint.PathLength >= base.Bot.Info.FileSettings.Move.RUN_TO_COVER_MIN && base.Bot.Mover.RunToPoint(position, getUrgency(), stopSprintEnemyVisible: false))
		{
			_wasCrawling = false;
			sprinting = true;
			return true;
		}
		if (tryWalk)
		{
			bool crawl = base.Bot.Decision.CurrentSelfDecision != ESelfDecision.None && base.Bot.Player.MovementContext.CanProne && coverPoint.StraightDistanceStatus == CoverStatus.FarFromCover && (_wasCrawling || base.Bot.Mover.Prone.ShallProneHide(enemy));
			if (base.Bot.Mover.GoToPoint(position, out var _, -1f, crawl, slowAtEnd: false))
			{
				_wasCrawling = base.Bot.Mover.Crawling;
				return true;
			}
			_wasCrawling = base.Bot.Mover.Crawling;
		}
		return result;
	}

	private ESprintUrgency getUrgency()
	{
		return (((CustomLogic)this).BotOwner.Memory.IsUnderFire || base.Bot.Suppression.IsSuppressed || base.Bot.Decision.CurrentSelfDecision != ESelfDecision.None) ? ESprintUrgency.High : ESprintUrgency.Middle;
	}

	public override void Start()
	{
		Toggle(value: true);
		_recalcMoveTimer = 0f;
		_shallJumpToCover = false;
		_sprinting = false;
		_moveSuccess = false;
		_runFailed = false;
		_wasCrawling = false;
	}

	public override void Stop()
	{
		Toggle(value: false);
		base.Bot.Mover.DogFight.ResetDogFightStatus();
		base.Bot.Cover.CheckResetCoverInUse();
	}

	public override void BuildDebugText(StringBuilder stringBuilder)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		stringBuilder.AppendLine("Run To Cover Info");
		BotPathFollowerClass pathFollower = base.Bot.Mover.PathFollower;
		GClass1437.AppendLabeledValue(stringBuilder, "Move Success?", $"{_moveSuccess}", Color.white, Color.yellow, true);
		GClass1437.AppendLabeledValue(stringBuilder, "Run Success?", $"{!_runFailed}", Color.white, Color.yellow, true);
		DebugOverlay.AddMoveData(base.Bot, stringBuilder);
		SAINCoverClass cover = base.Bot.Cover;
		GClass1437.AppendLabeledValue(stringBuilder, "CoverFinder State", $"{cover.CurrentCoverFinderState}", Color.white, Color.yellow, true);
		GClass1437.AppendLabeledValue(stringBuilder, "Cover Count", $"{cover.CoverPoints.Count}", Color.white, Color.yellow, true);
		CoverPoint coverInUse = base.Bot.Cover.CoverInUse;
		if (coverInUse != null)
		{
			stringBuilder.AppendLine("CoverInUse");
			GClass1437.AppendLabeledValue(stringBuilder, "Is Bad?", $"{coverInUse.CoverData.IsBad}", Color.white, Color.yellow, true);
			GClass1437.AppendLabeledValue(stringBuilder, "Straight Status", $"{coverInUse.StraightDistanceStatus}", Color.white, Color.yellow, true);
			GClass1437.AppendLabeledValue(stringBuilder, "Straight Distance", $"{coverInUse.Distance}", Color.white, Color.yellow, true);
			GClass1437.AppendLabeledValue(stringBuilder, "Path Length Status", $"{coverInUse.PathDistanceStatus}", Color.white, Color.yellow, true);
			GClass1437.AppendLabeledValue(stringBuilder, "Path Length", $"{coverInUse.PathLength}", Color.white, Color.yellow, true);
			GClass1437.AppendLabeledValue(stringBuilder, "Path Calc Status", $"{coverInUse.PathToPoint.status}", Color.white, Color.yellow, true);
			Vector3? val = coverInUse.PathToPoint.LastCorner();
			if (val.HasValue)
			{
				Vector3 val2 = val.Value - coverInUse.Position;
				float magnitude = ((Vector3)(ref val2)).magnitude;
				val2 = val.Value - base.Bot.Position;
				GClass1437.AppendLabeledValue(stringBuilder, "Distance To Last Corner", $"{((Vector3)(ref val2)).magnitude}", Color.white, Color.yellow, true);
				val2 = val.Value - coverInUse.Position;
				GClass1437.AppendLabeledValue(stringBuilder, "Last Path Corner to Position Difference", $"{((Vector3)(ref val2)).magnitude}", Color.white, Color.yellow, true);
			}
			GClass1437.AppendLabeledValue(stringBuilder, "Height / Value", $"{coverInUse.CoverHeight} {coverInUse.HardData.Value}", Color.white, Color.yellow, true);
		}
	}
}
