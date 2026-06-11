using DrakiaXYZ.BigBrain.Brains;
using EFT;
using SAIN.Helpers;
using SAIN.Models.Enums;
using SAIN.Preset.GlobalSettings;
using SAIN.SAINComponent.Classes.EnemyClasses;
using SAIN.SAINComponent.Classes.Mover;
using UnityEngine;

namespace SAIN.Layers.Combat.Solo;

internal class RushEnemyAction : CombatAction, ISAINAction
{
	private Vector3 _lastMovePos;

	private const float CHANGE_MOVE_THRESHOLD = 1f;

	private float TryJumpTimer;

	private bool _shallBunnyHop = false;

	private float _updateMoveTime = 0f;

	private Enemy _enemy;

	private bool _shallTryJump = false;

	public RushEnemyAction(BotOwner bot)
		: base(bot, "RushEnemyAction")
	{
	}

	public override void Update(ActionData data)
	{
		StartProfilingSample("Update");
		base.Bot.Mover.SetTargetPose(1f);
		base.Bot.Mover.SetTargetMoveSpeed(1f);
		updateRushBehavior();
		EndProfilingSample();
	}

	private void updateRushBehavior()
	{
		if (!checkHasEnemy())
		{
			base.Bot.Steering.SteerByPriority();
			return;
		}
		if (_enemy.InLineOfSight)
		{
			enemyInSight();
			return;
		}
		checkUpdateMove();
		checkJump();
	}

	public void Toggle(bool value)
	{
		ToggleAction(value);
	}

	private bool checkHasEnemy()
	{
		_enemy = base.Bot.Enemy;
		return _enemy != null;
	}

	private void enemyInSight()
	{
		checkJumpEnemyInSight();
		base.Shoot.ShootAnyVisibleEnemies(_enemy);
		base.Bot.Mover.Sprint(value: false);
		base.Bot.Mover.DogFight.DogFightMove(aggressive: true, _enemy);
		if (_enemy.IsVisible && _enemy.CanShoot)
		{
			base.Bot.Steering.SteerByPriority(_enemy);
		}
		else
		{
			base.Bot.Steering.LookToEnemy(_enemy);
		}
	}

	private void checkJump()
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		if (!base.Bot.Info.FileSettings.Move.JUMP_TOGGLE || !GlobalSettingsClass.Instance.Move.JUMP_TOGGLE || !_shallTryJump || !(TryJumpTimer < Time.time) || !base.Bot.Player.IsSprintEnabled)
		{
			return;
		}
		Vector3? val = _enemy.Path.EnemyCorners.GroundPosition(ECornerType.Last);
		if (val.HasValue)
		{
			Vector3 val2 = val.Value - base.Bot.Position;
			if (((Vector3)(ref val2)).sqrMagnitude < 1f)
			{
				TryJumpTimer = Time.time + 3f;
				base.Bot.Mover.TryJump();
			}
		}
	}

	private void checkJumpEnemyInSight()
	{
		if (!base.Bot.Info.FileSettings.Move.JUMP_TOGGLE || !GlobalSettingsClass.Instance.Move.JUMP_TOGGLE || !_shallTryJump)
		{
			return;
		}
		if (_shallBunnyHop)
		{
			base.Bot.Mover.TryJump();
		}
		else if (TryJumpTimer < Time.time && base.Bot.Player.IsSprintEnabled)
		{
			TryJumpTimer = Time.time + 3f;
			if (!_shallBunnyHop && EFTMath.RandomBool(base.Bot.Info.PersonalitySettings.Rush.BunnyHopChance))
			{
				_shallBunnyHop = true;
			}
			base.Bot.Mover.TryJump();
		}
	}

	private void checkUpdateMove()
	{
		if (!(_updateMoveTime < Time.time))
		{
			return;
		}
		if (base.Bot.Mover.PathFollower.Moving && base.Bot.Mover.PathFollower.MoveData.Canceling)
		{
			_updateMoveTime = Time.time + 0.1f;
			return;
		}
		if (updateMove(_enemy))
		{
			_updateMoveTime = Time.time + 2f;
		}
		_updateMoveTime = Time.time + 0.1f;
	}

	private bool updateMove(Enemy enemy)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		Vector3? lastKnownPosition = enemy.KnownPlaces.LastKnownPosition;
		if (!lastKnownPosition.HasValue)
		{
			return false;
		}
		BotPathFollowerClass pathFollower = base.Bot.Mover.PathFollower;
		float pathLength = enemy.Path.PathLength;
		if (pathLength <= 1f && pathFollower.Moving)
		{
			return true;
		}
		if (pathFollower.Moving)
		{
			Vector3 val = _lastMovePos - lastKnownPosition.Value;
			if (((Vector3)(ref val)).sqrMagnitude < 1f)
			{
				return true;
			}
		}
		_lastMovePos = lastKnownPosition.Value;
		if (pathLength > ((CustomLogic)this).BotOwner.Settings.FileSettings.Move.RUN_TO_COVER_MIN && pathFollower.RunToPointByWay(enemy.Path.PathToEnemy, ESprintUrgency.High, stopSprintEnemyVisible: true))
		{
			return true;
		}
		if (pathFollower.Running)
		{
			return true;
		}
		if (base.Bot.Mover.GoToEnemy(enemy))
		{
			return true;
		}
		return false;
	}

	public override void Start()
	{
		checkHasEnemy();
		Toggle(value: true);
		_shallTryJump = base.Bot.Info.PersonalitySettings.Rush.CanJumpCorners && EFTMath.RandomBool(base.Bot.Info.PersonalitySettings.Rush.JumpCornerChance);
		_shallBunnyHop = false;
	}

	public override void Stop()
	{
		Toggle(value: false);
		base.Bot.Mover.DogFight.ResetDogFightStatus();
	}
}
