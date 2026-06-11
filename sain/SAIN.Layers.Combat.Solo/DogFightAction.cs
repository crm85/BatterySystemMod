using System.Text;
using DrakiaXYZ.BigBrain.Brains;
using EFT;
using SAIN.SAINComponent.Classes.EnemyClasses;

namespace SAIN.Layers.Combat.Solo;

internal class DogFightAction : CombatAction, ISAINAction
{
	public DogFightAction(BotOwner bot)
		: base(bot, "Dog Fight")
	{
	}

	public override void Update(ActionData data)
	{
		StartProfilingSample("Update");
		Enemy enemy = base.Bot.Decision.DogFightDecision.DogFightTarget ?? base.Bot.CurrentTarget.CurrentTargetEnemy;
		base.Bot.Mover.SetTargetPose(1f);
		base.Shoot.ShootAnyVisibleEnemies(enemy);
		base.Bot.Steering.SteerByPriority(enemy);
		base.Bot.Mover.DogFight.DogFightMove(aggressive: true, enemy);
		EndProfilingSample();
	}

	public override void Start()
	{
		Toggle(value: true);
		base.Bot.Mover.Sprint(value: false);
		((CustomLogic)this).BotOwner.Mover.SprintPause(0.5f);
	}

	public override void Stop()
	{
		Toggle(value: false);
		base.Bot.Mover.DogFight.ResetDogFightStatus();
		((CustomLogic)this).BotOwner.MovementResume();
	}

	public void Toggle(bool value)
	{
		ToggleAction(value);
	}

	public override void BuildDebugText(StringBuilder stringBuilder)
	{
		DebugOverlay.AddBaseInfo(base.Bot, ((CustomLogic)this).BotOwner, stringBuilder);
	}
}
