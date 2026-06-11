using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SAIN.Classes.Coverfinder;
using SAIN.Components;
using SAIN.Components.CoverFinder;
using SAIN.Helpers;
using SAIN.Preset.BotSettings.SAINSettings.Categories;
using SAIN.Preset.GlobalSettings;
using SAIN.SAINComponent.Classes.EnemyClasses;
using SAIN.SAINComponent.Classes.Mover;
using UnityEngine;

namespace SAIN.SAINComponent.Classes;

public class SAINCoverClass : BotComponentClassBase
{
	private float _spottedTime;

	private bool _coverEntered;

	private GUIObject debugCoverObject;

	private GameObject debugCoverLine;

	private bool _hasLimbCover;

	private float _checkLimbsTime = 0f;

	private CoverPoint _coverInUse;

	public CoverPoint CoverInUse
	{
		get
		{
			return _coverInUse;
		}
		set
		{
			if (value != _coverInUse)
			{
				_coverInUse = value;
				this.OnNewCoverInUse?.Invoke(value);
			}
		}
	}

	public CoverPoint CoverPointImAt
	{
		get
		{
			CoverPoint coverInUse = CoverInUse;
			if (coverInUse == null)
			{
				return null;
			}
			if (checkMoving(coverInUse))
			{
				return null;
			}
			return coverInUse;
		}
	}

	public bool SpottedInCover => _spottedTime > Time.time;

	public bool HasCover => CoverInUse != null;

	public bool InCover => HasCover && !checkMoving(CoverInUse);

	public bool IsMovingToCover => HasCover && checkMoving(CoverInUse);

	public CoverFinderState CurrentCoverFinderState { get; private set; }

	public List<CoverPoint> CoverPoints => CoverFinder.CoverPoints;

	public CoverFinderComponent CoverFinder { get; private set; }

	public CoverPoint FallBackPoint => CoverFinder.FallBackPoint;

	public float LastHitInCoverTime { get; private set; }

	public float TimeSinceLastHitInCover => Time.time - LastHitInCoverTime;

	public event Action<CoverPoint> OnNewCoverInUse;

	public event Action<CoverPoint> OnEnterCover;

	public event Action OnSpottedInCover;

	public SAINCoverClass(BotComponent bot)
		: base(bot)
	{
		base.TickRequirement = ESAINTickState.OnlyNoSleep;
		CoverFinder = GClass6.GetOrAddComponent<CoverFinderComponent>((MonoBehaviour)(object)bot);
	}

	public override void Init()
	{
		CoverFinder.Init(base.Bot);
		base.Init();
	}

	public override void ManualUpdate()
	{
		base.ManualUpdate();
		bool flag = base.Bot.SAINLayersActive && base.Bot.Decision.HasDecision;
		ActivateCoverFinder(flag);
		if (flag)
		{
			checkEnterCover();
			createDebug();
		}
	}

	public override void Dispose()
	{
		try
		{
			CoverFinder?.Dispose();
		}
		catch
		{
		}
		base.Dispose();
	}

	public CoverPoint FindPointInDirection(Vector3 direction, float dotThreshold = 0.33f, float minDistance = 8f)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = base.Bot.Position;
		for (int i = 0; i < CoverPoints.Count; i++)
		{
			CoverPoint coverPoint = CoverPoints[i];
			if (coverPoint != null && !coverPoint.Spotted && !coverPoint.CoverData.IsBad)
			{
				Vector3 position2 = coverPoint.Position;
				Vector3 val = position - position2;
				if (((Vector3)(ref val)).sqrMagnitude > minDistance * minDistance && Vector3.Dot(((Vector3)(ref val)).normalized, ((Vector3)(ref direction)).normalized) > dotThreshold)
				{
					return coverPoint;
				}
			}
		}
		return null;
	}

	private void checkEnterCover()
	{
		CoverPoint coverPointImAt = CoverPointImAt;
		if (coverPointImAt == null)
		{
			_coverEntered = false;
		}
		else if (!_coverEntered)
		{
			_coverEntered = true;
			this.OnEnterCover?.Invoke(coverPointImAt);
		}
	}

	private bool checkMoving([NotNull] CoverPoint cover)
	{
		if (!isMovingTo(cover, cover.StraightDistanceStatus))
		{
			return false;
		}
		if (!isMovingTo(cover, cover.PathDistanceStatus))
		{
			return false;
		}
		return true;
	}

	private bool isMovingTo(CoverPoint point, CoverStatus status)
	{
		if ((uint)(status - 1) <= 2u)
		{
			return true;
		}
		return false;
	}

	private void createDebug()
	{
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (SAINPlugin.DebugMode)
		{
			if (CoverInUse != null)
			{
				if (debugCoverObject == null)
				{
					debugCoverObject = DebugGizmos.CreateLabel(CoverInUse.Position, "Cover In Use");
					debugCoverLine = DebugGizmos.Line(CoverInUse.Position, base.Bot.Position + Vector3.up, 0.075f, -1f, taperLine: true);
				}
				debugCoverObject.WorldPos = CoverInUse.Position;
				DebugGizmos.UpdatePositionLine(CoverInUse.Position, base.Bot.Position + Vector3.up, debugCoverLine);
			}
		}
		else if (debugCoverObject != null)
		{
			DebugGizmos.DestroyLabel(debugCoverObject);
			debugCoverObject = null;
		}
	}

	public void GetHit(DamageInfoStruct DamageInfoStruct, EBodyPart bodyPart, float floatVal)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (InCover)
		{
			bool spotted = CoverInUse.Spotted;
			LastHitInCoverTime = Time.time;
			CoverInUse.GetHit(DamageInfoStruct, bodyPart, base.Bot.Enemy);
			if (CoverInUse.Spotted && !spotted)
			{
				_spottedTime = Time.time + 2f;
				this.OnSpottedInCover?.Invoke();
			}
		}
	}

	public void ActivateCoverFinder(bool value)
	{
		if (value)
		{
			CoverFinder?.LookForCover();
			CurrentCoverFinderState = CoverFinderState.on;
		}
		if (!value)
		{
			CoverFinder?.StopLooking();
			CurrentCoverFinderState = CoverFinderState.off;
		}
	}

	public void CheckResetCoverInUse()
	{
		CoverPoint coverInUse = base.Bot.Cover.CoverInUse;
		if (coverInUse != null && coverInUse.CoverData.IsBad)
		{
			base.Bot.Cover.CoverInUse = null;
			return;
		}
		ECombatDecision currentCombatDecision = base.Bot.Decision.CurrentCombatDecision;
		if (currentCombatDecision != ECombatDecision.MoveToCover && currentCombatDecision != ECombatDecision.RunToCover && currentCombatDecision != ECombatDecision.Retreat && currentCombatDecision != ECombatDecision.HoldInCover && currentCombatDecision != ECombatDecision.ShiftCover)
		{
			base.Bot.Cover.CoverInUse = null;
		}
	}

	public void SortPointsByPathDist()
	{
		CoverFinderComponent.OrderPointsByPathDist(CoverPoints);
	}

	public bool DuckInCover(Enemy enemy)
	{
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		CoverPoint coverInUse = CoverInUse;
		if (coverInUse != null)
		{
			SAINMoverClass mover = base.Bot.Mover;
			ProneClass prone = mover.Prone;
			SAINMoveSettings move = base.Bot.Info.FileSettings.Move;
			MoveSettings move2 = GlobalSettingsClass.Instance.Move;
			bool flag = move.PRONE_TOGGLE && move2.PRONE_TOGGLE && prone.ShallProneHide(enemy);
			if (flag && (base.Bot.Decision.CurrentSelfDecision != ESelfDecision.None || (move.PRONE_SUPPRESS_TOGGLE && base.Bot.Suppression.IsHeavySuppressed)))
			{
				prone.SetProne(value: true);
				return true;
			}
			if (mover.Pose.SetPoseToCover())
			{
				return true;
			}
			if (flag)
			{
				Bounds bounds = coverInUse.Collider.bounds;
				if (((Bounds)(ref bounds)).size.y < 0.5f)
				{
					prone.SetProne(value: true);
					return true;
				}
			}
		}
		return false;
	}

	public bool CheckLimbsForCover()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		Enemy enemy = base.Bot.Enemy;
		if (enemy != null && enemy.IsVisible)
		{
			if (_checkLimbsTime < Time.time)
			{
				_checkLimbsTime = Time.time + 0.1f;
				bool hasLimbCover = false;
				Vector3 position = enemy.EnemyIPlayer.WeaponRoot.position;
				if (CheckLimbForCover((BodyPartType)4, position, 3f) || CheckLimbForCover((BodyPartType)2, position, 3f))
				{
					hasLimbCover = true;
				}
				else if (CheckLimbForCover((BodyPartType)5, position, 3f) || CheckLimbForCover((BodyPartType)3, position, 3f))
				{
					hasLimbCover = true;
				}
				_hasLimbCover = hasLimbCover;
			}
		}
		else
		{
			_hasLimbCover = false;
		}
		return _hasLimbCover;
	}

	private bool CheckLimbForCover(BodyPartType bodyPartType, Vector3 target, float dist = 2f)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = base.BotOwner.MainParts[bodyPartType].Position;
		Vector3 val = target - position;
		return Physics.Raycast(position, val, dist, LayerMask.op_Implicit(LayerMaskClass.HighPolyWithTerrainMask));
	}

	public bool BotIsAtCoverInUse()
	{
		return CoverInUse?.BotInThisCover ?? false;
	}
}
