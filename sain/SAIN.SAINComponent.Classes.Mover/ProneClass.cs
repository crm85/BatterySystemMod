using SAIN.Components;
using SAIN.Helpers;
using SAIN.Preset.GlobalSettings;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Mover;

public class ProneClass : BotBase
{
	private float _nextChangeProneTime { get; set; }

	private bool _canshoot { get; set; }

	private float _nextCheckShootTime { get; set; }

	public ProneClass(BotComponent sain)
		: base(sain)
	{
	}

	public void SetProne(bool value)
	{
		base.BotOwner.BotLay.IsLay = value;
	}

	public bool ShallProne(bool withShoot, float mindist = 25f)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		if (!base.Bot.Info.FileSettings.Move.PRONE_TOGGLE || !GlobalSettingsClass.Instance.Move.PRONE_TOGGLE)
		{
			return false;
		}
		if (base.Player.MovementContext.CanProne)
		{
			Enemy enemy = base.Bot.Enemy;
			if (enemy != null)
			{
				Vector3 val = enemy.EnemyPosition - base.Bot.Position;
				float sqrMagnitude = ((Vector3)(ref val)).sqrMagnitude;
				if (sqrMagnitude > mindist * mindist)
				{
					if (withShoot)
					{
						return CanShootFromProne(enemy.EnemyPosition);
					}
					return true;
				}
			}
		}
		return false;
	}

	public bool ShallProneHide(Enemy enemy, float mindist = 10f)
	{
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		if (enemy == null)
		{
			return false;
		}
		if (!base.Bot.Info.FileSettings.Move.PRONE_TOGGLE || !GlobalSettingsClass.Instance.Move.PRONE_TOGGLE)
		{
			return false;
		}
		if (_nextChangeProneTime > Time.time)
		{
			return base.Player.IsInPronePose;
		}
		if (!base.Player.MovementContext.CanProne)
		{
			return false;
		}
		Vector3? lastKnownPosition = enemy.LastKnownPosition;
		if (!lastKnownPosition.HasValue)
		{
			return false;
		}
		if (base.Bot.CurrentTargetDistance < mindist)
		{
			return false;
		}
		bool flag = base.Bot.Decision.CurrentSelfDecision != ESelfDecision.None || base.Bot.Suppression.IsHeavySuppressed || !CheckShootProne(lastKnownPosition.Value, enemy);
		if (flag)
		{
			_nextChangeProneTime = Time.time + 3f;
		}
		return flag;
	}

	private bool CheckShootProne(Vector3? lastKnownPos, Enemy enemy)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (enemy.GetVisibilePathPoint(out var pathPoint))
		{
			return CanShootFromProne(pathPoint);
		}
		return CanShootFromProne(lastKnownPos.Value);
	}

	public bool CanShootFromProne(Vector3 target)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		Vector3 val = base.Bot.Transform.Position + Vector3.up * 0.14f;
		Vector3 val2 = target + Vector3.up - val;
		Vector3 val3 = val2;
		val3.y = val.y;
		float num = Vector3.Angle(val3, val2);
		float lAY_DOWN_ANG_SHOOT = HelpersGClass.LAY_DOWN_ANG_SHOOT;
		return num <= Mathf.Abs(lAY_DOWN_ANG_SHOOT) && Vector.CanShootToTarget(new ShootPointClass(target, 1f), val, base.BotOwner.LookSensor.Mask, doubleSide: true);
	}
}
