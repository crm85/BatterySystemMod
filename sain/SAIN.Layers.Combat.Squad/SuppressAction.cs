using DrakiaXYZ.BigBrain.Brains;
using EFT;
using SAIN.Models.Enums;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.Layers.Combat.Squad;

internal class SuppressAction : CombatAction, ISAINAction
{
	private bool _manualShooting;

	private float _nextShotTime;

	private bool _canSeeSuppTarget;

	private float _nextCheckVisTime;

	public SuppressAction(BotOwner bot)
		: base(bot, "SuppressAction")
	{
	}

	public void Toggle(bool value)
	{
		ToggleAction(value);
	}

	public override void Update(ActionData data)
	{
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		Enemy enemy = base.Bot.Enemy;
		if (enemy != null)
		{
			if (base.Shoot.ShootAnyVisibleEnemies(enemy))
			{
				base.Bot.Mover.StopMove();
				return;
			}
			if (base.Bot.ManualShoot.CanShoot() && FindSuppressionTarget(out var pos))
			{
				_manualShooting = true;
				base.Bot.Mover.StopMove();
				bool flag = base.Bot.Info.WeaponInfo.EWeaponClass == EWeaponClass.machinegun;
				if (flag && base.Bot.Mover.Prone.ShallProne(withShoot: true))
				{
					base.Bot.Mover.Prone.SetProne(value: true);
				}
				if (base.Bot.ManualShoot.TryShoot(enemy, pos.Value, checkFF: true, EShootReason.SquadSuppressing))
				{
					enemy.Status.EnemyIsSuppressed = true;
					float num = (flag ? 0.1f : 0.5f);
					_nextShotTime = Time.time + num * Random.Range(0.75f, 1.25f);
				}
				return;
			}
			Vector3? lastKnownPosition = enemy.LastKnownPosition;
			if (lastKnownPosition.HasValue)
			{
				base.Bot.Mover.GoToPoint(lastKnownPosition.Value, out var _, -1f, crawl: false, slowAtEnd: false, mustHaveCompletePath: false);
			}
		}
		ResetManualShoot();
		if (!base.Bot.Steering.SteerByPriority(enemy, lookRandom: false))
		{
			base.Bot.Steering.LookToLastKnownEnemyPosition(enemy);
		}
	}

	private void ResetManualShoot()
	{
		if (_manualShooting)
		{
			_manualShooting = false;
			base.Bot.ManualShoot.Reset();
		}
	}

	private bool FindSuppressionTarget(out Vector3? pos)
	{
		pos = base.Bot.Enemy?.SuppressionTarget;
		return pos.HasValue;
	}

	private bool CanSeeSuppressionTarget(Vector3? target)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (!target.HasValue)
		{
			_canSeeSuppTarget = false;
		}
		else if (_nextCheckVisTime < Time.time)
		{
			_nextCheckVisTime = Time.time + 0.5f;
			Vector3 headPosition = base.Bot.Transform.HeadPosition;
			Vector3 val = target.Value - headPosition;
			Vector3 val2 = target.Value - headPosition;
			_canSeeSuppTarget = !Physics.Raycast(headPosition, val, ((Vector3)(ref val2)).magnitude * 0.8f);
		}
		return _canSeeSuppTarget;
	}

	public override void Start()
	{
		Toggle(value: true);
	}

	public override void Stop()
	{
		Toggle(value: false);
		ResetManualShoot();
	}
}
