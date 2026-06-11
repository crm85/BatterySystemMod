using System;
using SAIN.Components;
using SAIN.Helpers;
using SAIN.Models.Enums;
using SAIN.Preset.GlobalSettings;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Mover;

public class BlindFireController : BotBase, IBotClass, IDisposable
{
	private float _nextUpdateAimTargetTime;

	private float _nextBlindFireCheck;

	private int _blindFire;

	private bool _manualShooting;

	private Vector3 BlindFireTargetPos;

	private float _changeBlindFireTime = 0f;

	public bool BlindFireActive => ActiveBlindFireSetting != 0;

	public int ActiveBlindFireSetting => base.Player.MovementContext.BlindFire;

	public BlindFireController(BotComponent sain)
		: base(sain)
	{
	}

	private bool CheckAllowBlindFire()
	{
		if (!base.Bot.SAINLayersActive || !base.BotOwner.WeaponManager.IsReady || !base.BotOwner.WeaponManager.HaveBullets || base.Bot.Player.IsSprintEnabled || base.Bot.Cover.CoverInUse == null)
		{
			return false;
		}
		Enemy enemy = base.Bot.Enemy;
		if (enemy == null || !enemy.Seen || enemy.TimeSinceSeen > 30f)
		{
			return false;
		}
		if (enemy.IsVisible && enemy.CanShoot)
		{
			return false;
		}
		if (GlobalSettingsClass.Instance.General.AILimit.LimitAIvsAIGlobal && enemy.IsAI && base.Bot.CurrentAILimit != AILimitSetting.None)
		{
			return false;
		}
		if (!base.Bot.ManualShoot.CanShoot())
		{
			return false;
		}
		return true;
	}

	public override void ManualUpdate()
	{
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		base.ManualUpdate();
		if (_nextBlindFireCheck > Time.time)
		{
			if (_blindFire != 0)
			{
				if (!CheckAllowBlindFire())
				{
					_blindFire = 0;
				}
				else
				{
					TryShoot(base.Bot.Enemy);
				}
			}
			return;
		}
		_nextBlindFireCheck = Time.time + 0.2f;
		if (!CheckAllowBlindFire())
		{
			ResetBlindFire();
			return;
		}
		Vector3? lastKnownPosition = base.Bot.Enemy.KnownPlaces.LastKnownPosition;
		if (!lastKnownPosition.HasValue)
		{
			ResetBlindFire();
			_changeBlindFireTime = Time.time + 0.5f;
			return;
		}
		if (base.Bot.Steering.FindLastKnownTarget(base.Bot.Enemy, out var Result))
		{
			Vector3 val = lastKnownPosition.Value - Result;
			if (!(((Vector3)(ref val)).sqrMagnitude > 10f))
			{
				int blindFire = _blindFire;
				_blindFire = checkBlindFire(Result);
				if (_blindFire == 0)
				{
					ResetBlindFire();
					_changeBlindFireTime = Time.time + 0.5f;
					return;
				}
				bool flag = blindFire == 0;
				if (flag || _changeBlindFireTime < Time.time)
				{
					_changeBlindFireTime = Time.time + 1f;
					SetBlindFire(_blindFire);
				}
				if (flag || _nextUpdateAimTargetTime < Time.time || BlindFireTargetPos == Vector3.zero)
				{
					_nextUpdateAimTargetTime = Time.time + 1.5f;
					Vector3 position = base.Bot.Position;
					Vector3 val2 = Vector.Rotate(Result - position, Vector.RandomRange(3f), Vector.RandomRange(3f), Vector.RandomRange(3f));
					BlindFireTargetPos = val2 + position;
				}
				TryShoot(base.Bot.Enemy);
				return;
			}
		}
		ResetBlindFire();
		_changeBlindFireTime = Time.time + 0.5f;
	}

	private void TryShoot(Enemy enemy)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (base.Bot.ManualShoot.TryShoot(enemy, BlindFireTargetPos, checkFF: false, EShootReason.Blindfire))
		{
			_manualShooting = true;
		}
	}

	public void ResetBlindFire()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (ActiveBlindFireSetting != 0)
		{
			base.Player.MovementContext.SetBlindFire(0);
		}
		if (_manualShooting)
		{
			_manualShooting = false;
			base.Bot.ManualShoot.Reset();
		}
		_blindFire = 0;
		BlindFireTargetPos = Vector3.zero;
	}

	private int checkBlindFire(Vector3 targetPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		LayerMask highPolyWithTerrainMask = LayerMaskClass.HighPolyWithTerrainMask;
		Vector3 weaponFirePort = base.Bot.Transform.WeaponFirePort;
		Vector3 val = targetPos - weaponFirePort;
		if (Physics.Raycast(weaponFirePort, val, 5f, LayerMask.op_Implicit(highPolyWithTerrainMask)))
		{
			weaponFirePort = base.Bot.Transform.HeadPosition + Vector3.up * 0.15f;
			if (!Vector.Raycast(weaponFirePort, targetPos, highPolyWithTerrainMask))
			{
				return 1;
			}
			Quaternion val2 = Quaternion.Euler(0f, 90f, 0f);
			Vector3 val3 = val2 * ((Vector3)(ref val)).normalized * 0.2f;
			weaponFirePort += val3;
			val = targetPos - weaponFirePort;
			if (!Physics.Raycast(weaponFirePort, val, ((Vector3)(ref val)).magnitude, LayerMask.op_Implicit(highPolyWithTerrainMask)))
			{
				return -1;
			}
		}
		return 0;
	}

	public void SetBlindFire(int value)
	{
		if (ActiveBlindFireSetting != value)
		{
			base.Player.MovementContext.SetBlindFire(value);
		}
	}
}
