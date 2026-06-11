using DrakiaXYZ.BigBrain.Brains;
using EFT;

namespace SAIN.Layers.Combat.Solo;

internal class ShootAction : CombatAction, ISAINAction
{
	public ShootAction(BotOwner bot)
		: base(bot, "ShootAction")
	{
	}

	public void Toggle(bool value)
	{
		ToggleAction(value);
	}

	public override void Start()
	{
		Toggle(value: true);
	}

	public override void Stop()
	{
		Toggle(value: false);
	}

	public override void Update(ActionData data)
	{
		StartProfilingSample("Update");
		base.Shoot.ShootAnyVisibleEnemies(base.Bot.Enemy);
		base.Bot.Steering.SteerByPriority(base.Bot.Enemy);
		EndProfilingSample();
	}
}
