using System;
using EFT;
using SAIN.Components;
using SAIN.Helpers;
using SAIN.Preset.GlobalSettings;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Mover;

public class PoseClass : BotBase
{
	private float _stopSprintAndPoseChangeTime;

	public bool ObjectInFront => ObjectTargetPoseCover.HasValue;

	public float? ObjectTargetPoseCover { get; private set; }

	private float UpdateFindObjectTimer { get; set; }

	private float UpdateFindObjectInCoverTimer { get; set; }

	public SmoothDampenedFloat PoseValue { get; } = new SmoothDampenedFloat(0.3f);

	public SmoothDampenedFloat SpeedValue { get; } = new SmoothDampenedFloat(0.3f);

	public PoseClass(BotComponent sain)
		: base(sain)
	{
	}

	public override void ManualUpdate()
	{
		if (base.Player.IsSprintEnabled)
		{
			_stopSprintAndPoseChangeTime = Time.time + 1f;
		}
		if (base.Bot.SAINLayersActive)
		{
			float worldTickDeltaTime = GameWorldComponent.WorldTickDeltaTime;
			if (_stopSprintAndPoseChangeTime > Time.time)
			{
				PoseValue.Set(1f);
				PoseValue.Get(worldTickDeltaTime);
				SetPlayerPoseLevel(1f);
				SpeedValue.Set(1f);
				SpeedValue.Get(worldTickDeltaTime);
				SetPlayerSpeed(1f);
			}
			else
			{
				SetPlayerPoseLevel(PoseValue.Get(worldTickDeltaTime));
				SetPlayerSpeed(SpeedValue.Get(worldTickDeltaTime));
			}
		}
	}

	private void SetPlayerPoseLevel(float value)
	{
		if (!base.Player.IsInPronePose && !base.Bot.Mover.Crawling)
		{
			BotOwner botOwner = base.BotOwner;
			if (botOwner != null)
			{
				botOwner.SetPose(value);
			}
			float num = value - base.Player.PoseLevel;
			if (Math.Abs(num) >= float.Epsilon)
			{
				base.Player.ChangePose(num * 1f);
			}
		}
	}

	public void SetTargetSpeed(float value)
	{
		SpeedValue.Set(value);
	}

	private void SetPlayerSpeed(float value)
	{
		float num = value - base.Player.Speed;
		BotOwner botOwner = base.BotOwner;
		if (botOwner != null)
		{
			botOwner.SetTargetMoveSpeed(value);
		}
		if (Math.Abs(num) >= float.Epsilon)
		{
			base.Player.ChangeSpeed(num * 1f);
		}
	}

	public bool SetPoseToCover()
	{
		if (!base.Bot.Info.FileSettings.Move.AUTOCROUCH_TOGGLE || !GlobalSettingsClass.Instance.Move.AUTOCROUCH_TOGGLE)
		{
			return false;
		}
		FindObjectsInFront();
		return SetTargetPose(ObjectTargetPoseCover);
	}

	public bool SetTargetPose(float num)
	{
		PoseValue.Set(num);
		return canChangePose();
	}

	private bool canChangePose()
	{
		return _stopSprintAndPoseChangeTime < Time.time && !base.Player.IsInPronePose && !base.Bot.Mover.Crawling;
	}

	public bool SetTargetPose(float? num)
	{
		return num.HasValue && SetTargetPose(num.Value);
	}

	private void FindObjectsInFront()
	{
		if (UpdateFindObjectTimer < Time.time)
		{
			UpdateFindObjectTimer = Time.time + 0.5f;
			if (FindCrouchFromCover(out var targetPose))
			{
				ObjectTargetPoseCover = targetPose;
			}
			else
			{
				ObjectTargetPoseCover = null;
			}
		}
	}

	private bool FindCrouchFromCover(out float targetPose, bool useCollider = false)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		targetPose = 1f;
		if (base.Bot.AILimit.CurrentAILimit != AILimitSetting.None)
		{
			Enemy enemy = base.Bot.Enemy;
			if (enemy == null || enemy.IsAI)
			{
				goto IL_00a9;
			}
		}
		Enemy enemy2 = base.Bot.Enemy;
		if (enemy2 != null && enemy2.LastKnownPosition.HasValue)
		{
			Vector3 target = enemy2.LastKnownPosition.Value + Vector3.up;
			if (useCollider)
			{
				targetPose = FindCrouchHeightColliderSphereCast(target);
			}
			else
			{
				targetPose = FindCrouchHeightRaycast(target);
			}
		}
		goto IL_00a9;
		IL_00a9:
		return targetPose < 1f;
	}

	private float FindCrouchHeightRaycast(Vector3 target, float rayLength = 4f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		LayerMask highPolyWithTerrainMask = LayerMaskClass.HighPolyWithTerrainMask;
		Vector3 val = Vector3.up * (1f / 6f);
		Vector3 val2 = base.Bot.Transform.Position + Vector3.up * 1.6f;
		Vector3 val3 = target - val2;
		float num = 1.6f;
		for (int i = 0; i <= 6; i++)
		{
			DebugGizmos.Ray(val2, val3, Color.red, rayLength, 0.05f, temporary: true, 0.5f, taperLine: true);
			if (Physics.Raycast(val2, val3, rayLength, LayerMask.op_Implicit(highPolyWithTerrainMask)))
			{
				return FindCrouchHeight(num);
			}
			val2 -= val;
			val3 = target - val2;
			num -= 1f / 6f;
		}
		return 1f;
	}

	private float FindCrouchHeightColliderSphereCast(Vector3 target, float rayLength = 3f, bool flatDir = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		LayerMask highPolyWithTerrainMask = LayerMaskClass.HighPolyWithTerrainMask;
		Vector3 val = base.Bot.Transform.Position + Vector3.up * 0.75f;
		Vector3 val2 = target - val;
		if (flatDir)
		{
			val2.y = 0f;
		}
		float num = 1f;
		RaycastHit val3 = default(RaycastHit);
		if (Physics.SphereCast(val, 0.26f, val2, ref val3, rayLength, LayerMask.op_Implicit(highPolyWithTerrainMask)))
		{
			Bounds bounds = ((RaycastHit)(ref val3)).collider.bounds;
			num = ((Bounds)(ref bounds)).size.y;
			return FindCrouchHeight(num);
		}
		return 1f;
	}

	private float FindCrouchHeight(float height)
	{
		return height - 0.5f;
	}
}
