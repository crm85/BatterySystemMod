using DrakiaXYZ.BigBrain.Brains;
using EFT;
using SAIN.SAINComponent.Classes.EnemyClasses;

namespace SAIN.Layers.Combat.Solo;

internal class FreezeAction : CombatAction
{
	public FreezeAction(BotOwner bot)
		: base(bot, "FreezeAction")
	{
	}

	public void Toggle(bool value)
	{
		ToggleAction(value);
	}

	public override void Update(ActionData data)
	{
		StartProfilingSample("Update");
		base.Bot.Mover.SetTargetPose(0f);
		Enemy enemy = base.Bot.Enemy;
		if (enemy != null)
		{
			base.Shoot.ShootAnyVisibleEnemies(enemy);
			if (!base.Bot.Steering.SteerByPriority(enemy, lookRandom: false))
			{
				base.Bot.Steering.LookToLastKnownEnemyPosition(enemy);
			}
		}
		EndProfilingSample();
	}

	public override void Start()
	{
		Toggle(value: true);
		base.Bot.Mover.StopMove();
	}

	public override void Stop()
	{
		Toggle(value: false);
	}
}
