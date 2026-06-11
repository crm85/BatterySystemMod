using DrakiaXYZ.BigBrain.Brains;
using EFT;
using UnityEngine;

namespace SAIN.Layers.Combat.Squad;

internal class RegroupAction : CombatAction, ISAINAction
{
	private float _nextChangeSprintTime;

	public RegroupAction(BotOwner bot)
		: base(bot, "RegroupAction")
	{
	}

	public void Toggle(bool value)
	{
		ToggleAction(value);
	}

	public override void Update(ActionData data)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		StartProfilingSample("Update");
		Vector3? val = base.Bot.Squad.LeaderComponent?.Position;
		if (val.HasValue)
		{
			base.Bot.Mover.GoToPoint(val.Value, out var _);
			CheckShouldSprint(val.Value);
		}
		base.Bot.Mover.SetTargetPose(1f);
		base.Bot.Mover.SetTargetMoveSpeed(1f);
		if (!base.Bot.Mover.PathFollower.Running)
		{
			base.Shoot.ShootAnyVisibleEnemies(base.Bot.Enemy);
			base.Bot.Steering.SteerByPriority(base.Bot.Enemy);
		}
		EndProfilingSample();
	}

	public override void Start()
	{
		Toggle(value: true);
	}

	private void CheckShouldSprint(Vector3 pos)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		bool hasEnemy = base.Bot.HasEnemy;
		bool flag = base.Bot.Enemy?.InLineOfSight ?? false;
		Vector3 val = pos - ((CustomLogic)this).BotOwner.Position;
		float magnitude = ((Vector3)(ref val)).magnitude;
		float num;
		if (!hasEnemy)
		{
			num = 999f;
		}
		else
		{
			val = base.Bot.Enemy.EnemyIPlayer.Position - ((CustomLogic)this).BotOwner.Position;
			num = ((Vector3)(ref val)).magnitude;
		}
		float num2 = num;
		bool flag2 = hasEnemy && magnitude > 30f && !flag && num2 > 50f;
		if (base.Bot.Steering.SteerByPriority(null, lookRandom: false))
		{
			flag2 = false;
		}
		if (_nextChangeSprintTime < Time.time)
		{
			_nextChangeSprintTime = Time.time + 1f;
			if (flag2)
			{
				base.Bot.Mover.Sprint(value: true);
				return;
			}
			base.Bot.Mover.Sprint(value: false);
			base.Bot.Steering.SteerByPriority();
		}
	}

	public override void Stop()
	{
		Toggle(value: false);
	}
}
