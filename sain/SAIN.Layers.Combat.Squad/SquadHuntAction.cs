using DrakiaXYZ.BigBrain.Brains;
using EFT;

namespace SAIN.Layers.Combat.Squad;

internal class SquadHuntAction : CombatAction, ISAINAction
{
	public SquadHuntAction(BotOwner bot)
		: base(bot, "SquadHuntAction")
	{
	}

	public void Toggle(bool value)
	{
		ToggleAction(value);
	}

	public override void Update(ActionData data)
	{
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
