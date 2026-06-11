using System;
using EFT;
using SAIN.Components;
using SAIN.Components.PlayerComponentSpace;
using SAIN.Components.RotationController;
using SAIN.Models.Enums;
using SAIN.Preset.GlobalSettings;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.Classes;

public class PlayerMovementSmoothingClass
{
	private Vector3 RandomSwayOffset;

	private float loopTime;

	public Vector3 CurrentControlLookDirection => ControlLookDirection.Current;

	public SmoothDampVectorDirectionNormal ControlLookDirection { get; } = new SmoothDampVectorDirectionNormal();

	public void ManualUpdate(float currentTime, float deltaTime, Player player, BotOwner botOwner, BotComponent botComponent)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		SteeringSettings steering = GlobalSettingsClass.Instance.Steering;
		if (steering.RANDOMSWAY_TOGGLE)
		{
			RandomSwayOffset = CalcRandomSway(deltaTime) * CalcRandomSwayModifier(player, botOwner, botComponent);
		}
		else
		{
			RandomSwayOffset = Vector3.zero;
		}
		TurnSettings turnSettings = GetTurnSettings(botOwner, botComponent);
		ControlLookDirection.Calculate(deltaTime, turnSettings.SmoothingValue, turnSettings.MaxTurnSpeed, steering.TURN_PITCH_MAX);
		player.CharacterController.SetSteerDirection(ControlLookDirection.Current);
	}

	private Vector3 CalcRandomSway(float deltaTime)
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		SteeringSettings steering = GlobalSettingsClass.Instance.Steering;
		loopTime += deltaTime;
		float num = loopTime % steering.RANDOMSWAY_LOOPDURATION / steering.RANDOMSWAY_LOOPDURATION;
		float num2 = num * (float)Math.PI * 2f;
		float num3 = Mathf.Cos(num2);
		float num4 = Mathf.Sin(num2);
		float num5 = Mathf.Sin(num2 * 2f) * 0.5f + Mathf.Sin(num2 * 3.1f) * 0.3f;
		Vector3 val = new Vector3(num3, num5, num4);
		Vector3 val2 = ((Vector3)(ref val)).normalized * steering.RANDOMSWAY_RADIUS;
		float num6 = Mathf.Sin(num2 * 4f + (float)Math.PI / 2f) * steering.RANDOMSWAY_SCALE;
		return val2 + new Vector3(num6, 0f - num6, num6 * 0.5f);
	}

	private static float CalcRandomSwayModifier(Player player, BotOwner botOwner, BotComponent botComponent)
	{
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Invalid comparison between Unknown and I4
		float num = 1f;
		if (player.IsSprintEnabled)
		{
			return 0.25f;
		}
		MovementContext movementContext = player.MovementContext;
		bool flag = movementContext.PhysicalConditionIs((EPhysicalCondition)16) || movementContext.PhysicalConditionIs((EPhysicalCondition)32);
		bool flag2 = player.Physical.Stamina.NormalValue <= 0.1f;
		int num2;
		if (botComponent == null || botComponent.Mover?.PathFollower?.Moving != true)
		{
			BotMover mover = botOwner.Mover;
			num2 = ((mover != null && mover.IsMoving) ? 1 : 0);
		}
		else
		{
			num2 = 1;
		}
		bool flag3 = (byte)num2 != 0;
		IBotAiming currentAiming = botOwner.AimingManager.CurrentAiming;
		BotAimingClass val = (BotAimingClass)(object)((currentAiming is BotAimingClass) ? currentAiming : null);
		bool flag4 = val != null && (int)val.aimStatus_0 != 1;
		IFirearmHandsController shootController = botOwner.WeaponManager.ShootController;
		bool flag5 = shootController != null && ((IHandsController)shootController).IsAiming;
		if (flag4)
		{
			num *= 0.66f;
		}
		if (flag5)
		{
			num *= 0.66f;
		}
		if (flag3)
		{
			num *= 2f;
		}
		if (flag)
		{
			num *= 1.5f;
		}
		if (flag2)
		{
			num *= 1.5f;
		}
		return Mathf.Clamp(num, 0.001f, 2.5f);
	}

	private static TurnSettings GetTurnSettings(BotOwner bot, BotComponent botComponent)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Invalid comparison between Unknown and I4
		SteeringSettings steering = GlobalSettingsClass.Instance.Steering;
		IBotAiming currentAiming = bot.AimingManager.CurrentAiming;
		TurnSettings value;
		if (currentAiming == null || !currentAiming.IsReady)
		{
			IBotAiming currentAiming2 = bot.AimingManager.CurrentAiming;
			BotAimingClass val = (BotAimingClass)(object)((currentAiming2 is BotAimingClass) ? currentAiming2 : null);
			if (val == null || (int)val.aimStatus_0 == 1)
			{
				if ((Object)(object)botComponent != (Object)null)
				{
					if (botComponent.SAINLayersActive && botComponent.Mover.PathFollower.Running)
					{
						if (steering.SMOOTHTURN_SETTINGS_BY_STATE.TryGetValue(EBotLookMode.CombatSprint, out value))
						{
							return value;
						}
						return new TurnSettings(0.25f, 500f);
					}
					if (botComponent.Steering.CurrentSteerPriority == ESteerPriority.RandomLook)
					{
						if (steering.SMOOTHTURN_SETTINGS_BY_STATE.TryGetValue(EBotLookMode.RandomLook, out value))
						{
							return value;
						}
						return new TurnSettings(0.75f, 240f);
					}
					Enemy currentTargetEnemy = botComponent.CurrentTarget.CurrentTargetEnemy;
					if (currentTargetEnemy != null)
					{
						if (currentTargetEnemy.IsVisible)
						{
							if (steering.SMOOTHTURN_SETTINGS_BY_STATE.TryGetValue(EBotLookMode.CombatVisibleEnemy, out value))
							{
								return value;
							}
							return new TurnSettings(0.4f, 500f);
						}
						if (steering.SMOOTHTURN_SETTINGS_BY_STATE.TryGetValue(EBotLookMode.Combat, out value))
						{
							return value;
						}
						return new TurnSettings(0.5f, 360f);
					}
				}
				else
				{
					if (bot.Memory.GoalEnemy != null)
					{
						if (steering.SMOOTHTURN_SETTINGS_BY_STATE.TryGetValue(EBotLookMode.Combat, out value))
						{
							return value;
						}
						return new TurnSettings(0.3f);
					}
					if (bot.Mover.Sprinting)
					{
						if (steering.SMOOTHTURN_SETTINGS_BY_STATE.TryGetValue(EBotLookMode.CombatSprint, out value))
						{
							return value;
						}
						return new TurnSettings(0.2f, 500f);
					}
				}
				if (steering.SMOOTHTURN_SETTINGS_BY_STATE.TryGetValue(EBotLookMode.Peace, out value))
				{
					return value;
				}
				return new TurnSettings(0.65f);
			}
		}
		if (steering.SMOOTHTURN_SETTINGS_BY_STATE.TryGetValue(EBotLookMode.Aiming, out value))
		{
			return value;
		}
		return new TurnSettings(0.2f, 500f);
	}

	public void SetTargetLookDirection(Vector3 targetDirection)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		ControlLookDirection.Target = targetDirection + RandomSwayOffset;
	}

	public void SetTargetMoveDirection(Vector3 direction, Player player)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (!(((Vector3)(ref direction)).sqrMagnitude > 0.0001f))
		{
			return;
		}
		Vector2 val = FindMoveDirection(direction, player.Rotation);
		if (!(((Vector2)(ref val)).sqrMagnitude > 0.0001f))
		{
			return;
		}
		player.Move(val);
		IAIData aIData = player.AIData;
		if (aIData == null)
		{
			return;
		}
		BotOwner botOwner = aIData.BotOwner;
		if (botOwner == null)
		{
			return;
		}
		AimingManager aimingManager = botOwner.AimingManager;
		if (aimingManager != null)
		{
			IBotAiming currentAiming = aimingManager.CurrentAiming;
			if (currentAiming != null)
			{
				currentAiming.Move(player.Speed);
			}
		}
	}

	public void SetTargetMovePoint(Vector3 point, Player player)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		SetTargetMoveDirection(point - player.Position, player);
	}

	private static Vector2 FindMoveDirection(Vector3 direction, Vector2 playerRotation)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		Vector3 direction2 = Quaternion.Euler(0f, 0f, playerRotation.x) * Vector2.op_Implicit(new Vector2(direction.x, direction.z));
		return MoveDirToVector2(direction2);
	}

	private static Vector2 MoveDirToVector2(Vector3 direction)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(direction.x, direction.y);
	}
}
