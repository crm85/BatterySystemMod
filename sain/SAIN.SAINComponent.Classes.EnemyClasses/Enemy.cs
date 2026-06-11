using System;
using System.Collections.Generic;
using EFT;
using SAIN.Components;
using SAIN.Components.BotComponentSpace.Classes.EnemyClasses;
using SAIN.Components.PlayerComponentSpace;
using SAIN.Components.PlayerComponentSpace.PersonClasses;
using SAIN.Helpers;
using SAIN.Models.Enums;
using SAIN.Preset.GlobalSettings;
using SAIN.Preset.GlobalSettings.Categories;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.EnemyClasses;

public class Enemy : BotBase
{
	private const float ENEMY_UPDATEFREQUENCY_MAX_SCALE = 5f;

	private const float ENEMY_UPDATEFREQUENCY_MAX_DIST = 500f;

	private const float ENEMY_UPDATEFREQUENCY_MIN_DIST = 50f;

	private const float SQUADREPORT_SIGHT_INTERVAL = 0.5f;

	private Vector3 _moveDirection;

	private float _nextCalcMoveDirTime;

	private bool _visPathPointIsCorner;

	public float NextCheckFlashLightTime;

	private float _nextUpdateCoefTime;

	private bool _hasBeenActive;

	private Vector3? _centerMass;

	private float _nextGetCenterTime;

	private float _nextReportSightTime;

	private float _timeLastActive;

	public string EnemyName { get; }

	public string EnemyProfileId { get; }

	public PlayerComponent EnemyPlayerComponent { get; }

	public PersonClass EnemyPerson { get; }

	public IPlayer EnemyIPlayer { get; private set; }

	public Player EnemyPlayer { get; private set; }

	public PersonTransformClass EnemyTransform { get; }

	public OtherPlayerData EnemyPlayerData { get; }

	public bool IsAI => EnemyPlayer.IsAI;

	public bool IsZombie => EnemyPlayer.UsedSimplifiedSkeleton;

	public EnemyEvents Events { get; }

	public EnemyKnownPlaces KnownPlaces { get; private set; }

	public SAINEnemyStatus Status { get; }

	public EnemyVisionClass Vision { get; }

	public SAINEnemyPath Path { get; }

	public EnemyInfo EnemyInfo { get; }

	public EnemyAim Aim { get; }

	public EnemyHearing Hearing { get; }

	public bool IsCurrentEnemy { get; private set; }

	public float RealDistance => EnemyPlayerData.DistanceData.Distance;

	public bool IsSniper { get; private set; }

	public Vector3? VisiblePathPoint { get; private set; }

	public float VisiblePathPointDistanceToBot { get; private set; }

	public float VisiblePathPointDistanceToEnemyLastKnown { get; private set; }

	public int? VisiblePathCornerIndex { get; private set; }

	public float? VisiblePathPointSignedAngle { get; private set; }

	public Vector3? SuppressionTarget
	{
		get
		{
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			if (!GlobalSettingsClass.Instance.Mind.TARGET_SUPPRESS_TOGGLE)
			{
				return null;
			}
			Vector3? lastKnownPosition = KnownPlaces.LastKnownPosition;
			if (!lastKnownPosition.HasValue)
			{
				return null;
			}
			if (GetVisibilePathPoint(out var pathPoint) && IsTargetInSuppRange(lastKnownPosition.Value, pathPoint))
			{
				return pathPoint;
			}
			return null;
		}
	}

	public bool EnemyKnown => Events.OnEnemyKnownChanged.Value;

	public bool EnemyNotLooking => IsVisible && !Status.EnemyLookingAtMe && !Status.ShotAtMeRecently;

	public bool WasValid => ValidChecker.WasValid;

	public Vector3? CenterMass
	{
		get
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			if (EnemyIPlayer == null)
			{
				return null;
			}
			if (_nextGetCenterTime < Time.time)
			{
				_nextGetCenterTime = Time.time + 0.05f;
				_centerMass = FindCenterMass();
			}
			return _centerMass;
		}
	}

	public HashSet<EEnemyTag> Tags { get; } = new HashSet<EEnemyTag>();

	public bool FirstContactOccured => Vision.FirstContactOccured;

	public bool FirstContactReported { get; set; }

	public EPathDistance EPathDistance => Path.EPathDistance;

	public Vector3? LastKnownPosition => KnownPlaces.LastKnownPosition;

	public Vector3 EnemyMoveDirection
	{
		get
		{
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			if (_nextCalcMoveDirTime < Time.time)
			{
				_nextCalcMoveDirTime = Time.time + 0.1f;
				Vector2 movementDirection = EnemyPlayer.MovementContext.MovementDirection;
				Vector3 val = default(Vector3);
				((Vector3)(ref val))._002Ector(movementDirection.x, 0f, movementDirection.y);
				if (EnemyTransform.VelocityMagnitudeNormal > 0.01f)
				{
					LastMoveDirection = val;
					if (EnemyPlayer.IsSprintEnabled)
					{
						LastSprintDirection = val;
					}
				}
				_moveDirection = val;
			}
			return _moveDirection;
		}
	}

	public Vector3 LastMoveDirection { get; private set; }

	public Vector3 LastSprintDirection { get; private set; }

	public Vector3 EnemyPosition => EnemyTransform.Position;

	public Vector3 EnemyDirection => EnemyPlayerData.DistanceData.Direction;

	public Vector3 EnemyDirectionNormal => EnemyPlayerData.DistanceData.DirectionNormal;

	public Vector3 EnemyHeadPosition => EnemyTransform.HeadPosition;

	public float TimeSinceLastKnownUpdated => KnownPlaces.TimeSinceLastKnownUpdated;

	public bool InLineOfSight => Vision.InLineOfSight;

	public bool IsVisible => Vision.IsVisible;

	public bool CanShoot => Vision.CanShoot;

	public bool Seen => Vision.Seen;

	public bool Heard => Hearing.Heard;

	public bool EnemyLookingAtMe => Status.EnemyLookingAtMe;

	public float TimeSinceSeen => Vision.TimeSinceSeen;

	public float TimeSinceHeard => Hearing.TimeSinceHeard;

	public float UpdateFrequencyCoef { get; private set; }

	public float UpdateFrequencyCoefNormal { get; private set; }

	public float TimeSinceCurrentEnemy => _hasBeenActive ? (Time.time - _timeLastActive) : float.MaxValue;

	private EnemyKnownChecker KnownChecker { get; }

	private EnemyActiveThreatChecker ActiveThreatChecker { get; }

	private EnemyValidChecker ValidChecker { get; }

	public event Action OnEnemyDisposed;

	public override void ManualUpdate()
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		IsCurrentEnemy = base.Bot.CurrentTarget.CurrentTargetEnemy == this;
		CalcFrequencyCoef();
		UpdateDistAndDirection();
		UpdateSniperStatus();
		KnownChecker.ManualUpdate();
		ActiveThreatChecker.ManualUpdate();
		UpdateActiveState();
		Vision.ManualUpdate();
		KnownPlaces.ManualUpdate();
		Path.ManualUpdate();
		Status.ManualUpdate();
		if (IsCurrentEnemy && GetVisibilePathPoint(out var pathPoint))
		{
			DebugGizmos.Sphere(pathPoint, 0.06f, Color.red, 0.02f);
			DebugGizmos.Line(pathPoint, base.Bot.Transform.HeadPosition, Color.red, 0.015f, 0.02f);
		}
		base.ManualUpdate();
	}

	public Enemy(BotComponent bot, PlayerComponent enemyComponent, EnemyInfo enemyInfo)
		: base(bot)
	{
		EnemyPlayerComponent = enemyComponent;
		EnemyIPlayer = enemyComponent.IPlayer;
		EnemyPlayer = enemyComponent.Player;
		EnemyPerson = enemyComponent.Person;
		EnemyTransform = enemyComponent.Transform;
		EnemyName = enemyComponent.Name + " (" + enemyComponent.Person.Nickname + ")";
		EnemyInfo = enemyInfo;
		EnemyProfileId = enemyComponent.ProfileId;
		EnemyPlayerData = bot.PlayerComponent.OtherPlayersData.DataDictionary[enemyComponent.ProfileId];
		Events = new EnemyEvents(this);
		EnemyEvents.EnemyToggleEventTimeTracked onEnemyKnownChanged = Events.OnEnemyKnownChanged;
		onEnemyKnownChanged.OnToggle = (Action<bool, Enemy>)Delegate.Combine(onEnemyKnownChanged.OnToggle, new Action<bool, Enemy>(OnEnemyKnownChanged));
		ActiveThreatChecker = new EnemyActiveThreatChecker(this);
		ValidChecker = new EnemyValidChecker(this);
		KnownChecker = new EnemyKnownChecker(this);
		Status = new SAINEnemyStatus(this);
		Vision = new EnemyVisionClass(this);
		Path = new SAINEnemyPath(this);
		KnownPlaces = new EnemyKnownPlaces(this);
		Aim = new EnemyAim(this);
		Hearing = new EnemyHearing(this);
		UpdateDistAndDirection();
	}

	public override void Init()
	{
		Events.Init();
		ValidChecker.Init();
		KnownChecker.Init();
		ActiveThreatChecker.Init();
		KnownPlaces.Init();
		Vision.Init();
		Path.Init();
		Hearing.Init();
		Status.Init();
		base.Init();
	}

	public override void Dispose()
	{
		this.OnEnemyDisposed?.Invoke();
		Events.Dispose();
		EnemyEvents.EnemyToggleEventTimeTracked onEnemyKnownChanged = Events.OnEnemyKnownChanged;
		onEnemyKnownChanged.OnToggle = (Action<bool, Enemy>)Delegate.Remove(onEnemyKnownChanged.OnToggle, new Action<bool, Enemy>(OnEnemyKnownChanged));
		ValidChecker.Dispose();
		KnownChecker.Dispose();
		ActiveThreatChecker.Dispose();
		KnownPlaces.Dispose();
		Vision.Dispose();
		Path.Dispose();
		Hearing.Dispose();
		Status.Dispose();
		base.Dispose();
	}

	private void OnEnemyKnownChanged(bool value, Enemy enemy)
	{
		if (!value)
		{
			ClearVisiblePathPoint();
			IsSniper = false;
			FirstContactReported = false;
			IsCurrentEnemy = false;
		}
	}

	private void UpdateVisiblePathPointDist(Vector3 headPosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		TryUpdateVisPathDist(headPosition, VisiblePathPoint, LastKnownPosition, out var distToBot, out var distToEnemy);
		VisiblePathPointDistanceToBot = distToBot;
		VisiblePathPointDistanceToEnemyLastKnown = distToEnemy;
	}

	private static void TryUpdateVisPathDist(Vector3 headPosition, Vector3? visPathPoint, Vector3? lastKnown, out float distToBot, out float distToEnemy)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		distToBot = float.MaxValue;
		distToEnemy = float.MaxValue;
		if (visPathPoint.HasValue && lastKnown.HasValue)
		{
			Vector3 val = visPathPoint.Value - headPosition;
			distToBot = ((Vector3)(ref val)).magnitude;
			val = visPathPoint.Value - lastKnown.Value;
			distToEnemy = ((Vector3)(ref val)).magnitude;
		}
	}

	public void SetIsCurrentEnemy(bool value)
	{
		IsCurrentEnemy = value;
	}

	public bool GetVisibilePathPoint(out Vector3 pathPoint)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (VisiblePathPoint.HasValue)
		{
			pathPoint = VisiblePathPoint.Value;
			if (_visPathPointIsCorner)
			{
				pathPoint += base.Bot.Steering.WeaponRootOffset;
			}
			return true;
		}
		pathPoint = Vector3.zero;
		return false;
	}

	public bool AddTag(EEnemyTag tag)
	{
		return Tags.Add(tag);
	}

	public bool RemoveTag(EEnemyTag tag)
	{
		return Tags.Remove(tag);
	}

	public bool HasTag(EEnemyTag tag)
	{
		return Tags.Contains(tag);
	}

	public void ClearVisiblePathPoint()
	{
		_visPathPointIsCorner = false;
		VisiblePathPoint = null;
		VisiblePathPointSignedAngle = null;
		VisiblePathCornerIndex = null;
		VisiblePathPointDistanceToBot = float.MaxValue;
		VisiblePathPointDistanceToEnemyLastKnown = float.MaxValue;
	}

	public void SetLastVisiblePathPoint(Vector3 Point, int CornerIndex)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		_visPathPointIsCorner = false;
		VisiblePathPoint = Point;
		VisiblePathCornerIndex = CornerIndex;
		Vector3? lastKnownPosition = LastKnownPosition;
		UpdateVisiblePathPointDist(base.Bot.Transform.EyePosition);
		if (lastKnownPosition.HasValue)
		{
			Vector3 position = base.Bot.Position;
			Vector3 eyePosition = base.Bot.Transform.EyePosition;
			position.y = eyePosition.y;
			VisiblePathPointSignedAngle = Vector.FindFlatSignedAngle(Point, lastKnownPosition.Value, position);
		}
	}

	public void SetLastCornerAsVisiblePathPoint(Vector3 LastCorner, int CornerIndex)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		_visPathPointIsCorner = true;
		VisiblePathPoint = LastCorner;
		VisiblePathCornerIndex = CornerIndex;
		VisiblePathPointSignedAngle = null;
		UpdateVisiblePathPointDist(base.Bot.Transform.EyePosition);
	}

	public bool FindLookPoint(out Vector3 Position, out EEnemySteerDir EnemySteerDir)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		if (IsVisible)
		{
			EnemySteerDir = EEnemySteerDir.VisibleEnemyPos;
			Position = EnemyPosition + base.Bot.Steering.WeaponRootOffset;
			return true;
		}
		if (GetVisibilePathPoint(out Position))
		{
			EnemySteerDir = EEnemySteerDir.PathNode;
			return true;
		}
		EnemyKnownPlaces knownPlaces = KnownPlaces;
		EnemyPlace lastKnownPlace = knownPlaces.LastKnownPlace;
		if (lastKnownPlace == null)
		{
			EnemySteerDir = EEnemySteerDir.NullLastKnown_ERROR;
			return false;
		}
		EnemyPlace lastSeenPlace = knownPlaces.LastSeenPlace;
		if (lastSeenPlace != null)
		{
			if (lastSeenPlace != lastKnownPlace)
			{
				Vector3 val = lastSeenPlace.Position - lastKnownPlace.Position;
				if (!(((Vector3)(ref val)).sqrMagnitude < GlobalSettingsClass.Instance.Steering.STEER_LASTSEEN_TO_LASTKNOWN_DISTANCE.Sqr()))
				{
					goto IL_00ea;
				}
			}
			EnemySteerDir = EEnemySteerDir.LastSeenPos;
			Position = lastSeenPlace.Position + base.Bot.Steering.WeaponRootOffset;
			return true;
		}
		goto IL_00ea;
		IL_00ea:
		EnemySteerDir = EEnemySteerDir.LastKnownPos;
		Position = lastKnownPlace.Position + base.Bot.Steering.WeaponRootOffset;
		return true;
	}

	public bool CheckValid()
	{
		return ValidChecker.CheckValid();
	}

	private bool IsTargetInSuppRange(Vector3 target, Vector3 suppressPoint)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		MindSettings mind = GlobalSettingsClass.Instance.Mind;
		Vector3 val = target - suppressPoint;
		float sqrMagnitude = ((Vector3)(ref val)).sqrMagnitude;
		if (sqrMagnitude <= mind.TARGET_SUPPRESS_DIST.Sqr())
		{
			return true;
		}
		if (sqrMagnitude > mind.TARGET_SUPPRESS_DIST_MAX.Sqr())
		{
			return false;
		}
		Vector3 val2 = suppressPoint - base.Bot.Position;
		Vector3 val3 = target - base.Bot.Position;
		float num = Vector3.Angle(((Vector3)(ref val2)).normalized, ((Vector3)(ref val3)).normalized);
		if (num < mind.MAX_TARGET_SUPPRESS_ANGLE.Sqr())
		{
			return true;
		}
		return false;
	}

	private void UpdateDistAndDirection()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			EnemyInfo.Direction = EnemyDirection;
			EnemyInfo.Distance = RealDistance;
		}
		catch
		{
		}
	}

	private void UpdateSniperStatus()
	{
		if (IsSniper)
		{
			if (!EnemyKnown)
			{
				IsSniper = false;
			}
			else
			{
				IsSniper = RealDistance < BotBase.GlobalSettings.Mind.ENEMYSNIPER_DISTANCE_END;
			}
		}
	}

	private void CalcFrequencyCoef()
	{
		if (_nextUpdateCoefTime < Time.time)
		{
			_nextUpdateCoefTime = Time.time + 0.1f;
			UpdateFrequencyCoef = CalcUpdateFrequencyCoef(out var normal);
			UpdateFrequencyCoefNormal = normal;
		}
	}

	private float CalcUpdateFrequencyCoef(out float normal)
	{
		float realDistance = RealDistance;
		float num = 50f;
		if (realDistance <= num)
		{
			normal = 0f;
			return 1f;
		}
		float num2 = 500f;
		if (realDistance >= num2)
		{
			normal = 1f;
			return num2;
		}
		float num3 = num2 - num;
		float num4 = realDistance - num;
		normal = num4 / num3;
		return Mathf.Lerp(1f, 5f, normal);
	}

	private void UpdateActiveState()
	{
		if (IsCurrentEnemy && !_hasBeenActive)
		{
			_hasBeenActive = true;
		}
		if (IsCurrentEnemy || IsVisible || Status.HeardRecently)
		{
			_timeLastActive = Time.time;
		}
	}

	private Vector3 FindCenterMass()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		PlayerComponent enemyPlayerComponent = EnemyPlayerComponent;
		Vector3 position = enemyPlayerComponent.Player.MainParts[(BodyPartType)0].Position;
		Vector3 position2 = enemyPlayerComponent.Position;
		return Vector3.Lerp(position, position2, SAINPlugin.LoadedPreset.GlobalSettings.Aiming.CenterMassVal);
	}

	public void UpdateLastSeenPosition(Vector3 position)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		EnemyPlace place = KnownPlaces.UpdateSeenPlace(position);
		base.Bot.Squad.SquadInfo?.ReportEnemyPosition(this, place, seen: true);
	}

	public void UpdateCurrentEnemyPos(Vector3 position)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		EnemyPlace place = KnownPlaces.UpdateSeenPlace(position);
		if (_nextReportSightTime < Time.time)
		{
			_nextReportSightTime = Time.time + 0.5f;
			base.Bot.Squad.SquadInfo?.ReportEnemyPosition(this, place, seen: true);
		}
	}

	public void EnemyPositionReported(EnemyPlace place, bool seen)
	{
		if (seen)
		{
			KnownPlaces.UpdateSquadSeenPlace(place);
		}
		else
		{
			KnownPlaces.UpdateSquadHeardPlace(place);
		}
	}

	public void SetEnemyAsSniper(bool isSniper)
	{
		IsSniper = isSniper;
		if (isSniper && base.Bot.Squad.BotInGroup && base.Bot.Talk.GroupTalk.FriendIsClose)
		{
			base.Bot.Talk.TalkAfterDelay((EPhraseTrigger)71, (ETagStatus)4, Random.Range(0.33f, 0.66f));
		}
	}
}
