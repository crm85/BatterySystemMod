using DrakiaXYZ.BigBrain.Brains;
using EFT;

namespace SAIN.Layers.Combat.Squad;

internal class HuntAction : CombatAction, ISAINAction
{
	public HuntAction(BotOwner bot)
		: base(bot, "HuntAction")
	{
	}

	public void Toggle(bool value)
	{
		ToggleAction(value);
	}

	public override void Update(ActionData data)
	{
		StartProfilingSample("Update");
		EndProfilingSample();
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
