using DrakiaXYZ.BigBrain.Brains;
using EFT;
using SAIN.SAINComponent.Classes.EnemyClasses;

namespace SAIN.Layers.Combat.Solo;

internal class FightZombiesAction : CombatAction, ISAINAction
{
	public FightZombiesAction(BotOwner bot)
		: base(bot, "Fight Zombies")
	{
	}

	public override void Update(ActionData data)
	{
		Enemy currentTargetEnemy = base.Bot.CurrentTarget.CurrentTargetEnemy;
		Enemy enemyToShoot = base.Shoot.GetEnemyToShoot(currentTargetEnemy);
		if (enemyToShoot != null)
		{
			if (enemyToShoot.RealDistance < 10f)
			{
				base.Bot.Mover.DogFight.BackUpFromEnemy(enemyToShoot);
				base.Bot.Mover.SetTargetMoveSpeed(1f);
				base.Bot.Mover.SetTargetPose(1f);
			}
			else if (enemyToShoot.RealDistance > 20f)
			{
				base.Bot.Mover.StopMove();
			}
		}
		else
		{
			base.Bot.Mover.DogFight.DogFightMove(aggressive: true, currentTargetEnemy);
			base.Bot.Steering.SteerByPriority(currentTargetEnemy);
		}
	}

	public override void Start()
	{
		Toggle(value: true);
	}

	public override void Stop()
	{
		Toggle(value: false);
	}

	public void Toggle(bool value)
	{
		ToggleAction(value);
	}
}
