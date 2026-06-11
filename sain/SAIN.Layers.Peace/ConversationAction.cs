using DrakiaXYZ.BigBrain.Brains;
using EFT;

namespace SAIN.Layers.Peace;

internal class ConversationAction : CombatAction, ISAINAction
{
	public ConversationAction(BotOwner bot)
		: base(bot, "Extract")
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

	public override void Update(ActionData data)
	{
		StartProfilingSample("Update");
		EndProfilingSample();
	}

	public void Toggle(bool value)
	{
		ToggleAction(value);
	}
}
