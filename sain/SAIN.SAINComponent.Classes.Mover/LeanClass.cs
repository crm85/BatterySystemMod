using System.Linq;
using SAIN.Components;
using SAIN.Preset.GlobalSettings;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Mover;

public class LeanClass : BotBase
{
	private const float LEAN_UPDATE_FOUND_FREQ = 0.75f;

	private const float LEAN_UPDATE_NOT_FOUND_FREQ = 0.25f;

	private const float LEAN_RAYCAST_OFFSET_DIST = 0.66f;

	private const float LEAN_MAX_RAYCAST_DIST = 16f;

	private const float RESET_LEAN_AFTER_TIME = 1f;

	private const float MAX_CORNER_DISTANCE_LEAN = 30f;

	private const float MAX_CORNER_DISTANCE_LEAN_SQR = 900f;

	private static readonly ECombatDecision[] DontLean = new ECombatDecision[4]
	{
		ECombatDecision.Retreat,
		ECombatDecision.RunToCover,
		ECombatDecision.RunAway,
		ECombatDecision.MeleeAttack
	};

	private float _stopHoldLeanTime;

	private float _leanTimer = 0f;

	private float _timeLastLeaned;

	public LeanSetting LeanDirection { get; private set; }

	public LeanSetting LastLeanDirection { get; private set; }

	public bool CanLeanByState { get; private set; }

	public bool IsRaycastLeaning { get; private set; }

	public SmoothDampenedFloat LeanAngleValue { get; } = new SmoothDampenedFloat(0.2f);

	public bool IsHoldingLean => _stopHoldLeanTime > Time.time;

	public bool DirectLineOfSight { get; set; }

	public bool LeftLos { get; set; }

	public Vector3? LeftLosPos { get; set; }

	public bool LeftHalfLos { get; set; }

	public Vector3? LeftHalfLosPos { get; set; }

	public bool RightLos { get; set; }

	public Vector3? RightLosPos { get; set; }

	public bool RightHalfLos { get; set; }

	public Vector3? RightHalfLosPos { get; set; }

	public LeanClass(BotComponent sain)
		: base(sain)
	{
		base.TickInterval = 0.05f;
	}

	public override void ManualUpdate()
	{
		float time = Time.time;
		UpdateLeanSetting(time);
		SetTilt();
	}

	private void SetTilt()
	{
		if (base.Player.IsSprintEnabled)
		{
			base.Player.MovementContext.SetTilt(0f, false);
			return;
		}
		LeanSetting leanDirection = LeanDirection;
		if (1 == 0)
		{
		}
		float num = leanDirection switch
		{
			LeanSetting.Left => -5f, 
			LeanSetting.Right => 5f, 
			_ => 0f, 
		};
		if (1 == 0)
		{
		}
		float targetValue = num;
		LeanAngleValue.Set(targetValue);
		float num2 = LeanAngleValue.Get(GameWorldComponent.WorldTickDeltaTime);
		base.Player.MovementContext.SetTilt(num2, false);
	}

	public void FastLean(LeanSetting value)
	{
		if (value != LeanSetting.None)
		{
			_timeLastLeaned = Time.time;
		}
		if (LeanDirection != value)
		{
			LastLeanDirection = LeanDirection;
			LeanDirection = value;
		}
	}

	private void UpdateLeanSetting(float time)
	{
		if (!ShallTick(time))
		{
			return;
		}
		CanLeanByState = CheckCanLeanByState(out var resetLean);
		if (CanLeanByState)
		{
			if (_leanTimer < Time.time)
			{
				Enemy currentTargetEnemy = base.Bot.CurrentTarget.CurrentTargetEnemy;
				FindLean(currentTargetEnemy);
				float num = ((LeanDirection == LeanSetting.None) ? 0.25f : 0.75f);
				_leanTimer = Time.time + num;
			}
		}
		else if (resetLean)
		{
			ResetLean();
		}
	}

	private bool CheckCanLeanByState(out bool resetLean)
	{
		resetLean = true;
		if (!base.Bot.Info.FileSettings.Move.LEAN_TOGGLE || !GlobalSettingsClass.Instance.Move.LEAN_TOGGLE)
		{
			return false;
		}
		if (!base.Bot.SAINLayersActive)
		{
			return false;
		}
		if (base.Bot.Mover.PathFollower.Running)
		{
			return false;
		}
		ECombatDecision currentCombatDecision = base.Bot.Decision.CurrentCombatDecision;
		Enemy currentTargetEnemy = base.Bot.CurrentTarget.CurrentTargetEnemy;
		if (currentTargetEnemy == null || DontLean.Contains(currentCombatDecision) || base.Bot.Suppression.IsHeavySuppressed)
		{
			return false;
		}
		if (IsHoldingLean)
		{
			resetLean = false;
			return false;
		}
		if (currentTargetEnemy.IsVisible && base.Bot.Decision.CurrentSelfDecision != ESelfDecision.None)
		{
			return false;
		}
		if (GlobalSettingsClass.Instance.General.AILimit.LimitAIvsAIGlobal && currentTargetEnemy.IsAI && base.Bot.CurrentAILimit != AILimitSetting.None)
		{
			return false;
		}
		if (currentCombatDecision == ECombatDecision.HoldInCover)
		{
			resetLean = false;
			return false;
		}
		return true;
	}

	private void FindLean(Enemy enemy)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		IsRaycastLeaning = false;
		DirectLineOfSight = false;
		EnemyPlace lastKnownPlace = enemy.KnownPlaces.LastKnownPlace;
		if (lastKnownPlace == null)
		{
			FastLean(LeanSetting.None);
			return;
		}
		LeanSetting leanSetting = FindLeanFromBlindCornerAngle(enemy, 1f);
		if (leanSetting != LeanSetting.None)
		{
			FastLean(leanSetting);
			return;
		}
		DirectLineOfSight = CheckOffSetRay(lastKnownPlace.Position, 0f, 0f, out var _);
		if (DirectLineOfSight)
		{
			if (Time.time - _timeLastLeaned > 1f)
			{
				FastLean(LeanSetting.None);
			}
			return;
		}
		LeanSetting leanSetting2 = FindLeanDirectionRayCast(lastKnownPlace.Position);
		if (leanSetting2 != LeanSetting.None || Time.time - _timeLastLeaned > 1f)
		{
			IsRaycastLeaning = leanSetting2 != LeanSetting.None;
			FastLean(leanSetting2);
		}
	}

	public LeanSetting FindLeanFromBlindCornerAngle(Enemy enemy, float minAngle = -1f)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		Vector3? visiblePathPoint = enemy.VisiblePathPoint;
		if (!visiblePathPoint.HasValue)
		{
			return LeanSetting.None;
		}
		float? visiblePathPointSignedAngle = enemy.VisiblePathPointSignedAngle;
		if (!visiblePathPointSignedAngle.HasValue)
		{
			return LeanSetting.None;
		}
		if (minAngle > 0f && Mathf.Abs(visiblePathPointSignedAngle.Value) < minAngle)
		{
			return LeanSetting.None;
		}
		Vector3 val = visiblePathPoint.Value - base.Bot.Position;
		if (((Vector3)(ref val)).sqrMagnitude > 900f)
		{
			return LeanSetting.None;
		}
		return (visiblePathPointSignedAngle > 0f) ? LeanSetting.Left : LeanSetting.Right;
	}

	public void HoldLean(float duration)
	{
		if (LeanDirection != LeanSetting.None)
		{
			_stopHoldLeanTime = Time.time + duration;
		}
	}

	public void ResetLean()
	{
		FastLean(LeanSetting.None);
	}

	public LeanSetting FindLeanDirectionRayCast(Vector3 targetPos)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		RightLos = CheckOffSetRay(targetPos, 90f, 0.66f, out var Point);
		Vector3 val;
		if (!RightLos)
		{
			RightLosPos = Point;
			Point.y = base.BotOwner.Position.y;
			val = Point - base.BotOwner.Position;
			float dist = ((Vector3)(ref val)).magnitude / 2f;
			RightHalfLos = CheckOffSetRay(targetPos, 90f, dist, out var Point2);
			if (!RightHalfLos)
			{
				RightHalfLosPos = Point2;
			}
			else
			{
				RightHalfLosPos = null;
			}
		}
		else
		{
			RightLosPos = null;
			RightHalfLosPos = null;
		}
		LeftLos = CheckOffSetRay(targetPos, -90f, 0.66f, out var Point3);
		if (!LeftLos)
		{
			LeftLosPos = Point3;
			Point3.y = base.BotOwner.Position.y;
			val = Point3 - base.BotOwner.Position;
			float dist2 = ((Vector3)(ref val)).magnitude / 2f;
			LeftHalfLos = CheckOffSetRay(targetPos, -90f, dist2, out var Point4);
			if (!LeftHalfLos)
			{
				LeftHalfLosPos = Point4;
			}
			else
			{
				LeftHalfLosPos = null;
			}
		}
		else
		{
			LeftLosPos = null;
			LeftHalfLosPos = null;
		}
		return GetSettingFromResults();
	}

	public LeanSetting GetSettingFromResults()
	{
		if (DirectLineOfSight)
		{
			return LeanSetting.None;
		}
		return ((LeftLos || LeftHalfLos) && !RightLos) ? LeanSetting.Left : ((!LeftLos && (RightLos || RightHalfLos)) ? LeanSetting.Right : LeanSetting.None);
	}

	private bool CheckOffSetRay(Vector3 targetPos, float angle, float dist, out Vector3 Point)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = base.BotOwner.Position;
		position.y = base.Bot.Transform.HeadPosition.y;
		if (dist > 0f)
		{
			Vector3 val = targetPos - base.BotOwner.Position;
			Vector3 normalized = ((Vector3)(ref val)).normalized;
			Quaternion val2 = Quaternion.Euler(0f, angle, 0f);
			Vector3 direction = val2 * normalized;
			Point = FindOffset(position, direction, dist);
			val = Point - position;
			if (((Vector3)(ref val)).magnitude < dist / 3f)
			{
				return true;
			}
		}
		else
		{
			Point = position;
		}
		bool result = LineOfSight(Point, targetPos);
		Point.y = base.BotOwner.Position.y;
		return result;
	}

	private bool LineOfSight(Vector3 start, Vector3 target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = target - start;
		float num = Mathf.Clamp(((Vector3)(ref val)).magnitude, 0f, 16f);
		return !Physics.Raycast(start, ((Vector3)(ref val)).normalized, num, LayerMask.op_Implicit(LayerMaskClass.HighPolyWithTerrainMask));
	}

	private Vector3 FindOffset(Vector3 start, Vector3 direction, float distance)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 normalized = ((Vector3)(ref direction)).normalized;
		RaycastHit val = default(RaycastHit);
		if (Physics.Raycast(start, normalized, ref val, distance, LayerMask.op_Implicit(LayerMaskClass.HighPolyWithTerrainMask)))
		{
			return ((RaycastHit)(ref val)).point;
		}
		return start + normalized * distance;
	}
}
