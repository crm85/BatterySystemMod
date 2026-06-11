using DrakiaXYZ.BigBrain.Brains;
using EFT;
using SAIN.Classes.Coverfinder;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.Layers.Combat.Squad;

internal class AssaultAction : CombatAction
{
	private float _recalcPathTime;

	private CoverPoint PointDestination;

	public AssaultAction(BotOwner bot)
		: base(bot, "AssaultAction")
	{
	}

	public override void Update(ActionData data)
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		Enemy enemy = base.Bot.Enemy;
		base.Shoot.ShootAnyVisibleEnemies(enemy);
		if (!base.Bot.Steering.SteerByPriority(enemy, lookRandom: false) && enemy != null)
		{
			base.Bot.Steering.LookToEnemy(enemy);
		}
		if (enemy == null)
		{
			return;
		}
		if (PointDestination == null)
		{
			PointDestination = base.Bot.Cover.FindPointInDirection(enemy.EnemyDirection);
		}
		if (PointDestination == null)
		{
			return;
		}
		Vector3 position = PointDestination.Position;
		Vector3 val = position - base.Bot.Position;
		if (((Vector3)(ref val)).sqrMagnitude < 1f)
		{
			PointDestination = null;
		}
		else if (_recalcPathTime < Time.time)
		{
			bool calculating;
			if (true && ((CustomLogic)this).BotOwner.BotRun.Run(position, false, SAINPlugin.LoadedPreset.GlobalSettings.General.SprintReachDistance))
			{
				base.Bot.Steering.LookToMovingDirection(sprint: true);
				_recalcPathTime = Time.time + 1f;
			}
			else if (base.Bot.Mover.GoToPoint(position, out calculating))
			{
				_recalcPathTime = Time.time + 1f;
			}
			else
			{
				_recalcPathTime = Time.time + 0.5f;
			}
		}
	}

	public override void Start()
	{
	}

	public override void Stop()
	{
	}
}
