using DrakiaXYZ.BigBrain.Brains;
using EFT;
using SAIN.Classes.Coverfinder;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.Layers.Combat.Solo;

internal class MoveToEngageAction : CombatAction, ISAINAction
{
	private float RecalcPathTimer;

	public MoveToEngageAction(BotOwner bot)
		: base(bot, "MoveToEngageAction")
	{
	}

	public void Toggle(bool value)
	{
		ToggleAction(value);
	}

	public override void Update(ActionData data)
	{
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		StartProfilingSample("Update");
		Enemy enemy = base.Bot.Enemy;
		if (enemy == null)
		{
			base.Bot.Steering.SteerByPriority();
			EndProfilingSample();
			return;
		}
		base.Bot.Mover.SetTargetPose(1f);
		base.Bot.Mover.SetTargetMoveSpeed(1f);
		if (CheckShoot(enemy))
		{
			base.Shoot.ShootAnyVisibleEnemies(enemy);
			base.Bot.Steering.SteerByPriority(enemy);
			EndProfilingSample();
			return;
		}
		Vector3? lastKnownPosition = enemy.KnownPlaces.LastKnownPosition;
		Vector3 val;
		if (lastKnownPosition.HasValue)
		{
			val = lastKnownPosition.Value;
		}
		else
		{
			if (!(enemy.TimeSinceSeen < 5f))
			{
				base.Shoot.ShootAnyVisibleEnemies(enemy);
				base.Bot.Steering.SteerByPriority(enemy);
				EndProfilingSample();
				return;
			}
			val = enemy.EnemyPosition;
		}
		CoverPoint coverPoint = base.Bot.Cover.FindPointInDirection(val - base.Bot.Position, 0.5f, 3f);
		if (coverPoint != null)
		{
			val = coverPoint.Position;
		}
		float realDistance = enemy.RealDistance;
		if (realDistance > 40f && !((CustomLogic)this).BotOwner.Memory.IsUnderFire)
		{
			if (RecalcPathTimer < Time.time)
			{
				RecalcPathTimer = Time.time + 2f;
				((CustomLogic)this).BotOwner.BotRun.Run(val, false, SAINPlugin.LoadedPreset.GlobalSettings.General.SprintReachDistance);
				base.Bot.Steering.LookToMovingDirection(sprint: true);
			}
			EndProfilingSample();
			return;
		}
		base.Bot.Mover.Sprint(value: false);
		if (RecalcPathTimer < Time.time)
		{
			RecalcPathTimer = Time.time + 2f;
			((CustomLogic)this).BotOwner.MoveToEnemyData.TryMoveToEnemy(val);
		}
		if (!base.Bot.Steering.SteerByPriority(null, lookRandom: false))
		{
			base.Bot.Steering.LookToMovingDirection();
		}
		EndProfilingSample();
	}

	private bool CheckShoot(Enemy enemy)
	{
		float realDistance = enemy.RealDistance;
		bool enemyLookingAtMe = enemy.EnemyLookingAtMe;
		float effectiveWeaponDistance = base.Bot.Info.WeaponInfo.EffectiveWeaponDistance;
		if (enemy.IsVisible)
		{
			if (enemyLookingAtMe)
			{
				return true;
			}
			if (realDistance <= effectiveWeaponDistance && enemy.CanShoot)
			{
				return true;
			}
		}
		return false;
	}

	public override void Start()
	{
		Toggle(value: true);
	}

	public override void Stop()
	{
		Toggle(value: false);
	}
}
